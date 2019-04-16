using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerator : MonoBehaviour {
    private float mNextScreenY;
    public GameController mGameControllerScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Spawner")
        {
            float rand = Random.Range(0f, 1f);

            //Debug.Log("Local position of this: " + this.transform.localPosition);

            if (rand <= 0.6f)
            {
                Debug.Log("Screen Height" + collision.transform.localPosition);

                mGameControllerScript.SpawnMovingObstacle(new Vector2(this.transform.localPosition.x, collision.transform.parent.localPosition.y + mGameControllerScript.GetScreenHeight()), 1);
            }

            GameObject clone = Instantiate(this.gameObject, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.localPosition.y + mGameControllerScript.GetScreenHeight()), this.gameObject.transform.rotation);

            clone.tag = "ObstacleGeneratorClone";
            clone.transform.parent = this.transform.parent;

            if (this.tag != "ObstacleGenerator")
            {
                Destroy(this.gameObject);
            }
        }
    }
}
