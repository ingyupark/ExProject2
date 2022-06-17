using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Main : UI_Base
{
    [SerializeField]
    Text LevelText = null;

    [SerializeField]
    Text UserNameText = null;

    [SerializeField]
    Text GoldText = null;

    [SerializeField]
    Text BagText = null;

    [SerializeField]
    ExpBar _ExpBar;

    [SerializeField]
    public GameObject ChatInfo;

    public UI_Chat chat { get; private set; }

    public void UpdateExpBar()
    {
        MyPlayerController player = Managers.Object.MyPlayer;

        if (_ExpBar == null)
            return;

        float ratio = 0.0f;
        if (player.Stat.TotalExp >=0)
            ratio = ((float)player.Stat.Exp) / player.Stat.TotalExp;
        
        //Total Exp 말고 현재 Exp 추가 해야함..

        _ExpBar.SetExpBar(ratio);
    }
    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        chat = GetComponentInChildren<UI_Chat>();
        RefreshUI();
    }



    public void RefreshUI()
    {
        MyPlayerController player = Managers.Object.MyPlayer;


        if (player != null)
        {
            UserNameText.text = $"{player.name}";
            UpdateExpBar();
            LevelText.text = $"Level : {player.Stat.Level}";

            GoldText.text = $"{player.Stat.Gold}";
        }

        int inventorycount = Managers.Inven.Items.Count;
        BagText.text = $"{inventorycount}/20";

    }
}
