using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraBehaviour : MonoBehaviour
{
    private TimeManager timeManager;

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineFramingTransposer cinemachineFramingTransposer;

    [Header("Camera Position")]
    public float cameraDistance;
    public Vector3 CameraOffset;

    [Header("Camera Movement")]
    [Range(0, 1)] public float lookAheadTime;
    [Range(0, 30)] public float lookAheadSmoothing;
    [Range(0, 20)] public float xDamping;
    [Range(0, 20)] public float yDamping;
    [Range(0, 20)] public float zDamping;
    
    void Awake()
    {
        timeManager = GameObject.Find("Time_Manager").GetComponent<TimeManager>();

        cinemachineVirtualCamera = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        cinemachineFramingTransposer = cinemachineVirtualCamera.AddCinemachineComponent<CinemachineFramingTransposer>();
        
    }

    void Start()
    {
        SetCameraValues();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetCameraValues()
    {
        // Camera Position
        cinemachineFramingTransposer.m_CameraDistance = cameraDistance;
        cinemachineFramingTransposer.m_TrackedObjectOffset = CameraOffset;

        // Camera Movement
        cinemachineFramingTransposer.m_LookaheadTime = lookAheadTime;
        cinemachineFramingTransposer.m_LookaheadSmoothing = lookAheadSmoothing;
        cinemachineFramingTransposer.m_XDamping = xDamping;
        cinemachineFramingTransposer.m_YDamping = yDamping;
        cinemachineFramingTransposer.m_ZDamping = zDamping;
    }

    public void AdjustVelocityForTimeScale()
    {
        cinemachineFramingTransposer.m_XDamping = xDamping / timeManager.timeScale;
        cinemachineFramingTransposer.m_YDamping = yDamping / timeManager.timeScale;
        cinemachineFramingTransposer.m_ZDamping = zDamping / timeManager.timeScale;
    }
}
