using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    public GameObject trainingProjectile; // The projectile prefab
    public float fireDelay = 0.5f; // Cooldown time between shots
    private float nextFireTime = 0f; // Time when next shot can be fired
    
    public Vector3 positionOffset; // Offset for position
    public Vector3 rotationOffset; // Offset for rotation

    void Update()
    {
        if (Input.GetButton("ShootProjectile") && Time.time >= nextFireTime)
        {
            SpawnProjectile();
            nextFireTime = Time.time + fireDelay; // Set next fire time
        }
    }

    void SpawnProjectile()
    {
        Quaternion adjustedRotation = transform.rotation * Quaternion.Euler(rotationOffset);
        Vector3 adjustedPosition = transform.position + transform.TransformDirection(positionOffset);
        Instantiate(trainingProjectile, adjustedPosition, adjustedRotation);
    }
}

