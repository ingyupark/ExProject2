using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Data;
using Google.Protobuf.Protocol;

public class UI_BuyPopup : UI_Popup
{
    Button BackBT;
    Button SelleckBT;

    public List<UI_Popup_Item> Items { get; } = new List<UI_Popup_Item>();
    List<UI_Popup_Item> SelectitemCom { get; } = new List<UI_Popup_Item>();

    List<Item> _items = new List<Item>();

    Dictionary<int, ItemlistData> itemlistData = new Dictionary<int, ItemlistData>();
    public int Npcid { get; set; }

    public override void Init()
    {
        base.Init();

        BackBT = this.transform.Find("BackBT").GetComponent<Button>();
        SelleckBT = this.transform.Find("SelleckBT").GetComponent<Button>();

        GameObject grid = transform.Find("Scroll View").Find("Viewport").Find("Content").Find("ItemGrid").gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        //

        NpcData npcData = null;
        if(Npcid == 1)
        {
            Managers.Data.NpcDict.TryGetValue(1, out npcData);

            for (int i = 0; i < npcData.itemlist.Count; i++)
            {

                _items.Add(Item.MakeNPCItem(npcData.itemlist[i]));
            }
        }
        else if(Npcid == 2)
        {
            Managers.Data.NpcDict.TryGetValue(2, out npcData);

            for (int i = 0; i < npcData.itemlist.Count; i++)
            {

                _items.Add(Item.MakeNPCItem(npcData.itemlist[i]));
            }
        }

        for (int i = 0; i < _items.Count; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_Popup_Item", grid.transform);
            go.name = i.ToString();
            UI_Popup_Item item = go.GetOrAddComponent<UI_Popup_Item>();
            Transform ts = go.transform;
            Items.Add(item);
        }

        if (BackBT != null)
            BackBT.onClick.AddListener(BackClick);

        if (SelleckBT != null)
            SelleckBT.onClick.AddListener(SelleckClick);

        RefreshUI();
    }

    public void OpenPop()
    {
        GameObject grid = transform.Find("Scroll View").Find("Viewport").Find("Content").Find("ItemGrid").gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        NpcData npcData = null;
        if (Npcid == 1)
        {
            Managers.Data.NpcDict.TryGetValue(1, out npcData);

            for (int i = 0; i < npcData.itemlist.Count; i++)
            {
                _items.Add(Item.MakeNPCItem(npcData.itemlist[i]));
                itemlistData.Add(npcData.itemlist[i].itemId, npcData.itemlist[i]);
            }
        }
        else if (Npcid == 2)
        {
            Managers.Data.NpcDict.TryGetValue(2, out npcData);

            for (int i = 0; i < npcData.itemlist.Count; i++)
            {

                _items.Add(Item.MakeNPCItem(npcData.itemlist[i]));
                itemlistData.Add(npcData.itemlist[i].itemId, npcData.itemlist[i]);
                
            }
        }

        for (int i = 0; i < _items.Count; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_Popup_Item", grid.transform);
            go.name = i.ToString();
            UI_Popup_Item item = go.GetOrAddComponent<UI_Popup_Item>();
            Transform ts = go.transform;
            Items.Add(item);
        }

        if (BackBT != null)
            BackBT.onClick.AddListener(BackClick);

        if (SelleckBT != null)
            SelleckBT.onClick.AddListener(SelleckClick);

        RefreshUI();
    }
    public void RefreshUI( )
    {

        for(int i = 0; i < _items.Count; i++)
        {
            Items[i].SetItem(_items[i]);
            ItemlistData date = null;
            itemlistData.TryGetValue(Items[i].ItemDbId, out date);
            Items[i]._goldtext.text = date.goldcount.ToString();
        }
    }
    

    void UpClick_Down(Transform transform)
    {
        UI_Popup_Item itemCom = transform.GetComponent<UI_Popup_Item>();
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

    private void BackClick()
    {
        Managers.Object.MyPlayer.isBuy = false;
        ClosePopupUI();
    }

    private void SelleckClick()
    {
        foreach (UI_Popup_Item item in SelectitemCom)
        {
            if (item.IsSelect)
            {

                item.IsSelect = false;
                item._selecframe.gameObject.SetActive(item.IsSelect);

                // 버리기할때 그냥 Count조절이엇지 참..!! 삭제 할 필요 없음!!

                C_Buyitem BuyPacket = new C_Buyitem();
                BuyPacket.ItemDbId = item.TemplateId;
                Managers.Network.Send(BuyPacket);
            }
        }
    }


    public override void ClosePopupUI()
    {
        base.ClosePopupUI();

    }

    public override void CloseAllPopupUI()
    {
        base.CloseAllPopupUI();
    }

    public UI_BuyPopup PopUP()
    {
        //이건 NPC한테 만들어줄것.
        return Managers.UI.ShowPopupUI<UI_BuyPopup>("UI_BuyPopup");
    }

}
