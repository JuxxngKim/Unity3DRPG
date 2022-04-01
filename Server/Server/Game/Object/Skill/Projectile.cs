using Google.Protobuf.Protocol;
using Server.Data;
using UnityEngine;

namespace Server.Game.Object
{
    class Projectile : SkillObject
    {
        bool _collision;

        public override void Init(ObjModel level, BaseActor owner, SkillInfo skillInfo)
        {
            base.Init(level, owner, skillInfo);

            _commandHandle = UpdateCommandProjectile;
            _stateHandle = ProcessSkill;

            _position = skillInfo.SpawnPosition.ToVector3();
            _direction = skillInfo.SkillDirection.ToVector3();

            PosInfo.Position = _position.ToFloat3();
            PosInfo.Direction = _direction.ToFloat3();

            var projectileData = DataPresets.BasicProjectile;
            Info.StatInfo.Attack = projectileData.Damage;
            Info.StatInfo.Speed = projectileData.MoveSpeed;

            _stateEndFrame = projectileData.StateFrame;
        }

        protected override void ProcessSkill()
        {
            if (--_stateEndFrame > 0)
                return;

            _commandHandle = null;
            _stateHandle = null;

            var room = Room;
            room.LeaveGame(Id);
        }

        protected virtual void UpdateCommandProjectile()
        {
            var targetPos = _position + _direction;
            var nextPos = Vector3.MoveTowards(_position, targetPos, _timeStamp * Stat.Speed);

            _position = nextPos;
            _postProcessHandles.Add(BroadcastMove);

            if (_collision)
                return;

            var targets = Room?.IsCollisition(Info.TeamType, _position, 0.8f);
            if (targets == null || targets.Count <= 0)
                return;

            var target = targets[0];
            target.OnDamaged(this, 1);
            
            _collision = true;
            _stateEndFrame = 3;
        }

        protected override void BroadcastMove()
        {
            // 다른 플레이어한테도 알려준다
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = Util.Vector3ToPosInfo(_position, _direction);
            movePacket.PosInfo.State = ActorState.Moving;
            Room?.Broadcast(movePacket);
        }
    }
}
