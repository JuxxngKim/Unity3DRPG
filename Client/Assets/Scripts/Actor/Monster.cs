using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeongJ.Inagme
{
    public class Monster : BaseActor
    {
        public override void UseSkill(SkillInfo skillInfo)
        {
            base.UseSkill(skillInfo);

            _animator.SetTrigger(Const.TriggerAttack);
        }
    }
}