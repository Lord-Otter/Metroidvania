using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVelocity : MonoBehaviour
{
    public PlayerInputScript playerInputScript;
    public DashScript dashScript;
    public BasicMovementScript basicMovementScript;
    public PlayerChecks playerChecks;
    public AttackScript attackScript;

    private Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        playerInputScript = GetComponent<PlayerInputScript>();
        dashScript = GetComponent<DashScript>();
        basicMovementScript = GetComponent<BasicMovementScript>();
        playerChecks = GetComponent<PlayerChecks>();
        attackScript = GetComponent<AttackScript>();

        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        Vector3 velocity = rigidBody.velocity;

        // Coyote Jump
        /*if (playerChecks.IsGroundedCheck())
        {
            airTimeStart = Time.time;
            canGroundJump = true;

            airJumps = maxAirJumps;

            //Dash
            //dashGroundReset = true;
        }
        else if (Time.time - airTimeStart >= jumpGracePeriod)
        {
            canGroundJump = false;
        }

        // Movement

        float targetSpeed = (movingRight && movingLeft) ? 0 : (movingRight ? maxMoveSpeed : (movingLeft ? -maxMoveSpeed : 0));
        float acceleration = IsGroundedCheck() ? moveAcceleration : airAcceleration;

        if (!dashScript.isDashing)
        {
            if (movingLeft || movingRight)
            {
                velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
            }
            // Drag 
            else if ((!movingRight && !movingLeft) || (movingRight && movingLeft))
            {
                float drag = playerChecks.IsGroundedCheck() ? horizontalGroundDrag : !playerChecks.IsGroundedCheck() && (!movingLeft || !movingRight) ? horizontalAirDrag : 0;

                velocity.x = Mathf.MoveTowards(velocity.x, 0, drag * Time.fixedDeltaTime);
            }
        }
        if (dashScript.isDashing)
        {
            velocity.y = 0;
            velocity.x = isFacingRight ? dashScript.dashSpeed : -dashScript.dashSpeed;
        }


        // Jumping
        if (jumping)
        {
            velocity.y = jumpForce;
            jumping = false;
            canGroundJump = false;
        }
        else if (airJumping)
        {
            airJumps--;
            velocity.y = airJumpForce;
            airJumping = false;
        }

        // Jump canceling
        if (!highJumping && velocity.y > 0)
        {
            velocity.y = Mathf.Max(velocity.y - jumpCancelForce * Time.fixedDeltaTime, 0);
        }

        // Apply gravity
        if (!dashScript.isDashing)
        {
            velocity.y -= gravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        }*/

        rigidBody.velocity = velocity;
    }
}
