using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInputScript;

public class CameraTargetScript : MonoBehaviour
{
    private PlayerChecks playerChecks;
    private PlayerInputScript playerInputScript;
    private PlayerVelocity playerVelocity;

    [Header("Mouse Camera Behaviour")]
    public float mouseAimWeightX = 0.5f;
    public float mouseAimWeightY = 0.3f;
    public float mouseAimMaxX = 2.5f;
    public float mouseAimMaxY = 2f;

    [Header("Movement Camera Behaviour")]
    public float targetPositionX = 1.5f;
    public float targetPositionY = 2f;
    public float speedX = 5f;
    public float speedY = 5f;
    public float yMovementOffset = 2f;
    public float yOffsetThreshold = 4;
    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        playerChecks = GetComponentInParent<PlayerChecks>();
        playerInputScript = GetComponentInParent<PlayerInputScript>();
        playerVelocity = GetComponentInParent<PlayerVelocity>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveTarget();
    }

    void MoveTarget()
    {
        if (playerInputScript.aimMode == AimMode.Stick || playerInputScript.aimMode == AimMode.Move)
        {
            if (playerVelocity.velocity.y > yOffsetThreshold || playerVelocity.velocity.y < -yOffsetThreshold)
            {
                targetPosition = playerChecks.isFacingRight
                    ? new Vector3(targetPositionX, targetPositionY + -yMovementOffset, 0)
                    : new Vector3(-targetPositionX, targetPositionY + -yMovementOffset, 0);
            }
            else
            {
                targetPosition = playerChecks.isFacingRight
                    ? new Vector3(targetPositionX, targetPositionY, 0)
                    : new Vector3(-targetPositionX, targetPositionY, 0);
            }

            Vector3 newPosition = transform.localPosition;

            newPosition.x = Mathf.MoveTowards(transform.localPosition.x, targetPosition.x, Time.deltaTime * speedX);
            newPosition.y = Mathf.MoveTowards(transform.localPosition.y, targetPosition.y, Time.deltaTime * speedY);

            transform.localPosition = newPosition;
        }
        else if (playerInputScript.aimMode == AimMode.Mouse)
        {
            targetPosition = new Vector3(playerInputScript.mousePosition.x, playerInputScript.mousePosition.y, 0);
            targetPosition.x = playerInputScript.mousePosition.x;

            Vector3 newPosition = transform.localPosition;

            newPosition.x = targetPosition.x > mouseAimMaxX
                ? mouseAimMaxX
                : targetPosition.x < -mouseAimMaxX
                ? -mouseAimMaxX
                : targetPosition.x;

            newPosition.y = targetPosition.y > mouseAimMaxY
                ? mouseAimMaxY
                : targetPosition.y < -mouseAimMaxY
                ? -mouseAimMaxY
                : targetPosition.y;

            transform.localPosition = newPosition;
            
        }
    }
}
