using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_Item : UI_Popup
{
	[SerializeField]
	Image _icon = null;

	[SerializeField]
	public Image _selecframe = null;


	[SerializeField]
	public Text _goldtext = null;

	Transform root;

	public int ItemDbId { get; private set; }
	public int TemplateId { get; private set; }
	public int Count { get; private set; }

	public bool IsSelect;

	public override void Init()
	{

		root = transform.root;

		_icon.gameObject.BindEvent((e) =>
		{

			root.BroadcastMessage("UpClick_Down", gameObject.transform, SendMessageOptions.DontRequireReceiver);
        });

	}

    private void Update()
    {
	}
	
    public void SetItem(Item item)
	{
		if (item == null)
		{
			ItemDbId = 0;
			TemplateId = 0;
			Count = 0;
			_icon.gameObject.SetActive(false);
			_selecframe.gameObject.SetActive(false);
			_goldtext.gameObject.SetActive(false);
			IsSelect = false;
		}
		else
		{
			ItemDbId = item.ItemDbId;
			TemplateId = item.TemplateId;
			Count = item.Count;

			Data.ItemData itemData = null;
			Managers.Data.ItemDict.TryGetValue(TemplateId, out itemData);
			Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
            _icon.sprite = icon;
			_goldtext.gameObject.SetActive(true);
			_icon.gameObject.SetActive(true);
			_selecframe.gameObject.SetActive(IsSelect);

		}
	}

	public void RemoveItem(Item item = null)
	{
		ItemDbId = 0;
		TemplateId = 0;
		Count = 0;

		_icon.sprite = null;
		_icon.gameObject.SetActive(false);
		_selecframe.gameObject.SetActive(IsSelect);
		item = null;
	}


}
