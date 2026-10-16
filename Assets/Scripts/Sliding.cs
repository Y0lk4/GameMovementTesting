using UnityEngine;

public class SlidingScript : MonoBehaviour
{

    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private PlayerController pc;
    private Rigidbody rb;

    [Header("Sliding")]
    private float slidingDecel = 15.0f;
    public float slideForce = 20.0f;
    
    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;
    public KeyCode slideKey = KeyCode.LeftControl;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerController>();

        
    }

    // Update is called once per frame
    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0) && pc.isSprinting && pc.isGrounded && !pc.isSliding)
        {
            StartSlide();
        }

        if(Input.GetKeyUp(slideKey) && pc.isSliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (pc.isSliding)
        {
            Slide();
        }
    }

    // handle the start of the slide where u get an extra force at the start of the slide for a speed boost
    private void StartSlide()
    {
        pc.isSliding = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, 0.5f, playerObj.localScale.z);

        //to make sure when the player gets scaled down, the player does not float a bit
        rb.AddForce(Vector3.down * 10f, ForceMode.Impulse); 

        rb.linearDamping = 0f;

        Vector3 slideDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        //adds a small boost towards the direction i start sliding
        rb.AddForce(slideDirection * slideForce, ForceMode.Impulse);
    }

    private void Slide()
    {

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(-flatVelocity.normalized * slidingDecel, ForceMode.Force);
        // irl time based deceleration
        // Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // flatVelocity = Vector3.MoveTowards(flatVelocity, Vector3.zero, slidingDecel * Time.fixedDeltaTime);

        // rb.linearVelocity = new Vector3(flatVelocity.x, rb.linearVelocity.y, flatVelocity.z);

        if(flatVelocity.magnitude < 0.2f)
        {
            pc.isSliding = false;
            pc.isSprinting = false;
            pc.isCrouching = true;
            StopSlide();
        }
    }

    private void StopSlide()
    {
        pc.isSliding = false;
    }
}
