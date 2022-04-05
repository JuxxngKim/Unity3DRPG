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

        SkillInfo _skillInfo;
        float _currentStateTime;
        Vector3 _skillStartPosition;

        public override void UseSkill(SkillInfo skillInfo)
        {
            base.UseSkill(skillInfo);

            Vector3 skillDir = skillInfo.SkillDirection.ToVector3();
            _currentVelocity = skillDir;
            _model.transform.rotation = Quaternion.LookRotation(skillDir);
            _skillInfo = skillInfo;

            if (skillInfo.SkillId == -1)
            {
                _animator.enabled = false;
                _animator.gameObject.SetActive(false);
                SpawnTeleportEffect();

                _skillStartPosition = this.transform.position;
                _skillStartPosition.y = 0.0f;
                _currentStateTime = 0.0f;
                _commandHandle = UpdateCommandTeleport;
                return;
            }

            switch (skillInfo.SkillId)
            {
                case 1: _animator.SetTrigger(Const.TriggerAttack); break;
                default: _animator.SetTrigger(Const.TriggerSkill); break;
            }
        }

        protected virtual void UpdateCommandTeleport()
        {
            var currentPosition = transform.position;
            currentPosition.y = 0.0f;

            float ratio = _currentStateTime <= 0.0f ? 0.0f : _currentStateTime / _skillInfo.StateTime;
            currentPosition = Vector3.Lerp(currentPosition, _skillInfo.SpawnPosition.ToVector3(), Mathf.Clamp01(ratio));
            this.transform.position = currentPosition;

            UpdateHeight();

            if(ratio >= 1.0f)
            {
                _animator.enabled = true;
                _animator.gameObject.SetActive(true);
                SpawnTeleportEffect();

                _skillInfo = null;
                ServerDir = Vector3.zero;
                ServerPos = currentPosition;
                _commandHandle = UpdateCommandIdleMove;
            }

            _currentStateTime += Time.deltaTime;
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