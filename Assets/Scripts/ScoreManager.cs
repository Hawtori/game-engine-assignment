using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager _instance;

    private int score;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }

    public void ChangeScore(int coinValue)
    {
        score += coinValue;
        Debug.Log("score: " + score);
    }
}
