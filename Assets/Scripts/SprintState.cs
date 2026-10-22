using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class SprintState : GroundedState
{

    public SprintState(PlayerController player) : base(player) {}
    public override void Enter()
    {
        base.Enter();
        player.moveSpeed = player.sprintSpeed;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if(!Input.GetKey(KeyCode.LeftShift))
        {
            player.SwitchState(player.WalkState);
            return;
        }

        if(Input.GetKey(KeyCode.LeftControl) && (player.horizontalInput != 0 || player.verticalInput != 0))
        {
            player.SwitchState(player.SlideState);
            return;
        }

    }

    
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

}