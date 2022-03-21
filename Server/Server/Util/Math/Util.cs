using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

static class Util
{
    public static Vector3 ProtoPositionToVector3(PositionInfo posInfo)
    {
        return new Vector3(posInfo.PosX, posInfo.PosY, posInfo.PosZ);
    }

    public static Vector3 ToVector3(this Float3 float3)
    {
        return new Vector3(float3.X, float3.Y, float3.Z);
    }

    public static Vector3 ProtoDirectionToVector3(PositionInfo posInfo)
    {
        return new Vector3(posInfo.DirX, posInfo.DirY, posInfo.DirZ);
    }

    public static PositionInfo Vector3ToPosInfo(Vector3 position, Vector3 diretion)
    {
        var posInfo = new PositionInfo();
        posInfo.DirX = diretion.x;
        posInfo.DirY = diretion.y;
        posInfo.DirZ = diretion.z;

        posInfo.PosX = position.x;
        posInfo.PosY = position.y;
        posInfo.PosZ = position.z;

        return posInfo;
    }

}
