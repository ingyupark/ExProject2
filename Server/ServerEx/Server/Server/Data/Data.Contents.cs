using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Data
{
	#region Stat
	[Serializable]
	public class StatData : ILoader<int, StatInfo>
	{
		public List<StatInfo> stats = new List<StatInfo>();

		public Dictionary<int, StatInfo> MakeDict()
		{
			Dictionary<int, StatInfo> dict = new Dictionary<int, StatInfo>();
			foreach (StatInfo stat in stats)
			{
				stat.Hp = stat.MaxHp;
				stat.Sp = stat.Maxsp;
				dict.Add(stat.Level, stat);
			}	
			return dict;
		}
	}
	#endregion

	#region Skill
	[Serializable]
	public class Skill
	{
		public int id;
		public string name;
		public float cooldown;
		public int damage;
		public SkillType skillType;
		public ProjectileInfo projectile;
	}

	public class ProjectileInfo
	{
		public string name;
		public float speed;
		public int range;
		public string prefab;
	}

	[Serializable]
	public class SkillData : ILoader<int, Skill>
	{
		public List<Skill> skills = new List<Skill>();

		public Dictionary<int, Skill> MakeDict()
		{
			Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
			foreach (Skill skill in skills)
				dict.Add(skill.id, skill);
			return dict;
		}
	}
	#endregion

	#region Item
	[Serializable]
	public class ItemData
	{
		public int id;
		public string name;
		public ItemType itemType;
	}

	public class WeaponData : ItemData
	{
		public WeaponType weaponType;
		public int damage;
	}

	public class ArmorData : ItemData
	{
		public ArmorType armorType;
		public int defence;
	}

	public class ConsumableData : ItemData
	{
		public ConsumableType consumableType;
		public int maxCount;
		public int damage;
	}


	[Serializable]
	public class ItemLoader : ILoader<int, ItemData>
	{
		public List<WeaponData> weapons = new List<WeaponData>();
		public List<ArmorData> armors = new List<ArmorData>();
		public List<ConsumableData> consumables = new List<ConsumableData>();

		public Dictionary<int, ItemData> MakeDict()
		{
			Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();
			foreach (ItemData item in weapons)
			{
				item.itemType = ItemType.Weapon;
				dict.Add(item.id, item);
			}
			foreach (ItemData item in armors)
			{
				item.itemType = ItemType.Armor;
				dict.Add(item.id, item);
			}
			foreach (ItemData item in consumables)
			{
				item.itemType = ItemType.Consumable;
				dict.Add(item.id, item);
			}
			return dict;
		}
	}
	#endregion


	#region WeaponsStat
	[Serializable]
	public class WeaponsStatData
	{
        //id는 쓸일이 없을거 같음..
        public int id;
		public int level;
		public int damage;
		public int probability;
	}

	public class SwordData : WeaponsStatData
	{
	}

	public class BowData : WeaponsStatData
	{
	}
	public class GunData : WeaponsStatData
	{
	}
	public class SickleData : WeaponsStatData
	{
	}

	[Serializable]
	public class WeaponsStatLoader : ILoader<string, WeaponsStatData>
	{
		public List<SwordData> Sword = new List<SwordData>();
		public List<BowData> Bow = new List<BowData>();
		public List<GunData> Gun = new List<GunData>();
		public List<SickleData> Sickle = new List<SickleData>();


		public Dictionary<string, WeaponsStatData> MakeDict()
		{
			// 같은 키가 존재한다 에러남.. level로 묶어서 해서 그런거같음..  id 별 level로 관리 하지 않는 이상 같이 못넣을듯..
			Dictionary<string, WeaponsStatData> dict = new Dictionary<string, WeaponsStatData>();
			foreach (WeaponsStatData item in Sword)
			{
				dict.Add($"1{item.level}", item);
			}
			foreach (WeaponsStatData item in Bow)
			{
				dict.Add($"2{item.level}", item);
			}
			foreach (WeaponsStatData item in Gun)
			{
				dict.Add($"3{item.level}", item);
			}
			foreach (WeaponsStatData item in Sickle)
			{
				dict.Add($"4{item.level}", item);
			}
			return dict;
		}
	}
	#endregion

	#region Potal
	[Serializable]
	public class PotalData
	{
		public int id;
		public string name;
	}

	[Serializable]
	public class PotalLoader : ILoader<int, PotalData>
	{
		public List<PotalData> potals = new List<PotalData>();

		public Dictionary<int, PotalData> MakeDict()
		{
			Dictionary<int, PotalData> dict = new Dictionary<int, PotalData>();
			foreach (PotalData potal in potals)
				dict.Add(potal.id, potal);
			return dict;
		}
	}

	#endregion

	#region Npc
	[Serializable]
	public class ItemlistData
	{
		public int itemId;
		public int goldcount;
		public int count;
	}

	[Serializable]
	public class NpcData
	{
		public int id;
		public string name;
		public List<ItemlistData> itemlist;
		//public string prefabPath;
	}

	[Serializable]
	public class NpcLoader : ILoader<int, NpcData>
	{
		public List<NpcData> npcs = new List<NpcData>();

		public Dictionary<int, NpcData> MakeDict()
		{
			Dictionary<int, NpcData> dict = new Dictionary<int, NpcData>();
			foreach (NpcData npc in npcs)
			{
				dict.Add(npc.id, npc);
			}
			return dict;
		}
	}
	//NpcLoader
	#endregion
	#region Monster
	[Serializable]
	public class RewardData
	{
		public int probability; // 100분율
		public int itemId;
		public int count;
	}

	[Serializable]
	public class MonsterData
	{
		public int id;
		public string name;
		public int gold;
		public int totalExp;
		public StatInfo stat;
		public List<RewardData> rewards;
		//public string prefabPath;
    }

	[Serializable]
	public class MonsterLoader : ILoader<int, MonsterData>
	{
		public List<MonsterData> monsters = new List<MonsterData>();

		public Dictionary<int, MonsterData> MakeDict()
		{
			Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
			foreach (MonsterData monster in monsters)
			{
				dict.Add(monster.id, monster);
			}
			return dict;
		}
	}

	#endregion
}
