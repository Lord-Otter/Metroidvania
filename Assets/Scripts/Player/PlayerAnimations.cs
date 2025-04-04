using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private PlayerChecks playerChecks;
    private PlayerVelocity playerVelocity;
    private PlayerMovement playerMovement;
    private PlayerInputs playerInputs;
    private TimeManager timeManager;
    private Animator animator;

    public float rotationSpeed;

    private void Awake()
    {
        playerChecks = GetComponentInParent<PlayerChecks>();
        playerVelocity = GetComponentInParent<PlayerVelocity>();
        playerInputs = GetComponentInParent<PlayerInputs>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        timeManager = GameObject.Find("Time_Manager").GetComponent<TimeManager>();        
        animator = GetComponent<Animator>();
    }

    void Start()
    {

    }

    void Update()
    {
        //Animations();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(timeManager.worldPause)
        {
            return;
        }

        if(timeManager.tpPause)
        {
            return;
        }

        FaceTravelDirection();
    }

    /*void Animations()
    {
        float speedParam = Mathf.Abs(playerVelocity.velocity.x) / playerMovement.maxMoveSpeed;

        if (playerInputs.movingRight)
        {
            if (playerChecks.IsGrounded())
            {                
                if (playerChecks.isFacingRight)
                {
                    animator.SetFloat("Speed", speedParam);
                }
                else
                {
                    animator.SetFloat("Speed", -speedParam);
                }          
            }
            else
            {
                animator.SetFloat("Speed", 0);
            }
        }
        else if (playerInputs.movingLeft)
        {
            if (playerChecks.IsGrounded())
            {
                if (!playerChecks.isFacingRight)
                {
                    animator.SetFloat("Speed", speedParam);
                }
                else
                {
                    animator.SetFloat("Speed", -speedParam);
                } 
            }
            else
            {
                animator.SetFloat("Speed", 0);
            }

        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }*/

    void FaceTravelDirection()
    {
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, playerChecks.isFacingRight ? 120 : 240, 0), rotationSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, playerChecks.isFacingRight ? 1 : 179, 0), rotationSpeed * Time.fixedDeltaTime);
    }
}