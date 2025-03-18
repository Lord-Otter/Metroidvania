using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileVelocity : MonoBehaviour
{
    private PlayerAttack playerAttack;

    [HideInInspector] public Rigidbody rigidBody;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public new Transform transform;

    public float speed;
    //public float maxSpeed;
    public float speedMultiplier;
    public float despawnTimer;
    public bool isCritical = false;
    public float angleDifference;

    private void Awake()
    {
        playerAttack = GameObject.Find("Player").GetComponent<PlayerAttack>();

        transform = GetComponent<Transform>();
        rigidBody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Destroy(gameObject, despawnTimer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        if (playerAttack.teleportProjectile.Contains(gameObject))
        {
            playerAttack.teleportProjectile.Clear();
        }
    }

    void FixedUpdate()
    {
        Traveling();
    }

    void Traveling()
    {
        velocity = rigidBody.linearVelocity;

        velocity = transform.right * speed;

        rigidBody.linearVelocity = velocity;
    }

    public void ChangeTrajectory(float angle, float angleDiff)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle);
        angleDifference = angleDiff;
    }
}
