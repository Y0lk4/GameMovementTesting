using UnityEngine;
using UnityEngine.InputSystem;

public class AirborneState : PlayerState
{
    public AirborneState(PlayerController player) : base(player) {}

    public override void UpdateState()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            player.AirJump();
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}