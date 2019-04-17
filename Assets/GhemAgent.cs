using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityEngine.SceneManagement;

public class GhemAgent : Agent
{

    Rigidbody2D rBody;

    public bool mHitGarbageCollector;
    public bool mHitWall;
    public bool mHitObstacle;
    public float mStickiness;
    public Rigidbody2D mRbody;
    public bool mJumpReady;
    public int mDirection;
    public int mHighestPoint;
    private SpriteRenderer mSpriteRenderer;

    [HideInInspector]
    public bool mJumpedOnce;
    private bool mIsInvulnerable;

    public GameObject background;
    public GameObject walls;
    public GameObject mCamera;
    public GameObject obstacleGenerator;
    public GameObject mGarbageCollector;

    public Vector3 mInitialPos;
    private Vector3 mWallInitialPos;
    private Vector3 mBackgroundInitialPos;
    private float highestY;
    //private float mScore = 0;
    private bool isOnPlatform = false;

    void Start()
    {
        mRbody = GetComponent<Rigidbody2D>();
        mSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        mInitialPos = this.transform.position;
        mWallInitialPos = walls.transform.position;
        mBackgroundInitialPos = background.transform.position;
        highestY = this.transform.position.y;
        mIsInvulnerable = false;
        mHighestPoint = (int)transform.position.y;
        mJumpedOnce = false;
        mJumpReady = true;
        mDirection = 1;
    }

    public void Jump()
    {
        if (mJumpReady)
        {
            mRbody.velocity = Vector2.zero;
            mRbody.AddForce(new Vector2(1100 * mDirection, 950));
            mJumpReady = false;
            mDirection *= -1;
            mHitWall = false;
        }
    }

    public override void AgentReset()
    {

        foreach(Transform child in transform.parent)
        {

            if(child.gameObject.tag == "ClonaObstacol" || child.gameObject.tag == "ObstacleGeneratorClone" || child.gameObject.tag == "PerkClone")
            {
                Destroy(child.gameObject);
            }
        }

        mCamera.transform.position = mCamera.GetComponent<CameraController>().mInitialPos;
        walls.transform.position = mWallInitialPos;
        mJumpedOnce = false;
        background.transform.position = mBackgroundInitialPos;


        // spawn obstacle generator

        GameObject cloneObstacleGenerator = Instantiate(this.obstacleGenerator, this.mInitialPos + (Vector3.up * 10), this.transform.rotation);
        cloneObstacleGenerator.tag = "ObstacleGeneratorClone";
        cloneObstacleGenerator.transform.parent = this.transform.parent;

        this.transform.position = this.mInitialPos;

    }

    public override void CollectObservations()
    {
        /*
         * This function collects observations from the enviorment that we pass and feeds them to the NN
         */
        int cnt = 0;
        Vector2 myPosition = new Vector2(this.transform.position.x, this.transform.position.y);

        foreach (Transform obstacle in transform.parent)
        {
            // Foreach obstacle in the current game scene. Can be max 2 obstacles spawned at the same time ( I think, I hope).
            if (cnt >= 2)
            {
                break;
            }

            if (obstacle.gameObject.tag == "ClonaObstacol")
            {
                cnt++;
                Vector2 vectorFromAgentToObstacle = new Vector2(obstacle.transform.position.x, obstacle.transform.position.y) - myPosition;
                ObstacolController controller = obstacle.gameObject.GetComponent<ObstacolController>();

                AddVectorObs(vectorFromAgentToObstacle.normalized); // vector from current position of agent to obstacle (I chose to put it this way because it's independend of any coordinates that obstacle or player may have wich can result in very large numbers and, so at any height these directions will be similar)
                AddVectorObs(controller.mDirection.normalized);   // direction in wich obstacle is headed
            }

        }

        // If we didn't find 2 and we only found one or none we put Zero vectors as input for Neural Network

        int i = 0;
        if (cnt < 3)
        {
            for(i = 1; i <= 2 - cnt; i++)
            {
                AddVectorObs(new Vector2(0, 0));    // position
                AddVectorObs(new Vector2(0, 0));    // direction
            }
        }

        cnt = 0;
        foreach (Transform child in transform.parent)
        {
            // search if there is a perk spawned
            if (cnt == 1)
            {
                // found it
                break;
            }
            if (child.gameObject.tag == "PerkClone")
            {
                cnt++;
                Vector2 fromMeToPerk = new Vector2(child.transform.position.x, child.transform.position.y) - myPosition;
                AddVectorObs(fromMeToPerk.normalized);  // same thing we did with obstacles 
            }
        }

        if(cnt == 0)
        {
            // if no perks were found
            AddVectorObs(new Vector2(0, 0));
        }

        AddVectorObs(mDirection);   // add as input in wich wall I am at the moment
        AddVectorObs(mIsInvulnerable);  // add info if its invulnerabile

        // I think it's inportant for the agent to know where the garbage collector is (aka the bottom of the screen) to know where not to go
        Vector2 garbageCollectorPos = new Vector2(mGarbageCollector.transform.position.x, mGarbageCollector.transform.position.y);
        Vector2 fromMeToGarbageCollector = garbageCollectorPos - myPosition;

        AddVectorObs(myPosition - fromMeToGarbageCollector.normalized);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        /*
         * Called every time Agent needs to make a decision, currently every 2 seconds, change it in Inspector at Agent game object
         */

        PushAgainstTheWall();

        if (vectorAction[0] == 1 )
        {  
            // jump action is true
            Jump();
            isOnPlatform = false;
        }

        if(this.transform.position.y > highestY)
        {
            SetReward(0.01f);
            highestY = this.transform.position.y;
        }

        if(isOnPlatform)
        {
            SetReward(-0.1f);
        }
    }

    public void PushAgainstTheWall()
    {
        if (mJumpedOnce)
        {
            // Daca playerul a sarit macar o data si se afla pe perete
            Vector2 direction = new Vector2(mDirection * -1, 0);
            mRbody.AddForce(direction * mStickiness);
        }
    }

    public void MakeInvulnerable()
    {
        mIsInvulnerable = true;
        mSpriteRenderer.color = new Color(251f, 0f, 255f, .5f);
        StartCoroutine(VulnerabilityTimer(10));
    }

    public void MakeVulnerable()
    {
        mIsInvulnerable = false;
        mSpriteRenderer.color = new Color(251f, 0f, 255f, 1f);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "PereteStangaBase" || collision.gameObject.tag == "Perete" || collision.gameObject.tag == "PereteDreaptaBase")
        {
            mJumpedOnce = true;
            mJumpReady = true;
        }

        if (collision.gameObject.tag == "GarbageCollector")
        {
            Done();
            SetReward(-1f);
            print("Died");

        }

        if (collision.gameObject.tag == "Ground")
        {
            isOnPlatform = true;
        }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ObstacolColider")
        {
            if (mIsInvulnerable)
            {
                GameObject parent = collision.transform.parent.gameObject;
                Destroy(parent);
                this.MakeVulnerable();
            }
            else
            {
                print("Died");
                SetReward(-1f);
                Done();
            }
        }
        else if (collision.gameObject.tag == "PerkClone")
        {
            print("Hit perk");
            SetReward(1f);
            Destroy(collision.gameObject);
            this.MakeInvulnerable();
        }

    }

    IEnumerator VulnerabilityTimer(float time)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        MakeVulnerable();
    }
}

