using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Server.Game
{
	public class GameObject
	{
		public GameObjectType ObjectType { get; protected set; } = GameObjectType.None;
		public int Id
		{
			get { return Info.ObjectId; }
			set { Info.ObjectId = value; }
		}

		public GameRoom Room { get; set; }

		public ObjectInfo2 Info { get; set; } = new ObjectInfo2();
		public PositionInfo2 PosInfo { get; private set; } = new PositionInfo2();
		public StatInfo Stat { get; private set; } = new StatInfo();

		protected Vector3 _position;
		protected Vector3 _direction;

		public GameObject()
		{
			Info.PosInfo = PosInfo;
			Info.StatInfo = Stat;
		}

		public virtual void Update(float deltaTime)
		{

		}

		public virtual void OnDamaged(GameObject attacker, int damage)
		{
			if (Room == null)
				return;

			Stat.Hp = Math.Max(Stat.Hp - damage, 0);

			S_ChangeHp changePacket = new S_ChangeHp();
			changePacket.ObjectId = Id;
			changePacket.Hp = Stat.Hp;
			Room.Broadcast(changePacket);

			if (Stat.Hp <= 0)
			{
				OnDead(attacker);
			}
		}

		public virtual void OnDead(GameObject attacker)
		{
			if (Room == null)
				return;

			//S_Die diePacket = new S_Die();
			//diePacket.ObjectId = Id;
			//diePacket.AttackerId = attacker.Id;
			//Room.Broadcast(diePacket);

			//GameRoom room = Room;
			//room.LeaveGame(Id);

			//Stat.Hp = Stat.MaxHp;
			//PosInfo.State = CreatureState.Idle;
			//PosInfo.MoveDir = MoveDir.Down;
			//PosInfo.PosX = 0;
			//PosInfo.PosY = 0;

			//room.EnterGame(this);
		}

		public virtual void SyncPos()
		{
			_position = Util.ProtoPositionToVector3(PosInfo);
			_direction = Util.ProtoDirectionToVector3(PosInfo);
		}
	}
}
