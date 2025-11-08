using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    int score = 0;

    public static Action<int> OnScoreUpdate;

    public static GameManager instance;

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
        score += amount;
        OnScoreUpdate?.Invoke(score);
    }
}
