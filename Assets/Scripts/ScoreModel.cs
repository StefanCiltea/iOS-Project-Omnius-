using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScoreModel
{

	private int mScore;
	private string mDate;

	public string getDate() 
	{
		return mDate;
	}
	public void setDate(string newDate) 
	{
		this.mDate = newDate;
	}

	public int getScore() 
	{
		return mScore;
	}
	public void setScore(int newScore) 
	{
		this.mScore = newScore;
	}
	public ScoreModel(int score,string date) 
	{
		this.mScore = score;
		this.mDate = date;
	}
	public ScoreModel() {}

}
