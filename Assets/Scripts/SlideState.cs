using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SlideState : GroundedState
{
    public SlideState(PlayerController player) : base(player)
    {
        rb = player.GetComponent<Rigidbody>();
        
    }

    [Header("References")]
    public Transform playerObj;
    private Rigidbody rb;

    [Header("Sliding")]
    private float slidingDecel = 15.0f;
    public float slideForce = 20.0f;
    
    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;
    public KeyCode slideKey = KeyCode.LeftControl;

    private bool isSliding;


    public override void Enter()
    {
        base.Enter();
        player.transform.localScale = new Vector3(player.transform.localScale.x, 0.5f, player.transform.localScale.z);
        rb = player.GetComponent<Rigidbody>();
        isSliding = false;
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

        if(!isSliding)
        {
            StartSlide();
        }
        if(isSliding)
        {
            Slide();
        }
        
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void StartSlide()
    {
        
        rb.AddForce(Vector3.down * 10f, ForceMode.Impulse); 

        rb.linearDamping = 0f;

        Vector3 slideDirection = player.orientation.forward * verticalInput + player.orientation.right * horizontalInput;
        //adds a small boost towards the direction i start sliding
        rb.AddForce(slideDirection * slideForce, ForceMode.Impulse);
        isSliding = true;
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
