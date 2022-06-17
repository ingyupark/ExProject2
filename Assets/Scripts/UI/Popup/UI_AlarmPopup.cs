using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AlarmPopup : UI_Popup
{
    [SerializeField]
    Text AlarmText = null;

    IEnumerator CoInputCooltime(float time)
    {
        yield return new WaitForSeconds(time);
        ClosePopupUI();
        Managers.Object.MyPlayer.isAlarm = false;
    }

    public override void Init()
    {
        base.Init();
    }
        public void Alarm_Pop(string text)
    {

        AlarmText.text = text;
        StartCoroutine(CoInputCooltime(3.0f));

    }
}
