using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int Id;

    [SerializeField] GameObject _model;

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

    public void SetServerPos(PositionInfo2 posInfo)
    {
        ServerDir = new Vector3(posInfo.DirX, posInfo.DirY, posInfo.DirZ);
        ServerPos = new Vector3(posInfo.PosX, posInfo.PosY, posInfo.PosZ);
    }

    protected virtual void Update()
    {
        UpdateMove();
    }

    protected virtual void UpdateMove()
    {
        if (ServerPos == this.transform.position)
            return;

        //float targetX = ServerPos.x + ServerDir.x * Time.deltaTime * Stat.Speed;
        //float targetY = ServerPos.y;
        //float targetZ = ServerPos.z + ServerDir.z * Time.deltaTime * Stat.Speed;

        //ServerPos = new Vector3(targetX, targetY, targetZ);
        this.transform.position = Vector3.MoveTowards(this.transform.position, ServerPos, Time.deltaTime * Stat.Speed);
        _model.transform.rotation = Quaternion.Lerp(_model.transform.rotation, Quaternion.LookRotation(ServerDir), Time.deltaTime * 10f);
    }

    public void SyncPos()
    {
        transform.position = new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
    }
}
