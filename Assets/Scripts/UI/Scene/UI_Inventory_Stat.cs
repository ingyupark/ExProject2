using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public partial class UI_Inventory : UI_Base
{
	enum Images
	{
		Slot_Helmet,
		Slot_Armor,
		Slot_Boots,
		Slot_Weapon,
		Slot_Shield,
		Slot_Potion
	}

	bool _init = false;

	public void Stat_Init()
    {
		Bind<Image>(typeof(Images));

		_init = true;

		initcount = 1;

		Stat_RefreshUI();
	}


	

	public void Stat_RefreshUI()
	{
		if (_init == false)
			return;

		// 우선은 다 가린다
		Get<Image>((int)Images.Slot_Helmet).enabled = false;
		Get<Image>((int)Images.Slot_Armor).enabled = false;
		Get<Image>((int)Images.Slot_Boots).enabled = false;
		Get<Image>((int)Images.Slot_Weapon).enabled = false;
		Get<Image>((int)Images.Slot_Shield).enabled = false;
		Get<Image>((int)Images.Slot_Potion).enabled = false;

		// 채워준다
		foreach (Item item in Managers.Inven.Items.Values)
		{
			if (item.Equipped == false)
				continue;

			ItemData itemData = null;
			Managers.Data.ItemDict.TryGetValue(item.TemplateId, out itemData);
			Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);

			if (item.ItemType == ItemType.Weapon)
			{
				Get<Image>((int)Images.Slot_Weapon).enabled = true;
				Get<Image>((int)Images.Slot_Weapon).sprite = icon;
			}
			else if (item.ItemType == ItemType.Armor)
			{
				Armor armor = (Armor)item;
				switch (armor.ArmorType)
				{
					case ArmorType.Helmet:
						Get<Image>((int)Images.Slot_Helmet).enabled = true;
						Get<Image>((int)Images.Slot_Helmet).sprite = icon;
						break;
					case ArmorType.Armor:
						Get<Image>((int)Images.Slot_Armor).enabled = true;
						Get<Image>((int)Images.Slot_Armor).sprite = icon;
						break;
					case ArmorType.Boots:
						Get<Image>((int)Images.Slot_Boots).enabled = true;
						Get<Image>((int)Images.Slot_Boots).sprite = icon;
						break;
				}
			}
			else if (item.ItemType == ItemType.Consumable)
            {
				Get<Image>((int)Images.Slot_Potion).enabled = true;
				Get<Image>((int)Images.Slot_Potion).sprite = icon;
			}
		}

	}

}
