using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerInputs;

public class CameraTargetScript : MonoBehaviour
{
    private PlayerChecks playerChecks;
    private PlayerInputs playerInputs;
    private PlayerVelocity playerVelocity;

    [Header("Mouse Camera Behaviour")]
    public float mouseCamMultiplier = 0.1f;
    public int xMax = 50;
    public int yMax = 50;

    [Header("Movement Camera Behaviour")]
    public float targetPositionX = 1.5f;
    public float targetPositionY = 2f;
    public float speedX = 5f;
    public float speedY = 5f;
    public float yMovementOffset = 2f;
    public float yOffsetThreshold = 4;
    private Vector3 targetPosition;

    private void Awake()
    {
        playerChecks = GetComponentInParent<PlayerChecks>();
        playerInputs = GetComponentInParent<PlayerInputs>();
        playerVelocity = GetComponentInParent<PlayerVelocity>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveTarget();
    }

    void MoveTarget()
    {
        if (playerInputs.aimMode == AimMode.Stick || playerInputs.aimMode == AimMode.Move)
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
        else if (playerInputs.aimMode == AimMode.Mouse)
        {
            targetPosition = new Vector3(playerInputs.mousePosition.x, playerInputs.mousePosition.y, 0);
            targetPosition.x = playerInputs.mousePosition.x;

            Vector3 newPosition = transform.localPosition;
                
            newPosition.x = targetPosition.x * mouseCamMultiplier;
            newPosition.y = (targetPosition.y * mouseCamMultiplier) + targetPositionY;

            newPosition.x = Mathf.Clamp(newPosition.x, -xMax, xMax);
            newPosition.y = Mathf.Clamp(newPosition.y, -yMax + targetPositionY, yMax + targetPositionY);           

            transform.localPosition = newPosition;
            
        }
    }
}
