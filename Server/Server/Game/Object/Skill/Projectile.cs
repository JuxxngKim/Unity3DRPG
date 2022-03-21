using Google.Protobuf.Protocol;
using UnityEngine;

namespace Server.Game.Object
{
    class Projectile : SkillObject
    {
        public override void Init(ObjModel level, BaseActor owner, SkillInfo skillInfo)
        {
            base.Init(level, owner, skillInfo);

            _commandHandle = UpdateCommandProjectile;
            _stateHandle = ProcessSkill;

            _position = skillInfo.SpawnPosition.ToVector3();
            _direction = skillInfo.SkillDirection.ToVector3();

            PosInfo.PosX = _position.x;
            PosInfo.PosY = 0;
            PosInfo.PosZ = _position.z;

            PosInfo.DirX = _direction.x;
            PosInfo.DirY = _direction.y;
            PosInfo.DirZ = _direction.z;

            Info.StatInfo.Speed = 20f;
            Info.StatInfo.Attack = 10;

            _stateEndFrame = 15;
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
        }

        protected override void BroadcastMove()
        {
            // 다른 플레이어한테도 알려준다
            S_Move movePacket = new S_Move();
            movePacket.ObjectId = Id;
            movePacket.PosInfo = Util.Vector3ToPosInfo(_position, _direction);
            Room?.Broadcast(movePacket);
        }
    }
}
