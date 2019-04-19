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
                Debug.Log("Screen Height" + collision.transform.position);
                float screenWidthInCoords = mGameControllerScript.GetScreenWidth();

                float randomX = Random.Range(
                    this.gameObject.transform.position.x - screenWidthInCoords / 2 + mGameControllerScript.mWallBoundsSize.x * 2, 
                    this.gameObject.transform.position.x + screenWidthInCoords / 2 - mGameControllerScript.mWallBoundsSize.x * 2
                    );

                mGameControllerScript.SpawnMovingObstacle(
                    new Vector2(randomX, collision.transform.parent.position.y + mGameControllerScript.GetScreenHeight() * 2),
                    1
                    );
            }
           
            GameObject clone = Instantiate(this.gameObject, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + mGameControllerScript.GetScreenHeight()), this.gameObject.transform.rotation);
            print("Spawning obgen == " + mGameControllerScript.GetScreenHeight());
            clone.tag = "ObstacleGeneratorClone";
            clone.transform.parent = this.transform.parent;

            if (this.tag != "ObstacleGenerator")
            {
                Destroy(this.gameObject);
            }
        }
    }
}
