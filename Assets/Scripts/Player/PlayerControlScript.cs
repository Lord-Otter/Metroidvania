using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControlScript : MonoBehaviour
{
    //Controls
    private bool movingRight = false;
    private bool movingLeft = false;
    private bool jumping = false;
    private bool highJumping = false;
    private bool airJumping = false;

    //Running
    public float maxMoveSpeed;
    public float moveAcceleration;
    public float airControl;
    public float maxAirMoveSpeed;

    //Jumping
    public float jumpForce;
    public float airJumpForce;
    public float jumpCancelForce;
    public int maxAirJumps;
    [SerializeField] private int airJumps;
    public bool canGroundJump = true;
    private float airTimeStart;
    public float jumpGracePeriod;


    public float rotationSpeed;
    public bool isFacingRight;

    //Physics
    public float gravity;
    public float horizontalAirDrag;
    public float horizontalGroundDrag;
    //public float jumpFallDelay;
    public float maxFallSpeed;

    private Rigidbody rigidBody;
    private new Transform transform;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        player = gameObject;
        airJumps = maxAirJumps;
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
        movingRight = Input.GetAxisRaw("Horizontal") > 0;
        movingLeft = Input.GetAxisRaw("Horizontal") < 0;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canGroundJump)
            {
                jumping = true;
            }
            else if (!IsGroundedCheck() && airJumps > 0)
            {
                airJumping = true;
            }
        }

        highJumping = Input.GetKey(KeyCode.Space);
    }
    private float drag;
    void Movement()
    {
        Vector3 velocity = rigidBody.velocity;

        if (IsGroundedCheck())
        {
            airTimeStart = Time.time;
            canGroundJump = true;
            airJumps = maxAirJumps;
        }
        else if (Time.time - airTimeStart >= jumpGracePeriod)
        {
            canGroundJump = false;
        }

        //WORK IN PROGRESS------------FIX AIR MAX SPEED-------------------------------------------------------------------------
        // Movement
        float targetSpeed = (movingRight && movingLeft) ? 0 : (movingRight ? maxMoveSpeed : (movingLeft ? -maxMoveSpeed : 0));
        float acceleration = IsGroundedCheck() ? moveAcceleration : airControl;
        if (IsGroundedCheck())
        {
            velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
        }

        // Drag application
        if (!movingRight && !movingLeft)
        {
            //float drag; //= IsGroundedCheck() ? horizontalGroundDrag :!IsGroundedCheck && (!movingLeft || movingRight) ? horizontalAirDrag;
            float drag = IsGroundedCheck() ? horizontalGroundDrag : !IsGroundedCheck() && (!movingLeft || !movingRight) ? horizontalAirDrag : 0;
            /*if (IsGroundedCheck())
            {
                drag = horizontalGroundDrag;
            }else if(!IsGroundedCheck() && (!movingLeft || movingRight))
            {
                drag = horizontalAirDrag;
            }*/
            velocity.x = Mathf.MoveTowards(velocity.x, 0, drag * Time.fixedDeltaTime);
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

        // Apply gravity
        if (!IsGroundedCheck())
        {
            velocity.y -= gravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        }

        rigidBody.velocity = velocity;

    }

    bool IsGroundedCheck()
    {
        Vector3 origin1 = transform.position + new Vector3(-0.5f, 0f, 0f);
        Vector3 origin2 = transform.position + new Vector3(0.5f, 0f, 0f);
        Vector3 direction = Vector3.down;
        float raycastDistance = 1.05f;

        Debug.DrawRay(origin1, direction * raycastDistance, Color.green);
        Debug.DrawRay(origin2, direction * raycastDistance, Color.green);

        bool hit1 = Physics.Raycast(origin1, direction, raycastDistance);
        bool hit2 = Physics.Raycast(origin2, direction, raycastDistance);

        return hit1 || hit2;
    }

    void FaceTravelDirection()
    {
        if (movingRight && !movingLeft)
        {
            isFacingRight = true;
        }
        else if (movingLeft && !movingRight)
        {
            isFacingRight = false;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, isFacingRight ? 1 : 179, 0), rotationSpeed * Time.fixedDeltaTime);
    }
}
