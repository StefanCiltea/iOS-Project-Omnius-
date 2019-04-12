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

    void Start()
    {
        mActionController = GetComponentInChildren<AgentMovementControll>();
        rBody = GetComponent<Rigidbody2D>();
    }

    public override void AgentReset()
    {
    
    }

    public override void CollectObservations()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("ClonaObstacol");
        int cnt = 0;
        foreach (GameObject obstacle in obstacles)
        {
            if (cnt > 2)
            {
                break;
            }
            cnt++;
            AddVectorObs(obstacle.transform.position);
        }

        GameObject perk = GameObject.FindGameObjectWithTag("Perk");
        AddVectorObs(perk.transform.position);
        AddVectorObs(this.transform.position);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        mActionController.pushAgainstTheWall();

        if (vectorAction[0] != 0 /*Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began*/)
        {
            mActionController.Jump();
        }

        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("ClonaObstacol");

        foreach (GameObject obstacle in obstacles)
        {
            if (Vector2.Distance(obstacle.transform.position, this.transform.position) < 1.42f)
            {
                SetReward(-1f);
                Done();
            }
        }

        GameObject[] perks = GameObject.FindGameObjectsWithTag("Perk");

        foreach (GameObject perk in perks)
        {
            if (Vector2.Distance(perk.transform.position, this.transform.position) < 1.42f)
            {
                SetReward(2f);
            }
        }

        if (mActionController.mHitWall)
        {
            SetReward(0.1f);
        }

        if (mActionController.mHitGarbageCollector)
        {
            SetReward(-1f);
            Done();
        }
    }

}
