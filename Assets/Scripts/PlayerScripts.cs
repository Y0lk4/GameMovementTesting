using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.DualShock.LowLevel;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float defaultMoveSpeed = 10.0f;
    private float moveSpeed;
    private float sprintSpeed;
    public float jumpForce = 7.0f;
    public  float dashForce = 50.0f;
    public float wallrunSpeed = 8.0f;
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
    private bool isGrounded;
    private bool isSliding;
    private bool isSprinting;
    public bool wallrunning;
    public bool jumpRequested;

    Rigidbody rb;
    public float mouseSensitivity = 2.0f;


    public MovementState moveState;
    public enum MovementState
    {
        WallRunning,
        Walking,
        Sprinting,
        Sliding,
        Climbing
    }

    public EnvironmentState envState;
    public enum EnvironmentState
    {
        Grounded,
        Airborne
    }


 


    void Start()
    {
        moveSpeed = defaultMoveSpeed;
        sprintSpeed = moveSpeed * 1.5f;
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
        
        //* Drag section
        if(isGrounded) rb.linearDamping = groundDrag;
        else rb.linearDamping = 0f;    
        
        //* Grounded or Airborne section    
        if (isGrounded && !wallrunning) envState = EnvironmentState.Grounded;        
        else if (!isGrounded && !wallrunning) envState = EnvironmentState.Airborne;
            

        Inputs();
       

        StateHandler();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Sprint();
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            moveState = MovementState.Walking;
        }
        // Capture jump input in Update so frames aren't missed
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
        }

        LimitSpeed();
    }

    void FixedUpdate()
    {
        MovePlayer();

        if (wallrunning) return;

       
       if (jumpRequested)
       {
            rb.AddForce(jumpForce*Vector3.up,ForceMode.VelocityChange);
            jumpRequested = false;    
        }
        
        
        
    }

    private void Inputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);


    }

    private void Sprint()
    {
        moveSpeed = sprintSpeed;
        moveState = MovementState.Sprinting;
        LimitSpeed();
    }

    //in case the walk speed goes faster than intended
    private void LimitSpeed()
    {
        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if(flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitVelocity = flatVelocity.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitVelocity.x, rb.linearVelocity.y, limitVelocity.z);
        }
    }

    private void StateHandler()
    {
        if (wallrunning)
        {
            moveState = MovementState.WallRunning;
            moveSpeed = wallrunSpeed;
        }
    }
}