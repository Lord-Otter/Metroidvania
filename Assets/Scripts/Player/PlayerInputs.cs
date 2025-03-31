using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerInputs : MonoBehaviour
{
    private PlayerAttack playerAttack;
    private PlayerMovement playerMovement;
    private PlayerChecks playerChecks;
    private PlayerVelocity playerVelocity;

    private TimeManager timeManager;
    
    public enum AimMode { Mouse, Stick, Move }
    [Header("Aiming Mode")]
    public AimMode aimMode;
    [HideInInspector] public Transform aimObject;

    [Header("Aiming")]
    public float aimHorizontalR;
    public float aimVerticalR;
    public float aimHorizontalL;
    public float aimVerticalL;
    [HideInInspector] public Vector3 inputDirectionR;
    [HideInInspector] public Vector3 mousePosition;

    [Header("Movement")]
    public float moveHorizontal;
    public float moveVertical;

    public bool movingRight;
    public bool movingLeft;
    public bool jumping;
    public bool highJumping;
    public bool airJumping;
    public bool dashing;
    public bool teleporting;

    [Header("Attacks")]
    public bool attacking;

    // Missinput Prevention
    private float inputHoldTime = 0f;
    private bool isHoldingInput = false;
    [Header("Missinput Prevention")]
    public float stickDeadZone;
    public float inputHoldThreshold = 0.025f; // 1 second threshold

    private float aimInputHoldTime = 0f;
    private bool isHoldingAimInput = false;
    public float aimInputHoldThreshold = 0.025f; // Delay threshold for aiming

    private void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerMovement = GetComponent<PlayerMovement>();
        playerChecks = GetComponent<PlayerChecks>();
        playerVelocity = GetComponent<PlayerVelocity>();

        timeManager = GameObject.Find("Time_Manager").GetComponent<TimeManager>();

        aimObject = GameObject.Find("Attack_Direction").transform;
    }

    void Start()
    {

    }

    void Update()
    {
        if(timeManager.worldPause)
        {
            return;
        }        
        
        if (playerAttack.canAimAttack)
        {
            AimModeFunction();
        }

        if(timeManager.tpPause)
        {
            return;
        }

        Controls();


    }

    #region Inputs
    void Controls()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(horizontalInput) > stickDeadZone || Mathf.Abs(verticalInput) > stickDeadZone)
        {
            if (!isHoldingInput)
            {
                isHoldingInput = true;
                inputHoldTime = 0f;
            }
            else
            {
                inputHoldTime += Time.deltaTime;
            }
        }
        else
        {
            isHoldingInput = false;
            inputHoldTime = 0f;
        }

        if (inputHoldTime >= inputHoldThreshold)
        {
            moveHorizontal = horizontalInput;
            moveVertical = verticalInput;
            movingRight = horizontalInput > stickDeadZone;
            movingLeft = horizontalInput < -stickDeadZone;

            aimHorizontalL = horizontalInput;
            aimVerticalL = verticalInput;
        }
        else
        {
            moveHorizontal = 0f;
            moveVertical = 0f;
            movingRight = false;
            movingLeft = false;
            aimHorizontalL = 0f;
            aimVerticalL = 0f;
        }

        // Aiming Stick Input Delay
        float horizontalInputR = Input.GetAxisRaw("RightStickX");
        float verticalInputR = Input.GetAxisRaw("RightStickY");

        if (Mathf.Abs(horizontalInputR) > stickDeadZone || Mathf.Abs(verticalInputR) > stickDeadZone)
        {
            if (!isHoldingAimInput)
            {
                isHoldingAimInput = true;
                aimInputHoldTime = 0f;
            }
            else
            {
                aimInputHoldTime += Time.deltaTime;
            }
        }
        else
        {
            isHoldingAimInput = false;
            aimInputHoldTime = 0f;
        }

        if (aimInputHoldTime >= aimInputHoldThreshold)
        {
            aimHorizontalR = horizontalInputR;
            aimVerticalR = verticalInputR;
        }
        else
        {
            aimHorizontalR = 0f;
            aimVerticalR = 0f;
        }

        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (playerMovement.canJump && !playerMovement.isDashing)
            {
                jumping = true;
            }
            else if (!playerMovement.canJump && !playerMovement.isDashing && (playerMovement.airJumps > 0))
            {
                airJumping = true;
            }
        }

        highJumping = Input.GetButton("Jump");
        
        // Dash
        dashing = Input.GetButton("Dash");

        // Attack
        attacking = (aimMode == AimMode.Stick) 
        ? Input.GetAxisRaw("TriggerAttack") > 0.1f 
        : Input.GetButton("ButtonAttack");

        // Teleport
        if(Input.GetButtonDown("Teleport") && playerAttack.teleportProjectile.Count > 0)
        {
            teleporting = true; // teleporting is set to false in playerMovement.Teleporting()
        }
    }
    #endregion



    #region Aiming
    void AimModeFunction()
    {
        switch (aimMode)
        {
            case AimMode.Mouse:
                MouseAim();
                break;
            case AimMode.Stick:
                StickAim();
                break;
            case AimMode.Move:
                MoveAim();
                break;
        }
    }

    void MouseAim()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        float distance;

        if (plane.Raycast(ray, out distance))
        {
            Vector3 worldMousePosition = ray.GetPoint(distance);
            mousePosition = worldMousePosition - transform.position;
            float angle = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;

            aimObject.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void StickAim()
    {
        inputDirectionR = new Vector3(aimHorizontalR, aimVerticalR, 0f).normalized;

        if (inputDirectionR.magnitude > 0f)
        {
            float angle = Mathf.Atan2(inputDirectionR.y, inputDirectionR.x) * Mathf.Rad2Deg;
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

    void MoveAim()
    {
        Vector3 inputDirection = new Vector3(aimHorizontalL, aimVerticalL, 0f).normalized;

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
    #endregion
}