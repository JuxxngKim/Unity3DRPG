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

            _commandHandle = UpdateCommandMeteo;

            var skillData = DataPresets.Meteo;
            Info.StatInfo.Attack = skillData.Damage;
            Info.StatInfo.Speed = skillData.MoveSpeed;
            Info.StatInfo.Radius = skillData.Range;
            Info.Name = skillData.Name;
            
            _spawnDelay = skillData.SpawnDelayTick;
            _stateEndFrame = skillData.StateFrame;
            _hitDelayFrame = skillData.HitDelayFrame;

            //Room.PushAfter(_spawnDelay, Room.EnterGame, this, Info.TeamType);
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
                target.OnDamaged(this, Stat.Attack);

                S_Hit hitPacket = new S_Hit();
                hitPacket.AttackerId = Id;
                hitPacket.DefenderId = target.Id;
                Room?.Broadcast(hitPacket);
            }
        }
    }
}
