using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private bool isCrouching = false;
    private bool isSprinting = false;

    public float motorSpeed = 5f;
    public float crouchSpeed = 2.5f;
    public float sprintSpeed = 10f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;
    public float standingHeight = 2f;
    public float crouchingHeight = 1f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x; // forward
        moveDirection.z = input.y; // backward

        // checks for crouching / sprinting
        float currentSpeed = motorSpeed;
        if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }
        else if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }

        controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0) {
            playerVelocity.y = -2f;
        }
        controller.Move(playerVelocity * Time.deltaTime);
        Debug.Log(playerVelocity.y);
    }

    public void Jump()
    {
        if ( isGrounded )
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    public void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        controller.height = isCrouching ? crouchingHeight : standingHeight;
    }

    public void ToggleSprint(bool sprinting)
    {
        isSprinting = sprinting;
    }
}
