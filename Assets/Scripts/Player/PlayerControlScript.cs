using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControlScript : MonoBehaviour
{
    // Controls----------------------
    // Standard Controls
    private bool movingRight = false;
    private bool movingLeft = false;
    private bool jumping = false;
    private bool highJumping = false;

    // Special Controls
    private bool airJumping = false;
    //private bool dashing = false;

    // Running
    public float maxMoveSpeed;
    public float moveAcceleration;
    public float airAcceleration;

    // Jumping
    public float jumpForce;
    public float airJumpForce;
    public float jumpCancelForce;
    public int maxAirJumps;
    private int airJumps;
    public float jumpGracePeriod;
    public bool canGroundJump = true;
    private float airTimeStart;
    
    /* Dashing
    public float dashSpeed;
    public float dashRange;
    public float dashCooldown;
    public bool canDash = true;
    private bool dashCooldownReset;
    private bool dashGroundReset;
    private float dashStartTime;
    private bool isDashing = false;
    */
    public float rotationSpeed;
    public bool isFacingRight;

    // Physics
    public float gravity;
    public float horizontalAirDrag;
    public float horizontalGroundDrag;
    public float maxFallSpeed;

    private Rigidbody rigidBody;
    private GameObject player;

    public Vector3 velocity;
    public DashScript dashScript;

    // Start is called before the first frame update
    void Start()
    {
        dashScript = GetComponent<DashScript>();

        rigidBody = GetComponent<Rigidbody>();
        player = gameObject;
        airJumps = maxAirJumps;

        //StartCoroutine(DashCooldown());
    }



    // Update is called once per frame
    void Update()
    {
        Controls();
    }

    private void FixedUpdate()
    {
        Movement();
        FaceTravelDirection();
    }

    void Controls()
    {
        // Standard Movement
        movingRight = Input.GetKey(KeyCode.D);
        movingLeft = Input.GetKey(KeyCode.A);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canGroundJump && !dashScript.isDashing)
            {
                jumping = true;
            }
            else if (!IsGroundedCheck() && !dashScript.isDashing && (airJumps > 0))
            {
                airJumping = true;
            }
        }

        highJumping = Input.GetKey(KeyCode.Space);

        // Special Moves
        //dashing = Input.GetKey(KeyCode.F);

    }

    public void Movement()
    {
       velocity = rigidBody.velocity;

        // Coyote Jump
        if (IsGroundedCheck())
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
                float drag = IsGroundedCheck() ? horizontalGroundDrag : !IsGroundedCheck() && (!movingLeft || !movingRight) ? horizontalAirDrag : 0;

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

        /* Dashing        
        if(dashing && canDash)
        {
            dashStartTime = Time.time;
            dashCooldownReset = false;
            dashGroundReset = false;
            canDash = false;
            isDashing = true;
        }*/

        /*if (isDashing)
        {
            velocity.y = 0;
            velocity.x = isFacingRight ? dashScript.dashSpeed : -dashScript.dashSpeed;
            
            float dashDuration = dashRange / dashSpeed;

            // Dash Duration
            if (((Time.time - dashStartTime) > dashDuration) && isFacingRight)
            {
                velocity.x = maxMoveSpeed;
                StartCoroutine(DashCooldown());
                isDashing = false;
            }
            else if (((Time.time - dashStartTime) > dashDuration) && !isFacingRight)
            {
                velocity.x = -maxMoveSpeed;
                StartCoroutine(DashCooldown());
                isDashing = false;
                
            }
            
        }*/

        /*if(dashGroundReset && dashCooldownReset)
        {
            canDash = true;
        }*/

        // Apply gravity
        if (!dashScript.isDashing)
        {
            velocity.y -= gravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        }

        rigidBody.velocity = velocity;
        //velocityX = velocity.x;
        //velocityY = velocity.y;
    }

    /*IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        dashCooldownReset = true;
        StopCoroutine(DashCooldown());
    }*/

    void FaceTravelDirection()
    {
        if (!dashScript.isDashing)
        {
            if (movingRight && !movingLeft)
            {
                isFacingRight = true;
            }
            else if (movingLeft && !movingRight)
            {
                isFacingRight = false;
            }
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, isFacingRight ? 1 : 179, 0), rotationSpeed * Time.fixedDeltaTime);
    }

    public bool IsGroundedCheck()
    {
        Vector3 origin1 = transform.position + new Vector3(-0.4f, 0f, 0f);
        Vector3 origin2 = transform.position + new Vector3(0.4f, 0f, 0f);
        Vector3 direction = Vector3.down;
        float raycastDistance = 1.1f;

        Debug.DrawRay(origin1, direction * raycastDistance, Color.red);
        Debug.DrawRay(origin2, direction * raycastDistance, Color.red);

        bool hit1 = Physics.Raycast(origin1, direction, raycastDistance);
        bool hit2 = Physics.Raycast(origin2, direction, raycastDistance);

        return hit1 || hit2;
    }
}
