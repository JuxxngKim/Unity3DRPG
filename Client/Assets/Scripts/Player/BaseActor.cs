using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeongJ.Inagme
{
    public class BaseActor : MonoBehaviour
    {
        [SerializeField] GameObject _model;
        [SerializeField] Animator _animator;
        [SerializeField] float _groundedRayDistance = 30f;

        public int Id { get; private set; }
        public StatInfo Stat { get { return _stat; } set { _stat = value; } }
        public PositionInfo PosInfo { get { return _posInfo; } set { _posInfo = value; } }
        
        public Vector3 ServerDir
        {
            get { return new Vector3(_posInfo.DirX, _posInfo.DirY, _posInfo.DirZ); }
            set
            {
                _posInfo.DirX = value.x;
                _posInfo.DirY = value.y;
                _posInfo.DirZ = value.z;
            }
        }

        public Vector3 ServerPos
        {
            get { return new Vector3(_posInfo.PosX, _posInfo.PosY, _posInfo.PosZ); }
            set 
            { 
                _posInfo.PosX = value.x; 
                _posInfo.PosY = value.y; 
                _posInfo.PosZ = value.z; 
            }
        }

        protected StatInfo _stat;
        protected PositionInfo _posInfo;
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
            ServerDir = new Vector3(posInfo.DirX, posInfo.DirY, posInfo.DirZ);
            ServerPos = new Vector3(posInfo.PosX, posInfo.PosY, posInfo.PosZ);
        }

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

        public void SyncPos()
        {
            transform.position = new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
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
                return;
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
            _model.transform.rotation = Quaternion.Lerp(_model.transform.rotation, Quaternion.LookRotation(_currentVelocity), Time.deltaTime * 10f);
        }
    }
}