using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeongJ.Inagme
{
    public class ShakeInstance : MonoBehaviour
    {
        [SerializeField] float _shakeDelay;
        [SerializeField] float _shakeIntensity;

        float _remainTime;

        void Start()
        {
            _remainTime = _shakeDelay;
        }

        void Update()
        {
            if (_remainTime <= 0.0f)
                return;

            _remainTime -= Time.deltaTime;
            if (_remainTime > 0.0f)
                return;

            CameraShaker.Instance.StartShake(_shakeDelay, _shakeIntensity, 0.5f);
        }
    }
}