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

		public bool IsAlive
        {
            get
            {
				return Info?.StatInfo?.Hp > 0;
            }
        }

        public virtual void Init(ObjModel level)
		{
			if (level == null)
				return;

			Level = level;

			Vector3 myPosition = PosInfo.Position.ToVector3();

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
			if (_position == PosInfo.Position.ToVector3())
				return;

			if (_currentNavMesh == null)
				return;

			var targetPos = PosInfo.Position.ToVector3();
			_direction = targetPos - _position;
            _direction.Normalize();

            if (_direction != Vector3.zero)
                PosInfo.LookDirection = _direction.ToFloat3();

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

			_direction = Vector3.zero;
			PosInfo.Position = _position.ToFloat3();
			PosInfo.Direction = _direction.ToFloat3();

			_postProcessHandles.Add(BroadcastMove);
		}

		protected virtual void ProcessSkill()
        {
			if (--_stateEndFrame > 0)
				return;

			_stateHandle = null;
			_commandHandle = UpdateCommandIdleMove;
		}

		protected virtual void BroadcastMove()
		{
			if (_position == PosInfo.Position.ToVector3())
			{
				_direction = Vector3.zero;
			}

			// 다른 플레이어한테도 알려준다
			S_Move movePacket = new S_Move();
			movePacket.ObjectId = Id;

			movePacket.PosInfo = new PositionInfo();
			movePacket.PosInfo.Position = _position.ToFloat3();
			movePacket.PosInfo.Direction = _direction.ToFloat3();
			movePacket.PosInfo.LookDirection = PosInfo.LookDirection;
			movePacket.PosInfo.State = PosInfo.State;

			Room?.Broadcast(movePacket);
		}

		protected virtual void LeaveGame()
        {
			GameRoom room = Room;
			room.LeaveGame(Id);
		}

		protected virtual void RespawnGame(GameRoom room) { }
	}
}
