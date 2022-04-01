using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YeongJ.Inagme;

namespace YeongJ.Inagme
{
    public class SkillObject : BaseActor
    {
        [SerializeField] GameObject _hitEffect;
        [SerializeField] float _heightOffset = 1f;
        [SerializeField] bool _isShake;
        [SerializeField] float _shakeDelay;
        [SerializeField] float _shakeIntensity;

        public override void Init(int Id)
        {
            base.Init(Id);

            if (!_isShake)
                return;

            CameraShaker.Instance.StartShake(_shakeDelay, _shakeIntensity, 0.5f);
        }

        public override void Remove()
        {
            base.Remove();

            if (_hitEffect == null)
                return;

            var hitEffect = GameObjectCache.Make(_hitEffect.transform, this.transform.parent);
            hitEffect.transform.position = transform.position;

            GameObjectCache.DeleteDelayed(hitEffect, delayTime: 1.08f);
        }

        protected override void UpdateMove()
        {
            var currentPosition = this.transform.position;
            currentPosition.y = 0.0f;

            if (ServerPos == this.transform.position)
                return;

            transform.position = Vector3.SmoothDamp(currentPosition, ServerPos, ref _currentVelocity, 0.101f, maxSpeed: Stat.Speed);
            UpdateHeight();
        }

        protected override void UpdateHeight()
        {
            var layerMask = LayerMask.NameToLayer("Ground");

            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * _groundedRayDistance, -Vector3.up);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~layerMask))
            {
                var currentPosition = this.transform.position;
                currentPosition.y = hit.point.y + _heightOffset;
                this.transform.position = currentPosition;
            }
        }

        protected override void UpdateRotation() { }
    }
}