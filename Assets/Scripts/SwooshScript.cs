using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwooshScript : MonoBehaviour
{
    private TimeManager timeManager;
    private PlayerAttack playerAttack;

    // Start is called before the first frame update
    void Start()
    {
        timeManager = GameObject.Find("Time_Manager").GetComponent<TimeManager>();
        playerAttack = GameObject.Find("Player").GetComponent<PlayerAttack>();
        //Destroy(gameObject, destroyTime / timeManager.customTimeScale);
    }

    // Update is called once per frame
    void Update()
    {
        if((playerAttack.attackI >= (playerAttack.damageDuration + playerAttack.attackBuildUpTime) * 100f ) || (!playerAttack.isAttacking))
        {
            Destroy(gameObject);
        }
    }
}
