using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;
public class CrouchingState : GroundedState
{
    private Rigidbody rb;
    public CrouchingState(PlayerController player) : base(player)
    {
        rb = player.GetComponent<Rigidbody>();
    }
    public override void Enter()
    {
        base.Enter();
        player.transform.localScale = new Vector3(player.transform.localScale.x, 0.7f, player.transform.localScale.z);
        Vector3 pushDown = new Vector3(0f, -5f, 0f);
        rb.AddForce(pushDown, ForceMode.Impulse);
        player.moveSpeed = player.crouchSpeed;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if(!Input.GetKey(KeyCode.LeftControl))
        {
            player.SwitchState(player.WalkState);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
        
    }

    public override void Exit()
    {
        base.Exit();
        player.transform.localScale = new Vector3(player.transform.localScale.x, 1f, player.transform.localScale.z);

    }
}