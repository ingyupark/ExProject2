using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameScene : UI_Scene
{
    public UI_Stat StatUI { get; private set; }
    public UI_Inventory InvenUI { get; private set; }
    public UI_Main mainUI { get; private set; }
    public UI_Chat chatUI { get; private set; }

    public override void Init()
	{
        base.Init();

        StatUI = GetComponentInChildren<UI_Stat>();
        InvenUI = GetComponentInChildren<UI_Inventory>();
        mainUI = GetComponentInChildren<UI_Main>();
        chatUI = mainUI.GetComponentInChildren<UI_Chat>();

        StatUI.gameObject.SetActive(false);
        InvenUI.gameObject.SetActive(false);
        chatUI.gameObject.SetActive(false);
    }
}
