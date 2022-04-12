using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YeongJ.Inagme;

namespace YeongJ.UI
{
    public class HitManager : UISingleton<HitManager>
    {
        [SerializeField] DamageFont _templateDamageFont;

        public void AddDamageFont(int objectId, int damage)
        {
            var ownerActor = GetActor(objectId);
            if (ownerActor == null)
                return;

            var spawnPosition = Camera.main.WorldToScreenPoint(ownerActor.UIRoot.transform.position);
            var newDamageFont = GameObjectCache.Make(_templateDamageFont, this.transform);

            newDamageFont.Init(damage, lifeTime: 1.0f);
            newDamageFont.transform.position = spawnPosition;

            GameObjectCache.DeleteDelayed(newDamageFont.transform, 1.0f);
        }

        public void AddHitEffect(int attackerId, int defenderId)
        {
            var attacker = GetActor(attackerId);
            if (attacker == null)
                return;

            var defender = GetActor(defenderId);
            if (defender == null)
                return;

            Transform hitEffect = attacker.GetComponent<BaseActor>()?.HitEffect?.transform;
            if (hitEffect == null)
                return;

            var spawnPosition = defender.ActorRoot?.transform?.position ?? Vector3.zero;
            var attackEffect = GameObjectCache.Make(hitEffect, defender.transform.parent);
            attackEffect.transform.position = spawnPosition;

            GameObjectCache.DeleteDelayed(attackEffect, delayTime: 1.1f);
        }

        private BaseActor GetActor(int objectId)
        {
            return Managers.Object.FindById(objectId)?.GetComponent<BaseActor>();
        }
    }
}