using Google.Protobuf.Protocol;
using Server.Data;
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
			_commandHandle = UpdateCommandIdleMove;

			if(_stateEndHandle != null)
            {
				_stateEndHandle();
				_stateEndHandle = null;
			}

			PosInfo.State = ActorState.Idle;
			Room.Push(BroadcastMove);
		}

		public void UseTeleportSkill(SkillInfo skillInfo)
		{
			UseSkill(skillInfo);
			_stateEndHandle = () =>
			{
				PosInfo.Position = skillInfo.SpawnPosition;
				_position = PosInfo.Position.ToVector3();
			};
		}
	}
}
