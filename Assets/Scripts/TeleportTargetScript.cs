using UnityEngine;
using UnityEngine.UIElements;

public class TeleportTargetScript : MonoBehaviour
{
    private GameObject player;
    private PlayerAttack playerAttack;
    private SpriteRenderer spriteRenderer;
    void Awake()
    {
        player = GameObject.Find("Player");
        playerAttack = GameObject.Find("Player").GetComponent<PlayerAttack>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        StickToTeleportProjectile();
        Rotate();
    }

    void StickToTeleportProjectile()
    {
        if(playerAttack.teleportProjectile.Count != 0 && playerAttack.teleportProjectile[0] != null)
        {
            spriteRenderer.enabled = true;
            transform.position = playerAttack.teleportProjectile[0].transform.position;
        }
        else
        {
            spriteRenderer.enabled = false;
            transform.position = player.transform.position;
        }
    }

    void Rotate()
    {
        gameObject.transform.rotation = Quaternion.Euler(0, 0, playerAttack.angle);
    }
}
