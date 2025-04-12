using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public float amplitude;
    public float frequency;
    
    double currAmp;
    CinemachineVirtualCamera vcam;
    CinemachineBasicMultiChannelPerlin noise;
    bool isShaking = false;
    float shakeDuration;

    float shakeTimeElapsed;
    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    private void OnDestroy()
    {
        
    }

    public void ShakeCamera(float duration) {
        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;
        isShaking = true;
        shakeDuration = duration;
        shakeTimeElapsed = 0f;
        currAmp = amplitude;
    }

    void StopShakeCamera() {
        isShaking = false;
        shakeTimeElapsed = 0f;
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;

    }

    void Update() {
        shakeTimeElapsed =  shakeTimeElapsed + Time.deltaTime;
        currAmp = currAmp * 0.9;
        if (shakeTimeElapsed > shakeDuration) {
            StopShakeCamera();
        }
    }
}
