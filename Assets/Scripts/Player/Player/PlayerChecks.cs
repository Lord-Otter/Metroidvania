using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChecks : MonoBehaviour
{
    private AttackScript attackScript;
    private BasicMovementScript basicMovementScript;
    private DashScript dashScript;
    private PlayerInputScript playerInputScript;
    private PlayerVelocity playerVelocity;

    public bool isFacingRight;

    // Start is called before the first frame update
    void Start()
    {
        attackScript = GetComponent<AttackScript>();
        basicMovementScript = GetComponent<BasicMovementScript>();
        dashScript = GetComponent<DashScript>();
        playerInputScript = GetComponent<PlayerInputScript>();
        playerVelocity = GetComponent<PlayerVelocity>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((playerInputScript.movingRight && !playerInputScript.movingLeft && !dashScript.isDashing) || (!playerInputScript.movingRight && playerInputScript.movingLeft && !dashScript.isDashing))
        {
            IsFacingRight();
        }
    }

    public bool IsGrounded()
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

        return hit1 || hit2;
    }

    /*public bool CanGroundJump()
    {
        return CanGroundJump;
    }*/

    void IsFacingRight()
    {
        if (playerInputScript.movingRight && !playerInputScript.movingLeft)
        {
            isFacingRight = true;
        }
        else if (playerInputScript.movingLeft && !playerInputScript.movingRight)
        {
            isFacingRight = false;
        }
    }
}
