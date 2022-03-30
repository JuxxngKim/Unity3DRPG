using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game.Object;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Server.Game
{
	public class GameRoom : JobSerializer
	{
		public int RoomId { get; set; }
		public ObjModel Level { get; private set; }

		Dictionary<int, Player> _players = new Dictionary<int, Player>();
		Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
		Dictionary<int, SkillObject> _skills = new Dictionary<int, SkillObject>();

		public void Init(int mapId)
		{
			string path = $"NavMesh{mapId:D2}.obj";
			Level = new ObjModel(path);

			// TODO : Monster Load
			//path = $"Monster{mapId:D2}.obj";

			Vector3 spawnPos = new Vector3(147f, 0.0f, 160f);
			Monster monster = ObjectManager.Instance.Add<Monster>();

			monster.Info.Name = $"Player_{monster.Info.ObjectId}";
			monster.Info.PosInfo.State = ActorState.Idle;
			monster.Info.PosInfo.DirX = 0;
			monster.Info.PosInfo.DirY = 0;
			monster.Info.PosInfo.DirZ = 0;

			monster.Info.PosInfo.PosX = spawnPos.x;
			monster.Info.PosInfo.PosY = spawnPos.y;
			monster.Info.PosInfo.PosZ = spawnPos.z;
			monster.Info.TeamType = TeamType.Friendly;

			StatInfo stat = new StatInfo();
			stat.Attack = 1;
			stat.Hp = stat.MaxHp = 10;
			stat.Speed = 7f;
			monster.Stat.MergeFrom(stat);

			monster.SyncPos();
			monster.Init(Level);

			Push(EnterGame, monster, TeamType.Opponent);
		}

		// 누군가 주기적으로 호출해줘야 한다
		public void Update()
		{
			Flush();
		}

		public void EnterGame(GameObject gameObject, TeamType teamType)
		{
			if (gameObject == null)
				return;

			GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

			if (type == GameObjectType.Player)
			{
				Player player = gameObject as Player;
				_players.Add(gameObject.Id, player);
				player.Room = this;

				// 본인한테 정보 전송
				{
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = player.Info;
                    player.Session.Send(enterPacket);

                    S_Spawn spawnPacket = new S_Spawn();
                    foreach (Player p in _players.Values)
                    {
                        if (player != p)
                            spawnPacket.Objects.Add(p.Info);
                    }

                    player.Session.Send(spawnPacket);
                }

				player.Update();
			}
			else if (type == GameObjectType.Monster)
            {
				Monster monster = gameObject as Monster;

				_monsters

			}
            else if (type == GameObjectType.Skill)
			{
				SkillObject skillObject = gameObject as SkillObject;
				_skills.Add(gameObject.Id, skillObject);

				skillObject.Room = this;
				skillObject.Update();
			}

            // 타인한테 정보 전송
            {
                S_Spawn spawnPacket = new S_Spawn();
                spawnPacket.Objects.Add(gameObject.Info);
                foreach (Player p in _players.Values)
                {
                    if (p.Id != gameObject.Id)
                        p.Session.Send(spawnPacket);
                }
            }
		}

		public void LeaveGame(int objectId)
		{
			GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

			if (type == GameObjectType.Player)
			{
				Player player = null;
				if (_players.Remove(objectId, out player) == false)
					return;

				player.Room = null;

				// 본인한테 정보 전송
				{
					S_LeaveGame leavePacket = new S_LeaveGame();
					player.Session.Send(leavePacket);
				}
			}
			else if(type == GameObjectType.Skill)
            {
				SkillObject skillObject = null;
				if (_skills.Remove(objectId, out skillObject) == false)
					return;
			}

			// 타인한테 정보 전송
			{
				S_Despawn despawnPacket = new S_Despawn();
				despawnPacket.ObjectIds.Add(objectId);
				foreach (Player p in _players.Values)
				{
					if (p.Id != objectId)
						p.Session.Send(despawnPacket);
				}
			}
		}

		public void HandleMove(Player player, C_Move movePacket)
        {
			PositionInfo movePosInfo = movePacket.PosInfo;
			ObjectInfo info = player.Info;

			info.PosInfo.State = movePosInfo.State;
			info.PosInfo.PosX = movePosInfo.PosX;
			info.PosInfo.PosY = 0;
			info.PosInfo.PosZ = movePosInfo.PosZ;
		}

        public void HandleSkill(Player player, C_Skill skillPacket)
        {
			if (player == null)
				return;

			ObjectInfo info = player.Info;
			if (info.PosInfo.State == ActorState.Attack)
				return;

			SkillObject skillObject = null;
            int skillId = skillPacket.Info.SkillId;
            switch (skillId)
            {
                case 0: skillObject = ObjectManager.Instance.Add<Projectile>(); break;
                case 1: skillObject = ObjectManager.Instance.Add<SkillObject>(); break;
                default: return;
            }

            skillObject.Init(Level, player, skillPacket.Info);
			player.UseSkill(skillPacket.Info);

			PushAfter(250, EnterGame, skillObject, info.TeamType);
		}

		public void Broadcast(IMessage packet)
		{
			foreach (Player p in _players.Values)
			{
				p.Session.Send(packet);
			}
		}
	}
}
