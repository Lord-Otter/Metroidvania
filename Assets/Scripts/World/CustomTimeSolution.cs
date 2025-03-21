using UnityEngine;
using UnityEngine.Android;

public class CustomTimeSolution : MonoBehaviour
{
    public GameObject trainingProjectile;
    private ProjectileVelocity projectileVelocity;
    private PlayerVelocity playerVelocity;

    public bool tpPause = false;

    void Awake()
    {
        projectileVelocity = trainingProjectile.GetComponent<ProjectileVelocity>();
        playerVelocity = GameObject.Find("Player").GetComponent<PlayerVelocity>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
