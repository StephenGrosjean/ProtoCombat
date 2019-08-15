using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class CameraShake : MonoBehaviour {

    public float shakeDuration = 0.3f;          // Time the Camera Shake effect will last
    public float shakeAmplitude = 1.2f;         // Cinemachine Noise Profile Parameter
    public float shakeFrequency = 2.0f;         // Cinemachine Noise Profile Parameter

    private float shakeElapsedTime = 0f;
    private bool canShake;
    // Cinemachine Shake
    public CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;

    // Use this for initialization
    void Start() {
        // Get Virtual Camera Noise Profile
        if (VirtualCamera != null)
            virtualCameraNoise = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCam(float duration, float intensity) {
        shakeDuration = duration;
        shakeAmplitude = intensity;
        shakeElapsedTime = shakeDuration;

    }

    // Update is called once per frame
    void Update() {

        // If the Cinemachine componet is not set, avoid update
        if (VirtualCamera != null && virtualCameraNoise != null) {
            // If Camera Shake effect is still playing
            if (shakeElapsedTime > 0) {
                // Set Cinemachine Camera Noise parameters
                virtualCameraNoise.m_AmplitudeGain = shakeAmplitude;
                virtualCameraNoise.m_FrequencyGain = shakeFrequency;

                // Update Shake Timer
                shakeElapsedTime -= Time.deltaTime;
            }
            else {
                // If Camera Shake effect is over, reset variables
                virtualCameraNoise.m_AmplitudeGain = 0f;
                shakeElapsedTime = 0f;
            }
        }
    }
}