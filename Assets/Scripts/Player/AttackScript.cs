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

    public Transform aimObject;

    // Raycast
    public float coneAngle = 45f;
    public float coneRadius = 5f;
    public int rayCount = 10;
    public LayerMask targetLayer;

    void Start()
    {
        basicMovementScript = GetComponent<BasicMovementScript>();
        dashScript = GetComponent<DashScript>();
        playerChecks = GetComponent<PlayerChecks>();
        playerInputScript = GetComponent<PlayerInputScript>();
        playerVelocity = GetComponent<PlayerVelocity>();

        aimObject = GameObject.Find("Attack_Direction").transform;
    }

    void FixedUpdate()
    {
        // Draw Rays
        if (playerInputScript.attacking)
        {
            CastConeRay(transform.position, aimObject.right);
        }
    }

    void CastConeRay(Vector3 origin, Vector3 direction)
    {
        float halfAngle = coneAngle / 2f;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = -halfAngle + (i / (float)(rayCount - 1)) * coneAngle;
            Vector3 rayDirection = Quaternion.Euler(0, 0, angle) * direction;

            Debug.DrawRay(origin, rayDirection * coneRadius, Color.green, 0.5f);

            RaycastHit[] hits = Physics.RaycastAll(origin, rayDirection, coneRadius, targetLayer);

            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    Debug.DrawLine(origin, hit.point, Color.red, 0.5f);
                    Debug.Log("Hit: " + hit.collider.name);
                }
            }
        }
    }
}