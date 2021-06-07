using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamersShake : MonoBehaviour
{
    private CinemachineVirtualCamera cineMachineVirtualCamera;

    float shakerTimer;
    float shakeTimerTotal;
    float startingIntensity;

    float widenTimer = 0;
    float widenTimerTotal;
    
    Vector3 offset;
    

    private void Awake()
    {
        cineMachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        Server.CameraEvent += ShakeCamera;
        ClientManager.RowCreated += WidenRange;
    }

    private void OnDisable()
    {
        Server.CameraEvent -= ShakeCamera;
        ClientManager.RowCreated -= WidenRange;
    }

    /// <summary>
    /// Shakes camera whenever damage is taken
    /// </summary>
    /// <param name="intensity">intenstiy of shake</param>
    /// <param name="time">duration of shake</param>
    public void ShakeCamera(float intensity,float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultichannelPerlin =
            cineMachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultichannelPerlin.m_AmplitudeGain = intensity;
        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakerTimer = time;
    }

    /// <summary>
    /// Used to widen view whenever a new row is created ont the back
    /// </summary>
    public void WidenRange()
    {
        widenTimer = 5;
        widenTimerTotal = widenTimer;
        offset = transform.position + Vector3.back *8f;

        //startingFOV = cineMachineVirtualCamera.m_Lens.FieldOfView;
        //targetFOV = startingFOV + 1;
    }

    private void Update()
    {
        if (shakerTimer > 0)
        {
            shakerTimer -= Time.deltaTime;
            
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultichannelPerlin =
            cineMachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultichannelPerlin.m_AmplitudeGain = 
                Mathf.Lerp(startingIntensity, 0f, (1 - (shakerTimer / shakeTimerTotal)));
            
        }
        if (widenTimer > 0)
        {
            widenTimer -= Time.deltaTime;
            transform.position = 
                Vector3.Lerp(transform.position, offset, (1 - (widenTimer / widenTimerTotal)));

            //cineMachineVirtualCamera.m_Lens.FieldOfView =
            //    Mathf.Lerp(startingFOV,targetFOV, (1 - (widenTimer / widenTimerTotal))) ;
        }
    }



}
