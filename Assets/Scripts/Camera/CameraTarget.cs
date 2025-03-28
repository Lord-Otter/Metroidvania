using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerInputs;

public class CameraTarget : MonoBehaviour
{
    private PlayerChecks playerChecks;
    private PlayerInputs playerInputs;
    private PlayerVelocity playerVelocity;
    private PlayerMovement playerMovement;

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
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    void Start()
    {

    }

    void Update()
    {
        //MoveTarget();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveTarget();
    }

    void MoveTarget()
    {
        if(!playerMovement.isTeleporting)
        {
            float smoothTime = 1f;

            if (playerInputs.aimMode == AimMode.Stick || playerInputs.aimMode == AimMode.Move) // Camera behavior when movement/stick aiming
            {
                if(!playerChecks.IsTouchingWallRight() && !playerChecks.IsTouchingWallLeft())
                {
                    if(playerInputs.movingRight && !playerInputs.movingLeft)
                    {
                        targetPosition.x = targetPositionX;
                    }
                    else if(playerInputs.movingLeft && !playerInputs.movingRight)
                    {
                        targetPosition.x = -targetPositionX;
                    }
                    else
                    {
                        targetPosition.x = playerChecks.isFacingRight
                            ? targetPositionX
                            : -targetPositionX;
                    }
                }
                targetPosition.y = targetPositionY;

                Vector3 newPosition = transform.localPosition;

                if (Mathf.Abs(playerVelocity.velocity.y) > yOffsetThreshold)
                {
                    targetPosition.y += yMovementOffset * playerVelocity.velocity.y * 0.1f;
                }

                newPosition.x = Mathf.Lerp(newPosition.x, targetPosition.x, Time.fixedDeltaTime / smoothTime);
                newPosition.y = Mathf.Lerp(newPosition.y, targetPosition.y, Time.fixedDeltaTime / smoothTime);

                newPosition.x = Mathf.Clamp(newPosition.x, -xMax, xMax);
                newPosition.y = Mathf.Clamp(newPosition.y, -yMax + targetPositionY, yMax + targetPositionY);

                transform.localPosition = newPosition;
            }
            else if (playerInputs.aimMode == AimMode.Mouse) // Camera behavior when mouse aiming
            {
                targetPosition = new Vector3(playerInputs.mousePosition.x, playerInputs.mousePosition.y, 0);
                targetPosition.x = playerInputs.mousePosition.x;

                Vector3 newPosition = transform.localPosition;                

                float targetX = targetPosition.x * mouseCamMultiplier;
                float targetY = (targetPosition.y * mouseCamMultiplier) + targetPositionY;
                
                
                if (Mathf.Abs(playerVelocity.velocity.y) >= yOffsetThreshold)
                {
                    targetY += yMovementOffset * playerVelocity.velocity.y * 0.3f;
                }

                
                newPosition.x = Mathf.Lerp(newPosition.x, targetX, Time.fixedDeltaTime / smoothTime);
                newPosition.y = Mathf.Lerp(newPosition.y, targetY, Time.fixedDeltaTime / smoothTime);

                newPosition.x = Mathf.Clamp(newPosition.x, -xMax, xMax);
                newPosition.y = Mathf.Clamp(newPosition.y, -yMax + targetPositionY, yMax + targetPositionY);           

                transform.localPosition = newPosition;
            }
        }
        else
        {
            targetPosition = new Vector3(0, 1, 0);
            transform.localPosition = targetPosition;
        }
    }
}
