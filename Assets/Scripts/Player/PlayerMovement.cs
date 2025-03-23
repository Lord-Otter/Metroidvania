using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Cinemachine;
using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerAttack playerAttack;
    private PlayerChecks playerChecks;
    private PlayerInputs playerInputs;
    private PlayerVelocity playerVelocity;
    private Transform playerTransform;
    private TimeManager timeManager;
    private CustomTimeSolution customTimeSolution;
    private CameraBehaviour cameraBehaviour;
    private CapsuleCollider[] capsuleColliders;


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
    private Vector3 dashStartPosition;
    [HideInInspector]public bool isDashing = false;
    private float dashDirection;
    public bool canDash = true;

    private GameObject teleportTarget;
    private GameObject finalTeleportTarget;
    [Header("Teleport")]
    public float tpMaxSpeed;
    public float tpAcceleration;
    public int tpLimitMax;
    public int tpLimit;
    private float tpStartTime;
    public float tpStartDelay;    
    private float currentTPSpeed;
    public float tpCooldown;
    public bool canTP = true;
    public bool isTeleporting = false;

    [Header("Pogo")]
    public float pogoForce;
    [HideInInspector]public bool isPogo;

    [Header("Drag")]
    public float horizontalGroundDrag = 100f;
    public float horizontalAirDrag = 5f;

    private void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerChecks = GetComponent<PlayerChecks>();
        playerInputs = GetComponent<PlayerInputs>();
        playerVelocity = GetComponent<PlayerVelocity>();
        playerTransform = GetComponent<Transform>();
        timeManager = GameObject.Find("Time_Manager").GetComponent<TimeManager>();
        customTimeSolution = GameObject.Find("Time_Manager").GetComponent<CustomTimeSolution>();
        cameraBehaviour = GameObject.Find("Main Camera").GetComponent<CameraBehaviour>();
        capsuleColliders = GetComponents<CapsuleCollider>();

        // Teleport
        teleportTarget = GameObject.Find("Teleport_Target");
        finalTeleportTarget = GameObject.Find("Final_Teleport_Target");
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

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(timeManager.worldPause)
        {
            return;
        }

        // Teleport
        Teleporting();

        if(timeManager.tpPause)
        {
            return;
        }

        // Run
        Movement();

        // Jump
        Jumping();

        // Dash
        Dashing();

        
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
            dashStartPosition = transform.position;
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

        //KEEP WORKING ----------------------------------------------------------------------------
        if (isDashing)
        {
            playerVelocity.velocity.y = 0;

            if(Mathf.Abs(transform.position.x - dashStartPosition.x) >= dashRange)
            {
                playerVelocity.velocity.x = dashDirection * maxMoveSpeed;
                StartCoroutine(DashCooldown());
                isDashing = false;
            }
            else
            {
                playerVelocity.velocity.x = dashDirection * dashSpeed;
            }
        }
        
        /*if (isDashing)
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
        }*/

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
            canTP = false;
            playerInputs.teleporting = false;
            isTeleporting = true;
            tpStartTime = Time.time;

            playerVelocity.velocity = Vector3.zero;

            ResetMovementAbilities();

            capsuleColliders[0].enabled = false;
            capsuleColliders[1].enabled = false;

            timeManager.TPPause(true);
        }

        if(isTeleporting && Time.time - tpStartTime > tpStartDelay)
        {
            Vector3 targetPos = teleportTarget.transform.position;
            Vector3 direction = (targetPos - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPos);

            currentTPSpeed = Mathf.Min(currentTPSpeed + tpAcceleration * Time.fixedDeltaTime, tpMaxSpeed);

            if (distance < currentTPSpeed * Time.fixedDeltaTime)
            {
                transform.position = targetPos;

                isTeleporting = false;
                canTP = true;

                capsuleColliders[0].enabled = true;
                capsuleColliders[1].enabled = true;

                timeManager.TPPause(false);

                currentTPSpeed = 0f;
            }
            else
            {
                // Continue moving toward the target
                transform.position += direction * currentTPSpeed * Time.fixedDeltaTime;
            }
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
