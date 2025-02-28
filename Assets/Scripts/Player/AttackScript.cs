using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    private BasicMovementScript basicMovementScript;
    private DashScript dashScript;
    private PlayerChecks playerChecks;
    private PlayerInputScript playerInputScript;
    private PlayerVelocity playerVelocity;

    [Header("Collider")]
    private Transform atkDirection;
    private Transform atkColliderTransform;
    private BoxCollider2D atkColliderTrigger;
    public float defaultXOffset;
    public float defaultYOffset;
    public float defaultWidth;
    public float defaultHeight;
    public float downXOffset;
    public float downYOffset;
    public float downWidth;
    public float downHeight;

    void Start()
    {
        basicMovementScript = GetComponent<BasicMovementScript>();
        dashScript = GetComponent<DashScript>();
        playerChecks = GetComponent<PlayerChecks>();
        playerInputScript = GetComponent<PlayerInputScript>();
        playerVelocity = GetComponent<PlayerVelocity>();

        atkDirection = GameObject.Find("Attack_Direction").transform;
        atkColliderTransform = GameObject.Find("Attack_Trigger").transform;
        atkColliderTrigger = GetComponentInChildren<BoxCollider2D>();
    }

    void Update()
    {
        MoveAttackTrigger();
    }

    void FixedUpdate()
    {
        if (playerInputScript.attacking)
        {
            
        }
    }

    void MoveAttackTrigger()
    {
        if (playerInputScript == null) return;

        float aimAngle = playerInputScript.aimObject.rotation.eulerAngles.z;
        float[] angles = { 0, 45, 90, 135, 180, 225, 270, 315 };

        float closestAngle = angles[0];
        float smallestDifference = Mathf.Abs(Mathf.DeltaAngle(aimAngle, closestAngle));

        foreach (float angle in angles)
        {
            float difference = Mathf.Abs(Mathf.DeltaAngle(aimAngle, angle));

            if (difference < smallestDifference)
            {
                smallestDifference = difference;
                closestAngle = angle;
            }
        }
        if(closestAngle == 315 || closestAngle == 270 || closestAngle == 225)
        {
            atkColliderTrigger.offset = new Vector2(downXOffset, downYOffset);
            atkColliderTrigger.size = new Vector2(downWidth, downHeight);
            atkColliderTrigger.transform.localRotation = Quaternion.Euler(0, 0, 270);
        }
        else
        {
            atkColliderTrigger.offset = new Vector2(defaultXOffset, defaultYOffset);
            atkColliderTrigger.size = new Vector2(defaultWidth, defaultHeight);
            atkColliderTrigger.transform.localRotation = Quaternion.Euler(0, 0, closestAngle);
        }
    }
}