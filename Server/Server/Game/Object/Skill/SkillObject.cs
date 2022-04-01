using Google.Protobuf.Protocol;
using System.Collections.Generic;

namespace Server.Game.Object
{
    public class SkillObject : BaseActor
    {
        public BaseActor Owener { get; private set; }
        public int SpawnDelay => _spawnDelay;

        protected List<BaseActor> _alreadyAttackTargets;
        protected SkillInfo _skillInfo;
        protected int _spawnDelay;

        public SkillObject()
        {
            this.ObjectType = GameObjectType.Skill;
            _alreadyAttackTargets = new List<BaseActor>();
        }

        public virtual void Init(ObjModel level, BaseActor owner, SkillInfo skillInfo)
        {
            base.Init(level);
            
            Owener = owner;

            _commandHandle = null;
            _stateHandle = ProcessSkill;
            _skillInfo = skillInfo;

            _position = skillInfo.SpawnPosition.ToVector3();
            _direction = skillInfo.SkillDirection.ToVector3();

            PosInfo.Position = _position.ToFloat3();
            PosInfo.Direction = _direction.ToFloat3();

            //Room.PushAfter(_spawnDelay, Room.EnterGame, this, Info.TeamType);
        }

        public override void Remove()
        {
            base.Remove();

            _alreadyAttackTargets = null;
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
    }
}