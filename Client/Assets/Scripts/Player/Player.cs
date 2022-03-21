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

            _animator.SetTrigger(Const.TriggerSkill);

            Vector3 skillDir = skillInfo.SkillDirection.ToVector3();
            _currentVelocity = skillDir;
            _model.transform.rotation = Quaternion.LookRotation(skillDir);
        }
    }
}