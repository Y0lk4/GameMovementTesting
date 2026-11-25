using System.Diagnostics.CodeAnalysis;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class AirborneState : PlayerState
{

    private Rigidbody rb;
    public AirborneState(PlayerController player) : base(player)
    {
        rb = player.GetComponent<Rigidbody>();
    }

    public override void Enter()
    {
        base.Enter();
        rb.linearDamping = 0f;
        player.DisableGravity();
    }
    public override void UpdateState()
    {
        if(Input.GetKeyDown(KeyCode.Space) && player.airJumpCount < player.airJumps)
        {
            player.AirJump();
        }
        if(player.isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            player.SwitchState(player.WalkState);
        }
    }

    public override void FixedUpdateState()
    {
        player.MovePlayer();
        player.LimitSpeed();

        base.FixedUpdateState();
        if(rb.linearVelocity.y <= 0.1f)
        {
            Vector3 customGravity = new Vector3(0f, -30f, 0f);
            rb.AddForce(customGravity, ForceMode.Acceleration);
        }
        else
        {
            Vector3 jumpGravity = new Vector3(0f, -16.5f, 0f);
            rb.AddForce(jumpGravity, ForceMode.Acceleration);
        }
        
        
    }

    public override void Exit()
    {
        base.Exit();
        player.EnableGravity();
    }

    
}