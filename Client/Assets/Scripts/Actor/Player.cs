using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeongJ.Inagme
{
    public class Player : BaseActor
    {
        [SerializeField] GameObject _teleportEffect;
        [SerializeField] float _teleportDelay = 0.2f;

        SkillInfo _skillInfo;
        float _currentStateTime;
        float _skillEndRemainTime;
        Vector3 _skillStartPosition;
        Vector3 _skillTargetPosition;

        const float _skillDelay = 0.2f;

        public override void UseSkill(SkillInfo skillInfo)
        {
            base.UseSkill(skillInfo);

            _skillInfo = skillInfo;

            if (skillInfo.SkillId == -1)
            {
                SpawnTeleportEffect();

                _skillStartPosition = this.transform.position;
                _skillStartPosition.y = 0.0f;
                _skillTargetPosition = _skillInfo.SpawnPosition.ToVector3();
                _skillTargetPosition.y = 0.0f;

                _currentStateTime = 0.0f;
                _currentVelocity = 0.3f;
                _commandHandle = UpdateCommandTeleport;
                _animator.SetTrigger("Dash");
                _animator.SetFloat("Velocity", _currentVelocity);
                _skillEndRemainTime = _teleportDelay;
                UpdateRotation(isLerp: false);
                return;
            }

            switch (skillInfo.SkillId)
            {
                case 1: _animator.SetTrigger(Const.TriggerAttack); break;
                default: _animator.SetTrigger(Const.TriggerSkill); break;
            }

            _skillEndRemainTime = _skillInfo.StateTime + _skillDelay;
        }

        protected virtual void UpdateCommandTeleport()
        {
            var currentPosition = transform.position;
            currentPosition.y = 0.0f;

            float ratio = _currentStateTime <= 0.0f ? 0.0f : _currentStateTime / _skillInfo.StateTime;
            currentPosition = Vector3.Lerp(currentPosition, _skillTargetPosition, Mathf.Clamp01(ratio));
            this.transform.position = currentPosition;

            UpdateHeight();

            if(ratio >= 1.0f)
            {
                //SpawnTeleportEffect();

                _skillInfo = null;
                ServerDir = Vector3.zero;
                ServerPos = currentPosition;
                _commandHandle = UpdateCommandIdleMove;
            }

            _currentStateTime += Time.deltaTime;
            UpdateSkillEnd();
        }

        protected override void UpdateCommandIdleMove()
        {
            base.UpdateCommandIdleMove();
            UpdateSkillEnd();
        }

        protected void UpdateSkillEnd()
        {
            if (_skillEndRemainTime <= 0.0f)
                return;

            _skillEndRemainTime -= Time.deltaTime;
            if (_skillEndRemainTime <= 0.0f)
            {
                _animator.SetTrigger("SkillEnd");
            }
        }

        void SpawnTeleportEffect()
        {
            if (_teleportEffect == null)
                return;

            var makerEffect = GameObjectCache.Make(_teleportEffect.transform, this.transform.parent);
            var spawnPosition = this.transform.position;
            spawnPosition.y += 0.5f;
            makerEffect.transform.position = spawnPosition;
            GameObjectCache.DeleteDelayed(makerEffect, delayTime: 1.0f);
        }
    }
}