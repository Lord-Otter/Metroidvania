using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualRotation : MonoBehaviour
{
    private PlayerChecks playerChecks;

    public float rotationSpeed;

    private void Awake()
    {
        playerChecks = GetComponentInParent<PlayerChecks>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FaceTravelDirection();
    }

    void FaceTravelDirection()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, playerChecks.isFacingRight ? 1 : 179, 0), rotationSpeed * Time.fixedDeltaTime);
    }
}
