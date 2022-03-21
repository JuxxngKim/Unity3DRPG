using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YeongJ.Inagme;

namespace YeongJ.Inagme
{
    public class SkillObject : BaseActor
    {
        [SerializeField] Transform _rootTransform;
        [SerializeField] GameObject _hitEffect;

        public override void Init(int Id)
        {
            base.Init(Id);
            _heightOffset = 1f;
        }

        public override void Remove()
        {
            base.Remove();

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

            transform.position = Vector3.SmoothDamp(transform.position, ServerPos, ref _currentVelocity, 0.1f, maxSpeed: Stat.Speed);
            UpdateHeight();
        }

        protected override void UpdateRotation()
        {
            if (_rootTransform == null)
                return;

            _rootTransform.transform.rotation = Quaternion.Lerp(_rootTransform.transform.rotation, Quaternion.LookRotation(_currentVelocity), Time.deltaTime * 10f);
        }
    }
}