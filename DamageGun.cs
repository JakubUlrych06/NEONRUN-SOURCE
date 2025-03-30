using System.Collections;
using UnityEngine;

public class DamageGun : MonoBehaviour
{
    public float damage;
    public float range;
    public Transform PlayerCamera;
    public GameObject hitEffectPrefab; // Reference to the particle system prefab
    public LineRenderer lineRenderer; // Reference to the LineRenderer
    public Transform muzzlePoint; // The point at the tip of the gun barrel
    public float shootCooldown = 0.1f; // Cooldown between shots to prevent multiple shots in one frame

    private bool canShoot = true; // Flag to prevent multiple shots in one frame

    void Start()
    {
        PlayerCamera = Camera.main.transform;
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; // Ensure LineRenderer is initially disabled
        }
    }

    public void Shoot()
    {
        // Prevent shooting if the cooldown is not finished
        if (!canShoot) return;

        // Set canShoot to false to block multiple shots in one frame
        canShoot = false;

        Ray gunRay = new Ray(PlayerCamera.position, PlayerCamera.forward);

        // Raycast to detect the hit point
        if (Physics.Raycast(gunRay, out RaycastHit hitInfo, range))
        {
            // Check if the hit object is an enemy
            if (hitInfo.collider.gameObject.TryGetComponent(out Entity enemy))
            {
                enemy.Health -= damage;
            }

            // Instantiate the particle effect at the hit point
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            }

            // Trigger Line Renderer flash to the hit point
            ShowMuzzleFlash(hitInfo.point);
        }
        else
        {
            // If no hit, just show the line for the maximum range
            ShowMuzzleFlash(gunRay.GetPoint(range));
        }

        // Start cooldown after the shot
        StartCoroutine(ShootCooldown());
    }

    // Function to handle the LineRenderer flash
    void ShowMuzzleFlash(Vector3 hitPoint)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, muzzlePoint.position); // Start from the gun's muzzle
            lineRenderer.SetPosition(1, hitPoint); // End at the hit point

            // Disable the line renderer after a short time (e.g., 0.1 seconds)
            StartCoroutine(DisableLineRenderer());
        }
    }

    // Coroutine to disable the LineRenderer after the flash
    IEnumerator DisableLineRenderer()
    {
        yield return new WaitForSeconds(0.1f);
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    // Coroutine to handle the cooldown period between shots
    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true; // Allow shooting again after the cooldown
    }
}
