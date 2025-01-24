using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlScript : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float jumpForce = 10f;         // Force for a light jump (tap)
    public float maxHoldJumpForce = 20f;  // Maximum force for holding the jump key
    public float jumpCancelForce = 10f;   // Force to cancel upward motion when releasing jump
    private float jumpHoldTime = 0f;      // Timer for how long the space key is held
    private bool isJumping = false;       // Whether the player is jumping
    private bool isJumpCanceled = false;  // Whether the jump is canceled
    private Rigidbody rigidBody;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        player = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Movement();
        if (IsGroundedCheck())
        {
            isJumping = false; // Allow jumping again when grounded
            isJumpCanceled = false; // Reset jump cancel flag
            jumpHoldTime = 0f; // Reset jump hold time when grounded
        }
    }

    void Movement()
    {
        Vector3 movement = Vector3.zero;

        // Check if player is grounded
        if (gameObject.name == "Player")
        {
            // Light tap for jump
            if (IsGroundedCheck() && Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                isJumping = true;
                StartCoroutine(HighJumpCheck()); // Start coroutine to check for holding jump
                rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Light jump on tap
            }

            // If space is held and the player is in the air
            if (!IsGroundedCheck() && Input.GetKey(KeyCode.Space) && !isJumpCanceled)
            {
                jumpHoldTime += Time.fixedDeltaTime; // Track how long the key is held down

                // Apply a gradual higher jump force based on how long the player holds the space key
                if (jumpHoldTime < 1f) // Optional: limit the max time the jump can be held
                {
                    rigidBody.AddForce(Vector3.up * maxHoldJumpForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
                }
            }

            // Jump cancel when space is released or after a certain condition
            if (!Input.GetKey(KeyCode.Space) && rigidBody.velocity.y > 0 && !IsGroundedCheck())
            {
                // Cancel upward velocity when player releases the space key early
                rigidBody.AddForce(Vector3.down * jumpCancelForce, ForceMode.Force);
                isJumpCanceled = true;
            }

            // Reset jump cancel if grounded
            if (IsGroundedCheck())
            {
                isJumpCanceled = false;
                jumpHoldTime = 0f; // Reset jump hold time when grounded
            }

            // Horizontal movement (left and right)
            if (Input.GetKey(KeyCode.A))
            {
                movement.x = -1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                movement.x = 1f;
            }
        }

        // Apply force to move the player horizontally
        rigidBody.AddForce(movement * moveSpeed, ForceMode.Force);
    }

    IEnumerator HighJumpCheck()
    {
        yield return new WaitForSeconds(0.2f); // Small delay to allow for holding space
        if (Input.GetKey(KeyCode.Space))
        {
            // Player is holding the space key longer, you can trigger higher jump logic here
            isJumping = true;
        }
    }

    bool IsGroundedCheck()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1f);
    }
}
