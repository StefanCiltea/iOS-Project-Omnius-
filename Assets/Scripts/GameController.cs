using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System;
using System.Xml;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private struct Axis
    {
        public const int Horizontal = 1;
        public const int Vertical = 2;
    };

    public GameObject mPausePanel;
    public GameObject mGameOverPanel;
    public GameObject mCamera;
    public bool mGameIsOver;
    public Text mScoreText;
    public Button mPlayButton;
    public Text mHighestScoreText;
    public Text mLastScoreText;
    public GameObject mPerkClone;


    private int mScore;
    private float mScreenHeight;
    private float mScreenWidth;
    private bool mGameIsPaused;
    private ScoreModel mHighestScore;
    private XmlDocument mScoreHistoryDB;
    private Vector3 mWallBoundsSize;
    public GameObject mCloneObstacle;
    private Vector2[] mDirections = {
        new Vector2(1, 0),
        new Vector2(-1, 0),
        new Vector2(0, 1),
        new Vector2(0, -1),
        new Vector2(1, 1),
        new Vector2(-1, -1),
        new Vector2(1, -1),
        new Vector2(-1, 1)
    };

    private float mThirdPercentHeight;
    private float mThirdPercentWidth;
    private float mOffsetFromWalls;
    private float mScreenLeftMarginX;
    private float mScreenRightMarginX;


    private void Awake()
    {
        StartCoroutine(SpawnPerkAfterTime(15));
        // Se updateaza scorul maxim
        mHighestScore = DatabaseModel.Instance.GetMaxScore();


        // Configuratie initiala
        mPlayButton.onClick.AddListener(RestartGame);
        mGameIsPaused = false;
        mScoreText.text = "Score: " + 0;
        mGameIsOver = false;
        mGameOverPanel.SetActive(false);
        mPausePanel.SetActive(false);
        // Se calculeaza dimensiunile ecranului
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);  // Contine coordonatele coltului dreapta-sus al ecranului in starea initiala
        mScreenHeight = edgeVector.y * 2;
        mScreenWidth = edgeVector.x * 2;
        // Se calculeaza marginile impreuna cu alte dimensiuni pentru calcule ulterioare
        mWallBoundsSize = GameObject.FindGameObjectWithTag("PereteDreaptaBase").GetComponent<BoxCollider2D>().bounds.size;  // .x = width , .y = height, .z = depth of the gameobject
        mThirdPercentHeight = 0.33f * mScreenHeight;
        mThirdPercentWidth = 0.33f * (mScreenWidth - 2 * mWallBoundsSize.x);
        mOffsetFromWalls = mCloneObstacle.transform.Find("HorizontalColider").GetComponent<BoxCollider2D>().bounds.size.x / 2f + 0.3f; //!!!!!!!
        mScreenLeftMarginX = (mCamera.transform.position.x - (mScreenWidth - 2 * mWallBoundsSize.x) / 2f);
        mScreenRightMarginX = (mCamera.transform.position.x + (mScreenWidth - 2 * mWallBoundsSize.x) / 2f);

    }

    public void SpawnMovingObstacle(Vector3 atOrigin, int numberOfObjects)
    {
        float speedRand = UnityEngine.Random.Range(1, 2);
        AbstractObiectController.ComputeTargetPoint(atOrigin, Vector2.up, 0.5f);
        GameObject newObstacle = Instantiate(mCloneObstacle, atOrigin, mCloneObstacle.transform.rotation);
        newObstacle.GetComponent<ObstacolController>().SetAndComputeProperties(Vector2.up, newObstacle.transform.position, speedRand, 2);
        newObstacle.gameObject.tag = "ClonaObstacol";
        newObstacle.transform.parent = this.transform.parent;
    }

    public void UpdateScoreView()
    {
        mScore += 1;
        mScoreText.text = "Score: " + mScore;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("gameplay", LoadSceneMode.Single);
    }

    public void GameOver()
    {
        mHighestScoreText.text = "Highscore: " + mHighestScore.getScore();
        mLastScoreText.text = "Score: " + mScore;

        mGameIsOver = true;
        PauseGame();
        mGameOverPanel.SetActive(true);

        if (mHighestScore.getScore() < mScore)
        {
            mHighestScore = new ScoreModel(mScore, DateTime.Now.ToString("MM/dd/yyyy"));
            // Insert highest score to database
            DatabaseModel.Instance.AddHighestScore(mHighestScore);
        }
    }
    public bool GameIsPaused()
    {
        return mGameIsPaused;
    }
    public float GetScreenHeight()
    {
        return mScreenHeight;
    }
    public void PauseGame()
    {
        if(mGameIsOver == false)
        {
            mPausePanel.SetActive(true);
        }
        mGameIsPaused = true;
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        mGameIsPaused = false;
        mPausePanel.SetActive(false);
    }
    public void ToMainMenu()
    {
        // todo implement this
    }
    IEnumerator SpawnPerkAfterTime(float time)
    {

        float modifier;
        while (true)
        {
            modifier = UnityEngine.Random.RandomRange(-5, 5);

            yield return new WaitForSeconds(time + modifier);

            Transform lookInObject = transform.parent ? transform.parent : transform;   // If every

            foreach (Transform child in lookInObject)
            {
                if(child.gameObject.tag == "PerkClone")
                {
                    Destroy(child.gameObject);
                }
            }

            GameObject clone = Instantiate(mPerkClone, new Vector3(mCamera.transform.position.x + UnityEngine.Random.Range(0, 1), mCamera.transform.position.y + 10, mPerkClone.transform.position.z), mPerkClone.transform.rotation);
            clone.tag = "PerkClone";
            clone.transform.parent = this.transform.parent;
        }
    }
}
