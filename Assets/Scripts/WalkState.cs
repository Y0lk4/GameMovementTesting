using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class WalkState : GroundedState
{

    public WalkState(PlayerController player) : base(player) {}
    public override void Enter()
    {
        base.Enter();
        player.moveSpeed = player.defaultMoveSpeed;
        
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if(Input.GetKey(KeyCode.LeftShift))
        {
            player.SwitchState(player.SprintState);
            return;
        }

        if(Input.GetKey(KeyCode.LeftControl))
        {
            player.SwitchState(player.CrouchingState);
            return;
        }

    }

    
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        player.MovePlayer();
        player.LimitSpeed();
    }

}