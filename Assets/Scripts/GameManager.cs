using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int score = 0;

    public static Action<int> OnScoreUpdate;

    public static GameManager instance;
    private float currentMultiplier = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this);
    }

    public void AddScore(int amount)
    {
        int scoreToAdd = (int)(amount * currentMultiplier);
        score += scoreToAdd;
        OnScoreUpdate?.Invoke(score);
    }

    public void SetMultiplier(float multiplier)
    {
        // Logic to set score multiplier
        currentMultiplier = multiplier;
        Debug.Log("Multiplier set to: " + currentMultiplier + "x");
    }

    public void ResetScore()
    {
        score = 0;
        OnScoreUpdate?.Invoke(score);
        Debug.Log("Score reset to zero.");
    }
}
