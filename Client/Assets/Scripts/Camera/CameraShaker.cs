using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _virtualCamera;

    public static CameraShaker Instance { get; private set; }

    float _shakeTimer;
    float _velocity;
    CinemachineBasicMultiChannelPerlin _perlin;

    void Awake()
    {
        Instance = this;
        _perlin = _virtualCamera?.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void StartShake(float delay, float intensity, float time)
    {
        _velocity = 0.0f;
        _perlin.m_AmplitudeGain = intensity;
        _shakeTimer = time;
    }

    void Update()
    {
        if(_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;
        }
        else
        {
            if (_perlin.m_AmplitudeGain <= 0.0f)
                return;

            _perlin.m_AmplitudeGain = Mathf.SmoothDamp(_perlin.m_AmplitudeGain, target: 0.0f, ref _velocity, smoothTime: 0.1f);
        }
    }
}
