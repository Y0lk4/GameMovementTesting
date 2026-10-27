using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public abstract class GroundedState : PlayerState
{
    protected bool jumpTriggered;
    public GroundedState(PlayerController player) : base(player) {}

    public override void Enter()
    {   
        base.Enter();
        player.isGrounded = true;
        jumpTriggered = false;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpTriggered = true;
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        if(jumpTriggered)
        {
            player.Jump();
            jumpTriggered = false;
            player.SwitchState(player.AirborneState);
        }
        player.MovePlayer();
        player.LimitSpeed();
    }
}