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
		public ObjModel Level { get; private set; }

		private NavMeshTriangle _currentNavMesh;

		public Player()
		{
			ObjectType = GameObjectType.Player;
		}

        public void Init(ObjModel level)
        {
            if (level == null)
                return;

			Level = level;

			Vector3 myPosition = Util.ProtoPositionToVector3(PosInfo);

			var triangles = level.Triangles;
			for (int i = 0; i < triangles.Count; ++i)
			{
				NavMeshTriangle navMeshTriangle = triangles[i];
				bool inSide = navMeshTriangle.InSidePoint(myPosition);
				if (inSide)
				{
					_currentNavMesh = navMeshTriangle;
					break;
				}
			}
		}

        public override void OnDamaged(GameObject attacker, int damage)
		{
			base.OnDamaged(attacker, damage);
		}

		public override void OnDead(GameObject attacker)
		{
			base.OnDead(attacker);
		}

        public override void Update()
		{
            base.Update();

			if (_position == Util.ProtoPositionToVector3(PosInfo))
				return;

            UpdateMove(0.1f);
		}

		private void UpdateMove(float deltaTime)
        {
			if (_currentNavMesh == null)
				return;

			var targetPos = Util.ProtoPositionToVector3(PosInfo);
			_direction = targetPos - _position;
			_direction.Normalize();

			var nextPos = Vector3.MoveTowards(_position, targetPos, deltaTime * Stat.Speed);
			bool inSide = _currentNavMesh.InSidePoint(nextPos);
			if (inSide)
			{
				_position = nextPos;
				BroadcastMove();
				return;
			}

			var nextNavMesh = _currentNavMesh.CalcInSideSiblingNavMesh(nextPos);
            if (nextNavMesh != null)
            {
                _currentNavMesh = nextNavMesh;
                _position = nextPos;
				BroadcastMove();
				return;
			}


            var triangles = Level.Triangles;
            for (int i = 0; i < triangles.Count; ++i)
            {
                NavMeshTriangle navMeshTriangle = triangles[i];
                inSide = navMeshTriangle.InSidePoint(nextPos);
                if (inSide)
                {
                    _currentNavMesh = navMeshTriangle;
					_position = nextPos;
					BroadcastMove();
					return;
                }
            }

			int id = Level.Triangles.IndexOf(_currentNavMesh);
			_currentNavMesh.InSidePoint(nextPos, true);

			Console.WriteLine($"next pos null : {nextPos}");
            Console.WriteLine($"nav Id : {id}");

			var d_enum = _currentNavMesh.Siblings.GetEnumerator();
			while (d_enum.MoveNext())
			{
				int navIndex = d_enum.Current.Key;
				NavMeshTriangle sibling = d_enum.Current.Value;
				id = Level.Triangles.IndexOf(sibling);
				Console.WriteLine($"Sibling nav Id : {id}");
			}

			Console.WriteLine($"============================");

			_direction = Vector3.zero;
            PosInfo.PosX = _position.x;
            PosInfo.PosY = 0;
            PosInfo.PosZ = _position.z;

            PosInfo.DirX = _direction.x;
            PosInfo.DirY = _direction.y;
            PosInfo.DirZ = _direction.z;
			BroadcastMove();
		}

		void BroadcastMove()
		{
			if (_position == Util.ProtoPositionToVector3(PosInfo))
            {
				_direction = Vector3.zero;
			}

			// 다른 플레이어한테도 알려준다
			S_Move2 movePacket = new S_Move2();
			movePacket.ObjectId = Id;
			movePacket.PosInfo = Util.Vector3ToPosInfo(_position, _direction);
			Room?.Broadcast(movePacket);
		}
	}
}
