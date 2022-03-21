using Google.Protobuf.Protocol;
using System.Collections.Generic;

namespace Server.Game.Object
{
    public class SkillObject : BaseActor
    {
        public BaseActor Owener { get; private set; }

        protected List<BaseActor> _alreadyAttackTargets;
        protected SkillInfo _skillInfo;

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

            _stateHandle = null;

            var room = Room;
            room.LeaveGame(Id);
        }
    }
}