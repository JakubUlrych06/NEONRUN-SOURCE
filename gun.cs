using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class gun : MonoBehaviour
{
    public Transform gunObj;
    public UnityEvent OnGunShoot;
    public float cooldown;
    public bool automatic;

    public Vector3 recoilPositionOffset = new Vector3(0f, -0.1f, 0.2f);
    public Vector3 recoilRotationOffset = new Vector3(-5f, 0f, 0f);
    public float recoilDuration = 0.1f;

    private float curCooldown;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        curCooldown = cooldown;
        originalPosition = gunObj.localPosition;
        originalRotation = gunObj.localRotation;
    }

    void Update()
    {
        if (automatic)
        {
            if (Input.GetMouseButton(0) && curCooldown <= 0f)
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && curCooldown <= 0f)
            {
                Shoot();
            }
        }

        curCooldown -= Time.deltaTime;
    }

    void Shoot()
    {
        OnGunShoot?.Invoke();
        curCooldown = cooldown;
        StartCoroutine(RecoilAnimation());
    }

    IEnumerator RecoilAnimation()
    {
        // Animate recoil
        gunObj.localPosition = originalPosition + recoilPositionOffset;
        gunObj.localRotation = Quaternion.Euler(originalRotation.eulerAngles + recoilRotationOffset);

        yield return new WaitForSeconds(recoilDuration);

        // Return to original position smoothly
        float elapsedTime = 0f;
        while (elapsedTime < recoilDuration)
        {
            gunObj.localPosition = Vector3.Lerp(gunObj.localPosition, originalPosition, elapsedTime / recoilDuration);
            gunObj.localRotation = Quaternion.Lerp(gunObj.localRotation, originalRotation, elapsedTime / recoilDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gunObj.localPosition = originalPosition;
        gunObj.localRotation = originalRotation;
    }
}