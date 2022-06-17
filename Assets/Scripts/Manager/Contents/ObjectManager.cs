using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
	public MyPlayerController MyPlayer { get; set; }
	Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
	
	public static GameObjectType GetObjectTypeById(int id)
	{
		int type = (id >> 24) & 0x7F;
		return (GameObjectType)type;
	}

	public void Add(ObjectInfo info, bool myPlayer = false)
	{
		if (MyPlayer != null && MyPlayer.Id == info.ObjectId)
			return;
		if (_objects.ContainsKey(info.ObjectId))
			return;

		GameObjectType objectType = GetObjectTypeById(info.ObjectId);
		if (objectType == GameObjectType.Player)
		{
			if (myPlayer)
			{
				GameObject go = Managers.Resource.Instantiate("Creature/MyPlayer");
				go.name = info.Name;
				_objects.Add(info.ObjectId, go);
				MyPlayer = go.GetComponent<MyPlayerController>();
			
				MyPlayer.Id = info.ObjectId;
				MyPlayer.NameText.text = go.name;
				MyPlayer.PosInfo = info.PosInfo;
				MyPlayer.P_Skill = info.StatInfo.SkillType;
				MyPlayer.Stat.MergeFrom(info.StatInfo);
				MyPlayer.SyncPos();
			}
			else
			{
				GameObject go = Managers.Resource.Instantiate("Creature/Player");
				go.name = info.Name;
				_objects.Add(info.ObjectId, go);

				PlayerController pc = go.GetComponent<PlayerController>();
				pc.Id = info.ObjectId;
				pc.NameText.text = info.Name;
				pc.PosInfo = info.PosInfo;
				pc.Stat.MergeFrom(info.StatInfo);
				pc.SyncPos();
			}
		}
		else if (objectType == GameObjectType.Monster)
		{
			GameObject go = MonsterRespon(info);

			if (go == null)
				return;

			go.name = "Monster" + Convert.ToString(info.ObjectId);
			_objects.Add(info.ObjectId, go);

			MonsterController mc = go.GetComponent<MonsterController>();
			mc.NameText.text = info.Name;
			mc.Id = info.ObjectId;
			mc.PosInfo = info.PosInfo;
			mc.Stat = info.StatInfo;
			mc.SyncPos();
		}
        //화살 적용

        else if (objectType == GameObjectType.Projectile)
        {
			if(info.Name == "총 쏘기")
            {
				GameObject go = Managers.Resource.Instantiate("Creature/Pistol");
				go.name = "Pistol";
				_objects.Add(info.ObjectId, go);

				PistolController ac = go.GetComponent<PistolController>();
				ac.PosInfo = info.PosInfo;
				ac.Stat = info.StatInfo;
				ac.SyncPos();
			}
			else if (info.Name == "화살 쏘기")
            {
				GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
				go.name = "Arrow";
				_objects.Add(info.ObjectId, go);

				ArrowController ac = go.GetComponent<ArrowController>();
				ac.PosInfo = info.PosInfo;
				ac.Stat = info.StatInfo;
				ac.SyncPos();
			}
			
        }
		else if (objectType == GameObjectType.Npc)
		{
			if(info.Name == "요리사")
            {
				GameObject go = Managers.Resource.Instantiate("Creature/Npc01");
				go.name = "Npc" + Convert.ToString(info.ObjectId);
				_objects.Add(info.ObjectId, go);

				NpcController ac = go.GetComponent<NpcController>();
				ac.NameText.text = info.Name;
				ac.PosInfo = info.PosInfo;

				ac.SyncPos();
			}
			else if (info.Name == "대장장이")
            {
				GameObject go = Managers.Resource.Instantiate("Creature/Npc02");
				go.name = "Npc" + Convert.ToString(info.ObjectId);
				_objects.Add(info.ObjectId, go);

				NpcController ac = go.GetComponent<NpcController>();
				ac.NameText.text = info.Name;
				ac.PosInfo = info.PosInfo;

				ac.SyncPos();
			}
		}
		else if (objectType == GameObjectType.Potal)
		{
			GameObject go = Managers.Resource.Instantiate("Map/Tilemap_Portal");
			go.name = "Potal" + Convert.ToString(info.ObjectId);
			_objects.Add(info.ObjectId, go);

			PotalController ac = go.GetComponent<PotalController>();
			ac.PosInfo = info.PosInfo;

			ac.SyncPos();
		}
	}

	public GameObject MonsterRespon(ObjectInfo info)
    {
		GameObject go = null;
		switch (info.Name)
        {
			case "도플갱어":
				return go = Managers.Resource.Instantiate("Creature/Monster/Monster1");
			case "좀비":
				return go = Managers.Resource.Instantiate("Creature/Monster/Monster2");
			case "나무괴물":
				return go = Managers.Resource.Instantiate("Creature/Monster/Monster3");
			case "오크괴물":
				return go = Managers.Resource.Instantiate("Creature/Monster/Monster4");
			default:
				return go;
		}
    }


	public void Remove(int id)
	{
		if (MyPlayer != null && MyPlayer.Id == id)
			return;
		if (_objects.ContainsKey(id) == false)
			return;

		GameObject go = FindById(id);
		if (go == null)
			return;

		_objects.Remove(id);
		Managers.Resource.Destroy(go);
	}

	public GameObject FindById(int id)
	{
		GameObject go = null;
		_objects.TryGetValue(id, out go);
		return go;
	}

	public GameObject FindCreature(Vector3Int cellPos)
	{
		foreach (GameObject obj in _objects.Values)
		{
			CreatureController cc = obj.GetComponent<CreatureController>();
			if (cc == null)
				continue;

			if (cc.CellPos == cellPos)
				return obj;
		}

		return null;
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
