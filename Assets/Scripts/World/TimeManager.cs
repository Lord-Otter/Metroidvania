using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float timeScale = 1.0f;
    [HideInInspector]public float lastTimeScale;

    private void Start()
    {
        //lastTimeScale = timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;

        if (Input.GetKeyDown(KeyCode.P))
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
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            timeScale = 0.1f;
            Debug.Log($"Time Scale: {timeScale * 100}% ");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            timeScale = 0.5f;
            Debug.Log($"Time Scale: {timeScale * 100}% ");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
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
