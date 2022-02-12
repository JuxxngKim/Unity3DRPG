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

        base.Update();
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
