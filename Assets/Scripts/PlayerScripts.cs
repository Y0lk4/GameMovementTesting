
using System;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public PlayerState visibleState;
    public GroundedState GroundedState {get; private set;}
    public AirborneState AirborneState {get; private set;}
    public WalkState WalkState { get; private set;}
    public SprintState SprintState { get; private set;}
    public WallRunningState WallRunningState { get; private set;}
    public SlideState SlideState { get; private set;}
    public ClimbingState ClimbingState { get; private set;}
    public CrouchingState CrouchingState { get; private set;}
    public BulletJumpingState BulletJumpingState { get; private set;}


    private void Awake()
{
    // 1. ALWAYS initialize the states first!
    GroundedState = new GroundedState(this);
    AirborneState = new AirborneState(this);
    WalkState = new WalkState(this);
    SprintState = new SprintState(this);
    WallRunningState = new WallRunningState(this);
    SlideState = new SlideState(this);
    ClimbingState = new ClimbingState(this);
    CrouchingState = new CrouchingState(this);
    BulletJumpingState = new BulletJumpingState(this);
}
    void Start()
    {
        visibleState = currentState;
        moveSpeed = defaultMoveSpeed;
        sprintSpeed = moveSpeed * 2f;
        crouchSpeed = defaultMoveSpeed / 2f;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // don't let collisions tip the player over
        SwitchState(WalkState); 
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        // Yaw: turn the whole player left/right
        transform.Rotate(0f, mouseX, 0f, Space.World);

        //* Ground Check using a Raycast downwards
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);

        currentState?.UpdateState(); 
        Inputs();
        visibleState = currentState;
    }

    void FixedUpdate()
    {
        currentState?.FixedUpdateState();   
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
            SwitchState(WalkState);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.layer == 6 && currentState == AirborneState)
        {
            SwitchState(WallRunningState);
        }
    }

    //in case the walk speed goes faster than intended
    public void LimitSpeed()
    {

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitVelocity = flatVelocity.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitVelocity.x, rb.linearVelocity.y, limitVelocity.z);
        }
    }

    public void EnableGravity()
    {
        rb.useGravity = true;
    }

    public void DisableGravity()
    {
        rb.useGravity = false;
    }



}