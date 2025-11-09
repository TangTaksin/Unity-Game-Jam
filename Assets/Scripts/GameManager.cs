using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{

    int score = 0;

    public static Action OnGameStart;
    public static Action OnGameEnd;

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

    private void OnEnable()
    {
        DrainAndRespawn.OnTriesChanged += OnBallFell;
    }

    private void OnDisable()
    {
        DrainAndRespawn.OnTriesChanged -= OnBallFell;
    }

    private void Start()
    {
        HandGameStart();
    }

    void HandGameStart()
    {
        score = 0;
        OnScoreUpdate?.Invoke(score);

        OnGameStart?.Invoke();
    }

    public void AddScore(int amount)
    {
        score += amount;
        OnScoreUpdate?.Invoke(score);
    }

    public int Getscore()
    {
        return score;
    }

    void OnBallFell(int tries_count)
    {
        if (tries_count <= 0)
            HandleGameOver();
    }

    void HandleGameOver()
    {
        OnGameEnd?.Invoke();
    }

    public void BeginNewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        HandGameStart();
    }
}
