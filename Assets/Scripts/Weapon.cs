using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public Camera playerCamera;

    // shooting
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    // burst
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    // spread
    public float spreadIntensity;

    // bullets
    public float bulletVelocity = 30;
    public float bulletPrefabLifetime = 3f; // destroys after 3 sec

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
    }

    // Update is called once per frame
    void Update()
    {
        CheckShooting();
    }

    private void CheckShooting()
    {
        if (currentShootingMode == ShootingMode.Auto)
        {
            // Holding down
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            // Clicking once
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (isShooting && readyToShoot)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirAndSpread().normalized;

        // Instantiate the bullet at the bulletSpawn position with the camera's rotation
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, playerCamera.transform.rotation);

        // points bullet to shooting dir
        bullet.transform.forward = shootingDirection;

        // shoot
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * bulletVelocity, ForceMode.Impulse);

        // destroy bullet
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifetime));

        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay / bulletsPerBurst);
        }
        else
        {
            if (allowReset)
            {
                Invoke("ResetShot", shootingDelay);
                allowReset = false;
            }
        }
    }

    public Vector3 CalculateDirAndSpread()
    {
        // Check middle of screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            // Shoot at air
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
