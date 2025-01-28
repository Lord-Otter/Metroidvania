using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControlScript : MonoBehaviour
{
    public float maxMoveSpeed;
    public float moveAcceleration;
    public float airControl;
    public float maxAirMoveSpeed;
    public float jumpForce;
    public float jumpCancelForce;
    public float rotationSpeed;

    //Physics
    public float gravity;
    public float horizontalAirDrag;
    public float horizontalGroundDrag;
    public float jumpFallDelay;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        Vector3 movement = Vector3.zero;
        Vector3 velocity = rigidBody.velocity;

        bool isFacingRight = true;

        if (gameObject.name == "Player")
        {
            //Movement
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                isFacingRight = true;
                if (!IsGroundedCheck() && velocity.x < maxAirMoveSpeed)
                {
                    velocity.x += 1f * airControl;
                }
                else
                {
                    movement.x = 1f;
                }
            }
            else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                isFacingRight = false;
                if (!IsGroundedCheck() && velocity.x > -maxAirMoveSpeed)
                {
                    velocity.x -= 1f * airControl;
                }
                else
                {                                       
                    movement.x = -1f;                   
                }
            }
            

            if (IsGroundedCheck())
            {
                if (velocity.x < maxMoveSpeed && velocity.x > -maxMoveSpeed)
                {
                    velocity.x += movement.x * moveAcceleration;
                }

                if (velocity.x > 0)
                {
                    velocity.x -= horizontalGroundDrag;
                    if(horizontalGroundDrag > velocity.x)
                    {
                        velocity.x = 0;
                    }
                }
                else if (velocity.x < 0)
                {
                    velocity.x += horizontalGroundDrag;
                    if(-horizontalGroundDrag < velocity.x)
                    {
                        velocity.x = 0;
                    }
                }
                
            }

            //Jump
            if (Input.GetKeyDown(KeyCode.Space) && IsGroundedCheck())
            {
                velocity.y = jumpForce;
            }            
            if (!Input.GetKey(KeyCode.Space) && !IsGroundedCheck() && velocity.y > 0)
            {
                velocity.y -= 1f * jumpCancelForce;

                if (velocity.y < 3)
                {
                    velocity.y += jumpFallDelay;
                }
            }

            if (!IsGroundedCheck())
            {
                if (velocity.y > -maxFallSpeed)
                {                    
                    velocity.y -= gravity * Time.fixedDeltaTime;                    
                }

                if(velocity.x > 0 && !Input.GetKey(KeyCode.D) || velocity.x > 0 && Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
                {
                    velocity.x -= horizontalAirDrag;
                }
                else if(velocity.x < 0 && !Input.GetKey(KeyCode.A) || velocity.x < 0 && Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
                {
                    velocity.x += horizontalAirDrag;
                }
            }

            rigidBody.velocity = velocity;

            FaceTravelDirection(isFacingRight);
        }
    }


    //WORK IN PROGRESS-----------------------------------------------------------------------------------
    void FaceTravelDirection(bool isFacingRight)
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
    }


    bool IsGroundedCheck()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1f);
    }
}
