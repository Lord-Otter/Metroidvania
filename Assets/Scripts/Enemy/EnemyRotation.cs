using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRotation : MonoBehaviour
{
    private EnemyChecks enemyChecks;
    private EnemyVelocity enemyVelocity;
    private EnemyMovement enemyMovement;
    private TimeManager timeManager;
    private Animator animator;

    public float rotationSpeed;

    private void Awake()
    {
        enemyChecks = GetComponentInParent<EnemyChecks>();
        enemyVelocity = GetComponentInParent<EnemyVelocity>();
        enemyMovement = GetComponentInParent<EnemyMovement>();
        timeManager = GameObject.Find("Time_Manager").GetComponent<TimeManager>();
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

    void FaceTravelDirection()
    {
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, enemyChecks.isFacingRight ? 120 : 240, 0), rotationSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, enemyChecks.isFacingRight ? 1 : 179, 0), rotationSpeed * Time.fixedDeltaTime);
    }
}
