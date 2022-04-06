using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using UnityEngine;

namespace Server.Game
{
    public class Monster : BaseActor
    {
        protected Vector3 _spawnPosition;
        protected Vector3 _spawnDirection;

        protected GameObject _target;

        public Monster()
        {
            ObjectType = GameObjectType.Monster;
        }

        public override void SyncPos()
        {
            base.SyncPos();

            _spawnPosition = _position;
            _spawnDirection = _direction;
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
