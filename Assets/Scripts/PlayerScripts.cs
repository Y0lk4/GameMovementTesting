
using System.Security.Cryptography;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem.DualShock.LowLevel;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float defaultMoveSpeed = 10.0f;
    private float moveSpeed;
    private float sprintSpeed;
    private float crouchSpeed;
    public float jumpForce = 7.0f;
    public float dashForce = 50.0f;
    public float wallrunSpeed = 8.0f;
    public float airJumpForce = 3.0f;
    public int airJumps = 1;
    public int airJumpCount = 0;



    public Transform orientation;
    public float groundDrag;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundMask;
    public float groundCheckDistance = 0.2f;


    [Header("Movement Bool")]
    public bool isGrounded;
    public bool isSliding;
    public bool isSprinting;
    public bool wallrunning;
    private bool jumpRequested;
    public bool isCrouching;
    public bool isBulletJumping;

    Rigidbody rb;
    [SerializeField] private GameObject playerObject;

    public float mouseSensitivity = 2.0f;


    [Header("States")]
    public MovementState moveState;

    public EnvironmentState envState;
    public enum MovementState
    {
        WallRunning,
        Walking,
        Sprinting,
        Sliding,
        Climbing,
        Crouching,
        BulletJumping
    }

    public enum EnvironmentState
    {
        Grounded,
        Airborne
    }

    void Start()
    {
        moveSpeed = defaultMoveSpeed;
        sprintSpeed = moveSpeed * 2f;
        crouchSpeed = defaultMoveSpeed / 2f;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // don't let collisions tip the player over
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        // Yaw: turn the whole player left/right
        transform.Rotate(0f, mouseX, 0f, Space.World);

        //* Ground Check using a Raycast downwards
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);

        //* Grounded or Airborne section and not wall running + drag
        if (isGrounded && !wallrunning && !isSliding)
        {
            airJumpCount = 0;
            rb.linearDamping = groundDrag;
            envState = EnvironmentState.Grounded;
        }
        else if (!isGrounded && !wallrunning)
        {
            rb.linearDamping = 0f;
            envState = EnvironmentState.Airborne;
        }

        Inputs();
        StateHandler();

        // Capture jump input in Update so frames aren't missed
        if (Input.GetButtonDown("Jump"))
        {
            jumpRequested = true;
        }

    }

    void FixedUpdate()
    {
        if (wallrunning || isGrounded)
        {
            airJumpCount = 0;
        }
        if (jumpRequested)
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (!isGrounded && !wallrunning && airJumpCount != airJumps)
            {
                AirJump();
            }

        }
        if (isSliding)
        {
            transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z);
            // see sliding script for sliding logic
        }
        else if (isCrouching)
        {
            isSprinting = false; // to make sure i am not sprinting during the sprint even thought its impossible
                                 // its just to make sure sprint boolean is false after a slide
            transform.localScale = new Vector3(transform.localScale.x, 0.7f, transform.localScale.z);

            MovePlayer();
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, 1.0f, transform.localScale.z);
            MovePlayer();
        }


        LimitSpeed();
    }

    private void StateHandler()
    {
        if (wallrunning)
        {
            moveState = MovementState.WallRunning;
            moveSpeed = wallrunSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && isSprinting && !isSliding)
        {
            // StartSlide();
        }
        else if (Input.GetKey(KeyCode.LeftControl) && isSliding)
        {
            moveState = MovementState.Sliding;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            Crouching();
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCrouching();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(transform.localScale.x, 1.0f, transform.localScale.z);
            isCrouching = false;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            if (!isCrouching)
            {
                Sprint();
            }
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            moveState = MovementState.Walking;
            defaultBoolean();
        }
    }


    private void Inputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

    }

    private void MovePlayer()
    {
        if(envState == EnvironmentState.Airborne)
        {
            return;
        }
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void Jump()
    {
        rb.AddForce(jumpForce * Vector3.up, ForceMode.VelocityChange);
        jumpRequested = false;
    }

    private void AirJump()
    {
        rb.AddForce(airJumpForce * Vector3.up, ForceMode.VelocityChange);
        jumpRequested = false;
        airJumpCount++;
    }

    //make sure that the player model doesn't float in the air for a bit
    // if in the air, it doesn't do anything
    private void StartCrouching()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
    }

    private void Crouching()
    {
        isCrouching = true;
        moveSpeed = crouchSpeed;
        moveState = MovementState.Crouching;
    }

    private void Sprint()
    {

        if(envState == EnvironmentState.Airborne)
        {
            isSprinting = false;
            return;
        }
        isSprinting = true;
        moveSpeed = sprintSpeed;
        moveState = MovementState.Sprinting;

    }

    //in case the walk speed goes faster than intended
    private void LimitSpeed()
    {
        if (isSliding) return;

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitVelocity = flatVelocity.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitVelocity.x, rb.linearVelocity.y, limitVelocity.z);
        }
    }

    private void defaultBoolean()
    {
        isSprinting = false;
        isSliding = false;
        isCrouching = false;
    }

}