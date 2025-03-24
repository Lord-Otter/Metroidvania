using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVelocity : MonoBehaviour
{
    private PlayerAttack playerAttack;
    private PlayerMovement playerMovement;
    private PlayerChecks playerChecks;
    private PlayerInputs playerInputs;
    private TimeManager timeManager;

    [HideInInspector] public Rigidbody rigidBody;
    [HideInInspector] public Vector3 velocity;

    // Physics
    public float gravity = 17.5f;
    public float jumpGravity = 15f;
    public float maxFallSpeed = 20f;
    public float wallSlideGravity = 8f;
    public float wallSlideMaxSpeed = 10f;

    [Header("Pause")]
    private Vector3 pauseVelocity;

    private void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerMovement = GetComponent<PlayerMovement>();
        playerChecks = GetComponent<PlayerChecks>();
        playerInputs = GetComponent<PlayerInputs>();
        timeManager = GameObject.Find("Time_Manager").GetComponent<TimeManager>();

        rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(timeManager.worldPause)
        {
            OnPause();
            return;
        }

        if(timeManager.tpPause)
        {
            OnPause();
            return;
        }

        Physics();        
    }

    public void Physics()
    {
        velocity = rigidBody.linearVelocity;

        if((playerChecks.AttachedToRWall() || playerChecks.AttachedToLWall()) && velocity.y < 0)  // Gravity while wall sliding
        {
            velocity.y -= wallSlideGravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, -wallSlideMaxSpeed);
        }
        else if (!playerMovement.isDashing &&  playerInputs.highJumping && velocity.y > 0) // Gravity while jumping
        {
            velocity.y -= jumpGravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        }
        else if (!playerMovement.isDashing) // Gravity normally
        {
            velocity.y -= gravity * Time.fixedDeltaTime;
            velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        }

        rigidBody.linearVelocity = velocity;
    }

    void OnPause()
    {
        pauseVelocity = velocity;
        rigidBody.linearVelocity = Vector3.zero;
    }

    public void OnResume()
    {
        rigidBody.linearVelocity = pauseVelocity;
    }
}
