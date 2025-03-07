using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileVelocity : MonoBehaviour
{
    [HideInInspector] public Rigidbody rigidBody;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public new Transform transform;

    public float speed;
    //public float maxSpeed;
    public float speedMultiplier;

    private void Awake()
    {
        transform = GetComponent<Transform>();
        rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Destroy(gameObject, 20);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Traveling();
    }

    void Traveling()
    {
        velocity = rigidBody.velocity;

        velocity = transform.right * speed;

        rigidBody.velocity = velocity;
    }

    public void ChangeTrajectory(float angle)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
