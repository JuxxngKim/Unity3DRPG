using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YeongJ.Inagme;

public class ObjectManager
{
	public MyPlayer MyPlayer { get; set; }
	Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
	
	public static GameObjectType GetObjectTypeById(int id)
	{
		int type = (id >> 24) & 0x7F;
		return (GameObjectType)type;
	}

	public void Add(ObjectInfo info, bool myPlayer = false)
	{
		GameObjectType objectType = GetObjectTypeById(info.ObjectId);
		if (objectType == GameObjectType.Player)
		{
            if (myPlayer)
            {
                GameObject go = Managers.Resource.Instantiate("Actor/MyPlayer");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                MyPlayer = go.GetComponent<MyPlayer>();
				MyPlayer.Init(info.ObjectId);
				MyPlayer.PosInfo = info.PosInfo;
                MyPlayer.Stat = info.StatInfo;
                MyPlayer.SyncPos();
            }
			else
            {
                GameObject go = Managers.Resource.Instantiate("Actor/Player");
                go.name = info.Name;
                _objects.Add(info.ObjectId, go);

                var player = go.GetComponent<Player>();
				player.Init(info.ObjectId);
				player.PosInfo = info.PosInfo;
				player.Stat = info.StatInfo;
				player.SyncPos();
			}
		}
		else if(objectType == GameObjectType.Skill)
		{
			GameObject go = Managers.Resource.Instantiate("Skill/Projectile");
			go.name = info.Name;
			_objects.Add(info.ObjectId, go);

			SkillObject skillObject = go.GetComponent<SkillObject>();
			skillObject.Init(info.ObjectId);
			skillObject.PosInfo = info.PosInfo;
			skillObject.Stat = info.StatInfo;
			skillObject.SyncPos();
		}
	}

	public void Remove(int id)
	{
		GameObject go = FindById(id);
		if (go == null)
			return;

		var baseActor = go.GetComponent<BaseActor>();
		baseActor?.Remove();

		_objects.Remove(id);
		Managers.Resource.Destroy(go);
	}

	public GameObject FindById(int id)
	{
		GameObject go = null;
		_objects.TryGetValue(id, out go);
		return go;
	}

	public GameObject Find(Func<GameObject, bool> condition)
	{
		foreach (GameObject obj in _objects.Values)
		{
			if (condition.Invoke(obj))
				return obj;
		}

		return null;
	}

	public void Clear()
	{
		foreach (GameObject obj in _objects.Values)
			Managers.Resource.Destroy(obj);
		_objects.Clear();
		MyPlayer = null;
	}
}
