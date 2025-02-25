using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVelocity : MonoBehaviour
{
    private AttackScript attackScript;
    private BasicMovementScript basicMovementScript;
    private DashScript dashScript;
    private PlayerChecks playerChecks;
    private PlayerInputScript playerInputScript;

    public Rigidbody rigidBody;
    public Vector3 velocity;

    // Physics
    public int gravity;
    public int maxFallSpeed;

    // Start is called before the first frame update
    void Start()
    {
        attackScript = GetComponent<AttackScript>();
        basicMovementScript = GetComponent<BasicMovementScript>();
        dashScript = GetComponent<DashScript>();
        playerChecks = GetComponent<PlayerChecks>();
        playerInputScript = GetComponent<PlayerInputScript>();

        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Physics();
    }

    public void Physics()
    {
        velocity = rigidBody.velocity;

        if (!dashScript.isDashing && !playerChecks.IsGrounded())
        {
            velocity.y -= gravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        }

        rigidBody.velocity = velocity;
    }
}
