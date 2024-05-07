using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public float fatalFallY = -15f;

    private void Start()
    {
        UpdateHealthDisplay();
    }

    private void Update()
    {
        CheckForFatalFall();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        UpdateHealthDisplay();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.PlayerDied();
        }

        Debug.Log("player died");

        Application.Quit();

        // for unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void UpdateHealthDisplay()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.UpdatePlayerHealthDisplay(health);
        }
    }

    private void CheckForFatalFall()
    {
        if (transform.position.y < fatalFallY)
        {
            Debug.Log("Fell off");
            Die();
        }
    }
}
