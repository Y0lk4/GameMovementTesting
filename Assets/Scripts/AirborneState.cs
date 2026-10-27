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
    }
    public override void UpdateState()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            player.AirJump();
        }
        if(player.isGrounded)
        {
            player.SwitchState(player.WalkState);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        Vector3 customGravity = new Vector3(0f, -12, 0f);
        rb.AddForce(customGravity, ForceMode.Acceleration);
    }

    public override void Exit()
    {
        base.Exit();
    }

    
}