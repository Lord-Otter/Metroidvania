using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwooshScript : MonoBehaviour
{
    private TimeManager timeManager;
    private PlayerAttack playerAttack;

    public float destroyTime = 0.2f;

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
        if(playerAttack.attackI >= (playerAttack.damageDuration + playerAttack.attackBuildUpTime) * 100f)
        {
            Destroy(gameObject);
        }
    }
}
