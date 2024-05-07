using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI timeDisplay;
    public TextMeshProUGUI playerHpDisplay;  // Display for player health

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        if (isPlayerAlive)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimeDisplay();
        }
    }

    private void UpdateTimeDisplay()
    {
        int minutes = (int)elapsedTime / 60;
        int seconds = (int)elapsedTime % 60;
        timeDisplay.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    private void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void UpdatePlayerHealthDisplay(float health)
    {
        playerHpDisplay.text = "HP: " + health.ToString();
    }

    public void PlayerDied()
    {
        isPlayerAlive = false;
        UpdatePlayerHealthDisplay(0); // Show 0 on death
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreDisplay.text = "Score: " + score;
    }
}
