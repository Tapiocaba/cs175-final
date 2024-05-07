using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f;
    public float uprightSpeed = 1f;
    public Transform playerTransform;
    public float moveSpeed = 2f;

    private Quaternion initialRotation;
    private bool shouldUpright = false;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        ScoreManager.Instance.AddScore(10);
        Destroy(gameObject);
    }

    void Update()
    {
        if (shouldUpright)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, Time.deltaTime * uprightSpeed);
            if (Quaternion.Angle(transform.rotation, initialRotation) < 1.0f)
            {
                transform.rotation = initialRotation;
                shouldUpright = false;
            }
        }

        // Movement towards the player
        if (playerTransform != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            print("collided with player");
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(20);
            }
            else
            {
                Debug.LogError("PlayerHealth not found");
            }
        }
    }
}
