using Unity.VisualScripting;
using UnityEngine;

public class SlideState : GroundedState
{
    public SlideState(PlayerController player) : base(player)
    {
        rb = player.GetComponent<Rigidbody>();
    }

    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;

    [Header("Sliding")]
    private float slidingDecel = 15.0f;
    public float slideForce = 20.0f;
    
    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;
    public KeyCode slideKey = KeyCode.LeftControl;


    public override void Enter()
    {
        base.Enter();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        horizontalInput = player.horizontalInput;
        verticalInput = player.verticalInput;

        if(Input.GetKeyUp(slideKey))
        {
            player.SwitchState(player.WalkState);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        StartSlide();
    }

    public override void Exit()
    {
        base.Exit();
    }

    // handle the start of the slide where u get an extra force at the start of the slide for a speed boost
    private void StartSlide()
    {
        //to make sure when the player gets scaled down, the player does not float a bit
        rb.AddForce(Vector3.down * 10f, ForceMode.Impulse); 

        rb.linearDamping = 0f;

        Vector3 slideDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        //adds a small boost towards the direction i start sliding
        rb.AddForce(slideDirection * slideForce, ForceMode.Impulse);
        Slide();
    }

    private void Slide()
    {

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(-flatVelocity.normalized * slidingDecel, ForceMode.Force);
       
        if(flatVelocity.magnitude < 0.2f)
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        player.SwitchState(player.CrouchingState);
    }
}
