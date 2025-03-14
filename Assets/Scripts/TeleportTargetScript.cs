using UnityEngine;

public class TeleportTargetScript : MonoBehaviour
{
    private GameObject player;
    private AttackScript attackScript;
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
        attackScript = GameObject.Find("Player").GetComponent<AttackScript>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        StickToTeleportProjectile();        
    }

    void StickToTeleportProjectile()
    {
        if(attackScript.teleportProjectile.Count != 0 && attackScript.teleportProjectile[0] != null)
        {
            spriteRenderer.enabled = true;
            transform.position = attackScript.teleportProjectile[0].transform.position;
        }
        else
        {
            spriteRenderer.enabled = false;
            transform.position = player.transform.position;
        }
    }
}
