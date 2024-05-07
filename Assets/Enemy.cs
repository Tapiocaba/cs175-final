using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f;
    public float uprightSpeed = 1f;
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(20);  // Assuming the player has a method to take damage
        }
    }
}
