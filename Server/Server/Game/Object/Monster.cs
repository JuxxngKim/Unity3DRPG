using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game.Object;
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

        protected BaseActor _target;

        public Monster()
        {
            ObjectType = GameObjectType.Monster;
        }

        public override void SyncPos()
        {
            base.SyncPos();

            _spawnPosition = _position;
            _spawnDirection = PosInfo.LookDirection.ToVector3();
        }

        public override void OnDamaged(BaseActor attacker, int damage)
        {
            if (_target == null)
            {
                if(attacker is SkillObject skilObject)
                {
                    _target = skilObject.Owener;
                }
                else
                {
                    _target = attacker;
                }
            }

            base.OnDamaged(attacker, damage);
        }

        protected override void UpdateCommandIdleMove()
        {
            if(_target != null && _target.IsAlive)
            {
                PosInfo.Position = _target.Position.ToFloat3();
            }

            base.UpdateCommandIdleMove();
        }

        protected override void BroadcastMove()
        {
            if (_position == PosInfo.Position.ToVector3())
            {
                _direction = Vector3.zero;
            }

            // 다른 플레이어한테도 알려준다
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;

            movePacket.PosInfo = new PositionInfo();
            movePacket.PosInfo.Position = _position.ToFloat3();
            movePacket.PosInfo.Direction = _direction.ToFloat3();
            movePacket.PosInfo.LookDirection = PosInfo.LookDirection;
            movePacket.PosInfo.State = PosInfo.State;

            Console.Clear();
            Console.WriteLine($"Pos : {_position}, Dir : {_direction}");

            Room?.Broadcast(movePacket);
        }



        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);

            _commandHandle = null;
            _target = null;

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
            _direction = Vector3.zero;

            Stat.Hp = Stat.MaxHp;
            PosInfo.State = ActorState.Idle;
            PosInfo.Position = _position.ToFloat3();
            PosInfo.Direction = _direction.ToFloat3();
            PosInfo.LookDirection = _spawnDirection.ToFloat3();

            SyncPos();
            Init(Level);

            room.EnterGame(this, Info.TeamType);
        }
    }
}
