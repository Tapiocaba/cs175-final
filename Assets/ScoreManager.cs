using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI timeDisplay;
    private int score = 0;
    private float elapsedTime = 0;
    private bool isPlayerAlive = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isPlayerAlive)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimeDisplay();
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreDisplay.text = "Score: " + score;
    }

    private void UpdateTimeDisplay()
    {
        int minutes = (int)elapsedTime / 60;
        int seconds = (int)elapsedTime % 60;
        timeDisplay.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    public void PlayerDied()
    {
        isPlayerAlive = false;
    }
}
