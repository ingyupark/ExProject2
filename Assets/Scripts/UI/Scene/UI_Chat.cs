using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class UI_Chat : UI_Base
{
    public List<Text> chatList = new List<Text>();

    [SerializeField]
    public Button sendBtn;
    //[SerializeField]
    //public Text chatLog;
    [SerializeField]
    public InputField input;

    public override void Init()
    {
        //Reset
        ResetFunction_UI();

        sendBtn.onClick.AddListener(Function_Button);
        input.onValueChanged.AddListener(Function_InputField);
        input.onEndEdit.AddListener(Function_InputField_EndEdit);

       
    }

    private void Start()
    {
        Init();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Function_Button();
        }

        if (_Scrollbar == null)
            return;

        if(_Scrollbar.value >= 0.00002f || _Scrollbar != null)
            _Scrollbar.value = 0.00002f;
    }

    Scrollbar _Scrollbar;
    public void addChat(S_Chat chatPacket)
    {
        if (chatList.Count > 20)
        {
            for(int i = 0;i < chatList.Count;i++)
            {
                Managers.Resource.Destroy(chatList[i].gameObject);
            }

            chatList.RemoveRange(0, chatList.Count);
        }

        GameObject S_grid = gameObject.transform.Find("Scroll View").Find("Viewport").Find("Content").gameObject;

        GameObject chat = Managers.Resource.Instantiate("UI/Scene/ChatLog", S_grid.transform);

        chat.GetOrAddComponent<Text>().text = $"[{chatPacket.ObjectId}] :  {chatPacket.Sending}";

        chatList.Add(chat.GetOrAddComponent<Text>());

        _Scrollbar = gameObject.transform.Find("Scroll View").Find("Scrollbar Vertical").GetComponent<Scrollbar>();

        if (_Scrollbar != null)
        {
            _Scrollbar.value = 0.00002f;
        }
    }

   

    private void Function_Button()
    {
        string txt = input.text;
        //chatLog.text = txt;

        C_Chat chatPacket = new C_Chat();
        chatPacket.ObjectId = Managers.Object.MyPlayer.Id;
        chatPacket.Sending = txt;
        Managers.Network.Send(chatPacket);

        input.text = null;
    }

    private void Function_InputField(string _data)
    {
        string txt = _data;
        input.text = _data;
    }
    private void Function_InputField_EndEdit(string _data)
    {
        string txt = _data;
        input.text = _data;
    }

    private void ResetFunction_UI()
    {
        sendBtn.onClick.RemoveAllListeners();
        input.placeholder.GetComponent<Text>().text = "Input..";
        input.onValueChanged.RemoveAllListeners();
        input.onEndEdit.RemoveAllListeners();
        input.contentType = InputField.ContentType.Standard;
        input.lineType = InputField.LineType.MultiLineNewline;

    }
}
