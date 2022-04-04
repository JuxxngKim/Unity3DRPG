using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeongJ.Inagme
{
    public class Player : BaseActor
    {
        public override void UseSkill(SkillInfo skillInfo)
        {
            base.UseSkill(skillInfo);

            switch (skillInfo.SkillId)
            {
                case 1: _animator.SetTrigger(Const.TriggerAttack); break;
                default: _animator.SetTrigger(Const.TriggerSkill); break;
            }

            Vector3 skillDir = skillInfo.SkillDirection.ToVector3();
            _currentVelocity = skillDir;
            _model.transform.rotation = Quaternion.LookRotation(skillDir);
        }
    }
}