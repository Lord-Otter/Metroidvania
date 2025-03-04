using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashScript : MonoBehaviour
{
    private AttackScript attackScript;
    private PlayerMovement playerMovement;
    private PlayerChecks playerChecks;
    private PlayerInputs playerInputs;
    private PlayerVelocity playerVelocity;

    public float dashSpeed;
    public float dashRange;
    public float dashCooldown;
    public bool canDash = true;
    private bool dashCooldownReset;
    private bool dashGroundReset;
    private float dashStartTime;
    public bool isDashing = false;
    private float dashDirection;

    private void Awake()
    {
        attackScript = GetComponent<AttackScript>();
        playerMovement = GetComponent<PlayerMovement>();
        playerChecks = GetComponent<PlayerChecks>();
        playerInputs = GetComponent<PlayerInputs>();
        playerVelocity = GetComponent<PlayerVelocity>();
    }

    void Start()
    {
        StartCoroutine(DashCooldown());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        DashExecution();
    }

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
                playerVelocity.velocity.x = dashDirection * playerMovement.maxMoveSpeed;
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
}
