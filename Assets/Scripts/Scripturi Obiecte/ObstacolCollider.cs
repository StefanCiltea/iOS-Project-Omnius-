using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacolCollider : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            if (playerController.IsInvulnerable())
            {
                GameObject parent = this.transform.parent.gameObject;
                Destroy(parent);
                playerController.MakeVulnerable();
            } else
            {
                GameObject gameControllerOb = GameObject.FindGameObjectWithTag("GameController");
                gameControllerOb.GetComponent<GameController>().GameOver();
            }
        } else if (collision.gameObject.tag == "Agent")
        {
            AgentMovementControll agentMovement = collision.gameObject.GetComponent<AgentMovementControll>();
            if (agentMovement.IsInvulnerable())
            {
                GameObject parent = this.transform.parent.gameObject;
                Destroy(parent);
                agentMovement.MakeVulnerable();
            } else
            {
                agentMovement.onObstacleHit();
            }
        }
    }
}
