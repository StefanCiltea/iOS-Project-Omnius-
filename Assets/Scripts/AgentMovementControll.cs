using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMovementControll : MonoBehaviour
{
    public bool mHitGarbageCollector;
    public bool mHitWall;
    public bool mHitObstacle;
    public float mStickiness;
    public Rigidbody2D mRbody;
    public bool mJumpReady;
    public int mDirection;
    public bool mJumpedOnce;
    public int mHighestPoint;
    private bool mIsInvulnerable;
    private SpriteRenderer mSpriteRenderer;

    void Start()
    {
        mHitGarbageCollector = false;
        mHitWall = false;
        mIsInvulnerable = false;
        mHighestPoint = (int)transform.position.y;
        mJumpedOnce = false;
        mRbody = GetComponentInParent<Rigidbody2D>();
        mJumpReady = true;
        mDirection = 1;
        mSpriteRenderer = GetComponent<SpriteRenderer>();

    }


    public void Jump()
    {
        if (mJumpReady)
        {
            mRbody.AddForce(new Vector2(1100 * mDirection , 950));
            mJumpReady = false;
            mDirection *= -1;
            mHitWall = false;
        }
    }

    public void pushAgainstTheWall()
    {
        if (mJumpedOnce)
        {
            // Daca playerul a sarit macar o data si se afla pe perete

            Vector2 direction = new Vector2(mDirection * -1, 0);
            mRbody.AddForce(direction * mStickiness);
        }
    }

    public bool isInAir()
    {
        if (!mJumpReady)
        {
            return true;
        }
        return false;
    }

    public bool IsInvulnerable()
    {
        return mIsInvulnerable;
    }

    public void MakeInvulnerable()
    {
        mIsInvulnerable = true;
        mSpriteRenderer.color = new Color(1f, 1f, 1f, .5f);
        StartCoroutine(VulnerabilityTimer(10));

    }
    public void MakeVulnerable()
    {
        mIsInvulnerable = false;
        mSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "PereteStangaBase" || collision.gameObject.tag == "Perete" || collision.gameObject.tag == "PereteDreaptaBase")
        {
            mJumpedOnce = true;
            mJumpReady = true;
            mHitWall = true;
            Debug.Log("Hit perete");

        }

        if (collision.gameObject.tag == "GarbageCollector")
        {
            mHitGarbageCollector = true;
            Debug.Log("Hit gb collector");

        }

    }

    public void onObstacleHit()
    {
            mHitObstacle = true;
            Debug.Log("Hit obstacle");
    }

    IEnumerator VulnerabilityTimer(float time)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        MakeVulnerable();
    }
}
