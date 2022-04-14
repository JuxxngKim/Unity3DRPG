using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game.Object
{
    class AreaSkill : SkillObject
    {
        int _hitDelayFrame;

        public override void Init(ObjModel level, BaseActor owner, SkillInfo skillInfo)
        {
            base.Init(level, owner, skillInfo);

            _direction = UnityEngine.Vector3.zero;
            PosInfo.Direction = _direction.ToFloat3();

            _commandHandle = UpdateCommandMeteo;
            _hitDelayFrame = _skillData.HitDelayFrame;
        }

        protected virtual void UpdateCommandMeteo()
        {
            if (--_hitDelayFrame > 0)
                return;

            _commandHandle = null;
            var targets = Room?.IsCollisition(Info.TeamType, _position, Stat.Radius);
            if (targets == null || targets.Count <= 0)
                return;

            foreach(var target in targets)
            {
                if (!target.IsAlive)
                    continue;

                target.OnDamaged(this, Stat.Attack);
            }
        }
    }
}
