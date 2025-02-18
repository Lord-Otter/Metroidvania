using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerInputScript : MonoBehaviour
{
    public BasicMovementScript basicMovementScript;
    public DashScript dashScript;
    public PlayerVelocity playerVelocity;
    public PlayerChecks playerChecks;
    public AttackScript attackScript;

    public bool movingRight;
    public bool movingLeft;
    public bool jumping;
    public bool highJumping;
    public bool airJumping;

    // Start is called before the first frame update
    void Start()
    {
        playerVelocity = GetComponent<PlayerVelocity>();
        dashScript = GetComponent<DashScript>();
        basicMovementScript = GetComponent<BasicMovementScript>();
        playerChecks = GetComponent<PlayerChecks>();
        attackScript = GetComponent<AttackScript>();
    }

    // Update is called once per frame
    void Update()
    {
        Controls();
    }

    void Controls()
    {
        // Standard Movement
        movingRight = Input.GetKey(KeyCode.D);
        movingLeft = Input.GetKey(KeyCode.A);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (basicMovementScript.canGroundJump && !dashScript.isDashing)
            {
                jumping = true;
            }
            else if (!playerChecks.IsGroundedCheck() && !dashScript.isDashing && (basicMovementScript.airJumps > 0))
            {
                airJumping = true;
            }
        }

        highJumping = Input.GetKey(KeyCode.Space);

        // Special Moves
        dashScript.inputDash = Input.GetKey(KeyCode.F);

    }
}
