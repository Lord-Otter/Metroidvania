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
    private CameraBehaviour cameraBehaviour;
    private CapsuleCollider[] capsuleColliders;
    private Collider[] enemyColliders;


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
    private Vector3 dashStartPosition;
    private bool dashCooldownReset;
    private bool dashGroundReset;
    private float dashStartTime;    
    [HideInInspector]public bool isDashing = false;
    private float timeWhenPaused;
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
        cameraBehaviour = GameObject.Find("Main Camera").GetComponent<CameraBehaviour>();
        capsuleColliders = GetComponents<CapsuleCollider>();

        // Teleport
        teleportTarget = GameObject.Find("Teleport_Target");
        finalTeleportTarget = GameObject.Find("Final_Teleport_Target");

        // Collider
        enemyColliders = GameObject.Find("Training_Dummy").GetComponents<Collider>();
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
            OnPause();
            return;
        }

        // Teleport
        TeleportingStart();

        if(timeManager.tpPause)
        {
            OnPause();
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
            : (playerInputs.movingRight ? maxMoveSpeed * horizontalSpeed * timeManager.customTimeScale 
            : (playerInputs.movingLeft ? -maxMoveSpeed * -horizontalSpeed * timeManager.customTimeScale
            : 0));
        float acceleration = playerChecks.IsGrounded() ? moveAcceleration * timeManager.customTimeScale 
            : airAcceleration * timeManager.customTimeScale;

        if (!isDashing)
        {
            if ((playerInputs.movingLeft || playerInputs.movingRight) && canRun)
            {
                playerVelocity.velocity.x = Mathf.MoveTowards(playerVelocity.velocity.x, targetSpeed , acceleration * Time.fixedDeltaTime * timeManager.customTimeScale);
            }
            // Drag 
            else if ((!playerInputs.movingRight && !playerInputs.movingLeft) || (playerInputs.movingRight && playerInputs.movingLeft))
            {
                float drag = playerChecks.IsGrounded() ? horizontalGroundDrag * timeManager.customTimeScale 
                : !playerChecks.IsGrounded() && (!playerInputs.movingLeft || !playerInputs.movingRight) ? horizontalAirDrag * timeManager.customTimeScale 
                : 0;

                playerVelocity.velocity.x = Mathf.MoveTowards(playerVelocity.velocity.x, 0, drag * Time.fixedDeltaTime * timeManager.customTimeScale);
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
            playerVelocity.velocity.y = jumpForce * timeManager.customTimeScale;
            playerInputs.jumping = false;
            canJump = false;
            isPogo = false;

            if(playerChecks.AttachedToWallRight())
            {
                playerVelocity.velocity.x = -maxMoveSpeed * jumpForce * 0.1f * timeManager.customTimeScale;
            }
            else if(playerChecks.AttachedToWallLeft())
            {
                playerVelocity.velocity.x = maxMoveSpeed * jumpForce * 0.1f * timeManager.customTimeScale;
            }
        }
        // Double Jump
        else if (playerInputs.airJumping)
        {
            airJumps--;
            playerVelocity.velocity.y = airJumpForce * timeManager.customTimeScale;
            playerInputs.airJumping = false;
            isPogo = false;
        }

        // Coyote Jump
        if (playerChecks.IsGrounded() || playerChecks.AttachedToWallRight() || playerChecks.AttachedToWallLeft())
        {
            airTimeStart = Time.time;
            canJump = true;
        }
        else if (Time.time - airTimeStart >= jumpGracePeriod)
        {
            canJump = false;
        }

        // Jump Canceling
        if (!playerInputs.highJumping && playerVelocity.velocity.y > -1 * timeManager.customTimeScale)
        {
            playerVelocity.velocity.y = Mathf.Max(playerVelocity.velocity.y - jumpCancelForce * Time.fixedDeltaTime * Mathf.Pow(timeManager.customTimeScale, 2));
        }
        else if(isPogo)
        {
            playerVelocity.velocity.y = Mathf.Max(playerVelocity.velocity.y - jumpCancelForce * Time.fixedDeltaTime * timeManager.customTimeScale);
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
        playerVelocity.velocity.y = pogoForce * timeManager.customTimeScale;

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
            dashStartPosition = playerTransform.position;
            dashCooldownReset = false;
            dashGroundReset = false;
            canDash = false;
            isDashing = true;
            capsuleColliders[0].excludeLayers = LayerMask.GetMask("Enemy");
            capsuleColliders[1].excludeLayers = LayerMask.GetMask("Enemy");


            if(!playerInputs.movingRight && !playerInputs.movingLeft)
            {
                dashDirection = playerChecks.isFacingRight ? 1f : -1f;
            }
            else
            {
                if(playerChecks.AttachedToWallRight() || (playerChecks.IsTouchingWallRight() && !playerChecks.isFacingRight))
                {
                    dashDirection = -1f;
                }
                else if(playerChecks.AttachedToWallLeft())
                {
                    dashDirection = 1f;
                }
                else
                {
                    dashDirection = playerInputs.movingRight ? 1f : -1f;
                }
            }
        }

        if (isDashing)
        {
            playerVelocity.velocity.y = 0;

            float distanceTraveled = Mathf.Abs(playerTransform.position.x - dashStartPosition.x);

            if((distanceTraveled >= dashRange) || (dashDirection > 0 && playerChecks.IsTouchingWallRight()) || (dashDirection < 0 && playerChecks.IsTouchingWallLeft()))
            {
                playerVelocity.velocity.x = dashDirection * maxMoveSpeed * timeManager.customTimeScale;
                StartCoroutine(DashCooldown());
                capsuleColliders[0].excludeLayers &= ~LayerMask.GetMask("Enemy");
                capsuleColliders[1].excludeLayers &= ~LayerMask.GetMask("Enemy");
                dashStartPosition = Vector3.zero;
                isDashing = false;
            }
            else
            {
                playerVelocity.velocity.x = dashDirection * dashSpeed * timeManager.customTimeScale;
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
    void TeleportingStart()
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
            Vector3 targetPos = finalTeleportTarget.transform.position;
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

                timeManager.customTimeScale = 0.01f;
                
                //TeleportingEnd();
            }
            else
            {
                // Continue moving toward the target
                transform.position += direction * currentTPSpeed * Time.fixedDeltaTime * timeManager.customTimeScale;
            }
        }

        playerVelocity.rigidBody.linearVelocity = playerVelocity.velocity;
    }

    void TeleportingEnd()
    {
        playerVelocity.velocity = playerVelocity.rigidBody.linearVelocity;

        Vector3 targetPos = finalTeleportTarget.transform.position;
        Vector3 direction = (targetPos - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPos);

        currentTPSpeed = Mathf.Min(currentTPSpeed + tpAcceleration * Time.fixedDeltaTime, tpMaxSpeed);

        if (distance < currentTPSpeed * Time.fixedDeltaTime)
        {
            transform.position = targetPos;

            isTeleporting = false;
            canTP = true;

            //capsuleColliders[0].enabled = true;
            //capsuleColliders[1].enabled = true;

            timeManager.TPPause(false);

            currentTPSpeed = 0f;
        }
        else
        {
            // Continue moving toward the target
            transform.position += direction * currentTPSpeed * Time.fixedDeltaTime;
        }

        playerVelocity.rigidBody.linearVelocity = playerVelocity.velocity;
    }
    #endregion



    #region Miscellaneous
    void OnPause()
    {
        // Dash
        //dashDuration += Time.deltaTime; // Extend dashDuration while the game is paused
    }

    public void ResetMovementAbilities()
    {
        dashGroundReset = true;
        airJumps = maxAirJumps;
    }
    #endregion
}
