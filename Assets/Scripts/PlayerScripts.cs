
using System;
using System.Security.Cryptography;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.InputSystem.DualShock.LowLevel;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float defaultMoveSpeed = 10.0f;
    public float moveSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float jumpForce = 7.0f;
    public float dashForce = 50.0f;
    public float wallrunSpeed = 8.0f;
    public float airJumpForce = 3.0f;
    public int airJumps = 1;
    public int airJumpCount = 0;



    public Transform orientation;
    public float groundDrag;
    public float horizontalInput;
    public float verticalInput;
    Vector3 moveDirection;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundMask;
    public float groundCheckDistance = 0.2f;
    public bool isGrounded = true;

    Rigidbody rb;
    [SerializeField] private GameObject playerObject;

    public float mouseSensitivity = 2.0f;



    [Header("States")]
    private PlayerState currentState;
    public GroundedState GroundedState {get: private set;}
    public AirborneState AirborneState {get; private set;}
    public WalkState WalkState { get; private set;}
    public SprintState SprintState { get; private set;}
    public WallRunningState WallRunningState { get; private set;}
    public SlideState SlideState { get; private set;}
    public ClimbingState ClimbingState { get; private set;}
    public CrouchingState CrouchingState { get; private set;}
    public BulletJumpingState BulletJumpingState { get; private set;}
    public JumpState JumpState {get; private set;}

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
        else
        {
            moveSpeed = defaultMoveSpeed;
            moveState = MovementState.Walking;
            defaultBoolean();
        }
    }

    public void SwitchState(PlayerState newState)
    {
        currentState = newState;
        currentState.Enter();
    }


    private void Inputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

    }

    public void MovePlayer()
    {
        if(currentState is AirborneState)
        {
            return;
        }
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    public void Jump()
    {
        rb.AddForce(jumpForce * Vector3.up, ForceMode.VelocityChange);
    }

    public void AirJump()
    {
        rb.AddForce(airJumpForce * Vector3.up, ForceMode.VelocityChange);
        airJumpCount++;
    }

    //make sure that the player model doesn't float in the air for a bit
    // if in the air, it doesn't do anything
    public void StartCrouching()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
    }
    //in case the walk speed goes faster than intended
    private void LimitSpeed()
    {

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitVelocity = flatVelocity.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitVelocity.x, rb.linearVelocity.y, limitVelocity.z);
        }
    }



}