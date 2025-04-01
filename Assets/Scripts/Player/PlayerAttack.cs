using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement playerMovement;
    private PlayerChecks playerChecks;
    private PlayerInputs playerInputs;
    private PlayerVelocity playerVelocity;
    private TimeManager timeManager;

    [Header("Player Attack")]
    public int attackDamage;
    public float attackBuildUpTime;
    public float damageDuration;
    public float attackCooldown;
    public float enemyKnockBack;
    public float playerKnockBack;
    public bool canAttack = true;
    private bool isAttacking = false;
    private float attackStartTime;
    [HideInInspector]public bool canAimAttack = true;
    [SerializeField] private LayerMask attackableLayers;
    public List<GameObject> hitTargets = new List<GameObject>();

    private BoxCollider attackTrigger;

    [Header("Projectile Deflection")]
    public List<(GameObject projectile, float absAngleDifference)> deflectedProjectiles = new List<(GameObject, float)>();
    public List<(GameObject projectile, float absAngleDifference)> critProjectiles = new List<(GameObject, float)>();
    public List<(GameObject projectile, float absAngleDifference)> tpProjectile = new List<(GameObject, float)>();
    public List<GameObject> teleportProjectile = new List<GameObject>();
    public bool deflectSpread;


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
    private float closestAngle;
    [HideInInspector]public float angle;

    [Header("Testing")]
    private Renderer aimStickRenderer;
    public GameObject swoosh; // Temporary sheisse
    private bool canSpawnSwoosh;
    private float[] angles = {  0, 22.5f, 45, 67.5f, 90, 112.5f, 135, 157.5f, 180, 
                            202.5f, 225,    // Down left
                            270,            // Down down
                            315, 337.5f };  // Down right


    private void Awake()
    {
        // References
        playerMovement = GetComponent<PlayerMovement>();
        playerChecks = GetComponent<PlayerChecks>();
        playerInputs = GetComponent<PlayerInputs>();
        playerVelocity = GetComponent<PlayerVelocity>();

        attackTrigger = GetComponentInChildren<BoxCollider>();

        timeManager = GameObject.Find("Time_Manager").GetComponent<TimeManager>();

        // Testing
        aimStickRenderer = GameObject.Find("Attack_Direction_Visual").GetComponent<Renderer>();


    }

    void Start()
    {
        attackTrigger.enabled = false; // Makes sure the attack trigger is disabled at the start
    }

    void Update()
    {
        if(timeManager.worldPause)
        {
            return;
        }        
        
        if (canAimAttack) // Makes sure the player can't redirect their attack while it's happening
        {
            RotateAttackTrigger();
        }

        if(timeManager.tpPause)
        {
            return;
        }


        AttackFunction();
    }

    void FixedUpdate()
    {
        AttackSequence();
    }

    #region Attacking
    void AttackFunction()
    {
        if (playerInputs.attacking && canAttack)
        {
            //StartCoroutine(PerformAttack());
            attackStartTime = Time.time;
            isAttacking = true;
            canAttack = false;     
        }        
    }

    void AttackSequence()
    {
        if(isAttacking)
        {
            if(Time.time - attackStartTime < attackBuildUpTime / timeManager.customTimeScale)
            {
                AttackBuildUpPhase();
            }
            else if(Time.time - attackStartTime < (damageDuration + attackBuildUpTime) / timeManager.customTimeScale)
            {
                AttackDamagePhase();
            }
            else if(Time.time - attackStartTime < (attackCooldown + damageDuration + attackBuildUpTime) / timeManager.customTimeScale)
            {
                AttackCooldownPhase();
            }
            else
            {
                AttackEnablingPhase();                
            }
        }
    }

    void AttackBuildUpPhase()
    {        
        // Attack is building up
        aimStickRenderer.material.color = new Color(1, 1, 0); // Makes attack stick yellow for visual aid in testing
        hitTargets.Clear();

        // Projectile stuffs
        deflectedProjectiles.Clear();
        critProjectiles.Clear();
        tpProjectile.Clear();
        
        //Code to initiate attack animation

        canSpawnSwoosh = true; // Temporary Sheisse
    }

    void AttackDamagePhase()
    {
        // Attack is now able to damage
        aimStickRenderer.material.color = new Color(0, 1, 0); // Makes attack stick green for visual aid in testing
        if(canSpawnSwoosh)
        {
            Instantiate(swoosh, attackTrigger.transform); // Temporary swoosh sprite
        }
        canSpawnSwoosh = false; // Temporary Sheisse
        canAimAttack = false;
        attackTrigger.enabled = true;
    }

    void AttackCooldownPhase()
    {
        // Attack can no longer damage
        aimStickRenderer.material.color = new Color(1, 0, 0); // Makes attack stick red for visual aid in testing
        canAimAttack = true;
        attackTrigger.enabled = false;
    }
    
    void AttackEnablingPhase()
    {
        aimStickRenderer.material.color = new Color(1, 1, 1); // Makes attack stick white for visual aid in testing
        canAttack = true;
        isAttacking = false;
    }

    IEnumerator PerformAttack()
    {
        // Attack is building up
        aimStickRenderer.material.color = new Color(1, 1, 0); // Makes attack stick yellow for visual aid in testing
        hitTargets.Clear();

        // Projectile stuffs
        deflectedProjectiles.Clear();
        critProjectiles.Clear();
        tpProjectile.Clear();

        canAttack = false;
        //Code to initiate attack animation
        yield return new WaitForSeconds(attackBuildUpTime / timeManager.customTimeScale); // Time before attack deals damage

        // Attack is now able to damage
        aimStickRenderer.material.color = new Color(0, 1, 0); // Makes attack stick green for visual aid in testing
        Instantiate(swoosh, attackTrigger.transform); // Temporary swoosh sprite
        canAimAttack = false;
        attackTrigger.enabled = true;
        yield return new WaitForSeconds(damageDuration / timeManager.customTimeScale); // Time before attack no longer deals damage

        // Attack can no longer damage
        aimStickRenderer.material.color = new Color(1, 0, 0); // Makes attack stick red for visual aid in testing
        canAimAttack = true;
        attackTrigger.enabled = false;
        yield return new WaitForSeconds(attackCooldown / timeManager.customTimeScale); // Time before player can attack again

        // New attack can now be started
        aimStickRenderer.material.color = new Color(1, 1, 1); // Makes attack stick white for visual aid in testing
        canAttack = true;
    }

    private void OnTriggerEnter(Collider target)
    {
        if (((1 << target.gameObject.layer) & attackableLayers.value) != 0)
        {
            if (attackTrigger.enabled && !hitTargets.Contains(target.gameObject))
            {
                hitTargets.Add(target.gameObject);
                if (target.CompareTag("Enemy"))
                {
                    if(closestAngle == 225 || closestAngle == 270 || closestAngle == 315)
                    {
                        if(!playerChecks.IsGrounded())
                        {
                            playerMovement.Pogo();
                        }
                    }

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
    #endregion
  


    #region Deflection
    void DeflectProjectile(GameObject target)
    {
        ProjectileVelocity projectileVelocity = target.GetComponent<ProjectileVelocity>();

        // Get vector direction from player to projectile
        Vector3 toProjectile = target.transform.position - transform.position;

        // Convert to angle relative to the world
        float projectileAngle = Mathf.Atan2(toProjectile.y, toProjectile.x) * Mathf.Rad2Deg;

        // Calculate the angle difference
        float angleDifference = Mathf.DeltaAngle(angle, projectileAngle);
        float absAngleDifference = Mathf.Abs(angleDifference);

        // Apply the new trajectory with the angleDifference offset
        if(deflectSpread)
        {
            projectileVelocity.ChangeTrajectory(angle + angleDifference * 0.2f, absAngleDifference);
        }
        else
        {
            projectileVelocity.ChangeTrajectory(angle, absAngleDifference);
        }

        // Check if this projectile should be added to teleportProjectile
        if (tpProjectile.Count == 0 || absAngleDifference < tpProjectile[0].absAngleDifference)
        {
            tpProjectile.Clear();
            teleportProjectile.Clear();

            tpProjectile.Add((target, absAngleDifference));
            teleportProjectile.Add(target);
        }
    }
    #endregion



    #region Move The Trigger
    void RotateAttackTrigger()
    {
        if (playerInputs == null) return;

        float aimAngle = playerInputs.aimObject.rotation.eulerAngles.z;
        angle = aimAngle;
        /*float[] angles = {  0, 22.5f, 45, 67.5f, 90, 112.5f, 135, 157.5f, 180, 
                            202.5f, 225,    // Down left
                            270,            // Down down
                            315, 337.5f };  // Down right*/

        closestAngle = angles[0];
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

        if(!playerChecks.IsGrounded())
        {
            if(closestAngle == 202.5f || closestAngle == 225)
            {
                if(aimAngle > 202.5f)
                {
                    attackTrigger.center = new Vector3(downXOffset, downYOffset, 0);
                    attackTrigger.size = new Vector3(downWidth, downHeight, triggerDepth);
                    attackTrigger.transform.localRotation = Quaternion.Euler(0, 0, 225);
                }
                else if(aimAngle < 202.5f)
                {
                    attackTrigger.center = new Vector3(defaultXOffset, defaultYOffset);
                    attackTrigger.size = new Vector3(defaultWidth, defaultHeight, triggerDepth);
                    attackTrigger.transform.localRotation = Quaternion.Euler(0, 0, 180);
                }
            }
            else if(closestAngle == 270) // Down down
            {
                attackTrigger.center = new Vector3(downXOffset, downYOffset, 0);
                attackTrigger.size = new Vector3(downWidth, downHeight, triggerDepth);
                attackTrigger.transform.localRotation = Quaternion.Euler(0, 0, 270);
            }
            else if(closestAngle == 315 || closestAngle == 337.5f) //Down right
            {
                if(aimAngle < 337.5f)
                {
                    attackTrigger.center = new Vector3(downXOffset, downYOffset, 0);
                    attackTrigger.size = new Vector3(downWidth, downHeight, triggerDepth);
                    attackTrigger.transform.localRotation = Quaternion.Euler(0, 0, 315);
                }
                else if(aimAngle > 337.5f)
                {
                    attackTrigger.center = new Vector3(defaultXOffset, defaultYOffset);
                    attackTrigger.size = new Vector3(defaultWidth, defaultHeight, triggerDepth);
                    attackTrigger.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else
            {
                attackTrigger.center = new Vector3(defaultXOffset, defaultYOffset);
                attackTrigger.size = new Vector3(defaultWidth, defaultHeight, triggerDepth);
                attackTrigger.transform.localRotation = Quaternion.Euler(0, 0, closestAngle);
            }
        }
        else
        {
            if(closestAngle == 270)
            {
                if(aimAngle > 270)
                {
                    attackTrigger.center = new Vector3(defaultXOffset, defaultYOffset);
                    attackTrigger.size = new Vector3(defaultWidth, defaultHeight, triggerDepth);
                    attackTrigger.transform.localRotation = Quaternion.Euler(0, 0, 315);
                }
                else
                {
                    attackTrigger.center = new Vector3(defaultXOffset, defaultYOffset);
                    attackTrigger.size = new Vector3(defaultWidth, defaultHeight, triggerDepth);
                    attackTrigger.transform.localRotation = Quaternion.Euler(0, 0, 225);
                }
            }
            else
            {
                attackTrigger.center = new Vector3(defaultXOffset, defaultYOffset);
                attackTrigger.size = new Vector3(defaultWidth, defaultHeight, triggerDepth);
                attackTrigger.transform.localRotation = Quaternion.Euler(0, 0, closestAngle);
            }

        }
    }
    #endregion
}