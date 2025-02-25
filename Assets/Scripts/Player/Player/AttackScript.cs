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

    public bool mouseAiming;

    // Raycast
    public float coneAngle = 45f;
    public float outerRadius = 5f;
    public float innerRadius = 1f;
    public LayerMask targetLayer;

    // Circle Debug Settings
    public int circleResolution = 36;

    void Start()
    {
        basicMovementScript = GetComponent<BasicMovementScript>();
        dashScript = GetComponent<DashScript>();
        playerChecks = GetComponent<PlayerChecks>();
        playerInputScript = GetComponent<PlayerInputScript>();
        playerVelocity = GetComponent<PlayerVelocity>();

        aimObject = GameObject.Find("Attack_Direction").transform;
    }

    void Update()
    {
        if (mouseAiming)
        {
            LookTowardMouse();
        }
        else
        {
            AimWithoutMouse();
        }

        if (playerInputScript.attacking)
        {
            CastFanRay(transform.position, aimObject.up);
            DrawDebugCircle(transform.position, outerRadius, Color.blue);
            DrawDebugCircle(transform.position, innerRadius, Color.cyan);
        }
    }

    void LookTowardMouse()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        float distance;

        if (plane.Raycast(ray, out distance))
        {
            Vector3 worldMousePosition = ray.GetPoint(distance);
            Vector3 direction = worldMousePosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            aimObject.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void AimWithoutMouse()
    {
        Vector3 inputDirection = new Vector3(playerInputScript.aimHorizontal, playerInputScript.aimVertical, 0f).normalized;

        if (inputDirection.magnitude > 0f)
        {
            float angle = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg;
            aimObject.rotation = Quaternion.Euler(0, 0, angle);
        }
        else if (playerChecks.isFacingRight)
        {
            aimObject.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            aimObject.rotation = Quaternion.Euler(0, 0, 180);
        }
    }

    void CastFanRay(Vector3 origin, Vector3 direction)
    {
        float halfAngle = coneAngle / 2f;

        Vector3 leftRayDirection = Quaternion.Euler(0, 0, -halfAngle) * direction;
        Vector3 rightRayDirection = Quaternion.Euler(0, 0, halfAngle) * direction;

        CastSingleRay(origin, leftRayDirection, outerRadius);
        CastSingleRay(origin, rightRayDirection, outerRadius);

        CastCircle(origin, outerRadius);
        CastCircle(origin, innerRadius);
    }

    void CastSingleRay(Vector3 origin, Vector3 direction, float length)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, length, targetLayer);

        if (hit.collider != null)
        {
            Debug.DrawLine(origin, hit.point, Color.red, 0.5f);
            Debug.Log("Edge Ray Hit: " + hit.collider.name);
        }
        else
        {
            Debug.DrawRay(origin, direction * length, Color.green, 0.5f);
        }
    }

    void CastCircle(Vector3 origin, float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, targetLayer);

        foreach (Collider2D hit in hits)
        {
            float distance = Vector2.Distance(origin, hit.transform.position);
            Vector3 directionToTarget = (hit.transform.position - origin).normalized;

            float angle = Vector3.Angle(aimObject.up, directionToTarget);
            if (angle <= coneAngle / 2 && distance >= innerRadius)
            {
                Debug.Log("Circle Hit: " + hit.name);
            }
        }
    }

    void DrawDebugCircle(Vector3 origin, float radius, Color color)
    {
        int segments = circleResolution;
        float angleStep = 360f / segments;
        Vector3 lastPoint = origin + (aimObject.right * radius);

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            float rad = Mathf.Deg2Rad * angle;
            Vector3 newPoint = origin + (Mathf.Cos(rad) * aimObject.right + Mathf.Sin(rad) * aimObject.up) * radius;

            Debug.DrawLine(lastPoint, newPoint, color, 0.5f);
            lastPoint = newPoint;
        }
    }
}
