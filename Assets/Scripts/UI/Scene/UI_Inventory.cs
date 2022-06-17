using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public partial class UI_Inventory : UI_Base
{
	public List<UI_Inventory_Item> Items { get; } = new List<UI_Inventory_Item>();

	List<UI_Inventory_Item> SelectitemCom { get; } = new List<UI_Inventory_Item>();
	GameObject SwapItem;	
	Button RemoveBT;
	Button EquippedBT;
	Button ItemUpBT;

	int initcount { get; set; } = 0;
	public override void Init()
	{
		if (initcount == 1)
			return;

		RemoveBT = this.transform.Find("RemoveBT").GetComponent<Button>();
		EquippedBT = this.transform.Find("EquippedBT").GetComponent<Button>();
		ItemUpBT = this.transform.Find("ItemUpBT").GetComponent<Button>();

		if (RemoveBT != null)
			RemoveBT.onClick.AddListener(RemoveClick);

		if(EquippedBT != null)
			EquippedBT.onClick.AddListener(EquippedClick);
		if (ItemUpBT != null)
			ItemUpBT.onClick.AddListener(ItemUpClick);

		Items.Clear();

		GameObject grid = transform.Find("ItemGrid").gameObject;
		GameObject S_grid = transform.Find("SwapGrid").gameObject;
		SwapItem = Managers.Resource.Instantiate("UI/Scene/SwapFrame", S_grid.transform);
		SwapItem.name = "00";
		SwapItem.SetActive(false);

		foreach (Transform child in grid.transform)
			Destroy(child.gameObject);

		for (int i = 0; i < 20; i++)
		{
			GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_Inventory_Item", grid.transform);
			go.name = i.ToString();
			UI_Inventory_Item item = go.GetOrAddComponent<UI_Inventory_Item>();
			Transform ts = go.transform;
			Items.Add(item);
		}
		
		Stat_Init();
	}

    private void EquippedClick()
    {

		foreach (UI_Inventory_Item item in SelectitemCom)
		{
			if (item.IsSelect)
			{

				item.IsSelect = false;
				item._selecframe.gameObject.SetActive(item.IsSelect);

				MyPlayerController Myp = GameObject.FindWithTag("Player").gameObject.GetComponent<MyPlayerController>();
				//아이템 템플릿2면 활 1이면 칼 나중에는 다시 나눠줘야함.. 수정 필요.
				if (item.TemplateId == 1 && !item.Equipped)
				{
					Myp.P_Skill = (int)SkillType.SkillAuto;
				}
				else if (item.TemplateId == 1 && item.Equipped)
				{
					Myp.P_Skill = (int)SkillType.SkillAuto;
				}
				else if (item.TemplateId == 2 && !item.Equipped)
				{
					Myp.P_Skill = (int)SkillType.SkillProjectile;
				}
				else if (item.TemplateId == 2 && item.Equipped)
				{
					Myp.P_Skill = (int)SkillType.SkillProjectile;
				}
				else if (item.TemplateId == 3 && !item.Equipped)
				{
					Myp.P_Skill = (int)SkillType.SkillGun;
				}
				else if (item.TemplateId == 3 && item.Equipped)
				{
					Myp.P_Skill = (int)SkillType.SkillGun;
				}
				else if (item.TemplateId == 4 && !item.Equipped)
				{
					Myp.P_Skill = (int)SkillType.SkillSickle;
				}
				else if (item.TemplateId == 4 && item.Equipped)
				{
					Myp.P_Skill = (int)SkillType.SkillAuto;
				}
				else
				{
				}

				C_EquipItem equipPacket = new C_EquipItem();
				equipPacket.ItemDbId = item.ItemDbId;
				equipPacket.Equipped = !item.Equipped;
				if(item.TemplateId > 0 && item.TemplateId < 100)
					equipPacket.SkillType = Myp.P_Skill;
				Managers.Network.Send(equipPacket);

			}
		}

		RefreshUI();
	}

	private void ItemUpClick()
    {
		foreach (UI_Inventory_Item item in SelectitemCom)
		{
			if (item.IsSelect)
			{
				item.IsSelect = false;
				item._selecframe.gameObject.SetActive(item.IsSelect);
				C_Levelup_Item c_itemup = new C_Levelup_Item();
				c_itemup.ItemDbId = item.ItemDbId;
                Managers.Network.Send(c_itemup);

			}
		}


		RefreshUI();

	}



	private void RemoveClick()
    {
		//삭제를 하려면.. 선택을 해야한다.. 장비 착용 프레임말고 선택 프레임 하나를 더 만들자..
		foreach(UI_Inventory_Item item in SelectitemCom)
        {


			if (item.IsSelect)
            {

				if (item.Equipped)
					return;

				item.IsSelect = false;
				item._selecframe.gameObject.SetActive(item.IsSelect);

				// 버리기할때 그냥 Count조절이엇지 참..!! 삭제 할 필요 없음!!

				C_Change_Icount countPacket = new C_Change_Icount();
				countPacket.ItemDbId = item.ItemDbId;
				countPacket.Count = 1;
				Managers.Network.Send(countPacket);
			}
        }


		RefreshUI();
	}

    public void RefreshUI()
	{
		if (Items.Count <= 0)
		{ }

		List<Item> items = Managers.Inven.Items.Values.ToList();

		items.Sort((left, right) => { return left.Slot - right.Slot; });

		foreach (Item item in items)
		{
			if (item.Slot < 0 || item.Slot > 20)
				continue;

			if (item.Count <= 0)
			{
				//아이템이 				Items[item.Slot].RemoveItem(item);
			}
			else
            {
				//아이템이 있으면..
				Items[item.Slot].SetItem(item);
			}
		}
	}

	void Click_Down(Transform transform)
    {
		UI_Inventory_Item itemCom = transform.GetComponent<UI_Inventory_Item>();
		if (itemCom.IsSelect)
		{
			itemCom.IsSelect = false;
			SelectitemCom.Remove(itemCom);
        }
		else
		{
			itemCom.IsSelect = true;
			SelectitemCom.Add(itemCom);
		}

		itemCom._selecframe.gameObject.SetActive(itemCom.IsSelect);

	}

	//SwapItem이 선택한 Item보다 아래에 있어야 Drop에서 선택된다.
	// 하.. 근데 안된다.. 마지막 Drag일때 하이라이키에서 맨 밑에 있어야 함..

	GameObject BiginOJ;
	GameObject DropOJ;

	void BiginDrag(Transform transform)
	{
		BiginOJ = transform.gameObject;
		SwapItem.SetActive(true);
		SwapItem.transform.parent.SetAsLastSibling();
	}

	void Drag(Transform transform)
	{
		SwapItem.transform.position = Input.mousePosition + new Vector3(0,16f);
	}

	void EndDrag(Transform transform)
	{
		SwapItem.transform.parent.SetAsFirstSibling();
		SwapItem.SetActive(false);
	}

	void Drop(Transform transform)
	{
		SwapItem.transform.parent.SetAsFirstSibling();
		SwapItem.SetActive(false);
		DropOJ = transform.gameObject;

		int Bigin_index = BiginOJ.transform.GetSiblingIndex();
		int Drop_index = DropOJ.transform.GetSiblingIndex();

		int Bigin_Slot = Items[Bigin_index].Slot;
		int Drop_Slot = Items[Drop_index].Slot;

		{ 
			C_Slot_Count Bigin_SlotPacket = new C_Slot_Count();
			Bigin_SlotPacket.ItemDbId = Items[Bigin_index].ItemDbId;
			Bigin_SlotPacket.Slot = Drop_Slot;
			Managers.Network.Send(Bigin_SlotPacket);
		}

		{ 
			C_Slot_Count Drop_SlotPacket = new C_Slot_Count();
			Drop_SlotPacket.ItemDbId = Items[Drop_index].ItemDbId;
			Drop_SlotPacket.Slot = Bigin_Slot;
			Managers.Network.Send(Drop_SlotPacket);
		}
		

		RefreshUI();
	}

	


}
