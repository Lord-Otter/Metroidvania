using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashScript : MonoBehaviour
{
    private AttackScript attackScript;
    private BasicMovementScript basicMovementScript;
    private PlayerChecks playerChecks;
    private PlayerInputScript playerInputScript;
    private PlayerVelocity playerVelocity;

    public bool inputDash;
    public float dashSpeed;
    public float dashRange;
    public float dashCooldown;
    public bool canDash = true;
    private bool dashCooldownReset;
    private bool dashGroundReset;
    private float dashStartTime;
    public bool isDashing = false;

    // Start is called before the first frame update
    void Start()
    {
        attackScript = GetComponent<AttackScript>();
        basicMovementScript = GetComponent<BasicMovementScript>();
        playerChecks = GetComponent<PlayerChecks>();
        playerInputScript = GetComponent<PlayerInputScript>();
        playerVelocity = GetComponent<PlayerVelocity>();

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

        if(inputDash && canDash)
        {
            dashStartTime = Time.time;
            dashCooldownReset = false;
            dashGroundReset = false;
            canDash = false;
            isDashing = true;
        }

        if (isDashing)
        {
            playerVelocity.velocity.y = 0;
            playerVelocity.velocity.x = playerChecks.isFacingRight ? dashSpeed : -dashSpeed;

            float dashDuration = dashRange / dashSpeed;

            // Dash Duration
            if (((Time.time - dashStartTime) > dashDuration) && playerChecks.isFacingRight)
            {
                playerVelocity.velocity.x = basicMovementScript.maxMoveSpeed;
                StartCoroutine(DashCooldown());
                isDashing = false;
            }
            else if (((Time.time - dashStartTime) > dashDuration) && !playerChecks.isFacingRight)
            {
                playerVelocity.velocity.x = -basicMovementScript.maxMoveSpeed;
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
