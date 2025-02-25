using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerInputScript : MonoBehaviour
{
    private AttackScript attackScript;
    private BasicMovementScript basicMovementScript;
    private DashScript dashScript;
    private PlayerChecks playerChecks;
    private PlayerVelocity playerVelocity;

    public Rigidbody rigidBody;

    // Movement
    public float moveHorizontal;

    public bool movingRight;
    public bool movingLeft;
    public bool jumping;
    public bool highJumping;
    public bool airJumping;

    // Aiming
    public float aimHorizontal;
    public float aimVertical;

    // Attacking
    public bool attacking;

    // Start is called before the first frame update
    void Start()
    {
        attackScript = GetComponent<AttackScript>();
        basicMovementScript = GetComponent<BasicMovementScript>();
        dashScript = GetComponent<DashScript>();
        playerChecks = GetComponent<PlayerChecks>();
        playerVelocity = GetComponent<PlayerVelocity>();
    }

    // Update is called once per frame
    void Update()
    {
        Controls();
    }

    void Controls()
    {
        // Standard Movement
        // Binary Movement
        movingRight = Input.GetAxisRaw("Horizontal") > 0;
        movingLeft = Input.GetAxisRaw("Horizontal") < 0;

        // Analog Movement
        moveHorizontal = Input.GetAxis("Horizontal");

        // Jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (basicMovementScript.canGroundJump && !dashScript.isDashing)
            {
                jumping = true;
            }
            else if (!basicMovementScript.canGroundJump && !dashScript.isDashing && (basicMovementScript.airJumps > 0))
            {
                airJumping = true;
            }
        }

        highJumping = Input.GetButton("Jump");

        // Special Moves
        dashScript.inputDash = Input.GetButton("Dash");

        // Aiming
        aimHorizontal = Input.GetAxisRaw("Horizontal");
        aimVertical = Input.GetAxisRaw("Vertical");

        // Attack
        attacking = Input.GetButton("Attack1");
    }
}
