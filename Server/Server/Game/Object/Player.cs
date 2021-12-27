using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Server.Game
{
	public class Player : GameObject
	{
		public ClientSession Session { get; set; }

		public Player()
		{
			ObjectType = GameObjectType.Player;
		}

		public override void OnDamaged(GameObject attacker, int damage)
		{
			base.OnDamaged(attacker, damage);
		}

		public override void OnDead(GameObject attacker)
		{
			base.OnDead(attacker);
		}

        public override void Update(float deltaTime)
		{
            base.Update(deltaTime);

			if(_position != Util.ProtoPositionToVector3(PosInfo))
            {
				UpdateMove(deltaTime);
			}
		}

		private void UpdateMove(float deltaTime)
        {
			var targetPos = Util.ProtoPositionToVector3(PosInfo);
			_direction = targetPos - _position;
			_direction.Normalize();

			_position = Vector3.MoveTowards(_position, targetPos, deltaTime * 10.0f);
			BroadcastMove();
		}

		void BroadcastMove()
		{
			// 다른 플레이어한테도 알려준다
			S_Move2 movePacket = new S_Move2();
			movePacket.ObjectId = Id;
			movePacket.PosInfo = Util.Vector3ToPosInfo(_position, _direction);
			Room.Broadcast(movePacket);
		}
	}
}
