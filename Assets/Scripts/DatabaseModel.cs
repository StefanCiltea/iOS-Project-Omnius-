using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

public class DatabaseModel
{
    private static DatabaseModel instance;
    private XmlDocument mScoreHistoryDB;
    private XmlNodeList xmlElements;

    private DatabaseModel()
    {
        this._openConnection();
    }

    private void _openConnection()
    {
        mScoreHistoryDB = new XmlDocument();
        mScoreHistoryDB.Load("Assets/Scripts/XML/ScoreHistory.xml");    // Get the xml file content
        xmlElements = mScoreHistoryDB.GetElementsByTagName("value");
    }

    public static DatabaseModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DatabaseModel();
            }
          
            instance._openConnection();
            return instance;
            

        }
    }

    public void AddHighestScore(ScoreModel newScore)
    {

        XmlElement xmlbase = mScoreHistoryDB.DocumentElement;
        XmlElement scoreElement = mScoreHistoryDB.CreateElement("score");
        XmlElement scoreValue = mScoreHistoryDB.CreateElement("value");
        scoreValue.InnerText = newScore.getScore().ToString();
        XmlElement scoreDate = mScoreHistoryDB.CreateElement("date");
        scoreDate.InnerText = newScore.getDate();
        scoreElement.AppendChild(scoreValue);
        scoreElement.AppendChild(scoreDate);
        xmlbase.AppendChild(scoreElement);
        mScoreHistoryDB.Save("Assets/Scripts/XML/ScoreHistory.xml");
    }

    public ScoreModel GetMaxScore()
    {
        int maxim = Int32.Parse(xmlElements[xmlElements.Count - 1].InnerText);
        return new ScoreModel(maxim, "dontcare");
    }

    public void updateMaxScore(int score)
    {

    }
}
