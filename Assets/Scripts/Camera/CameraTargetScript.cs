using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetScript : MonoBehaviour
{
    private PlayerChecks playerChecks;
    private PlayerVelocity playerVelocity;

    public float targetPositionX = 1.5f;
    public float targetPositionY = 2f;
    public float speedX = 5f;
    public float speedY = 5f;
    public float yMovementOffset = 2f;
    public float yOffsetThreshold = 4;
    Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        playerChecks = GetComponentInParent<PlayerChecks>();
        playerVelocity = GetComponentInParent<PlayerVelocity>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveTarget();
    }

    void MoveTarget()
    {
        Vector3 targetPosition;

        if (playerVelocity.velocity.y > yOffsetThreshold || playerVelocity.velocity.y < -yOffsetThreshold)
        {
            targetPosition = playerChecks.isFacingRight
                ? new Vector3(targetPositionX, targetPositionY + -yMovementOffset, 0f)
                : new Vector3(-targetPositionX, targetPositionY + -yMovementOffset, 0);
        }
        else
        {
            targetPosition = playerChecks.isFacingRight
                ? new Vector3(targetPositionX, targetPositionY, 0f)
                : new Vector3(-targetPositionX, targetPositionY, 0);
        }

        Vector3 newPosition = transform.localPosition;
        newPosition.x = Mathf.MoveTowards(transform.localPosition.x, targetPosition.x, Time.deltaTime * speedX);
        newPosition.y = Mathf.MoveTowards(transform.localPosition.y, targetPosition.y, Time.deltaTime * speedY);

        transform.localPosition = newPosition;
    }
}
