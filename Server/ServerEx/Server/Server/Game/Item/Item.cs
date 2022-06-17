using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public class Item
	{
		public ItemInfo Info { get; } = new ItemInfo();

		public int ItemDbId
		{
			get { return Info.ItemDbId; }
			set { Info.ItemDbId = value; }
		}

		public int TemplateId
		{
			get { return Info.TemplateId; }
			set { Info.TemplateId = value; }
		}

		public int Count
		{
			get { return Info.Count; }
			set { Info.Count = value; }
		}

		public int Slot
		{
			get { return Info.Slot; }
			set { Info.Slot = value; }
		}

		public bool Equipped
		{
			get { return Info.Equipped; }
			set { Info.Equipped = value; }
		}

		public bool IsSetCount
        {
			get { return Info.Issetcount; }
			set { Info.Issetcount = value; }
		}

		public int SkillType
		{
			get { return Info.SkillType; }
			set { Info.SkillType = value; }
		}

		public int Level
		{
			get { return Info.Level; }
			set { Info.Level = value; }
		}

		public ItemType ItemType { get; private set; }
		public bool Stackable { get; protected set; }

		public Item(ItemType itemType)
		{
			ItemType = itemType;
		}

		public static Item MakeItem(ItemDb itemDb)
		{
			Item item = null;

			ItemData itemData = null;
			DataManager.ItemDict.TryGetValue(itemDb.TemplateId, out itemData);

			if (itemData == null)
				return null;

			switch (itemData.itemType)
			{
				case ItemType.Weapon:
					item = new Weapon(itemDb.TemplateId);
					break;
				case ItemType.Armor:
					item = new Armor(itemDb.TemplateId);
					break;
				case ItemType.Consumable:
					item = new Consumable(itemDb.TemplateId);
					break;
			}

			if (item != null)
			{
				item.ItemDbId = itemDb.ItemDbId;
				item.Count = itemDb.Count;
				item.Slot = itemDb.Slot;
				item.Equipped = itemDb.Equipped;
				item.Level = itemDb.Level;

				if(item.ItemType == ItemType.Weapon && item.Level > 0)
                {
					WeaponsStatData _statData = null;
					DataManager.WeaponsStatDict.TryGetValue($"{item.TemplateId}{item.Level}", out _statData);
					((Weapon)item).SetDamage(_statData.damage);
				}
			}

			return item;
		}
	}

	public class Weapon : Item
	{
		public WeaponType WeaponType { get; private set; }
		public int Damage { get; private set; }

		//데미지 설정 안됨 레퍼런스가 없다고함.. ㅠㅠ
		public void SetDamage(int dam)
        {
			Damage = dam;
		}

		public Weapon(int templateId) : base(ItemType.Weapon)
		{
			Init(templateId);
		}

		void Init(int templateId)
		{
			ItemData itemData = null;
			DataManager.ItemDict.TryGetValue(templateId, out itemData);
			if (itemData.itemType != ItemType.Weapon)
				return;

			WeaponData data = (WeaponData)itemData;
			{
				TemplateId = data.id;
				Count = 1;
				WeaponType = data.weaponType;
				Damage = data.damage;
				Stackable = false;
			}

			
		}
	}

	public class Armor : Item
	{
		public ArmorType ArmorType { get; private set; }
		public int Defence { get; private set; }
		public void SetDef(int Def)
        {
			Defence = Def;
        }
		public Armor(int templateId) : base(ItemType.Armor)
		{
			Init(templateId);
		}

		void Init(int templateId)
		{
			ItemData itemData = null;
			DataManager.ItemDict.TryGetValue(templateId, out itemData);
			if (itemData.itemType != ItemType.Armor)
				return;

			ArmorData data = (ArmorData)itemData;
			{
				TemplateId = data.id;
				Count = 1;
				ArmorType = data.armorType;
				Defence = data.defence;
				Stackable = false;
			}
		}
	}

	public class Consumable : Item
	{
		public ConsumableType ConsumableType { get; private set; }
		public int MaxCount { get; set; }
		public int Damage { get; private set; }

		public Consumable(int templateId) : base(ItemType.Consumable)
		{
			Init(templateId);
		}

		void Init(int templateId)
		{
			ItemData itemData = null;
			DataManager.ItemDict.TryGetValue(templateId, out itemData);
			if (itemData.itemType != ItemType.Consumable)
				return;

			ConsumableData data = (ConsumableData)itemData;
			{
				TemplateId = data.id;
				Count = 1;
				MaxCount = data.maxCount;
				Damage = data.damage;
				ConsumableType = data.consumableType;
				Stackable = (data.maxCount > 1);
			}
		}
	}
}
