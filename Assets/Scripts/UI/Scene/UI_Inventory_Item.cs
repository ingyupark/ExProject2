using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory_Item : UI_Base
{
	[SerializeField]
	Image _icon = null;

	[SerializeField]
	Image _frame = null;

	[SerializeField]
	public Image _selecframe = null;

	[SerializeField]
	Text _counttext = null;

	Transform root;

	UI_Inventory ui;

	public int ItemDbId { get; private set; }
	public int TemplateId { get; private set; }
	public int Count { get; private set; }
	public int Slot { get; set; }
	public bool Equipped { get; private set; }

	public bool IsSelect;

	public override void Init()
	{
		ui = this.GetComponentInParent<UI_Inventory>();

		root = transform.root;

		_counttext.GetComponent<Text>();

		_icon.gameObject.BindEvent((e) =>
		{

			root.BroadcastMessage("Click_Down", gameObject.transform, SendMessageOptions.DontRequireReceiver);
		});

		_icon.gameObject.BindEvent((e) =>
		{

		}, Define.UIEvent.Click_Up);

		_icon.gameObject.BindEvent((e) =>
		{
			root.BroadcastMessage("BiginDrag", gameObject.transform, SendMessageOptions.DontRequireReceiver);
        }, Define.UIEvent.BinginDrag);

		_icon.gameObject.BindEvent((e) =>
		{
			root.BroadcastMessage("Drag", gameObject.transform, SendMessageOptions.DontRequireReceiver);
		}, Define.UIEvent.Drag);

		_icon.gameObject.BindEvent((e) =>
		{
			root.BroadcastMessage("EndDrag", gameObject.transform, SendMessageOptions.DontRequireReceiver);
		}, Define.UIEvent.EndDrag);

		_icon.gameObject.BindEvent((e) =>
		{
			root.BroadcastMessage("Drop", gameObject.transform, SendMessageOptions.DontRequireReceiver);

			//슬롯 변경 패킷 만들어서 추가...

		}, Define.UIEvent.Drop);
	}

    private void Update()
    {
		GetSlotInput();
	}

    public bool isDelay;
	public float delatTime = 1.0f;

	IEnumerator Drink()
	{
		yield return new WaitForSeconds(delatTime);
		isDelay = false;
	}
	// 키보드 입력 (물약 먹는거..)
	void GetSlotInput()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{

			if (!isDelay)
			{
				isDelay = true;
				DrunkAC();
				StartCoroutine(Drink());
			}
		}
	}

	public void DrunkAC()
    {
		if (!Equipped)
			return;

		Data.ItemData itemData = null;
		Managers.Data.ItemDict.TryGetValue(TemplateId, out itemData);

		if (itemData == null)
			return;

		// TODO : C_USE_ITEM 아이템 사용 패킷
		if (itemData.itemType == ItemType.Consumable)
		{
			C_Drunk_Item drunkPacket = new C_Drunk_Item();
			drunkPacket.ItemDbId = ItemDbId;
			Managers.Network.Send(drunkPacket);
			return;
		}
	}

    public void SetItem(Item item)
	{
		if (item == null)
		{
			ItemDbId = 0;
			TemplateId = 0;
			Count = 0;
			Equipped = false;
			_icon.gameObject.SetActive(false);
			_frame.gameObject.SetActive(false);
			_selecframe.gameObject.SetActive(false);
			_counttext.gameObject.SetActive(false);
			IsSelect = false;
		}
		else
		{
			ItemDbId = item.ItemDbId;
			TemplateId = item.TemplateId;
			Count = item.Count;
			Equipped = item.Equipped;
			Slot = item.Slot;

			Data.ItemData itemData = null;
			Managers.Data.ItemDict.TryGetValue(TemplateId, out itemData);
			Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
            _icon.sprite = icon;
			_counttext.text =Count.ToString("D2");
			//Debug.Log(_counttext.text);

			_icon.gameObject.SetActive(true);
            _frame.gameObject.SetActive(Equipped);
			_selecframe.gameObject.SetActive(IsSelect);

			if(TemplateId >= 200 && TemplateId <300)
				_counttext.gameObject.SetActive(true);
			else
				_counttext.gameObject.SetActive(false);
		}
	}

	public void RemoveItem(Item item = null)
	{
		ItemDbId = 0;
		TemplateId = 0;
		Count = 0;
		Equipped = false;

		_icon.sprite = null;
		_icon.gameObject.SetActive(false);
		_frame.gameObject.SetActive(Equipped);
		_selecframe.gameObject.SetActive(IsSelect);
		_counttext.gameObject.SetActive(false);
		item = null;
	}


}
