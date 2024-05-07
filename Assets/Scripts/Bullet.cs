using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    public float impactForce = 5f;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            print("hit " + collision.gameObject.name + "!");
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            print("hit wall");
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                print("hit enemy");
            }
            if (enemyRb != null)
            {
                Vector3 forceDirection = (collision.transform.position - transform.position).normalized;
                enemyRb.AddForce(forceDirection * impactForce, ForceMode.Impulse); // Control impactForce to be realistic
            }
            else
            {
                Debug.LogError("Enemy component not found on " + collision.gameObject.name);
            }
        }
        Destroy(gameObject);
    }
}

