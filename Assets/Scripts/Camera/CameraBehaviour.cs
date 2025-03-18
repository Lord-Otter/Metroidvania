using UnityEngine;
using Cinemachine;

public class CameraBehaviour : MonoBehaviour
{
    private CinemachineBrain cinemachineBrain;
    void Awake()
    {
        cinemachineBrain = GameObject.Find("Main Camera").GetComponent<CinemachineBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CameraIgnoreTimeScale(bool ignoreTimeScale)
    {
        if(ignoreTimeScale)
        {
            cinemachineBrain.m_IgnoreTimeScale = true;
            cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
        }
        else
        {
            cinemachineBrain.m_IgnoreTimeScale = false;
            cinemachineBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
        }
    }
}
