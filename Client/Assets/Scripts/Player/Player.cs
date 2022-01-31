using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Id;

    [SerializeField] GameObject _model;
    [SerializeField] Animator _animator;

    protected PositionInfo2 _posInfo;
    public PositionInfo2 PosInfo { get { return _posInfo; } set { _posInfo = value; } }

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
        set { _posInfo.PosX = value.x; _posInfo.PosY = value.y; _posInfo.PosZ = value.z; }
    }

    protected StatInfo _stat;
    public StatInfo Stat { get { return _stat; } set { _stat = value; } }

    private Vector3 _currentVelocity = Vector3.zero;

    public void SetServerPos(PositionInfo2 posInfo)
    {
        ServerDir = new Vector3(posInfo.DirX, posInfo.DirY, posInfo.DirZ);
        ServerPos = new Vector3(posInfo.PosX, posInfo.PosY, posInfo.PosZ);
        Debug.LogError($"ServerDir : {ServerDir}");
    }

    protected virtual void Update()
    {
        UpdateMove();
        UpdateRotation();
    }

    protected virtual void UpdateMove()
    {
        _animator.SetFloat("Velocity", ServerDir.magnitude);

        if (ServerDir != Vector3.zero)
        {
            float targetX = ServerPos.x + ServerDir.x * Time.deltaTime * Stat.Speed;
            float targetY = ServerPos.y;
            float targetZ = ServerPos.z + ServerDir.z * Time.deltaTime * Stat.Speed;

            var targetPos = new Vector3(targetX, targetY, targetZ);
            this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPos, ref _currentVelocity, Time.deltaTime, Stat.Speed * 1.4f);
            return;
        }

        if (ServerPos != this.transform.position)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, ServerPos, Time.deltaTime * Stat.Speed);
            return;
        }
    }

    protected virtual void UpdateRotation()
    {
        _model.transform.rotation = Quaternion.Lerp(_model.transform.rotation, Quaternion.LookRotation(_currentVelocity), Time.deltaTime * 10f);
    }

    public void SyncPos()
    {
        transform.position = new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
    }
}
