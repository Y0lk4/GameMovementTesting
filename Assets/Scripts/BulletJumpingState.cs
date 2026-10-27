using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;

public class BulletJumpingState : AirborneState
{

    public BulletJumpingState(PlayerController player) : base(player)
    {
        rb = player.GetComponent<Rigidbody>();
    }
    public float bulletJumpForce = 15.0f;
    [SerializeField] private GameObject head;

    private Rigidbody rb;

    public override void Enter()
    {
        base.Enter();
    }

    public override void UpdateState()
    {
        base.UpdateState();

    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        BulletJump();
    }

    public override void Exit()
    {
        base.Exit();

        player.SwitchState(player.AirborneState);
    }

    private void BulletJump()
    {   
            Vector3 jumpDirection = head.transform.forward;

            jumpDirection.y = Mathf.Abs(jumpDirection.y);

            jumpDirection.Normalize();

            // rb.AddForce(jumpDirection * bulletJumpForce, ForceMode.Impulse); 
            rb.linearVelocity = new Vector3(jumpDirection.x * bulletJumpForce, jumpDirection.y * bulletJumpForce, jumpDirection.z * bulletJumpForce);
    }
}
