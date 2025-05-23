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
    public bool canTakeDamage = true;

    [Header("Death")]
    private Renderer playerRenderer;

    void Awake()
    {
        // Renderer
        playerRenderer = GetComponentInChildren<Renderer>();
    }

    void Start()
    {
        health = maxHealth;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if(canTakeDamage)
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
}
