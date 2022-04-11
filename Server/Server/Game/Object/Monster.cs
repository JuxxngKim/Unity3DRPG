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
            if (_target != null)
            {
                if (_target.IsAlive)
                {
                    PosInfo.Position = _target.Position.ToFloat3();
                }
                else
                {
                    _target = null;
                }
            }

            if (_target?.IsAlive ?? false)
            {
                var diff = Position - _target.Position;
                if (diff.magnitude <= 1.5f)
                {
                    Console.WriteLine("ATTACK!");
                    AttackToTarget();
                    return;
                }
            }

            base.UpdateCommandIdleMove();
        }

        private void AttackToTarget()
        {
            _stateHandle = ProcessSkill;
            _commandHandle = null;

            _direction = Vector3.zero;

            PosInfo.State = ActorState.Attack;
            PosInfo.Position = _position.ToFloat3();
            PosInfo.Direction = _direction.ToFloat3();
            PosInfo.LookDirection = PosInfo.LookDirection.Clone();

            SkillInfo skillInfo = new SkillInfo();
            skillInfo.SkillId = DataPresets.DelayAttack.Id;
            skillInfo.SpawnPosition = _position.ToFloat3();
            skillInfo.SkillDirection = PosInfo.LookDirection.Clone();
            skillInfo.StateTime = Util.FrameToTime(DataPresets.DelayAttack.StateFrame);

            _stateEndFrame = DataPresets.DelayAttack.StateFrame;

            AddDamageToTarget(_target, DataPresets.DelayAttack);
            Room.Push(BroadcastMove);
            Room.Push(BroadCastSkill, skillInfo, DataPresets.DelayAttack);
        }

        private void AddDamageToTarget(GameObject target, SkillData skillData)
        {
            float delayTime = Util.FrameToTime(skillData.HitDelayFrame);
            int tickAfter = Util.TimeToTick(delayTime);

            Room.PushAfter(tickAfter, () =>
            {
                target.OnDamaged(this, skillData.Damage);

                S_Hit hitPacket = new S_Hit();
                hitPacket.AttackerId = Id;
                hitPacket.DefenderId = target.Id;
                Room?.Broadcast(hitPacket);
            });
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
