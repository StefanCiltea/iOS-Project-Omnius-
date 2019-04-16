using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityEngine.SceneManagement;

public class GhemAgent : Agent
{

    Rigidbody2D rBody;

    [HideInInspector]
    public bool mJumpedOnce;
    private bool mIsInvulnerable;
    private AgentMovementControll mActionController;

    public GameObject background;
    public GameObject walls;
    public GameObject mCamera;
    public GameObject obstacleGenerator;
    public GameObject mGarbageCollector;

    public Vector3 mInitialPos;
    private Vector3 mWallInitialPos;
    private Vector3 mBackgroundInitialPos;
    private float highestY;
    private float mScore = 0;
    private bool isOnPlatform = false;

    void Start()
    {
        mActionController = GetComponentInChildren<AgentMovementControll>();
        rBody = GetComponent<Rigidbody2D>();
        mInitialPos = this.transform.position;
        mWallInitialPos = walls.transform.position;
        mBackgroundInitialPos = background.transform.position;
        highestY = this.transform.position.y;
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
        this.mActionController.mJumpedOnce = false;
        background.transform.position = mBackgroundInitialPos;


        // spawn obstacle generator

        GameObject cloneObstacleGenerator = Instantiate(this.obstacleGenerator, this.mInitialPos + (Vector3.up * 10), this.transform.rotation);
        cloneObstacleGenerator.tag = "ObstacleGeneratorClone";
        cloneObstacleGenerator.transform.parent = this.transform.parent;

        this.transform.position = this.mInitialPos;

    }

    public override void CollectObservations()
    {
        int cnt = 0;
        Vector2 myPosition = new Vector2(this.transform.position.x, this.transform.position.y);

        foreach (Transform obstacle in transform.parent)
        {
            if (cnt >= 2)
            {
                break;
            }

            if (obstacle.gameObject.tag == "ClonaObstacol")
            {
                cnt++;
                AddVectorObs(new Vector2(obstacle.transform.position.x, obstacle.transform.position.y) - myPosition);
                Debug.Log("Obs Clona Obstacol: " + (new Vector2(obstacle.transform.position.x, obstacle.transform.position.y) - myPosition));
                ObstacolController controller = obstacle.gameObject.GetComponent<ObstacolController>();
                AddVectorObs(controller.mDirection);
            }

        }

        int i = 0;
        if (cnt < 3)
        {
            for(i = 1; i <= 2 - cnt; i++)
            {
                AddVectorObs(new Vector2(0, 0));
                AddVectorObs(new Vector2(0, 0));
            }
        }

        cnt = 0;
        foreach (Transform child in transform.parent)
        {

            if (cnt == 1)
            {
                break;
            }
            if (child.gameObject.tag == "PerkClone")
            {
                cnt++;
                AddVectorObs(new Vector2(child.transform.position.x, child.transform.position.y) - myPosition);
            }

        }

        if(cnt == 0)
        {
            AddVectorObs(new Vector2(0, 0));
        }

        AddVectorObs(myPosition);
        Vector2 gbCollectorPos = new Vector2(mGarbageCollector.transform.position.x, mGarbageCollector.transform.position.y);
        AddVectorObs(myPosition - gbCollectorPos);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        mActionController.pushAgainstTheWall();

        if (vectorAction[0] == 1 )
        {
            mActionController.Jump();
            isOnPlatform = false;
        }

        if(this.transform.position.y > highestY)
        {
            SetReward(0.05f);
            highestY = this.transform.position.y;
        }

        if(isOnPlatform)
        {
            SetReward(-0.1f);
        }
    }

    public void onGbCollectorHit()
    {
        Debug.Log("Hit gb collector");
        SetReward(-1f);
        Done();
    }

    public void onObstacleHit()
    {
        Debug.Log("Hit obstacle");
        SetReward(-1f);
        Done();
    }

    public void onPerkHit()
    {
        Debug.Log("Hit perk");
        SetReward(1f);
    }

    public void onWallHit()
    {
        Debug.Log("Hit perete");
    }

    public void onObstacleDestroyed()
    {
        SetReward(0.5f);
        Debug.Log("Destroyed Obstacle");

    }
    public void onPlatform()
    {
        isOnPlatform = true;

        if (mActionController.mJumpedOnce)
        {
            SetReward(-1f);
        }
        Debug.Log("On platform");

    }
}
