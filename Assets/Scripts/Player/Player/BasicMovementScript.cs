using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovementScript : MonoBehaviour
{
    private AttackScript attackScript;
    private DashScript dashScript;
    private PlayerChecks playerChecks;
    private PlayerInputScript playerInputScript;
    private PlayerVelocity playerVelocity;


    // Running
    public float maxMoveSpeed = 10f;
    public float moveAcceleration = 100f;
    public float airAcceleration = 50f;

    // Jumping
    public float jumpForce = 10f;
    public float airJumpForce = 10f;
    public float jumpCancelForce = 10f;
    public float jumpGracePeriod = 0.1f;
    public bool canGroundJump = false;
    public int maxAirJumps = 1;
    private float airTimeStart;
    public int airJumps;

    // Drag
    public float horizontalGroundDrag = 100f;
    public float horizontalAirDrag = 5f;

    // Start is called before the first frame update
    void Start()
    {
        // Object Reference
        attackScript = GetComponent<AttackScript>();
        dashScript = GetComponent<DashScript>();
        playerChecks = GetComponent<PlayerChecks>();
        playerInputScript = GetComponent<PlayerInputScript>();
        playerVelocity = GetComponent<PlayerVelocity>();

        airJumps = maxAirJumps;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
        Jumping();
    }

    public void Movement()
    {
        playerVelocity.velocity = playerVelocity.rigidBody.velocity;

        float targetSpeed = (playerInputScript.movingRight && playerInputScript.movingLeft) ? 0 : (playerInputScript.movingRight ? maxMoveSpeed * playerInputScript.moveHorizontal : (playerInputScript.movingLeft ? -maxMoveSpeed * -playerInputScript.moveHorizontal : 0));
        float acceleration = playerChecks.IsGrounded() ? moveAcceleration : airAcceleration;

        if (!dashScript.isDashing)
        {
            if (playerInputScript.movingLeft || playerInputScript.movingRight)
            {
                playerVelocity.velocity.x = Mathf.MoveTowards(playerVelocity.velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
            }
            // Drag 
            else if ((!playerInputScript.movingRight && !playerInputScript.movingLeft) || (playerInputScript.movingRight && playerInputScript.movingLeft))
            {
                float drag = playerChecks.IsGrounded() ? horizontalGroundDrag : !playerChecks.IsGrounded() && (!playerInputScript.movingLeft || !playerInputScript.movingRight) ? horizontalAirDrag : 0;

                playerVelocity.velocity.x = Mathf.MoveTowards(playerVelocity.velocity.x, 0, drag * Time.fixedDeltaTime);
            }
        }

        playerVelocity.rigidBody.velocity = playerVelocity.velocity;
    }

    public void Jumping()
    {
        playerVelocity.velocity = playerVelocity.rigidBody.velocity;

        // Jumping
        if (playerInputScript.jumping)
        {
            playerVelocity.velocity.y = jumpForce;
            playerInputScript.jumping = false;
            canGroundJump = false;
        }
        // Double Jump
        else if (playerInputScript.airJumping)
        {
            airJumps--;
            playerVelocity.velocity.y = airJumpForce;
            playerInputScript.airJumping = false;
        }

        // Coyote Jump
        if (playerChecks.IsGrounded() || playerChecks.IsTouchingWall())
        {
            airTimeStart = Time.time;
            canGroundJump = true;

            airJumps = maxAirJumps;
        }
        else if (Time.time - airTimeStart >= jumpGracePeriod)
        {
            canGroundJump = false;
        }

        // Jump Canceling
        if (!playerInputScript.highJumping && playerVelocity.velocity.y > 0)
        {
            playerVelocity.velocity.y = Mathf.Max(playerVelocity.velocity.y - jumpCancelForce * Time.fixedDeltaTime, 0);
        }

        playerVelocity.rigidBody.velocity = playerVelocity.velocity;
    }
}
