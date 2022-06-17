using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpText : MonoBehaviour
{
    [SerializeField]
    Text Hp = null;

  
    public void Init()
    {
    }

    public void MissText()
    {
        Hp.color = Color.gray;
        Hp.text = "Miss";
    }

    public void SetHpText(int newText = 0,int oldText =0)
    {

        if (oldText - newText < 0)
            Hp.color = Color.green;
        else
            Hp.color = Color.red;

        Hp.text = $"{newText - oldText}";

    }
}
