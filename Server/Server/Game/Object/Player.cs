using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Server.Game
{
	public class Player : BaseActor
	{
		public ClientSession Session { get; set; }

		public Player()
		{
			ObjectType = GameObjectType.Player;
		}

		protected override void ProcessSkill()
		{
			if (--_stateEndFrame > 0)
				return;

			_stateHandle = null;
		}

		public void UseSkill(int skillId)
        {
			_stateHandle = ProcessSkill;
			_commandHandle = null;

			_direction = Vector3.zero;

			PosInfo.State = ActorState.Attack;
			PosInfo.PosX = _position.x;
			PosInfo.PosY = 0;
			PosInfo.PosZ = _position.z;

			PosInfo.DirX = _direction.x;
			PosInfo.DirY = _direction.y;
			PosInfo.DirZ = _direction.z;

			Room.Push(BroadcastMove);
			Room.Push(BroadCastSkill, skillId);
		}

		protected void BroadCastSkill(int skillId)
		{
			SkillInfo skillInfo = new SkillInfo();
			skillInfo.SkillId = skillId;

			// 다른 플레이어한테도 알려준다
			S_Skill skillPacket = new S_Skill();
			skillPacket.ObjectId = Id;
			skillPacket.Info = skillInfo;
			Room?.Broadcast(skillPacket);
		}
	}
}
