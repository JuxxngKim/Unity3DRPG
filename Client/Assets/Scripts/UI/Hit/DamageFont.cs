using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YeongJ.Inagme;

namespace YeongJ.UI
{
    public class DamageFont : MonoBehaviour
    {
        [SerializeField] Text _damageText;

        private float _lifeTime;
        private float _remainTime;

        public void Init(int damage, float lifeTime)
        {
            _remainTime = _lifeTime = lifeTime;
        }

        void Update()
        {
            _remainTime -= Time.deltaTime;
            float ratio = _remainTime <= 0.0f ? 0.0f : Mathf.Clamp01(_remainTime / _lifeTime);
            var newColor = _damageText.color;
            newColor.a = ratio;

            _damageText.color = newColor;
        }
    }
}
