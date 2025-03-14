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
    private Transform playerTransform;


    [Header("Run")]
    public float maxMoveSpeed = 10f;
    public float moveAcceleration = 100f;
    public float airAcceleration = 50f;
    public bool canRun = true;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float airJumpForce = 10f;
    public float jumpCancelForce = 10f;
    public float jumpGracePeriod = 0.1f;
    public int maxAirJumps = 1;
    private float airTimeStart;
    public int airJumps;
    public bool canJump = false;

    [Header("Dash")]
    public float dashSpeed;
    public float dashRange;
    public float dashCooldown;
    private bool dashCooldownReset;
    private bool dashGroundReset;
    private float dashStartTime;
    [HideInInspector]public bool isDashing = false;
    private float dashDirection;
    public bool canDash = true;

    [Header("Teleport")]
    private GameObject teleportTarget;
    public float tpSpeed;
    public float tpAcceleration;
    public int tpLimitMax;
    public int tpLimit;
    public float tpCooldown;
    public bool canTP;

    [Header("Pogo")]
    public float pogoForce;
    [HideInInspector]public bool isPogo;

    [Header("Drag")]
    public float horizontalGroundDrag = 100f;
    public float horizontalAirDrag = 5f;

    private void Awake()
    {
        attackScript = GetComponent<AttackScript>();
        playerChecks = GetComponent<PlayerChecks>();
        playerInputs = GetComponent<PlayerInputs>();
        playerVelocity = GetComponent<PlayerVelocity>();
        playerTransform = GetComponent<Transform>();

        // Teleport
        teleportTarget = GameObject.Find("Teleport_Target");
    }

    void Start()
    {
        // Jump
        airJumps = maxAirJumps;

        // Dash
        StartCoroutine(DashCooldown());

        // Teleport
        tpLimit = tpLimitMax;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Run
        Movement();

        // Jump
        Jumping();

        // Dash
        Dashing();

        // Teleport
        Teleporting();
    }

    #region Run
    public void Movement()
    {
        playerVelocity.velocity = playerVelocity.rigidBody.linearVelocity;
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
            if ((playerInputs.movingLeft || playerInputs.movingRight) && canRun)
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

        playerVelocity.rigidBody.linearVelocity = playerVelocity.velocity;
    }
    #endregion

    #region Jump
    void Jumping()
    {
        playerVelocity.velocity = playerVelocity.rigidBody.linearVelocity;

        // Jumping
        if (playerInputs.jumping)
        {
            playerVelocity.velocity.y = jumpForce;
            playerInputs.jumping = false;
            canJump = false;
            isPogo = false;
        }
        // Double Jump
        else if (playerInputs.airJumping)
        {
            airJumps--;
            playerVelocity.velocity.y = airJumpForce;
            playerInputs.airJumping = false;
            isPogo = false;
        }

        // Coyote Jump
        if (playerChecks.IsGrounded())
        {
            airTimeStart = Time.time;
            canJump = true;
        }
        else if (Time.time - airTimeStart >= jumpGracePeriod)
        {
            canJump = false;
        }

        // Jump Canceling
        if (!playerInputs.highJumping && playerVelocity.velocity.y > -1)
        {
            playerVelocity.velocity.y = Mathf.Max(playerVelocity.velocity.y - jumpCancelForce * Time.fixedDeltaTime);
        }
        else if(isPogo)
        {
            playerVelocity.velocity.y = Mathf.Max(playerVelocity.velocity.y - jumpCancelForce * Time.fixedDeltaTime);
        }

        if(playerVelocity.velocity.y < 0 && isPogo)
        {
            isPogo = false;
        }

        playerVelocity.rigidBody.linearVelocity = playerVelocity.velocity;
    }

    public void Pogo()
    {
        isPogo = true;

        playerVelocity.velocity = playerVelocity.rigidBody.linearVelocity;

        playerVelocity.velocity.y = 0;
        playerVelocity.velocity.y = pogoForce;

        ResetMovementAbilities();

        playerVelocity.rigidBody.linearVelocity = playerVelocity.velocity;
    }
    #endregion

    #region Dash
    void Dashing()
    {
        playerVelocity.velocity = playerVelocity.rigidBody.linearVelocity;

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

        playerVelocity.rigidBody.linearVelocity = playerVelocity.velocity;
    }

    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        dashCooldownReset = true;
        StopCoroutine(DashCooldown());
    }
    #endregion

    #region Teleport
    void Teleporting()
    {
        playerVelocity.velocity = playerVelocity.rigidBody.linearVelocity;

        if(playerInputs.teleporting && canTP)
        {
            //playerVelocity.velocity.y = 10;

            playerTransform.position = teleportTarget.transform.position;
        }

        playerVelocity.rigidBody.linearVelocity = playerVelocity.velocity;
    }
    #endregion

    #region Miscellaneous
        public void ResetMovementAbilities()
    {
        dashGroundReset = true;
        airJumps = maxAirJumps;
    }
    #endregion
}
