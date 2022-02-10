using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public class GameRoom : JobSerializer
	{
		public int RoomId { get; set; }
		public ObjModel Level { get; private set; }

		private Dictionary<int, Player> _players = new Dictionary<int, Player>();

		public void Init(int mapId)
		{
			string path = $"NavMesh{mapId:D2}.obj";
			Level = new ObjModel(path);
		}

		// 누군가 주기적으로 호출해줘야 한다
		public void Update()
		{
			Flush();
		}

		public void EnterGame(GameObject gameObject)
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
                    S_EnterGame2 enterPacket = new S_EnterGame2();
                    enterPacket.Player = player.Info;
                    player.Session.Send(enterPacket);

                    S_Spawn2 spawnPacket = new S_Spawn2();
                    foreach (Player p in _players.Values)
                    {
                        if (player != p)
                            spawnPacket.Objects.Add(p.Info);
                    }

                    player.Session.Send(spawnPacket);
                }

				player.Update();
			}
			
			// 타인한테 정보 전송
			{
                S_Spawn2 spawnPacket = new S_Spawn2();
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

		public void HandleMove2(Player player, C_Move2 movePacket)
        {
			PositionInfo2 movePosInfo = movePacket.PosInfo;
			ObjectInfo2 info = player.Info;

			info.PosInfo.State = movePosInfo.State;
			info.PosInfo.PosX = movePosInfo.PosX;
			info.PosInfo.PosY = 0;
			info.PosInfo.PosZ = movePosInfo.PosZ;
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
