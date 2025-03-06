using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVelocity : MonoBehaviour
{
    private AttackScript attackScript;
    private PlayerMovement playerMovement;
    private PlayerChecks playerChecks;
    private PlayerInputs playerInputs;

    [HideInInspector] public Rigidbody rigidBody;
    [HideInInspector] public Vector3 velocity;

    // Physics
    public float gravity = 15f;
    public float jumpGravity = 10f;
    public float maxFallSpeed = 20f;

    private void Awake()
    {
        attackScript = GetComponent<AttackScript>();
        playerMovement = GetComponent<PlayerMovement>();
        playerChecks = GetComponent<PlayerChecks>();
        playerInputs = GetComponent<PlayerInputs>();

        rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Physics();
    }

    public void Physics()
    {
        velocity = rigidBody.velocity;

        if (!playerMovement.isDashing &&  playerInputs.highJumping && velocity.y > 0)
        {
            velocity.y -= jumpGravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        }
        else if (!playerMovement.isDashing)
        {
            velocity.y -= gravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        }

        rigidBody.velocity = velocity;
    }
}
