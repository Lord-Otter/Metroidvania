using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControlScript : MonoBehaviour
{
    //Controlls
    [SerializeField] private bool movingRight = false;
    [SerializeField] private bool movingLeft = false;
    [SerializeField] private bool jumping = false;
    [SerializeField] private bool highJumping = false;
    [SerializeField] private bool airJumping = false;

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
    }

    void Controls()
    {
        movingRight = Input.GetKey(KeyCode.D);
        movingLeft = Input.GetKey(KeyCode.A);
        
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

        // Movement
        float targetSpeed = movingRight ? maxMoveSpeed : movingLeft ? -maxMoveSpeed : 0;
        float acceleration = IsGroundedCheck() ? moveAcceleration : airControl;
        velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);

        // Drag application
        if (!movingRight && !movingLeft)
        {
            float drag = IsGroundedCheck() ? horizontalGroundDrag : horizontalAirDrag;
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


    //WORK IN PROGRESS-----------------------------------------------------------------------------------
    /*void FaceTravelDirection(bool isFacingRight)
    {
        Vector3 angularVelocity = rigidBody.angularVelocity;
        float yRotation = transform.eulerAngles.y;

        if (isFacingRight && yRotation > 0)
        {
            angularVelocity.y = -rotationSpeed; // Rotate in the opposite direction for right-facing
        }
        else if(!isFacingRight && transform.eulerAngles.y < 180)
        {
            rigidBody.constraints &= ~RigidbodyConstraints.FreezeRotationY;
            angularVelocity.y = rotationSpeed;  // Rotate in the regular direction for left-facing
            if(yRotation == 180)
            {
                rigidBody.constraints |= RigidbodyConstraints.FreezeRotationY;
            }else if(yRotation + rotationSpeed > 180)
            {
                yRotation = 180;
                rigidBody.constraints |= RigidbodyConstraints.FreezeRotationY;
            }
        }

        rigidBody.angularVelocity = angularVelocity;
    }*/


    bool IsGroundedCheck()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1f);
    }
}
