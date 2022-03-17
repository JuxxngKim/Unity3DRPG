using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YeongJ.Inagme;

namespace YeongJ.Inagme
{
    public class SkillObject : BaseActor
    {
        [SerializeField] GameObject _hitEffect;

        public override void Remove()
        {
            base.Remove();

            var hitEffect = GameObjectCache.Make(_hitEffect.transform, this.transform.parent);
            GameObjectCache.DeleteDelayed(hitEffect, delayTime: 1.08f);
        }
    }
}