using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using Server.Game;
using Server.Data;
using UnityEngine;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public Player MyPlayer { get; set; }
        public int SessionId { get; set; }

        public void Send(IMessage packet)
        {
            string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
            MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);
            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);
            Send(new ArraySegment<byte>(sendBuffer));
        }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            GameRoom room = RoomManager.Instance.Find(1);
            Vector3 spawnPos = room.Level?.GetRandomNavMeshPos() ?? Vector3.zero;

            MyPlayer = ObjectManager.Instance.Add<Player>();
            {
                MyPlayer.Info.Name = $"Player_{MyPlayer.Info.ObjectId}";
                MyPlayer.Info.PosInfo.State = CreatureState.Idle;
                MyPlayer.Info.PosInfo.DirX = 0;
                MyPlayer.Info.PosInfo.DirY = 0;
                MyPlayer.Info.PosInfo.DirZ = -1;

                MyPlayer.Info.PosInfo.PosX = spawnPos.x;
                MyPlayer.Info.PosInfo.PosY = spawnPos.y;
                MyPlayer.Info.PosInfo.PosZ = spawnPos.z;

                StatInfo stat = new StatInfo();
                stat.Attack = 1;
                stat.Hp = stat.MaxHp = 10;
                stat.Speed = 10;
                MyPlayer.Stat.MergeFrom(stat);

                MyPlayer.Session = this;

                MyPlayer.SyncPos();
                MyPlayer.InitMap(room.Level);
            }

            room.Push(room.EnterGame, MyPlayer);

            Ping();
        }

        private void Ping()
        {
            S_Ping packet = new S_Ping();
            packet.Time = System.DateTime.UtcNow.Ticks;
            Send(packet);

            GameRoom room = RoomManager.Instance.Find(1);
            room.PushAfter(5000, () => { Ping(); });
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            GameRoom room = RoomManager.Instance.Find(1);
            room.Push(room.LeaveGame, MyPlayer.Info.ObjectId);

            SessionManager.Instance.Remove(this);

            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
