using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    private float _latency;

    public void SetLatency(float latency)
    {
        _latency = latency;
    }

    protected override void Update()
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

        base.Update();
    }

    protected override void UpdateMove()
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
}
