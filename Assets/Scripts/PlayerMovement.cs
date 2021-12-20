using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public GameObject playerCamera;
    public float speed = 6f;
    public float sprintMultiplier = 2f;
    public float crouchMultiplier = 0.5f;
    public float gravity = -9.81f;
    public float gravityMultiplier = 2f;
    public float jumpHeight = 2.0f;
    public float crouchingJumpHeight = 1.0f;
    public float crouchingHeight = 2;
    public float cameraHeadOffset = 0.5f;
    public float constantGravitySpeed = -9.81f;
    public bool enabled = true;
    public bool enableMovement = true;
    public bool enableSprinting = true;
    public bool enableCrouching = true;
    public bool constantGravity = false;
    public Controls playerControls;
    public float slowdownSpeed = 1.1f;
    
    public float groundDistance = 0.4f;
    public float headObjectDistance = 0.3f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool onGround;
    private bool lastCrouchingState = false;
    private bool crouching = false;
    private bool sprinting = false;
    private float standingHeight = 4;
    private bool isJumping = false;

    void Awake()
    {
        playerControls = new Controls();
        playerControls.Player.Jump.started += _ => StartJump();
        playerControls.Player.Jump.canceled += _ => EndJump();
    }

    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    void Start()
    {
        standingHeight = controller.height;
    }

    void Update()
    {
        if (!enabled)
            return;
        
        if (enableMovement)
            UpdateMovement();

        UpdateCrouching();
        UpdateGravity();
    }

    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
    }

    private void UpdateMovement()
    {
        Vector2 buttons = playerControls.Player.Move.ReadValue<Vector2>();
        velocity.x += buttons.x * Time.deltaTime;
        velocity.z += buttons.y * Time.deltaTime;

        velocity.x -= slowdownSpeed * Time.deltaTime;
        velocity.z -= slowdownSpeed * Time.deltaTime;
        
        Vector3 movement = transform.right * velocity.x;
        movement += transform.forward * velocity.z;
        movement *= speed;

        // If crouching, decrease speed (or increase depending on whatever the fuck that value is).
        // If sprinting but not crouching, increase speed (again, depending on that value).
        if (crouching && enableCrouching)
            movement *= crouchMultiplier;
        else if (sprinting && enableSprinting)
            movement *= sprintMultiplier;

        controller.Move(movement * Time.deltaTime);
    }

    private void UpdateGravity()
    {
        Vector3 groundPos = new Vector3(0, -controller.height / 2, 0);
        onGround = Physics.CheckSphere(groundPos + transform.position, groundDistance, groundMask);
        if (onGround)
            if (isJumping && enableMovement)
            {
                float height = jumpHeight;
                if (crouching)
                    height = crouchingJumpHeight;
                
                velocity.y = Mathf.Sqrt(height * -2f * gravityMultiplier * gravity);
            }
            else
                velocity.y = -3.0f;

        if (constantGravity)
            velocity.y = constantGravitySpeed * gravityMultiplier;
        else
            velocity.y += gravity * gravityMultiplier * Time.deltaTime;
        
        controller.Move(velocity * Time.deltaTime);
    }

    private void UpdateCrouching()
    {
        if (!enableMovement)
            crouching = false;
        
        float expectedHeight = standingHeight;
        if (crouching) // Check if the player is standing
            expectedHeight = crouchingHeight;

        // Set the controller height to be the height that coresponds with the current crouch state
        controller.height = expectedHeight;
        // Set the camera position to the eye position
        playerCamera.transform.localPosition = new Vector3(0, controller.height / 2 - cameraHeadOffset, 0);
        
        if (crouching == lastCrouchingState)
            return;
        
        Vector3 headPosition = new Vector3(0,  controller.height / 2, 0);
        bool isObjectOnHead = Physics.CheckSphere(headPosition + transform.position, groundDistance, groundMask);
        if (isObjectOnHead && !crouching)
            return;
        
        Vector3 halfVector = new Vector3(0, (standingHeight - crouchingHeight) / 2, 0);
        if (crouching) // Check what the player crouch status changed to
            controller.Move(-halfVector);
        else
            controller.Move(halfVector);

        lastCrouchingState = false;
    }

    private void StartJump()
    {
        isJumping = true;
    }

    private void EndJump()
    {
        isJumping = false;
    }
}
