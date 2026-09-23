using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6.0f;
    public float jumpForce = 20.0f;
    public  float dashForce = 50.0f;
    public float wallrunSpeed = 6.0f;

    [Header("Ground Check")]
    public LayerMask groundMask;
    public float groundCheckDistance = 0.2f;

    Rigidbody rb;
    public float mouseSensitivity = 2.0f;


    public MovementState state;
    public enum MovementState
    {
        Grounded,
        WallRunning,
        Airborne
    }

    public bool wallrunning;
    private bool isGrounded;
    public bool jumpRequested;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // don't let collisions tip the player over
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        // Yaw: turn the whole player left/right
        transform.Rotate(0f, mouseX, 0f, Space.World);

        // Ground Check using a Raycast downwards
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
        if (isGrounded && !wallrunning)
            state = MovementState.Grounded;
        else if (!isGrounded && !wallrunning)
            state = MovementState.Airborne;

        StateHandler();

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            rb.AddForce(dashForce*transform.forward,ForceMode.VelocityChange);
        }
        // Capture jump input in Update so frames aren't missed
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpRequested = true;
        }
    }

    void FixedUpdate()
    {
        if (wallrunning) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 targetVelocity =   ( transform.forward * v + transform.right * h).normalized * moveSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x,rb.linearVelocity.y,targetVelocity.z);
       
       if (jumpRequested)
       {
            rb.AddForce(jumpForce*Vector3.up,ForceMode.VelocityChange);
            jumpRequested = false;    
        }
        
        
        
    }

    private void StateHandler()
    {
        if (wallrunning)
        {
            state = MovementState.WallRunning;
            moveSpeed = wallrunSpeed;
        }
    }
}