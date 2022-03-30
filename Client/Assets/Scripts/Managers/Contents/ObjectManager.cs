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
		BaseActor baseActor = null;

		GameObjectType objectType = GetObjectTypeById(info.ObjectId);
        switch (objectType)
        {
			case GameObjectType.Player:
                {
					if (myPlayer)
					{
						GameObject go = Managers.Resource.Instantiate("Actor/MyPlayer");
						go.name = info.Name;
						_objects.Add(info.ObjectId, go);

						MyPlayer = go.GetComponent<MyPlayer>();
						baseActor = MyPlayer;
					}
					else
					{
						GameObject go = Managers.Resource.Instantiate("Actor/Player");
						go.name = info.Name;
						_objects.Add(info.ObjectId, go);

						baseActor = go.GetComponent<Player>();
					}
				}
				break;

			case GameObjectType.Monster:
				{
					GameObject go = Managers.Resource.Instantiate("Actor/Monster");
					baseActor = go.GetComponent<SkillObject>();
				}
				break;

			case GameObjectType.Skill:
				{
					GameObject go = Managers.Resource.Instantiate("Skill/Projectile");
					go.name = info.Name;
					_objects.Add(info.ObjectId, go);

					baseActor = go.GetComponent<SkillObject>();
				}
				break;
		}

		if (baseActor == null)
			return;

		baseActor.gameObject.name = info.Name;
		baseActor.Init(info.ObjectId);
		baseActor.PosInfo = info.PosInfo;
		baseActor.Stat = info.StatInfo;
		baseActor.SyncPos();

		_objects.Add(info.ObjectId, baseActor.gameObject);
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
