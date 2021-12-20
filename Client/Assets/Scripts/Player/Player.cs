using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int ID;

    private PositionInfo2 _posInfo;
    public PositionInfo2 PosInfo { get { return _posInfo; } set { _posInfo = value; }  }

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

    private StatInfo _stat;
    public StatInfo Stat { get { return _stat; } set { _stat = value; } }

    private float _latency;

    public void SetServerPos(PositionInfo2 posInfo)
    {
        Direction = new Vector3(posInfo.DirX, posInfo.DirY, posInfo.DirZ);
        Position = new Vector3(posInfo.PosX, posInfo.PosY, posInfo.PosZ);
    }

    public void SetLatency(float latency)
    {
        _latency = latency;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            (bool, Vector3) result = GetClickPosition();
            if (!result.Item1)
            {
                return;
            }

            C_Move2 movePacket = new C_Move2();
            movePacket.PosInfo = PosInfo.Clone();
            movePacket.PosInfo.PosX = result.Item2.x;
            movePacket.PosInfo.PosY = result.Item2.y;
            movePacket.PosInfo.PosZ = result.Item2.z;
            Managers.Network.Send(movePacket);
        }

        UpdateMove();
    }

    private void UpdateMove()
    {
        if (Position == this.transform.position)
            return;

        float targetX = Position.x + Direction.x * Time.deltaTime * Stat.Speed * _latency;
        float targetY = Position.y;
        float targetZ = Position.z + Direction.z * Time.deltaTime * Stat.Speed * _latency;

        Position = new Vector3(targetX, targetY, targetZ);
        this.transform.position = Vector3.MoveTowards(this.transform.position, Position, Time.deltaTime * Stat.Speed);
    }

    private (bool, Vector3) GetClickPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, Mathf.Infinity))
        {
            return (true, hit.point);
        }

        return (true, Vector3.zero);
    }

    public void SyncPos()
    {
        //Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = new Vector3(PosInfo.PosX, PosInfo.PosY, PosInfo.PosZ);
    }
}
