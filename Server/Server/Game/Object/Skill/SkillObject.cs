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

        public SkillObject(BaseActor Owner)
        {
            this.ObjectType = GameObjectType.Projectile;
            this.Owener = Owener;

            _alreadyAttackTargets = new List<BaseActor>();
        }

        public override void Init(ObjModel level)
        {
            base.Init(level);

            
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
        }

    }
}