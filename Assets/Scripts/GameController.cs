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
    public Vector3 mWallBoundsSize;
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
        if(mPlayButton)
        {
            mPlayButton.onClick.AddListener(RestartGame);
        }
        mGameIsPaused = false;

        if (mScoreText)
        {
            mScoreText.text = "Score: " + 0;

        }
        mGameIsOver = false;
        if (mGameOverPanel)
        {
            mGameOverPanel.SetActive(false);
        }
        if(mPausePanel)
        {
            mPausePanel.SetActive(false);
        }
        // Se calculeaza dimensiunile ecranului
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 bottomLeftCorner = new Vector2(0, 0);

        Vector2 edgeVectorTopRight = Camera.main.ViewportToWorldPoint(topRightCorner);  // Contine coordonatele coltului dreapta-sus al ecranului in starea initiala
        Vector2 edgeVectorBottomLeft = Camera.main.ViewportToWorldPoint(bottomLeftCorner);  // Contine coordonatele coltului dreapta-sus al ecranului in starea initiala

        mScreenHeight = edgeVectorTopRight.y - edgeVectorBottomLeft.y;
        mScreenWidth = edgeVectorTopRight.x - edgeVectorBottomLeft.x;

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
    public float GetScreenWidth()
    {
        return mScreenWidth;
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
            float screenWidthInCoords = this.GetScreenWidth();
            print("walls" + mWallBoundsSize.x);
            float randomX = UnityEngine.Random.Range(
                this.gameObject.transform.position.x - screenWidthInCoords / 2 + mWallBoundsSize.x,
                this.gameObject.transform.position.x + screenWidthInCoords / 2 - mWallBoundsSize.x
                );

            GameObject clone = Instantiate(mPerkClone, new Vector3(randomX, mCamera.transform.position.y * 2 + 10, mPerkClone.transform.position.z), mPerkClone.transform.rotation);
            clone.tag = "PerkClone";
            clone.transform.parent = this.transform.parent;
        }
    }
}
