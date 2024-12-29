using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")] 
    [SerializeField] private int maxBirds = 3;
    [SerializeField] private GameObject birdPrefab;
    [SerializeField] private GameObject stagePrefab;
    
    [Header("타이틀 UI")]
    [SerializeField] private GameObject titleUI;
    [SerializeField] private TMP_Text birdText;
    [SerializeField] private TMP_Text scoreText;
    
    [Header("게임 종료 UI")]
    [SerializeField] private GameObject endUI;
    [SerializeField] private TMP_Text endText;
    
    [HideInInspector]
    public bool isLaunched;
    public bool IsGameActive { get; private set; }
    public int RemainingBirds { get; private set; }
    public int CurrentScore { get; private set; }
    public LineRenderer stringLine { get; private set; }

    private Transform spawnPoint;
    private Transform destructibleObjects;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        IsGameActive = true;
        RemainingBirds = maxBirds;
        CurrentScore = 0;
        isLaunched = false;
    }

    void StartGame()
    {
        InitializeValues();
        CreateStage();
        SetNextBird();
        Debug.Log("Game Initialized");
    }

    void InitializeValues()
    {
        IsGameActive = true;
        RemainingBirds = maxBirds;
        CurrentScore = 0;
        isLaunched = false;
        birdText.text = "InitBirds";
        scoreText.text = "Score : 0";
    }

    void CreateStage()
    {
        GameObject obj = Instantiate(stagePrefab);
        Stage stage = obj.GetComponent<Stage>();
        spawnPoint = stage.spawnPoint;
        stringLine = stage.stringLine;
        destructibleObjects = stage.destructibleObjects;
    }

    public void OnStartButtonClick()
    {
        titleUI.SetActive(false);
        StartGame();
    }

    public void OnAgainButtonClick()
    {
        endUI.SetActive(false);
        StartGame();
    }

    public void AddScore(int score)
    {
        CurrentScore += score;
        scoreText.text = $"Score : {CurrentScore}";
        Debug.Log($"Score updated: {CurrentScore}");
    }

    public void SetNextBird()
    {
        if (RemainingBirds <= 0)
        {
            GameOver();
            return;
        }

        isLaunched = false;
        RemainingBirds = Mathf.Max(RemainingBirds - 1, 0);
        GameObject bird = Instantiate(birdPrefab, spawnPoint.position, Quaternion.identity);

        birdText.text = $"Birds : {RemainingBirds}";
        Debug.Log($"Birds remaining: {RemainingBirds}");
    }

    void GameOver()
    {
        IsGameActive = false;
        endUI.SetActive(true);
        endText.text = "Game Over...";
        Debug.Log("Game Over");
    }

    void GameClear()
    {
        IsGameActive = false;
        endUI.SetActive(true);
        endText.text = "Game Clear!";
        Debug.Log("Game Clear");
    }

    public void CheckGameEnd()
    {
        if (RemainingBirds <= 0)
        {
            GameOver();
            return;
        }
        
        int count = destructibleObjects.childCount;
        if (count == 0)
        {
            // 게임 클리어!
            GameClear();
            return;
        }

        SetNextBird();
    }
}
