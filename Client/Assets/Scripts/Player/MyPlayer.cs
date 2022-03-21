using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeongJ.Inagme
{
    public class MyPlayer : Player
    {
        // Effect
        [SerializeField] GameObject _makerEffect;

        const float INPUT_DELAY = 0.1f;

        float _latency;
        float _inputCheckTime = 0.0f;

        public void SetLatency(float latency)
        {
            _latency = latency;
        }

        public override void Init(int Id)
        {
            base.Init(Id);
            _inputHandle = UpdateKeyInput;
            _inputCheckTime = INPUT_DELAY;
        }

        public void UpdateKeyInput()
        {
            if(Input.GetMouseButtonDown(0))
            {
                SendSkillPacket();
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                SendMovePacket();
                _inputCheckTime = INPUT_DELAY;
                return;
            }

            if (_inputCheckTime > 0.0f)
            {
                _inputCheckTime -= Time.deltaTime;
                return;
            }

            _inputCheckTime = INPUT_DELAY;
            if (Input.GetMouseButton(1))
            {
                SendMovePacket(makeMaker: false);
            }
        }

        private void SendMovePacket(bool makeMaker = true)
        {
            var clickResult = GetClickPosition();
            if (!clickResult.result)
            {
                return;
            }

            if (makeMaker)
            {
                MakeMakerEffect(clickResult.position);
            }

            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo.Clone();
            movePacket.PosInfo.PosX = clickResult.position.x;
            movePacket.PosInfo.PosY = 0;
            movePacket.PosInfo.PosZ = clickResult.position.z;
            Managers.Network.Send(movePacket);
        }

        private void SendSkillPacket(int skillId = 0)
        {
            if (PosInfo.State != ActorState.Idle && PosInfo.State != ActorState.Moving)
                return;

            var clickResult = GetClickPosition();
            if (!clickResult.result)
                return;

            var skillDir = clickResult.position - this.transform.position;
            skillDir.Normalize();

            var skillPacket = new C_Skill();
            skillPacket.Info = new SkillInfo();
            skillPacket.Info.SkillId = skillId;
            skillPacket.Info.SpawnPosition = transform.position.ToFloat3();
            skillPacket.Info.SkillDirection = skillDir.ToFloat3();

            Managers.Network.Send(skillPacket);
        }

        private (bool result, Vector3 position) GetClickPosition()
        {
            var layerMask = LayerMask.NameToLayer("Ground");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Mathf.Infinity, ~layerMask))
            {
                return (true, hit.point);
            }

            return (false, Vector3.zero);
        }

        private void MakeMakerEffect(Vector3 spawnPosition)
        {
            var makerEffect = GameObjectCache.Make(_makerEffect.transform, this.transform.parent);
            makerEffect.transform.position = spawnPosition;
            GameObjectCache.DeleteDelayed(makerEffect, delayTime: 1.0f);
        }
    }
}