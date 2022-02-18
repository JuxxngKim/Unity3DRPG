using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeongJ.Inagme
{
    public class MyPlayer : Player
    {
        private const float INPUT_DELAY = 0.1f;

        private float _latency;
        private float _inputCheckTime = 0.0f;

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
                SendMovePacket();
            }
        }

        private void SendMovePacket()
        {
            var result = GetClickPosition();
            if (!result.result)
            {
                return;
            }

            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo.Clone();
            movePacket.PosInfo.PosX = result.position.x;
            movePacket.PosInfo.PosY = 0;
            movePacket.PosInfo.PosZ = result.position.z;
            Managers.Network.Send(movePacket);
        }

        private void SendSkillPacket(int skillId = 1)
        {
            if (PosInfo.State != ActorState.Idle && PosInfo.State != ActorState.Moving)
                return;

            var skillPacket = new C_Skill();
            skillPacket.Info = new SkillInfo();
            skillPacket.Info.SkillId = skillId;
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
    }
}