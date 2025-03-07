using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth;
    public int health;

    [Header("Take Damage")]
    public float damageDelay;
    private float lastTimeDamaged;

    [Header("Death")]
    private Renderer playerRenderer;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;

        // Renderer
        playerRenderer = GameObject.Find("Player_Visuals").GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (Time.time - lastTimeDamaged >= damageDelay)
        {
            health -= damage;
            lastTimeDamaged = Time.time;
            Debug.Log("Damaged");

            if (health <= 0)
            {
                playerRenderer.material.color = new Color(0.2f, 0.2f, 0.2f);
            }
        }
    }
}
