using Unity.Hierarchy.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class WallRunningState : AirborneState
{

    public WallRunningState(PlayerController player) : base(player)
    {
        rb = player.GetComponent<Rigidbody>();
    }

    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float wallJumpForce;
    public float wallJumpSideForce;
    public float maxWallRunTime;
    // private float wallRunTimer;
    
    
    [Header("Input")]
    // private float horizontalInput;
    // private float verticalInput;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;
    private bool startWallJump = false;

    [Header("References")]
    public Transform orientation;
    private PlayerController pm;
    private Rigidbody rb;
    private Camera cam;


    public override void Enter()
    {
        base.Enter();
        player.DisableGravity();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        CheckWalls();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            startWallJump = true;
        }
        if(!wallLeft && !wallRight)
        {
            player.SwitchState(player.AirborneState);  
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        WallRunningMovement();

        if(startWallJump)
        {
            WallJump();
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.EnableGravity();
    }

    private void CheckWalls()
    {                                             
        wallRight = Physics.Raycast(player.transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(player.transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }

    private void WallRunningMovement()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
           // checks if its the right, if its the right, rightwallhit is normal else its the leftwallhit
        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        //takes the up direction and the normal wall direction (whether it be left or right) to get the forward vector
        Vector3 wallForward = Vector3.Cross(wallNormal, player.transform.up);

        //check which direction player is facing
        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        //TBA, lock the camera to the direction im wall running to
        orientation.rotation = Quaternion.Slerp(orientation.rotation, Quaternion.LookRotation(wallForward), Time.deltaTime * 5f);
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
    }
    
    private void WallJump()
    {
        
        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;

        // multiply the up with the force of the jump that goes up and add the force thats going outwards the wall which is multiplied by the wall normal
        Vector3 forceToApply = player.transform.up * wallJumpForce + wallNormal * wallJumpSideForce;

        // rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(forceToApply, ForceMode.VelocityChange);
        player.airJumpCount = 0; //ensure that we can double jump after a wall jump
        startWallJump = false;
 
    }
}
