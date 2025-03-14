using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float timeScale = 1.0f;
    private float lastTimeScale;

    private void Start()
    {
        lastTimeScale = timeScale;
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
                Debug.Log("Resume Time");
            }
            else
            {
                lastTimeScale = timeScale;
                timeScale = 0;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            timeScale = 0.1f;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            timeScale = 0.5f;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            timeScale = 1;
        }
    }

    public void SetTimeScale(float newTimeScale)
    {
        timeScale = newTimeScale;
    }
}
