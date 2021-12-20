using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    // The minimum distance the player has to be from the button
    // to activate it.
    public float minDistance = 1.0f;
    public Transform playerTransform;
    public AudioSource activateMusic;
    public Destructable glass;
    public PlayerMovement playerMovement;
    public GameObject timeMachine;
    public GameObject timeMachineDestroyed;
    public GameObject playerCamera;
    public GameObject timeMachineFrame;
    public GameObject blackHole;
    public Image blackFadePanel;
    public float explodeTimeAfterPress = 10.1f;
    public float distanceToEnd = 1f;
    public float rotationSpeed = 0.1f;
    public float movementSpeed = 0.1f;
    public float frameRotationSpeed = 0.1f;

    private bool pressed = false;
    private float timeSincePressed = 0.0f;
    private bool exploded = false;
    private bool ended = false;
    private bool loadedNext = false;
    private float timeMachineFrameVelocity = 0f;
    private float playerVelocity = 0f;
    
    // Update is called once per frame
    void Update()
    {
        if (false && !pressed)
        {
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            if (distance <= minDistance)
            {
                activateMusic.Play();
                pressed = true;
            }
        }

        if (pressed)
            timeSincePressed += Time.deltaTime;
        
        if (!exploded && timeSincePressed >= explodeTimeAfterPress)
        {
            Explode();
            
            CameraShake cameraShake = playerCamera.GetComponent<CameraShake>();
            if (cameraShake != null)
                cameraShake.Shake(0.5f, 0.1f);
            else
            {
                Debug.LogError("Camera must have a CameraShake script component!");
                return;
            }

            exploded = true;
        }

        if (exploded && !ended)
        {
            RotateFrame();
            GravitateOrSomething();

            Vector3 blackHolePos = blackHole.transform.position;

            float distance = Vector3.Distance(playerCamera.transform.position, blackHolePos);
            float distanceToEndZone = distance - distanceToEnd;
            float color = 1 - distanceToEndZone;
            blackFadePanel.color = new Color(0, 0, 0, color);

            if (distance <= distanceToEnd)
                ended = true;
        }
        
        if (exploded && ended && !loadedNext && !activateMusic.isPlaying)
        {
            FindObjectOfType<GameManager>().LoadNextLevel();
            loadedNext = true;
        }
    }

    void Explode()
    {
        // Damage glass
        glass.Destroy();

        MouseLook cameraLook = playerCamera.GetComponent<MouseLook>();
        if (cameraLook == null)
        {
            Debug.LogError("Camera must have a MouseLook script component!");
            return;
        }

        cameraLook.SetEnabled(false);
        playerMovement.SetEnabled(false);

        timeMachine.SetActive(false);
        timeMachineDestroyed.SetActive(true);
        blackHole.SetActive(true);
    }

    private void GravitateOrSomething()
    {
        playerVelocity += movementSpeed * Time.deltaTime;

        Vector3 cameraPos = playerCamera.transform.position;
        Vector3 cameraRotation = playerCamera.transform.rotation.eulerAngles;
        Vector3 blackHolePos = blackHole.transform.position;

        // Correct shit
        if (cameraRotation.x > 180f)
            cameraRotation.x = -(180f - (cameraRotation.x - 180f));
        if (cameraRotation.y > 180f)
            cameraRotation.y = -(180f - (cameraRotation.y - 180f));

        float yawOffset = RadToDeg(GetLookAtYaw(blackHolePos)) - cameraRotation.y;
        float pitchOffset = RadToDeg(GetLookAtPitch(blackHolePos)) - cameraRotation.x;

        yawOffset *= rotationSpeed;
        yawOffset *= Time.deltaTime;
        pitchOffset *= rotationSpeed;
        pitchOffset *= Time.deltaTime;
            
        playerCamera.transform.Rotate(new Vector3(pitchOffset, yawOffset, 0f));

        Vector3 offset = new Vector3();
        offset.x = blackHolePos.x - cameraPos.x;
        offset.y = blackHolePos.y - cameraPos.y;
        offset.z = blackHolePos.z - cameraPos.z;

        float biggest = offset.x;
        if (offset.y > biggest)
            biggest = offset.y;
        if (offset.z > biggest)
            biggest = offset.z;
            
        offset /= biggest;
        offset *= playerVelocity;
        
        playerCamera.transform.position = cameraPos + (offset * Time.deltaTime);
    }

    private void RotateFrame()
    {
        timeMachineFrameVelocity += frameRotationSpeed * Time.deltaTime;
        timeMachineFrame.transform.Rotate(new Vector3(timeMachineFrameVelocity * Time.deltaTime, 0, 0));
    }

    private float GetLookAtYaw(Vector3 lookAtPos)
    {
        Vector3 cameraPos = playerCamera.transform.position;
        if (lookAtPos.x == cameraPos.x && lookAtPos.y == cameraPos.y && lookAtPos.z == cameraPos.z)
            return 0f;
        
        Vector3 offset = new Vector3();
        offset.x = cameraPos.x - lookAtPos.x;
        offset.y = cameraPos.y - lookAtPos.y;
        offset.z = cameraPos.z - lookAtPos.z;
        
        float yaw = 0f;
        if (offset.x != 0f)
            yaw = (float)Math.Atan(offset.x / offset.z);
        if (offset.z > 0)
            yaw += Mathf.PI;

        return yaw;
    }

    private float GetLookAtPitch(Vector3 lookAtPos)
    {
        Vector3 cameraPos = playerCamera.transform.position;
        if (lookAtPos.x == cameraPos.x && lookAtPos.y == cameraPos.y && lookAtPos.z == cameraPos.z)
            return 0f;
        
        Vector3 offset = new Vector3();
        offset.x = cameraPos.x - lookAtPos.x;
        offset.y = cameraPos.y - lookAtPos.y;
        offset.z = cameraPos.z - lookAtPos.z;

        float pitch = 0f;
        if (offset.y != 0f)
        {
            float distance = (float)Math.Sqrt(offset.x * offset.x + offset.z * offset.z);
            pitch = (float)Math.Atan(offset.y / distance);
        }

        return pitch;
    }

    private float RadToDeg(float rad)
    {
        return rad * 180 / Mathf.PI;
    }
}
