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
		protected delegate void StateEndHandle();
		protected StateEndHandle _stateEndHandle = null;

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

		public void UseSkill(SkillInfo skillInfo)
        {
			_stateHandle = ProcessSkill;
			_commandHandle = null;

			_direction = Vector3.zero;

			PosInfo.State = ActorState.Attack;
			PosInfo.Position = _position.ToFloat3();
			PosInfo.Direction = _direction.ToFloat3();
			PosInfo.LookDirection = skillInfo.SkillDirection.Clone();

			if (!DataPresets.SkillDatas.TryGetValue(skillInfo.SkillId, out var skilldata))
				return;
			if (skilldata == null)
				return;

			_stateEndFrame = skilldata.StateFrame;

			Room.Push(BroadcastMove);
			Room.Push(BroadCastSkill, skillInfo, skilldata);
		}

		protected void BroadCastSkill(SkillInfo skillInfo, SkillData skilldata)
		{
			SkillInfo sendSkillInfo = new SkillInfo();
			sendSkillInfo.SkillId = skillInfo.SkillId;
			sendSkillInfo.SpawnPosition = skillInfo.SpawnPosition.Clone();
			sendSkillInfo.SkillDirection = skillInfo.SkillDirection.Clone();

			// 다른 플레이어한테도 알려준다
			S_Skill skillPacket = new S_Skill();
			skillPacket.ObjectId = Id;
			skillPacket.Info = skillInfo;
			skillPacket.Info.StateTime = Util.FrameToTime(skilldata.StateFrame);
			Room?.Broadcast(skillPacket);
		}
	}
}
