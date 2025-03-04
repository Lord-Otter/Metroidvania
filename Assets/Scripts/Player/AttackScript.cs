using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    [Header("Player Components")]
    private PlayerMovement playerMovement;
    private PlayerChecks playerChecks;
    private PlayerInputs playerInputs;
    private PlayerVelocity playerVelocity;

    [Header("Player Attack")]
    public int attackDamage;
    public float attackBuildUpTime;
    public float damageDuration;
    public float attackResetTime;
    public float enemyKnockBack;
    public float playerKnockBack;
    public bool canAttack = true;
    [HideInInspector]public bool canAimAttack = true;
    [SerializeField] private LayerMask attackableLayers;
    public List<GameObject> hitTargets = new List<GameObject>();

    private BoxCollider attackTrigger;
    [Header("Collider")]
    public float defaultXOffset;
    public float defaultYOffset;
    public float defaultWidth;
    public float defaultHeight;
    public float downXOffset;
    public float downYOffset;
    public float downWidth;
    public float downHeight;
    private float triggerDepth = 3;

    [Header("Testing")]
    private Renderer aimStickRenderer;


    private void Awake()
    {
        // Player Components
        playerMovement = GetComponent<PlayerMovement>();
        playerChecks = GetComponent<PlayerChecks>();
        playerInputs = GetComponent<PlayerInputs>();
        playerVelocity = GetComponent<PlayerVelocity>();

        attackTrigger = GetComponentInChildren<BoxCollider>();

        // Testing
        aimStickRenderer = GameObject.Find("Attack_Direction_Visual").GetComponent<Renderer>();
    }

    void Start()
    {
        attackTrigger.enabled = false; // Makes sure the attack trigger is disabled at the start
    }

    void Update()
    {
        if (canAimAttack) // Makes sure the player can't redirect their attack while it's happening
        {
            RotateAttackTrigger();
        }
        AttackFunction();    
    }

    void FixedUpdate()
    {

    }

    #region Attacking
    void AttackFunction()
    {
        if (playerInputs.attacking && canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        // Attack is building up
        aimStickRenderer.material.color = new Color(1, 1, 0); // Makes attack stick yellow for visual aid in testing
        hitTargets.Clear();
        canAttack = false;
        //Code to initiate attack animation
        yield return new WaitForSeconds(attackBuildUpTime); // Time before attack deals damage

        // Attack is now able to damage
        aimStickRenderer.material.color = new Color(0, 1, 0); // Makes attack stick green for visual aid in testing
        canAimAttack = false;
        attackTrigger.enabled = true;
        yield return new WaitForSeconds(damageDuration); // Time before attack no longer deals damage

        // Attack can no longer damage
        aimStickRenderer.material.color = new Color(1, 0, 0); // Makes attack stick red for visual aid in testing
        canAimAttack = true;
        attackTrigger.enabled = false;
        yield return new WaitForSeconds(attackResetTime); // Time before player can attack again

        // New attack can now be started
        aimStickRenderer.material.color = new Color(1, 1, 1); // Makes attack stick white for visual aid in testing
        canAttack = true;
    }

    private void OnTriggerEnter(Collider target)
    {
        if (attackTrigger.enabled && ((1 << target.gameObject.layer) & attackableLayers.value) != 0)
        {            
            if (!hitTargets.Contains(target.gameObject))
            {
                hitTargets.Add(target.gameObject);
                if (target.CompareTag("Enemy"))
                {
                    ApplyDamage(target.gameObject); // Deals damage if the target is an enemy
                }
                else if (target.CompareTag("Projectile"))
                {
                    DeflectProjectile(target.gameObject); // Deflects if the target is a projectile
                }
                
            }            
        }
    }

    void ApplyDamage(GameObject target)
    {
        TargetHealth targetHealth = target.GetComponent<TargetHealth>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage);
        }
    }

    void DeflectProjectile(GameObject target)
    {
        
    }
    #endregion

    #region Move The Trigger
    void RotateAttackTrigger()
    {
        if (playerInputs == null) return;

        float aimAngle = playerInputs.aimObject.rotation.eulerAngles.z;
        float[] angles = {  0, 22.5f, 45, 67.5f, 90, 112.5f, 135, 157.5f, 180, 
                            225,    // Down left
                            270,    // Down down
                            315 };  //Down Right

        float closestAngle = angles[0];
        float smallestDifference = Mathf.Abs(Mathf.DeltaAngle(aimAngle, closestAngle));

        foreach (float angle in angles)
        {
            float difference = Mathf.Abs(Mathf.DeltaAngle(aimAngle, angle));

            if (difference < smallestDifference)
            {
                smallestDifference = difference;
                closestAngle = angle;
            }
        }
        if(closestAngle == 225) // Down left
        {
            attackTrigger.center = new Vector3(downXOffset, downYOffset, 0);
            attackTrigger.size = new Vector3(downWidth, downHeight, triggerDepth);
            attackTrigger.transform.localRotation = Quaternion.Euler(0, 0, closestAngle);
        }
        else if(closestAngle == 270) // Down down
        {
            attackTrigger.center = new Vector3(downXOffset, downYOffset, 0);
            attackTrigger.size = new Vector3(downWidth, downHeight, triggerDepth);
            attackTrigger.transform.localRotation = Quaternion.Euler(0, 0, closestAngle);
        }
        else if(closestAngle == 315) //Down right
        {
            attackTrigger.center = new Vector3(downXOffset, downYOffset, 0);
            attackTrigger.size = new Vector3(downWidth, downHeight, triggerDepth);
            attackTrigger.transform.localRotation = Quaternion.Euler(0, 0, closestAngle);
        }
        else
        {
            attackTrigger.center = new Vector3(defaultXOffset, defaultYOffset);
            attackTrigger.size = new Vector3(defaultWidth, defaultHeight, triggerDepth);
            attackTrigger.transform.localRotation = Quaternion.Euler(0, 0, closestAngle);
        }
    }
    #endregion
}