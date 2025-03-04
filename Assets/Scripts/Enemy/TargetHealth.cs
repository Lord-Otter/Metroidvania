using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetHealth : MonoBehaviour
{
    public int maxHealth, health;
    [SerializeField]private float damageDelay;
    private float lastTimeDamaged;

    public bool canBePogod;

    // Start is called before the first frame update
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
        if (Time.time - lastTimeDamaged >= damageDelay)
        {
            health -= damage;
            lastTimeDamaged = Time.time;

            if (health <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
