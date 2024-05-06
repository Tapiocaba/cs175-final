using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    // reloading
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

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
        bulletsLeft = magazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        CheckShooting();

        // update ammo count
        if (AmmoManager.Instance.ammoDisplay != null)
        {
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft / bulletsPerBurst} / {magazineSize / bulletsPerBurst}";
        }
    }

    private void UpdateStatusMessage(string message)
    {
        if (AmmoManager.Instance.statusDisplay != null)
        {
            AmmoManager.Instance.statusDisplay.text = message;
        }
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

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading)
        {
            Reload();
            UpdateStatusMessage("Reloading...");
        }

        if (bulletsLeft <= 0 && !isReloading)
        {
            UpdateStatusMessage("Reload gun!");
            return;
        }

        // Only allow shooting if there are bullets left, the weapon is ready to shoot, and not currently reloading.
        if (isShooting && readyToShoot && !isReloading && bulletsLeft > 0)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }

    }

    private void FireWeapon()
    {
        Debug.Log("Firing Weapon"); // Debug statement
        bulletsLeft--;
        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirAndSpread().normalized;
        Debug.Log($"Shooting Direction: {shootingDirection}"); // Debug direction

        // Instantiate the bullet at the bulletSpawn position with the camera's rotation
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, playerCamera.transform.rotation);

        // Set bullet direction - it's important this is correct
        bullet.transform.forward = shootingDirection;

        // Apply force
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
        bulletRigidbody.AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

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


    private void Reload()
    {
        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
        UpdateStatusMessage("Reloading...");
    }

    private void ReloadCompleted()
    {
        bulletsLeft = magazineSize; // reload complete, now we have a full magazine
        isReloading = false;
        UpdateStatusMessage("");
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
