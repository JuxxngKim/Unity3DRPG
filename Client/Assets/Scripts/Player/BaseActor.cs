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

        protected StatInfo _stat;
        protected PositionInfo _serverPosInfo;
        protected Vector3 _currentVelocity = Vector3.zero;

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
            ServerDir = posInfo.Direction.ToVector3();
            ServerPos = posInfo.Position.ToVector3();
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
            transform.position = ServerPosInfo.Position.ToVector3();
            UpdateHeight();
        }

        protected virtual void UpdateMove()
        {
            _animator.SetFloat("Velocity", ServerDir.magnitude);

            var currentPosition = this.transform.position;
            currentPosition.y = 0.0f;

            if (ServerDir != Vector3.zero)
            {
                float targetX = ServerPos.x + ServerDir.x * Time.deltaTime * Stat.Speed;
                float targetY = ServerPos.y;
                float targetZ = ServerPos.z + ServerDir.z * Time.deltaTime * Stat.Speed;

                var targetPos = new Vector3(targetX, targetY, targetZ);
                this.transform.position = Vector3.SmoothDamp(currentPosition, targetPos, ref _currentVelocity, Time.deltaTime, Stat.Speed * 1.4f);
                UpdateHeight();
                return;
            }

            if (ServerPos != this.transform.position)
            {
                this.transform.position = Vector3.MoveTowards(currentPosition, ServerPos, Time.deltaTime * Stat.Speed);
                UpdateHeight();
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

        protected virtual void UpdateRotation()
        {
            if (_model == null)
                return;

            _model.transform.rotation = Quaternion.Lerp(_model.transform.rotation, Quaternion.LookRotation(_currentVelocity), Time.deltaTime * 10f);
        }

        public virtual void UseSkill(SkillInfo skillInfo) { }
    }
}