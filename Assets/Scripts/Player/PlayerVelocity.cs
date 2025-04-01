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
    [HideInInspector]public Vector3 unscaledVelocity;

    [Header("Physics")]
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
        velocity = rigidBody.linearVelocity / timeManager.customTimeScale;

        if((playerChecks.AttachedToWallRight() || playerChecks.AttachedToWallLeft()) && velocity.y < 0)  
        {
            velocity.y -= wallSlideGravity * Time.fixedDeltaTime * timeManager.customTimeScale;
            velocity.y = Mathf.Max(velocity.y, -wallSlideMaxSpeed);
        }
        else if (!playerMovement.isDashing && playerInputs.highJumping && velocity.y > 0) 
        {
            velocity.y -= jumpGravity * Time.fixedDeltaTime * timeManager.customTimeScale;
            velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        }
        else if (!playerMovement.isDashing) 
        {
            velocity.y -= gravity * Time.fixedDeltaTime * timeManager.customTimeScale;
            velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        }

        unscaledVelocity = velocity; // Store unscaled velocity
        rigidBody.linearVelocity = unscaledVelocity * timeManager.customTimeScale; // Apply scaling
    }

    public void AdjustVelocityForTimeScale()
    {
        rigidBody.linearVelocity = unscaledVelocity * timeManager.customTimeScale;
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
