using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;

    private void Start()
    {
        UpdateHealthDisplay();
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
        ScoreManager.Instance.PlayerDied();
    }

    private void UpdateHealthDisplay()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.UpdatePlayerHealthDisplay(health);  // Update specific to player health
        }
    }
}
