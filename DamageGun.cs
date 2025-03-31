using System.Collections;
using UnityEngine;

public class DamageGun : MonoBehaviour
{
    public float damage;
    public float range;
    public Transform PlayerCamera;
    public GameObject hitEffectPrefab; 
    public LineRenderer lineRenderer; 
    public Transform muzzlePoint; 
    public float shootCooldown = 0.1f; 

    private bool canShoot = true; 

    void Start()
    {
        PlayerCamera = Camera.main.transform;
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false; 
        }
    }

    public void Shoot()
    {
      
        if (!canShoot) return;

        canShoot = false;

        Ray gunRay = new Ray(PlayerCamera.position, PlayerCamera.forward);

      
        if (Physics.Raycast(gunRay, out RaycastHit hitInfo, range))
        {
           
            if (hitInfo.collider.gameObject.TryGetComponent(out Entity enemy))
            {
                enemy.Health -= damage;
            }

            
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            }

           
            ShowMuzzleFlash(hitInfo.point);
        }
        else
        {
           
            ShowMuzzleFlash(gunRay.GetPoint(range));
        }

        StartCoroutine(ShootCooldown());
    }

    void ShowMuzzleFlash(Vector3 hitPoint)
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, muzzlePoint.position); 
            lineRenderer.SetPosition(1, hitPoint);

            StartCoroutine(DisableLineRenderer());
        }
    }

    IEnumerator DisableLineRenderer()
    {
        yield return new WaitForSeconds(0.1f);
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true; 
    }
}
