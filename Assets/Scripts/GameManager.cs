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

    private void OnEnable()
    {
        DrainAndRespawn.OnTriesChanged += OnBallFell;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        DrainAndRespawn.OnTriesChanged -= OnBallFell;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        HandGameStart();
    }

    void HandGameStart()
    {
        ResetScore();

        OnGameStart?.Invoke();
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
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        HandGameStart();
    }
}
