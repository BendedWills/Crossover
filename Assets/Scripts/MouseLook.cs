using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 100;
    public float maxPitch = 90;
    public float minPitch = -90;
    public Transform playerBody;
    
    private float xRot = 0;
    private bool enabled = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!enabled)
            return;
        
        float mouseX = 0 * sensitivity * Time.deltaTime;
        float mouseY = 0 * sensitivity * Time.deltaTime;
        
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void SetEnabled(bool enabled)
    {
        if (enabled)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
        
        this.enabled = enabled;
    }
}
