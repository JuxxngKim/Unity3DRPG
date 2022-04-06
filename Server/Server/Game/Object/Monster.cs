using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using UnityEngine;

namespace Server.Game
{
    public class Monster : BaseActor
    {
        protected Vector3 _spawnPosition;
        protected Vector3 _spawnDirection;

        protected GameObject _target;

        public Monster()
        {
            ObjectType = GameObjectType.Monster;
        }

        public override void SyncPos()
        {
            base.SyncPos();

            _spawnPosition = _position;
            _spawnDirection = _direction;
        }

        public override void OnDamaged(GameObject attacker, int damage)
        {
            base.OnDamaged(attacker, damage);
        }

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);

            S_Die diePacket = new S_Die();
            diePacket.ObjectId = Id;
            diePacket.AttackerId = attacker.Id;
            Room.Broadcast(diePacket);

            GameRoom room = Room;
            room.PushAfter(3000, LeaveGame);
            room.PushAfter(8000, RespawnGame, room);
        }

        protected override void RespawnGame(GameRoom room)
        {
            base.RespawnGame(room);

            _position = _spawnPosition;
            _direction = _spawnDirection;

            Stat.Hp = Stat.MaxHp;
            PosInfo.State = ActorState.Idle;
            PosInfo.Position = _position.ToFloat3();
            PosInfo.Direction = _direction.ToFloat3();

            Init(Level);

            room.EnterGame(this, Info.TeamType);
        }
    }
}
