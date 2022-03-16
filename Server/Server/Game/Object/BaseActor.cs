using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Server.Game
{
    public abstract class BaseActor : GameObject
    {
		public ObjModel Level { get; private set; }

		private NavMeshTriangle _currentNavMesh;

		protected delegate void StateHandle();
		protected delegate void CommandHandle();
		protected delegate void PostProcessHandle();

		protected StateHandle _stateHandle = null;
		protected CommandHandle _commandHandle = null;
		protected List<PostProcessHandle> _postProcessHandles = new List<PostProcessHandle>();

		protected int _stateEndFrame = 0;

		public virtual void Init(ObjModel level)
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

			_stateHandle = null;
			_postProcessHandles.Clear();
			_commandHandle = UpdateCommandIdleMove;
		}

		public virtual void OnSkill()
        {

        }

		public override void Update()
		{
			ProcessCommand();
			ProcessState();
			PostPocess();

			base.Update();
		}

		public virtual void ProcessCommand()
		{
			if (_commandHandle == null)
				return;

			_commandHandle();
		}

		public virtual void ProcessState() 
		{
			if (_stateHandle == null)
				return;

			_stateHandle();
		}

		public virtual void PostPocess() 
		{
			if (_postProcessHandles.Count <= 0)
				return;

			foreach (var handle in _postProcessHandles)
            {
				handle();
			}

			_postProcessHandles.Clear();
		}

		protected virtual void UpdateCommandIdleMove()
		{
			if (_position == Util.ProtoPositionToVector3(PosInfo))
				return;

			if (_currentNavMesh == null)
				return;

			var targetPos = Util.ProtoPositionToVector3(PosInfo);
			_direction = targetPos - _position;
			_direction.Normalize();

			var nextPos = Vector3.MoveTowards(_position, targetPos, _timeStamp * Stat.Speed);
			bool inSide = _currentNavMesh.InSidePoint(nextPos);
			if (inSide)
			{
				_position = nextPos;
				_postProcessHandles.Add(BroadcastMove);
				return;
			}

			var nextNavMesh = _currentNavMesh.CalcInSideSiblingNavMesh(nextPos);
			if (nextNavMesh != null)
			{
				_currentNavMesh = nextNavMesh;
				_position = nextPos;
				_postProcessHandles.Add(BroadcastMove);
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
					_postProcessHandles.Add(BroadcastMove);
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
			_postProcessHandles.Add(BroadcastMove);
		}

		protected virtual void ProcessSkill()
        {
			if (--_stateEndFrame > 0)
				return;

			_stateHandle = null;
			_commandHandle = UpdateCommandIdleMove;
		}

		protected void BroadcastMove()
		{
			if (_position == Util.ProtoPositionToVector3(PosInfo))
			{
				_direction = Vector3.zero;
			}

			if (_direction == Vector3.zero)
			{
				PosInfo.State = ActorState.Idle;
			}
			else
			{
				PosInfo.State = ActorState.Moving;
			}

			// 다른 플레이어한테도 알려준다
			S_Move movePacket = new S_Move();
			movePacket.ObjectId = Id;
			movePacket.PosInfo = Util.Vector3ToPosInfo(_position, _direction);
			Room?.Broadcast(movePacket);
		}
	}
}
