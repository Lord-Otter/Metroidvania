using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerInputs;

public class PlayerChecks : MonoBehaviour
{
    private PlayerAttack playerAttack;
    private PlayerMovement playerMovement;
    private PlayerInputs playerInputs;
    private PlayerVelocity playerVelocity;

    private TimeManager timeManager;

    public enum RotateMode { Aim, Movement }
    [Header("Player Orientation")]
    public RotateMode rotateMode;
    public bool isFacingRight;

    [Header("Ground Check")]
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public LayerMask wallLayer;

    private void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInputs = GetComponent<PlayerInputs>();
        playerVelocity = GetComponent<PlayerVelocity>();

        timeManager = GameObject.Find("Time_Manager").GetComponent<TimeManager>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(timeManager.tpPause)
        {
            return;
        }
        //BoxCast();
        UpdateFacingDirection();
    }

    public bool IsGrounded()
    {
        Vector3 originL = transform.position + new Vector3(-0.4f, 0, 0);
        Vector3 originM = transform.position + new Vector3(0, 0, 0);
        Vector3 originR = transform.position + new Vector3(0.4f, 0, 0);
        Vector3 direction = Vector3.down;
        float raycastDistance = 1.1f;

        //Debug.DrawRay(originL, direction * raycastDistance, Color.red);
        //Debug.DrawRay(originM, direction * raycastDistance, Color.red);
        //Debug.DrawRay(originR, direction * raycastDistance, Color.red);

        bool hitL = Physics.Raycast(originL, direction, raycastDistance, groundLayer);
        bool hitM = Physics.Raycast(originM, direction, raycastDistance, groundLayer);
        bool hitR = Physics.Raycast(originR, direction, raycastDistance, groundLayer);

        if(hitL || hitM || hitR)
        {
            playerMovement.ResetMovementAbilities();
        }
        
        return hitL || hitM || hitR;
    }

    /*public bool IsTouchingWallR()
    {
        Vector3 originU = transform.position + new Vector3(0, 0.5f, 0);
        Vector3 originM = transform.position + new Vector3(0, 0, 0);
        Vector3 originB = transform.position + new Vector3(0, -0.5f, 0);
        Vector3 direction = Vector3.right;
        float raycastDistance = 0.55f;

        //Debug.DrawRay(originU, direction * raycastDistance, Color.red);
        //Debug.DrawRay(originM, direction * raycastDistance, Color.red);
        //Debug.DrawRay(originB, direction * raycastDistance, Color.red);

        bool hitU = Physics.Raycast(originU, direction, raycastDistance, wallLayer);
        bool hitM = Physics.Raycast(originM, direction, raycastDistance, wallLayer);
        bool hitB = Physics.Raycast(originB, direction, raycastDistance, wallLayer);

        return hitU || hitM || hitB;
    }*/

    public bool IsTouchingWallR()
    {
        Vector3 origin = transform.position; // Single origin at object position
        Vector3 halfExtents = new Vector3(0.18f, 0.5f, 0); // Box size (adjust as needed)
        Vector3 direction = Vector3.right;
        float raycastDistance = 0.36f;

        // Debug Draw the BoxCast
        DebugDrawBoxCast(origin, halfExtents, direction, raycastDistance, Color.red);

        // Perform the BoxCast
        bool hitWall = Physics.BoxCast(origin, halfExtents, direction, 
                                    out RaycastHit hit, Quaternion.identity, 
                                    raycastDistance, wallLayer);
        if(hitWall)
        {
            Debug.Log("LMAO GOTEEM");
        }
        return hitWall;
    }

    void DebugDrawBoxCast(Vector3 position, Vector3 halfExtents, Vector3 direction, float distance, Color color)
    {
        Vector3 endPosition = position + direction.normalized * distance;

        // Draw initial and final boxes
        DebugDrawBox(position, halfExtents, color);
        DebugDrawBox(endPosition, halfExtents, color);

        // Draw the cast line
        //Debug.DrawLine(position, endPosition, color);
    }

    void DebugDrawBox(Vector3 center, Vector3 halfExtents, Color color)
    {
        Vector3[] corners = new Vector3[8];

        for (int i = 0; i < 8; i++)
        {
            corners[i] = center + new Vector3(
                (i & 1) == 0 ? -halfExtents.x : halfExtents.x,
                (i & 2) == 0 ? -halfExtents.y : halfExtents.y,
                (i & 4) == 0 ? -halfExtents.z : halfExtents.z
            );
        }

        // Connect edges of the box
        Debug.DrawLine(corners[0], corners[1], color);
        Debug.DrawLine(corners[1], corners[3], color);
        Debug.DrawLine(corners[3], corners[2], color);
        Debug.DrawLine(corners[2], corners[0], color);

        Debug.DrawLine(corners[4], corners[5], color);
        Debug.DrawLine(corners[5], corners[7], color);
        Debug.DrawLine(corners[7], corners[6], color);
        Debug.DrawLine(corners[6], corners[4], color);

        Debug.DrawLine(corners[0], corners[4], color);
        Debug.DrawLine(corners[1], corners[5], color);
        Debug.DrawLine(corners[2], corners[6], color);
        Debug.DrawLine(corners[3], corners[7], color);
    }

    public bool AttachedToRWall()
    {
        if(IsTouchingWallR() && !IsGrounded() && playerInputs.movingRight)
        {
            playerMovement.ResetMovementAbilities();
            Debug.Log("Checking Right Wall...");
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsTouchingWallL()
    {
        Vector3 originU = transform.position + new Vector3(0, 0.5f, 0);
        Vector3 originM = transform.position + new Vector3(0, 0, 0);
        Vector3 originB = transform.position + new Vector3(0, -0.5f, 0);
        Vector3 direction = Vector3.left;
        float raycastDistance = 0.55f;

        //Debug.DrawRay(originU, direction * raycastDistance, Color.red);
        //Debug.DrawRay(originM, direction * raycastDistance, Color.red);
        //Debug.DrawRay(originB, direction * raycastDistance, Color.red);

        bool hitU = Physics.Raycast(originU, direction, raycastDistance, wallLayer);
        bool hitM = Physics.Raycast(originM, direction, raycastDistance, wallLayer);
        bool hitB = Physics.Raycast(originB, direction, raycastDistance, wallLayer);

        return hitU || hitM || hitB;
    }

    public bool AttachedToLWall()
    {
        if(IsTouchingWallL() && !IsGrounded() && playerInputs.movingLeft)
        {
            playerMovement.ResetMovementAbilities();
            Debug.Log("Checking Left Wall...");
            return true;
        }
        else
        {
            return false;
        }
    }

    void UpdateFacingDirection()
    {
        if (!playerMovement.isDashing)
        {
            RotateModeFunctions();
        }
    }

    void RotateModeFunctions()
    {
        switch (rotateMode)
        {
            case RotateMode.Aim:
                RotateAim();
                break;
            case RotateMode.Movement:
                RotateMove();
                break;
        }
    }

    void RotateAim()
    {
        float aimAngle = playerInputs.aimObject.rotation.eulerAngles.z;

        if(AttachedToRWall() && playerVelocity.velocity.y <= 0)
        {
            isFacingRight = false;
        }
        else if(AttachedToLWall() && playerVelocity.velocity.y <= 0)
        {
            isFacingRight = true;
        }
        else
        {
            if(playerInputs.inputDirectionR.magnitude == 0 && playerInputs.aimMode == AimMode.Stick)
            {
                if (playerInputs.movingRight && !playerInputs.movingLeft)
                {
                    isFacingRight = true;
                }
                else if (!playerInputs.movingRight && playerInputs.movingLeft)
                {
                    isFacingRight = false;
                }
            }
            else
            {
                if (aimAngle < 90 || aimAngle > 270)
                {
                    isFacingRight = true;
                }
                else if (aimAngle == 90 || aimAngle == 270)
                {

                }
                else
                {
                    isFacingRight = false;
                }
            }
        }
    }

    void RotateMove()
    {
        if(IsTouchingWallR() && playerInputs.movingRight)
        {
            isFacingRight = false;
        }
        else if(IsTouchingWallL() && playerInputs.movingLeft)
        {
            isFacingRight = true;
        }
        else
        {
            if (playerInputs.movingRight && !playerInputs.movingLeft)
            {
                isFacingRight = true;
            }
            else if (!playerInputs.movingRight && playerInputs.movingLeft)
            {
                isFacingRight = false;
            }
        }
    }
}