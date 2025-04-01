using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [Header("Custom Time Pausing")]
    public GameObject trainingProjectile;
    public ProjectileVelocity projectileVelocity;
    public PlayerVelocity playerVelocity;
    public CameraBehaviour cameraBehaviour;
    public bool worldPause = false;
    public bool tpPause = false;

    [Header("Time Scale")]
    public float timeScale = 1.0f;
    public float customTimeScale = 1.0f;
    [HideInInspector]public float lastTimeScale;

    private void Awake()
    {
        projectileVelocity = trainingProjectile.GetComponent<ProjectileVelocity>();
        playerVelocity = GameObject.Find("Player").GetComponent<PlayerVelocity>();
        cameraBehaviour = GameObject.Find("Main Camera").GetComponent<CameraBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        CustomTimeScaler();
        //TimeScale();       
    }

    void CustomTimeScaler()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(!worldPause)
            {
                WorldPause(true);
            }
            else
            {
                WorldPause(false);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            customTimeScale = 0.1f;
            TimeScaleChangeUpdate();
            Debug.Log($"Custom time Scale: {customTimeScale * 100}% ");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            customTimeScale = 0.5f;
            TimeScaleChangeUpdate();
            Debug.Log($"Custom time Scale: {customTimeScale * 100}% ");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            customTimeScale = 1;
            TimeScaleChangeUpdate();
            Debug.Log($"Custom time Scale: {customTimeScale * 100}% ");
        }
    }

    public void WorldPause(bool pause)
    {
        if(pause)
        {
            Debug.Log("World Pause");
            worldPause = true;
        }
        else
        {
            Debug.Log("World Unpause");
            worldPause = false;

            projectileVelocity.OnResume();
            playerVelocity.OnResume();
        }
    }

    public void TPPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("TP Pause");
            tpPause = true;                       
        }
        else
        {
            Debug.Log("TP Unpause");
            tpPause = false;

            projectileVelocity.OnResume();
            playerVelocity.OnResume();
        }        
    }

    void TimeScaleChangeUpdate()
    {
        playerVelocity.AdjustVelocityForTimeScale();
        projectileVelocity.AdjustVelocityForTimeScale();
        cameraBehaviour.AdjustVelocityForTimeScale();
    }

    void TimeScale()
    {
        Time.timeScale = timeScale;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (timeScale == 0)
            {
                timeScale = lastTimeScale;
                Debug.Log($"Time Scale: {timeScale * 100}% ");
            }
            else
            {
                lastTimeScale = timeScale;
                timeScale = 0;
                Debug.Log("Stopping Time");
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            timeScale = 0.1f;
            Debug.Log($"Time Scale: {timeScale * 100}% ");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            timeScale = 0.5f;
            Debug.Log($"Time Scale: {timeScale * 100}% ");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            timeScale = 1;
            Debug.Log($"Time Scale: {timeScale * 100}% ");
        }
    }

    public void SetTimeScale(float newTimeScale)
    {
        timeScale = newTimeScale;
    }
}
