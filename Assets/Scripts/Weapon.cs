using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour
{
    [System.Serializable]
    public struct GunProperties
    {
        public int magazineSize;
        public float reloadTime;
        public float shootingDelay;
        public float bulletVelocity;
        public float bulletPrefabLifetime;
        public float spreadIntensity;
        public int bulletsPerBurst;
    }

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public Camera playerCamera;

    public GameObject singleShotGun;
    public GameObject automaticGun;
    public GameObject spreadGun;

    public GunProperties singleShotProperties;
    public GunProperties automaticProperties;
    public GunProperties spreadProperties;

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
    public GameObject muzzleEffect;

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
        // define gun properties
        singleShotProperties = new GunProperties
        {
            magazineSize = 10,
            reloadTime = 1.5f,
            shootingDelay = 0.75f,
            bulletVelocity = 50,
            bulletPrefabLifetime = 3,
            spreadIntensity = 0.05f,
            bulletsPerBurst = 1
        };

        automaticProperties = new GunProperties
        {
            magazineSize = 30,
            reloadTime = 2.0f,
            shootingDelay = 0.1f,
            bulletVelocity = 45,
            bulletPrefabLifetime = 3,
            spreadIntensity = 0.1f,
            bulletsPerBurst = 1
        };

        spreadProperties = new GunProperties
        {
            magazineSize = 5,
            reloadTime = 2.5f,
            shootingDelay = 1.0f,
            bulletVelocity = 40,
            bulletPrefabLifetime = 3,
            spreadIntensity = 0.5f,
            bulletsPerBurst = 5
        };

        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        bulletsLeft = magazineSize;
        ActivateGun(singleShotGun, singleShotProperties);
        currentShootingMode = ShootingMode.Single;
    }

    // Update is called once per frame
    void Update()
    {
        CheckShooting();
        CheckWeaponSwitch();

        // update ammo count
        if (AmmoManager.Instance.ammoDisplay != null)
        {
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft / bulletsPerBurst} / {magazineSize / bulletsPerBurst}";
        }
    }

    private void CheckWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // single shot
        {
            ActivateGun(singleShotGun, singleShotProperties);
            currentShootingMode = ShootingMode.Single;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // auto
        {
            ActivateGun(automaticGun, automaticProperties);
            currentShootingMode = ShootingMode.Auto;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // spread
        {
            ActivateGun(spreadGun, spreadProperties);
            currentShootingMode = ShootingMode.Burst;
        }
    }


    private void ActivateGun(GameObject gunToActivate, GunProperties properties)
    {
        // Deactivate all guns
        singleShotGun.SetActive(false);
        automaticGun.SetActive(false);
        spreadGun.SetActive(false);

        // Activate the selected gun
        gunToActivate.SetActive(true);

        // Set the weapon properties
        magazineSize = properties.magazineSize;
        reloadTime = properties.reloadTime;
        shootingDelay = properties.shootingDelay;
        bulletVelocity = properties.bulletVelocity;
        bulletPrefabLifetime = properties.bulletPrefabLifetime;
        spreadIntensity = properties.spreadIntensity;
        bulletsPerBurst = properties.bulletsPerBurst;

        // Initialize ammo to full magazine
        bulletsLeft = properties.magazineSize;

        // Update UI for ammo, if needed
        UpdateAmmoDisplay();
    }

    private void UpdateAmmoDisplay()
    {
        if (AmmoManager.Instance.ammoDisplay != null)
        {
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft} / {magazineSize}";
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
        muzzleEffect.GetComponent<ParticleSystem>().Play();
        if (SoundManager.Instance == null || SoundManager.Instance.gunSound == null)
        {
            Debug.LogError("sound error");
            return;
        }

        SoundManager.Instance.gunSound.Play();
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
