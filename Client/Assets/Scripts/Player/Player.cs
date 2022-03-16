using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeongJ.Inagme
{
    public class Player : BaseActor
    {
        [SerializeField] SkillObject[] _skillObjects;

        public SkillObject GetSkillObject(int skillId)
        {
            if (_skillObjects.Length <= skillId)
                return null;

            return _skillObjects[skillId];
        }
    }
}