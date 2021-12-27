using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int ID;

    protected PositionInfo2 _posInfo;
    public PositionInfo2 PosInfo { get { return _posInfo; } set { _posInfo = value; } }

    public Vector3 Direction
    {
        get { return new Vector3(_posInfo.DirX, _posInfo.DirY, _posInfo.DirZ); }
        set { _posInfo.DirX = value.x; _posInfo.DirX = value.y; _posInfo.DirZ = value.z; }
    }

    public Vector3 Position
    {
        get { return new Vector3(_posInfo.PosX, _posInfo.PosY, _posInfo.PosZ); }
        set { _posInfo.PosX = value.x; _posInfo.PosY = value.y; _posInfo.PosZ = value.z; }
    }

    protected StatInfo _stat;
    public StatInfo Stat { get { return _stat; } set { _stat = value; } }

    public void SetServerPos(PositionInfo2 posInfo)
    {
        Direction = new Vector3(posInfo.DirX, posInfo.DirY, posInfo.DirZ);
        Position = new Vector3(posInfo.PosX, posInfo.PosY, posInfo.PosZ);
    }

    protected virtual void Update()
    {
        UpdateMove();
    }

    protected virtual void UpdateMove()
    {
        if (Position == this.transform.position)
            return;

        float targetX = Position.x + Direction.x * Time.deltaTime * Stat.Speed;
        float targetY = Position.y;
        float targetZ = Position.z + Direction.z * Time.deltaTime * Stat.Speed;

        Position = new Vector3(targetX, targetY, targetZ);
        this.transform.position = Vector3.MoveTowards(this.transform.position, Position, Time.deltaTime * Stat.Speed);
    }

    public void SyncPos()
    {
        transform.position = new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
    }
}
