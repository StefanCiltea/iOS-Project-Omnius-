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
        if(mGameControllerScript.GetScreenHeight() != 0)
        {
            mNextScreenY = transform.parent.Find("PereteStanga").transform.position.y + mGameControllerScript.GetScreenHeight();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Spawner")
        {
            mGameControllerScript.SpawnWalls(this.gameObject.transform.parent.gameObject, mGameControllerScript.GetScreenHeight());   // Spawn pereti noi
            mGameControllerScript.SpawnMovingObstacle(new Vector2(mGameControllerScript.mCamera.transform.position.x, mNextScreenY),UnityEngine.Random.Range(1,3));
        }
    }
}
