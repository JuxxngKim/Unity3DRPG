using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeongJ.Inagme
{
    public class BaseActor : MonoBehaviour
    {
        [SerializeField] protected GameObject _model;
        [SerializeField] protected GameObject _uIRoot;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected float _groundedRayDistance = 30f;

        public int Id { get; private set; }
        public StatInfo Stat { get { return _stat; } set { _stat = value; } }
        public PositionInfo ServerPosInfo { get { return _serverPosInfo; } set { _serverPosInfo = value; } }
        public GameObject UIRoot => _uIRoot;

        protected float _positionLerpTime;
        protected float _currentPositionLerpTime;

        public Vector3 ServerDir
        {
            get 
            {
                if (_serverPosInfo.Direction == null)
                    _serverPosInfo.Direction = Vector3.zero.ToFloat3();

                return _serverPosInfo.Direction.ToVector3();
            }
            set
            {
                if(_serverPosInfo.Direction == null)
                    _serverPosInfo.Direction = Vector3.zero.ToFloat3();

                _serverPosInfo.Direction = value.ToFloat3();
            }
        }

        public Vector3 ServerPos
        {
            get 
            {
                if (_serverPosInfo.Position == null)
                    _serverPosInfo.Position = Vector3.zero.ToFloat3();

                return _serverPosInfo.Position.ToVector3();
            }
            set
            {
                if (_serverPosInfo.Position == null)
                    _serverPosInfo.Position = Vector3.zero.ToFloat3();

                _serverPosInfo.Position = value.ToFloat3();
            }
        }

        public Vector3 PrevServerPos
        {
            get
            {
                if(_prevServerPosInfo == null)
                    _prevServerPosInfo = _serverPosInfo.Clone();

                if (_prevServerPosInfo.Position == null)
                    _prevServerPosInfo.Position = ServerPos.ToFloat3();

                return _prevServerPosInfo.Position.ToVector3();
            }
        }



        public Vector3 ServerLookDir
        {
            get
            {
                if (_serverPosInfo.LookDirection == null)
                    _serverPosInfo.LookDirection = Vector3.down.ToFloat3();

                return _serverPosInfo.LookDirection.ToVector3();
            }
            set
            {
                _serverPosInfo.LookDirection = value.ToFloat3();
            }
        }

        protected StatInfo _stat;
        protected PositionInfo _serverPosInfo;
        protected PositionInfo _prevServerPosInfo;
        protected float _currentVelocity;

        protected delegate void InputHandle();
        protected delegate void CommandHandle();

        protected InputHandle _inputHandle = null;
        protected CommandHandle _commandHandle = null;

        public virtual void Init(int Id)
        {
            this.Id = Id;
            _commandHandle = UpdateCommandIdleMove;
        }

        public virtual void SetServerPos(PositionInfo posInfo)
        {
            _prevServerPosInfo = _serverPosInfo;
            _serverPosInfo = posInfo;
            _currentPositionLerpTime = _positionLerpTime = Const.FrameTime + Managers.Network.Latency + Const.MoveLerpDelay;
        }

        public virtual void Remove() { }

        public void Update()
        {
            UpdateActor();
        }

        public void UpdateActor()
        {
            ProcessInput();
            ProcessCommand();
        }

        public virtual void ProcessCommand() 
        {
            if (_commandHandle == null)
                return;

            _commandHandle();
        }
        
        public virtual void ProcessInput()
        {
            if (_inputHandle == null)
                return;

            _inputHandle();
        }

        protected virtual void UpdateCommandIdleMove()
        {
            UpdateMove();
            UpdateRotation();
        }

        public virtual void SyncPos()
        {
            transform.position = ServerPos;
            UpdateHeight();
            UpdateRotation();
        }

        protected virtual void UpdateMove()
        {
            var currentPosition = this.transform.position;
            currentPosition.y = 0.0f;
            var newPositon = currentPosition;

            if (ServerPos != currentPosition)
            {
                _currentPositionLerpTime -= Time.deltaTime;
                float ratio = _currentPositionLerpTime <= 0.0f ? 1.0f : 1.0f - Mathf.Clamp01(_currentPositionLerpTime / _positionLerpTime);
                newPositon = Vector3.Lerp(PrevServerPos, ServerPos, ratio);
                transform.position = newPositon;
                _currentVelocity = 0.1f;
                UpdateHeight();
            }
            else
            {
                _currentVelocity = Mathf.Clamp01(_currentVelocity - Time.deltaTime);
            }

            if (_animator != null)
            {
                _animator.SetFloat("Velocity", _currentVelocity);
            }
        }

        protected virtual void UpdateHeight()
        {
            var layerMask = LayerMask.NameToLayer("Ground");

            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * _groundedRayDistance, -Vector3.up);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~layerMask))
            {
                var currentPosition = this.transform.position;
                currentPosition.y = hit.point.y;
                this.transform.position = currentPosition;
            }
        }

        protected virtual void UpdateRotation(bool isLerp = true)
        {
            if (_model == null)
                return;

            if (isLerp)
            {
                _model.transform.rotation = Quaternion.Lerp(_model.transform.rotation, Quaternion.LookRotation(ServerLookDir), Time.deltaTime * 10f);
            }
            else
            {
                _model.transform.rotation = Quaternion.LookRotation(ServerLookDir);
            }
        }

        public virtual void OnDead()
        {
            _animator.SetTrigger(Const.TriggerDeath);
        }

        public virtual void UseSkill(SkillInfo skillInfo) { }
    }
}