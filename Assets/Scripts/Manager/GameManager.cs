using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    public Image[] heartsIcons;
    public GameObject gameOverPanel;
    public GameObject mainMenuPanel;

    public TMP_Text bestScoreText;
    public Text gameBestScoreText;

    [Header("GameOverPanel")]
    public AudioSource gameOverSound;
    public TMP_Text endPoints;
    public TMP_Text newPersonalText;
    public TMP_Text bestPersonalText;

    [Header("Settings")]
    public int bombDestroyed = 0;
    private int _remainingLife = 2;
    public bool gameOver = true;
    private void Awake()
    {
        instance = this;
        gameOverPanel.SetActive(false);
  
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    
        bestScoreText.text = PointsManager.instance.GetBestPersonalPoints().ToString();
        gameBestScoreText.text = PointsManager.instance.GetBestPersonalPoints().ToString();
    }
    private void Update()
    {
       
        if (gameOver)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                RestartGame();
            }
        }
    }
    public void StartGame()
    {
        gameOver = false;
        SpawnerManager.instance.StartSpawn();
    }
    public void LoseLife()
    {
        if (gameOver) return;

        heartsIcons[_remainingLife].gameObject.SetActive(false);
        _remainingLife--;

        if (_remainingLife < 0)
            TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        gameOver = true;
        SpawnerManager.instance.Stop();
        StartCoroutine(GameOverDelay());

    }
    private IEnumerator GameOverDelay()
    {
        yield return new WaitForSeconds(1f);
        gameOverSound.Play();
        gameOverPanel.SetActive(true);
        endPoints.text = PointsManager.instance.GetPoints().ToString();

        int currentPoints = PointsManager.instance.GetPoints();
        int bestPoints = PointsManager.instance.GetBestPersonalPoints();
        bestPersonalText.text = $"Personal Best: {bestPoints}";
        bestPersonalText.gameObject.SetActive(true);
        newPersonalText.gameObject.SetActive(false);

        if (currentPoints > bestPoints)
        {
            PointsManager.instance.UpdateBestPoints();
            bestPersonalText.gameObject.SetActive(false);
            newPersonalText.gameObject.SetActive(true);
        }

        bestScoreText.text = PointsManager.instance.GetBestPersonalPoints().ToString();
        gameBestScoreText.text = PointsManager.instance.GetBestPersonalPoints().ToString();
    }
    public void RestartGame()
    {
        PointsManager.instance.ResetPoints();
        bombDestroyed = 0;
        _remainingLife = 2;
        foreach (Image heartIcon in heartsIcons)
        {
            heartIcon.gameObject.SetActive(true);
        }
        mainMenuPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }
}