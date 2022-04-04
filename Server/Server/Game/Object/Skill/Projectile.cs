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
        }

        protected virtual void UpdateCommandProjectile()
        {
            var targetPos = _position + _direction;
            var nextPos = Vector3.MoveTowards(_position, targetPos, _timeStamp * Stat.Speed);

            _position = nextPos;
            _postProcessHandles.Add(BroadcastMove);

            if (_collision)
                return;

            var targets = Room?.IsCollisition(Info.TeamType, _position, Stat.Radius);
            if (targets == null || targets.Count <= 0)
                return;

            var target = targets[0];
            target.OnDamaged(this, Stat.Attack);
            
            _collision = true;
            _stateEndFrame = 2;
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
