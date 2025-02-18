using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public PlayerInputScript playerInputScript;
    public DashScript dashScript;
    public PlayerVelocity playerVelocity;
    public PlayerChecks playerChecks;
    public BasicMovementScript basicMovementScript;

    public Transform aimObject;

    public bool mouseAiming;

    // Start is called before the first frame update
    void Start()
    {
        playerInputScript = GetComponent<PlayerInputScript>();
        dashScript = GetComponent<DashScript>();
        basicMovementScript = GetComponent<BasicMovementScript>();
        playerChecks = GetComponent<PlayerChecks>();
        playerVelocity = GetComponent<PlayerVelocity>();

        aimObject = GameObject.Find("Attack_Direction").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseAiming)
        {
            LookTowardMouse();
        }
        else
        {

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
}
