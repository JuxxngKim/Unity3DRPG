using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Server.Game.Object
{
    class Projectile : SkillObject
    {
        public override void Init(ObjModel level, BaseActor owner)
        {
            base.Init(level, owner);

            _commandHandle = UpdateCommandProjectile;
            _stateHandle = ProcessSkill;
        }

        protected override void ProcessSkill()
        {
            if (--_stateEndFrame > 0)
                return;

            _stateHandle = null;

            var room = Room;
            room.LeaveGame(Id);
        }

        protected virtual void UpdateCommandProjectile()
        {
            if (_position == Util.ProtoPositionToVector3(PosInfo))
                return;

            var targetPos = Util.ProtoPositionToVector3(PosInfo);
            _direction = targetPos - _position;
            _direction.Normalize();

            var nextPos = Vector3.MoveTowards(_position, targetPos, _timeStamp * Stat.Speed);
            _position = nextPos;
            _postProcessHandles.Add(BroadcastMove);
        }
    }
}
