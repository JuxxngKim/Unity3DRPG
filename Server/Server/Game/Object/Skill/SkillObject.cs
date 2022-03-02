using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Server.Game.Object
{
    public class SkillObject : BaseActor
    {
        public BaseActor Owener { get; private set; }

        protected List<BaseActor> _alreadyAttackTargets;

        public SkillObject()
        {
            this.ObjectType = GameObjectType.Skill;
            _alreadyAttackTargets = new List<BaseActor>();
        }

        public void Init(ObjModel level, BaseActor owner)
        {
            base.Init(level);
            
            Owener = owner;

            _commandHandle = null;
            _stateHandle = ProcessSkill;
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