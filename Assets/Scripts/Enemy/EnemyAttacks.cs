using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttacks : MonoBehaviour
{
    public int attackDamage;
    public int contactDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider target)
    {
        if(target.gameObject.CompareTag("Player"))
        {
            ContactDamage(target.gameObject, contactDamage);
        }
    }

    void ContactDamage(GameObject target, int damage)
    {
        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

    }
}
