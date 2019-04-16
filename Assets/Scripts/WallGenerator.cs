using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour {
    public GameObject mGameController;
    private float mNextScreenY;
    private GameController mGameControllerScript;
    private void Start()
    {
        mGameControllerScript = mGameController.GetComponent<GameController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Spawner")
        {
            float rand = Random.Range(0f, 1f);
            Debug.Log("rand = " + rand);
            if (rand <= 0.6f)
            {
                mGameControllerScript.SpawnMovingObstacle(new Vector2(mGameController.transform.parent.position.x, this.gameObject.transform.position.y + mGameControllerScript.GetScreenHeight()), 1);
            }
            GameObject clone = Instantiate(this.gameObject, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + mGameControllerScript.GetScreenHeight()), this.gameObject.transform.rotation);

            clone.tag = "ObstacleGeneratorClone";
            clone.transform.parent = this.transform.parent;

            if (this.tag != "ObstacleGenerator")
            {
                Destroy(this.gameObject);
            }
        }
    }
}
