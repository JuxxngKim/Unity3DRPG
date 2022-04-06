using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Server.Game
{
    public class Monster : BaseActor
    {
        public Monster()
        {
            ObjectType = GameObjectType.Monster;
        }

        public override void OnDamaged(GameObject attacker, int damage)
        {
            base.OnDamaged(attacker, damage);
        }

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);

            // TODO: AddExp
        }
    }
}
