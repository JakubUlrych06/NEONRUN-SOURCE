using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class playerCam : MonoBehaviour
{
    public KeyCode grapgun = KeyCode.Mouse2;
    public Transform gunObj;
    public Transform predict;
    public Transform dmgGun;

    public DamageGun dmgGunn;
    public gun gunScrt;

    public Grappling grapple;
    public Swinging swinging;

    public float sensX;
    public float sensY;

    public Transform orientation;
    float yRotation;
    float xRotation;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(grapgun)) gunswap();
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion. Euler (xRotation, yRotation, 0);
        orientation.rotation = Quaternion. Euler(0, yRotation, 0);
    }

 /*public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }*/
private void gunswap()
{
    if (grapple.enabled == true)
    {
        // Call the stop functions before disabling the scripts
        swinging.StopSwing();
        grapple.StopGrapple();

        gunObj.gameObject.SetActive(false);
        predict.gameObject.SetActive(false);
        swinging.enabled = false;
        grapple.enabled = false;
        dmgGunn.enabled = true;
        gunScrt.enabled = true;
        dmgGun.gameObject.SetActive(true);
    }
    else
    {
        gunObj.gameObject.SetActive(true);
        predict.gameObject.SetActive(true);
        swinging.enabled = true;
        grapple.enabled = true;
        dmgGunn.enabled = false;
        gunScrt.enabled = false;
        dmgGun.gameObject.SetActive(false);
    }
}


}
