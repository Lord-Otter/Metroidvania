using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVelocity : MonoBehaviour
{    
    [HideInInspector] public Rigidbody rigidBody;
    [HideInInspector] public Vector3 velocity;

    public float gravity = 15f;
    public float jumpGravity = 10f;
    public float maxFallSpeed = 20f;

        private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Physics();
    }

    public void Physics()
    {
        velocity = rigidBody.linearVelocity;

        velocity.y -= gravity * Time.fixedDeltaTime;
        velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
        
        rigidBody.linearVelocity = velocity;
    }
}
