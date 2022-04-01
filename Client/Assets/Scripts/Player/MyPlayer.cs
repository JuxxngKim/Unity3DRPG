using Cinemachine;
using Google.Protobuf.Protocol;
using UnityEngine;

namespace YeongJ.Inagme
{
    public class MyPlayer : Player
    {
        [SerializeField] CinemachineVirtualCamera _virtualCamera;
        [SerializeField] GameObject _makerEffect;

        public CinemachineFramingTransposer Transposer
        {
            get 
            {
                if(_transposer == null)
                    _transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

                return _transposer;
            }
        }


        float _latency;
        float _inputCheckTime = 0.0f;

        const float _inputDelay = 0.1f;
        const float _camDistanceMin = 2.0f;
        const float _camDistanceMax = 14.0f;
        const float _camAngleMin = 15.0f;
        const float _camAngleMax = 45.0f;

        CinemachineFramingTransposer _transposer;

        public void SetLatency(float latency)
        {
            _latency = latency;
        }

        public override void Init(int Id)
        {
            base.Init(Id);

            _inputHandle = UpdateKeyInput;
            _inputCheckTime = _inputDelay;
        }

        public void UpdateKeyInput()
        {
            UpdateMouseScroll();

            if (Input.GetMouseButtonDown(0))
            {
                SendSkillPacket(skillId: 1);
                return;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                SendSkillPacket(skillId: 2, isCliektSpawn: true);
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                SendMovePacket();
                _inputCheckTime = _inputDelay;
                return;
            }

            if (_inputCheckTime > 0.0f)
            {
                _inputCheckTime -= Time.deltaTime;
                return;
            }

            _inputCheckTime = _inputDelay;
            if (Input.GetMouseButton(1))
            {
                SendMovePacket(makeMaker: false);
            }
        }

        private void UpdateMouseScroll()
        {
            if (Transposer == null)
                return;

            float scroll = -Input.mouseScrollDelta.y * 3.0f;
            var distance = Transposer.m_CameraDistance + scroll;
            Transposer.m_CameraDistance = Mathf.Clamp(distance, _camDistanceMin, _camDistanceMax);

            if(Transposer.m_CameraDistance <= _camDistanceMin)
            {
            }
            else
            {

            }
        }

        private void SendMovePacket(bool makeMaker = true)
        {
            if (ServerPosInfo.State == ActorState.Attack)
                return;

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
            movePacket.PosInfo = ServerPosInfo.Clone();
            movePacket.PosInfo.Position = clickResult.position.ToFloat3();
            Managers.Network.Send(movePacket);
        }

        private void SendSkillPacket(int skillId = 0, bool isCliektSpawn = false)
        {
            if (ServerPosInfo.State == ActorState.Attack)
                return;

            var clickResult = GetClickPosition();
            if (!clickResult.result)
                return;

            var skillDir = clickResult.position - this.transform.position;
            skillDir.Normalize();

            var spawnPosition = isCliektSpawn ? clickResult.position.ToFloat3() : transform.position.ToFloat3();

            var skillPacket = new C_Skill();
            skillPacket.Info = new SkillInfo();
            skillPacket.Info.SkillId = skillId;
            skillPacket.Info.SpawnPosition = spawnPosition;
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