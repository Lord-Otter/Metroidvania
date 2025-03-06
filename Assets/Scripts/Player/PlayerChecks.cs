using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerInputs;

public class PlayerChecks : MonoBehaviour
{
    private AttackScript attackScript;
    private PlayerMovement playerMovement;
    private PlayerInputs playerInputs;
    private PlayerVelocity playerVelocity;

    public enum RotateMode { Aim, Movement }
    [Header("Player Orientation")]
    public RotateMode rotateMode;
    public bool isFacingRight;

    private void Awake()
    {
        attackScript = GetComponent<AttackScript>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInputs = GetComponent<PlayerInputs>();
        playerVelocity = GetComponent<PlayerVelocity>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {       
        if(!playerMovement.isDashing)
        { 
            IsFacingRight();
        }
    }

    public bool IsGrounded()
    {
        Vector3 origin1 = transform.position + new Vector3(-0.4f, 0f, 0f);
        Vector3 origin2 = transform.position + new Vector3(0.4f, 0f, 0f);
        Vector3 originC = transform.position + new Vector3(00f, 0f, 0f);
        Vector3 direction = Vector3.down;
        float raycastDistance = 1.1f;

        Vector3 directionC = Vector3.down;
        float raycastDistanceC = 1.1f;

        Debug.DrawRay(origin1, direction * raycastDistance, Color.red);
        Debug.DrawRay(origin2, direction * raycastDistance, Color.red);
        Debug.DrawRay(originC, directionC * raycastDistanceC, Color.red);

        bool hit1 = Physics.Raycast(origin1, direction, raycastDistance);
        bool hit2 = Physics.Raycast(origin2, direction, raycastDistance);
        bool hitC = Physics.Raycast(origin2, directionC, raycastDistanceC);

        if(hit1 || hit2 || hitC)
        {
            playerMovement.ResetMovementAbilities();
        }

        return hit1 || hit2 || hitC;
    }

    public bool IsTouchingWall()
    {
        Vector3 origin1 = transform.position + new Vector3(0f, 0f, 0f);
        Vector3 origin2 = transform.position + new Vector3(0f, 0f, 0f);
        Vector3 direction1 = Vector3.right;
        Vector3 direction2 = Vector3.left;
        float raycastDistance = 0.55f;

        Debug.DrawRay(origin1, direction1 * raycastDistance, Color.red);
        Debug.DrawRay(origin2, direction2 * raycastDistance, Color.red);

        bool hit1 = Physics.Raycast(origin1, direction1, raycastDistance);
        bool hit2 = Physics.Raycast(origin2, direction2, raycastDistance);

        if(hit1 || hit2)
        {
            playerMovement.ResetMovementAbilities();
        }

        return hit1 || hit2;
    }

    void IsFacingRight()
    {
        RotateModeFunctions();
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

    void RotateMove()
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