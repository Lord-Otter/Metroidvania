using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private AttackScript attackScript;
    private PlayerChecks playerChecks;
    private PlayerInputs playerInputs;
    private PlayerVelocity playerVelocity;


    [Header("Run")]
    public float maxMoveSpeed = 10f;
    public float moveAcceleration = 100f;
    public float airAcceleration = 50f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float airJumpForce = 10f;
    public float jumpCancelForce = 10f;
    public float jumpGracePeriod = 0.1f;
    [HideInInspector] public bool canGroundJump = false;
    public int maxAirJumps = 1;
    private float airTimeStart;
    public int airJumps;

    [Header("Dash")]
    public float dashSpeed;
    public float dashRange;
    public float dashCooldown;
    public bool canDash = true;
    private bool dashCooldownReset;
    private bool dashGroundReset;
    private float dashStartTime;
    public bool isDashing = false;
    private float dashDirection;

    [Header("Pogo")]
    public float pogoForce;

    [Header("Drag")]
    public float horizontalGroundDrag = 100f;
    public float horizontalAirDrag = 5f;

    private void Awake()
    {
        attackScript = GetComponent<AttackScript>();
        playerChecks = GetComponent<PlayerChecks>();
        playerInputs = GetComponent<PlayerInputs>();
        playerVelocity = GetComponent<PlayerVelocity>();
    }

    void Start()
    {
        airJumps = maxAirJumps;

        // Dash
        StartCoroutine(DashCooldown());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Run
        Movement();

        // Jump
        Jumping();

        // Dash
        DashExecution();
    }

    #region Run
    public void Movement()
    {
        playerVelocity.velocity = playerVelocity.rigidBody.velocity;
        float horizontalSpeed;

        if (Mathf.Abs(playerInputs.moveHorizontal) > 0.7f || Mathf.Abs(playerInputs.moveVertical) > 0.7f)
        {
            horizontalSpeed = playerInputs.moveHorizontal > 0
                ? 1
                : -1;   
        }
        else
        {
            horizontalSpeed = playerInputs.moveHorizontal;
        }

        float targetSpeed = (playerInputs.movingRight && playerInputs.movingLeft) ? 0 
            : (playerInputs.movingRight ? maxMoveSpeed * horizontalSpeed 
            : (playerInputs.movingLeft ? -maxMoveSpeed * -horizontalSpeed 
            : 0));
        float acceleration = playerChecks.IsGrounded() ? moveAcceleration 
            : airAcceleration;

        if (!isDashing)
        {
            if (playerInputs.movingLeft || playerInputs.movingRight)
            {
                playerVelocity.velocity.x = Mathf.MoveTowards(playerVelocity.velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
            }
            // Drag 
            else if ((!playerInputs.movingRight && !playerInputs.movingLeft) || (playerInputs.movingRight && playerInputs.movingLeft))
            {
                float drag = playerChecks.IsGrounded() ? horizontalGroundDrag : !playerChecks.IsGrounded() && (!playerInputs.movingLeft || !playerInputs.movingRight) ? horizontalAirDrag : 0;

                playerVelocity.velocity.x = Mathf.MoveTowards(playerVelocity.velocity.x, 0, drag * Time.fixedDeltaTime);
            }
        }

        playerVelocity.rigidBody.velocity = playerVelocity.velocity;
    }
    #endregion

    #region Jump
    public void Jumping()
    {
        playerVelocity.velocity = playerVelocity.rigidBody.velocity;

        // Jumping
        if (playerInputs.jumping)
        {
            playerVelocity.velocity.y = jumpForce;
            playerInputs.jumping = false;
            canGroundJump = false;
        }
        // Double Jump
        else if (playerInputs.airJumping)
        {
            airJumps--;
            playerVelocity.velocity.y = airJumpForce;
            playerInputs.airJumping = false;
        }

        // Coyote Jump
        if (playerChecks.IsGrounded())
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
        if (!playerInputs.highJumping && playerVelocity.velocity.y > 0)
        {
            playerVelocity.velocity.y = Mathf.Max(playerVelocity.velocity.y - jumpCancelForce * Time.fixedDeltaTime, 0);
        }

        playerVelocity.rigidBody.velocity = playerVelocity.velocity;
    }
    #endregion

    #region Dash
    void DashExecution()
    {
        playerVelocity.velocity = playerVelocity.rigidBody.velocity;

        if (playerChecks.IsGrounded())
        {
            dashGroundReset = true;
        }

        if (dashCooldownReset && dashGroundReset)
        {
            canDash = true;
        }

        if (playerInputs.dashing && canDash)
        {
            dashStartTime = Time.time;
            dashCooldownReset = false;
            dashGroundReset = false;
            canDash = false;
            isDashing = true;
            if(!playerInputs.movingRight && !playerInputs.movingLeft)
            {
                dashDirection = playerChecks.isFacingRight ? 1f : -1f;
            }
            else
            {
                dashDirection = playerInputs.movingRight ? 1f : -1f;
            }
        }

        if (isDashing)
        {
            playerVelocity.velocity.y = 0;
            playerVelocity.velocity.x = dashDirection * dashSpeed;

            float dashDuration = dashRange / dashSpeed;

            // Dash Duration
            if ((Time.time - dashStartTime) > dashDuration)
            {
                playerVelocity.velocity.x = dashDirection * maxMoveSpeed;
                StartCoroutine(DashCooldown());
                isDashing = false;
            }
        }

        playerVelocity.rigidBody.velocity = playerVelocity.velocity;
    }

    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        dashCooldownReset = true;
        StopCoroutine(DashCooldown());
    }
    #endregion
}
