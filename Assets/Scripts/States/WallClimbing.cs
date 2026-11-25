using UnityEngine;
using UnityEngine.Rendering.RenderGraphModule;

public class ClimbingState : PlayerState
{

    private Rigidbody rb;
    public ClimbingState(PlayerController player) : base(player)
    {
        rb = player.GetComponent<Rigidbody>();
    }
    public override void Enter()
    {
        base.Enter();
        rb.isKinematic = true;
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void Exit()
    {
        base.Exit();
        rb.isKinematic = false;
    }
}
