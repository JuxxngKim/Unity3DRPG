using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YeongJ.Inagme;

class PacketHandler
{
    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S_EnterGame enterGamePacket = packet as S_EnterGame;
        Managers.Object.Add(enterGamePacket.Player, myPlayer: true);
    }

    public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
    {
        S_LeaveGame leaveGameHandler = packet as S_LeaveGame;
        Managers.Object.Clear();
    }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        S_Spawn spawnPacket = packet as S_Spawn;
        foreach (ObjectInfo obj in spawnPacket.Objects)
        {
            Managers.Object.Add(obj, myPlayer: false);
        }
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        S_Despawn despawnPacket = packet as S_Despawn;
        foreach (int id in despawnPacket.ObjectIds)
        {
            Managers.Object.Remove(id);
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        S_Move movePacket = packet as S_Move;

        GameObject go = Managers.Object.FindById(movePacket.ObjectId);
        if (go == null)
            return;

        var player = go?.GetComponent<Player>();
        if (player == null)
            return;

        player.SetServerPos(movePacket.PosInfo);
    }

    public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
    {
        S_ChangeHp changePacket = packet as S_ChangeHp;

        GameObject go = Managers.Object.FindById(changePacket.ObjectId);
        if (go == null)
            return;

        ////CreatureController cc = go.GetComponent<CreatureController>();
        //if (cc != null)
        //{
        //	cc.Hp = changePacket.Hp;
        //}
    }

    public static void S_DieHandler(PacketSession session, IMessage packet)
    {
        S_Die diePacket = packet as S_Die;

        GameObject go = Managers.Object.FindById(diePacket.ObjectId);
        if (go == null)
            return;

        //CreatureController cc = go.GetComponent<CreatureController>();
        //if (cc != null)
        //{
        //	cc.Hp = 0;
        //	cc.OnDead();
        //}
    }

    public static void S_PingHandler(PacketSession session, IMessage packet)
    {
        S_Ping pingPacket = packet as S_Ping;
        var latencyTicks = DateTime.UtcNow.Ticks - pingPacket.Time;
        var timeSpan = TimeSpan.FromTicks(latencyTicks);
        float latency = timeSpan.Milliseconds / 1000f;

        var myPlayer = Managers.Object?.MyPlayer;
        if (myPlayer == null)
        {
            return;
        }

        myPlayer.SetLatency(latency);
    }

    public static void S_SkillHandler(PacketSession session, IMessage packet)
    {
    }
}