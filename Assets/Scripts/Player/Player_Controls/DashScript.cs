using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashScript : MonoBehaviour
{
    public PlayerInputScript playerInputScript;
    public BasicMovementScript basicMovementScript;
    public PlayerVelocity playerVelocity;
    public PlayerChecks playerChecks;
    public AttackScript attackScript;

    public PlayerControlScript playerControlScript;

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
        playerInputScript = GetComponent<PlayerInputScript>();
        playerVelocity = GetComponent<PlayerVelocity>();
        basicMovementScript = GetComponent<BasicMovementScript>();
        playerChecks = GetComponent<PlayerChecks>();
        attackScript = GetComponent<AttackScript>();

        playerControlScript = GetComponent<PlayerControlScript>();

        StartCoroutine(DashCooldown());
    }

    // Update is called once per frame
    void Update()
    {
        //inputDash = Input.GetKey(KeyCode.F);
        //UpdateDashVariables();
    }

    private void FixedUpdate()
    {
        DashExecution();
    }

    void DashExecution()
    {
        if (playerChecks.IsGroundedCheck())
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
            playerControlScript.velocity.y = 0;
            playerControlScript.velocity.x = playerControlScript.isFacingRight ? dashSpeed : -dashSpeed;

            float dashDuration = dashRange / dashSpeed;

            // Dash Duration
            if (((Time.time - dashStartTime) > dashDuration) && playerControlScript.isFacingRight)
            {
                playerControlScript.velocity.x = playerControlScript.maxMoveSpeed;
                StartCoroutine(DashCooldown());
                isDashing = false;
            }
            else if (((Time.time - dashStartTime) > dashDuration) && !playerControlScript.isFacingRight)
            {
                playerControlScript.velocity.x = -playerControlScript.maxMoveSpeed;
                StartCoroutine(DashCooldown());
                isDashing = false;
            }
        }
    }

    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        dashCooldownReset = true;
        StopCoroutine(DashCooldown());
    }
}
