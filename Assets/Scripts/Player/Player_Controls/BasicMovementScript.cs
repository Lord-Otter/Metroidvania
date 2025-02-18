using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovementScript : MonoBehaviour
{
    public PlayerInputScript playerInputScript;
    public DashScript dashScript;
    public PlayerVelocity playerVelocity;
    public PlayerChecks playerChecks;
    public AttackScript attackScript;

    // Running
    public float maxMoveSpeed;
    public float moveAcceleration;
    public float airAcceleration;

    // Jumping
    public float jumpForce;
    public float airJumpForce;
    public float jumpCancelForce;
    public bool canGroundJump = true;
    public int maxAirJumps;
    public float jumpGracePeriod;
    private float airTimeStart;
    public int airJumps;
    private bool airJumping = false;

    // Start is called before the first frame update
    void Start()
    {
        playerInputScript = GetComponent<PlayerInputScript>();
        dashScript = GetComponent<DashScript>();
        playerVelocity = GetComponent<PlayerVelocity>();
        playerChecks = GetComponent<PlayerChecks>();
        attackScript = GetComponent<AttackScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Jumping();
    }

    public void Jumping()
    {
        // Coyote Jump
        if (playerChecks.IsGroundedCheck())
        {
            airTimeStart = Time.time;
            canGroundJump = true;

            airJumps = maxAirJumps;
        }
        else if (Time.time - airTimeStart >= jumpGracePeriod)
        {
            canGroundJump = false;
        }

        /*if (playerInputScript.jumping)
        {
            playerVelocity.velocity.y = jumpForce;
            jumping = false;
            canGroundJump = false;
        }
        else if (airJumping)
        {
            airJumps--;
            velocity.y = airJumpForce;
            airJumping = false;
        }

        /* Jump canceling
        if (!highJumping && velocity.y > 0)
        {
            velocity.y = Mathf.Max(velocity.y - jumpCancelForce * Time.fixedDeltaTime, 0);
        }*/
    }
}
