using System;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class BulletJumping : MonoBehaviour
{

    public float bulletJumpForce = 15.0f;
    [SerializeField] private GameObject head;

    private Rigidbody rb;
    private PlayerController pc;

    private PlayerController.MovementState mState;
    private PlayerController.EnvironmentState eState;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerController>();
        mState = pc.moveState;
        eState = pc.envState;
    }

    // Update is called once per frame
    void Update()
    {
        CheckBulletJumpAvailable();
        StateMachine();
    }

    void FixedUpdate()
    {
        if(pc.isBulletJumping)
        {
            BulletJump();
        }
    }

    private void StateMachine()
    {
        if(!pc.isBulletJumping && (mState == PlayerController.MovementState.Sliding || mState == PlayerController.MovementState.Crouching) && eState == PlayerController.EnvironmentState.Grounded && Input.GetKeyDown(KeyCode.Space))
        {
            StartBulletJump();
        }
        else
        {
            StopBulletJump();
        }
    }

    private void CheckBulletJumpAvailable()
    {
        mState = pc.moveState;
        eState = pc.envState;
    }

    private void StartBulletJump()
    {
        pc.isBulletJumping = true;
    }

    private void BulletJump()
    {   
            pc.isSliding = false;
            pc.moveState = PlayerController.MovementState.BulletJumping;
            Vector3 jumpDirection = head.transform.forward;

            jumpDirection.y = Mathf.Abs(jumpDirection.y);

            rb.AddForce(jumpDirection * bulletJumpForce, ForceMode.Impulse); 
    }

    private void StopBulletJump()
    {
        pc.isBulletJumping = false;
    }
}
