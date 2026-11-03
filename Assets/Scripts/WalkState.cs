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
        player.transform.localScale = new Vector3(player.transform.localScale.x, 1f, player.transform.localScale.z);
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