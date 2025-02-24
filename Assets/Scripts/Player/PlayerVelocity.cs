using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVelocity : MonoBehaviour
{
    public PlayerInputScript playerInputScript;
    public DashScript dashScript;
    public BasicMovementScript basicMovementScript;
    public PlayerChecks playerChecks;
    public AttackScript attackScript;

    private Rigidbody rigidBody;

    public Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        playerInputScript = GetComponent<PlayerInputScript>();
        dashScript = GetComponent<DashScript>();
        basicMovementScript = GetComponent<BasicMovementScript>();
        playerChecks = GetComponent<PlayerChecks>();
        attackScript = GetComponent<AttackScript>();

        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        VelocityCalculations();
    }

    public void VelocityCalculations()
    {

    }
}
