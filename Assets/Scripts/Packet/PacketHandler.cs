using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{
	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		Managers.Object.Add(enterGamePacket.Player, myPlayer: true);

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		gameSceneUI.mainUI.RefreshUI();
		gameSceneUI.StatUI.RefreshUI();
		gameSceneUI.InvenUI.RefreshUI();
	}

	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGameHandler = packet as S_LeaveGame;
		Managers.Object.Clear();
	}

	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		foreach (ObjectInfo obj in spawnPacket.Objects)
		{
			Managers.Object.Add(obj, myPlayer: false);
		}

	}

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		foreach (int id in despawnPacket.ObjectIds)
		{
			Managers.Object.Remove(id);
		}
	}

	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;

		GameObject go = Managers.Object.FindById(movePacket.ObjectId);
		if (go == null)
			return;

		if (Managers.Object.MyPlayer.Id == movePacket.ObjectId)
			return;

		BaseController bc = go.GetComponent<BaseController>();

		if (bc == null)
			return;

		bc.PosInfo = movePacket.PosInfo;
	}

	public static void S_SkillHandler(PacketSession session, IMessage packet)
	{
		S_Skill skillPacket = packet as S_Skill;

		GameObject go = Managers.Object.FindById(skillPacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc != null)
		{
			cc.UseSkill(skillPacket.Info.SkillId);
		}
	}

	public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
	{
		S_ChangeHp changePacket = packet as S_ChangeHp;

		int newhp = changePacket.Hp;
		int oldhp;
		GameObject go = Managers.Object.FindById(changePacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc != null)
		{
			oldhp = cc.Hp;
			cc.Hp = changePacket.Hp;
			if (newhp - oldhp != 0)
				cc.AddText(newhp, oldhp);
			//크리처 스크롤러에 텍스트 추가.. 후 New 가 더 크면 초록색
			//Old가 더 크면 빨간색 표시.
		}

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		gameSceneUI.StatUI.RefreshUI();
	}

	public static void S_S_AlarmHandler(PacketSession session, IMessage packet)
    {
		//요기다가 알람 팝업 띄우자.
		S_Alarm _Alarm = (S_Alarm)packet;
		if (Managers.Object.MyPlayer.isAlarm)
			return;
		Managers.Object.MyPlayer.isAlarm = true;
		UI_AlarmPopup _BuyPopup = Managers.UI.ShowPopupUI<UI_AlarmPopup>("UI_AlarmPopup");
		_BuyPopup.Alarm_Pop(_Alarm.Alarm);

    }

	public static void S_BuyNpcHandler(PacketSession session, IMessage packet)
    {
		S_Buynpc npc = (S_Buynpc)packet;
		if (Managers.Object.MyPlayer != null)
		{
			UI_BuyPopup _BuyPopup = Managers.UI.ShowPopupUI<UI_BuyPopup>("UI_BuyPopup");
			_BuyPopup.Npcid = npc.NpcId;
			_BuyPopup.OpenPop();
			Managers.Object.MyPlayer.isBuy = true;

		}

	}

	public static void S_GoldListHandler(PacketSession session, IMessage packet)
    {
		S_GoldList goldList = (S_GoldList)packet;

		// 메모리에 아이템 정보 적용

		if (Managers.Object.MyPlayer != null)
		{
			Managers.Object.MyPlayer.Stat.Gold = goldList.Gold;
		}

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		gameSceneUI.InvenUI.Init();
		gameSceneUI.mainUI.RefreshUI();
	}

	public static void S_AddGoldHandler(PacketSession session, IMessage packet)
    {
		// 서버에서 + 시키고 요기서는 적용만 하자..

		S_AddGold goldList = (S_AddGold)packet;

		// 메모리에 아이템 정보 적용

		if (Managers.Object.MyPlayer != null)
		{
			Managers.Object.MyPlayer.Stat.Gold = goldList.Gold;
		}

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		gameSceneUI.InvenUI.Init();
		gameSceneUI.mainUI.RefreshUI();
	}

	public static void S_ChatHandler(PacketSession session, IMessage packet)
    {
		S_Chat chatPacket = packet as S_Chat;

		GameObject go = Managers.Object.FindById(chatPacket.ObjectId);
		if (go == null)
			return;

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;

		gameSceneUI.mainUI.ChatInfo.GetComponent<UI_Chat>().addChat(chatPacket);
	}

	public static void S_ChangeSpHandler(PacketSession session, IMessage packet)
    {
		S_ChangeSp changePacket = packet as S_ChangeSp;

		if (Managers.Object.MyPlayer == null)
			return;

		Managers.Object.MyPlayer.Sp = changePacket.Sp;

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		gameSceneUI.StatUI.RefreshUI();

		if (Managers.Object.MyPlayer != null)
			Managers.Object.MyPlayer.RefreshAdditionalStat();

	}

	public static void S_DieHandler(PacketSession session, IMessage packet)
	{
		S_Die diePacket = packet as S_Die;

		GameObject go = Managers.Object.FindById(diePacket.ObjectId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc != null)
		{
			cc.Hp = 0;
			cc.OnDead();
		}
	}

	public static void S_ConnectedHandler(PacketSession session, IMessage packet)
	{
		Debug.Log("S_ConnectedHandler");
		C_Login loginPacket = new C_Login();

		string path = Application.dataPath;
		loginPacket.UniqueId = path.GetHashCode().ToString();
		Managers.Network.Send(loginPacket);
	}

	// 로그인 OK + 캐릭터 목록
	public static void S_LoginHandler(PacketSession session, IMessage packet)
	{
		S_Login loginPacket = (S_Login)packet;
		Debug.Log($"LoginOk({loginPacket.LoginOk})");

		// TODO : 로비 UI에서 캐릭터 보여주고, 선택할 수 있도록
		if (loginPacket.Players == null || loginPacket.Players.Count == 0)
		{
			C_CreatePlayer createPacket = new C_CreatePlayer();
			createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}";
			Managers.Network.Send(createPacket);
		}
		else
		{
			// 무조건 첫번째 로그인
			LobbyPlayerInfo info = loginPacket.Players[0];
			C_EnterGame enterGamePacket = new C_EnterGame();
			enterGamePacket.Name = info.Name;
			//
			enterGamePacket.Roomid = 1;
			Managers.Network.Send(enterGamePacket);
		}
	}

	public static void S_CreatePlayerHandler(PacketSession session, IMessage packet)
	{
		S_CreatePlayer createOkPacket = (S_CreatePlayer)packet;


		if (createOkPacket.Player == null)
		{
			C_CreatePlayer createPacket = new C_CreatePlayer();
			createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}";
			Managers.Network.Send(createPacket);
		}
		else
		{
			C_EnterGame enterGamePacket = new C_EnterGame();
			enterGamePacket.Name = createOkPacket.Player.Name;
			enterGamePacket.Roomid = 1;
			Managers.Network.Send(enterGamePacket);


		}
	}

	public static void S_ItemListHandler(PacketSession session, IMessage packet)
	{
		S_ItemList itemList = (S_ItemList)packet;

		Managers.Inven.Clear();

		// 메모리에 아이템 정보 적용
		foreach (ItemInfo itemInfo in itemList.Items)
		{

			Item item = Item.MakeItem(itemInfo);
			Managers.Inven.Add(item);
		}

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		gameSceneUI.InvenUI.Init();
		gameSceneUI.mainUI.RefreshUI();
		if (Managers.Object.MyPlayer != null)
			Managers.Object.MyPlayer.RefreshAdditionalStat();
	}

	public static void S_AddItemHandler(PacketSession session, IMessage packet)
	{
		S_AddItem itemList = (S_AddItem)packet;
		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;

		// 메모리에 아이템 정보 적용
		foreach (ItemInfo itemInfo in itemList.Items)
		{
            Item item = Item.MakeItem(itemInfo);

			//요기서 인벤에 Add해준다.. Add 아니고 수정으로 바꾸자..
			// 인벤에 저장된 Item이 있으면 카운트만 바꾸어 준다.
			
			if (Managers.Inven.Get(item.ItemDbId) == null)
				Managers.Inven.Add(item);
			else
            {
				Managers.Inven.Get(item.ItemDbId).Count = itemInfo.Count;
			}
		}
		Debug.Log("아이템을 획득했습니다!");
		gameSceneUI.InvenUI.RefreshUI();
		gameSceneUI.InvenUI.Stat_RefreshUI();
		gameSceneUI.StatUI.RefreshUI();
		gameSceneUI.mainUI.RefreshUI();

		if (Managers.Object.MyPlayer != null)
			Managers.Object.MyPlayer.RefreshAdditionalStat();
	}

	public static void S_EquipItemHandler(PacketSession session, IMessage packet)
	{
		S_EquipItem equipItemOk = (S_EquipItem)packet;
		// 메모리에 아이템 정보 적용
		Item item = Managers.Inven.Get(equipItemOk.ItemDbId);
		if (item == null)
			return;
		PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
		if(item.ItemType == ItemType.Weapon)
			player.P_Skill = equipItemOk.SkillType;
		item.Equipped = equipItemOk.Equipped;

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		gameSceneUI.InvenUI.RefreshUI();
		gameSceneUI.InvenUI.Stat_RefreshUI();
		gameSceneUI.StatUI.RefreshUI();

		if (Managers.Object.MyPlayer != null)
			Managers.Object.MyPlayer.RefreshAdditionalStat();
	}
	//S_Change_IcountHandler
	public static void S_Change_IcountHandler(PacketSession session, IMessage packet)
	{
		S_Change_Icount CountitemOk = (S_Change_Icount)packet;

		// 메모리에 아이템 정보 적용
		Item item = Managers.Inven.Get(CountitemOk.ItemDbId);
		if (item == null)
			return;
		item.ItemDbId = CountitemOk.ItemDbId;
		item.Count = CountitemOk.Count;
		Debug.Log(item.Count);

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;

		gameSceneUI.InvenUI.RefreshUI();

		if (item.Count <= 0)
			item = Managers.Inven.Remove(CountitemOk.ItemDbId);

		gameSceneUI.mainUI.RefreshUI();

	}
	//S_Drunk_ItemHandler
	public static void S_Drunk_ItemHandler(PacketSession session, IMessage packet)
    {
		S_Drunk_Item DrunkItemOk = (S_Drunk_Item)packet;
		// 메모리에 아이템 정보 적용
		Item item = Managers.Inven.Get(DrunkItemOk.ItemDbId);
		if (item == null)
			return;


		item.Count = DrunkItemOk.Count;


		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;

		gameSceneUI.InvenUI.RefreshUI();
		if (item.Count <= 0)
			item = Managers.Inven.Remove(DrunkItemOk.ItemDbId);


		//이까지는 Hp받음..
		//FindByID에서 찾질 모담..ㅠ
		MyPlayerController player = GameObject.FindWithTag("Player").GetComponent<MyPlayerController>();
		if (player == null)
			return;
		int newHp = DrunkItemOk.Hp;
		int oldHp = player.Hp;
		player.Hp = DrunkItemOk.Hp;
		player.Sp = DrunkItemOk.Sp;

		if (player.Hp >= player.Stat.MaxHp)
			player.Hp = player.Stat.MaxHp;

		if (player.Sp >= player.Stat.Maxsp)
			player.Sp = player.Stat.Maxsp;
		if(oldHp - newHp != 0)
			player.AddText(newHp,oldHp);
		player.UpdateHpBar();
		player.UpdateSpBar();

		gameSceneUI.InvenUI.Stat_RefreshUI();
		gameSceneUI.StatUI.RefreshUI();
		gameSceneUI.mainUI.RefreshUI();

		//스텟 UI 가 등록되어 있어야함..

	}

	public static void S_SlotCountHandler(PacketSession session, IMessage packet)
	{
		S_Slot_Count SlotItemOk = (S_Slot_Count)packet;

		// 메모리에 아이템 정보 적용
		Item item = Managers.Inven.Get(SlotItemOk.ItemDbId);
		if (item == null)
			return;

		item.Slot = SlotItemOk.Slot;

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;

		gameSceneUI.InvenUI.RefreshUI();

	}

	public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
	{
		S_ChangeStat itemList = (S_ChangeStat)packet;
		// 이패킷 받아서 업데이트하자.
		// TODO
		foreach(StatInfo statInfo in itemList.StatInfo)
        {
			if (Managers.Object.MyPlayer != null)
			{
				Managers.Object.MyPlayer.Stat = statInfo;
				Managers.Object.MyPlayer.RefreshAdditionalStat();
			}
		}

		

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		gameSceneUI.InvenUI.RefreshUI();
		gameSceneUI.InvenUI.Stat_RefreshUI();
		gameSceneUI.StatUI.RefreshUI();
		gameSceneUI.mainUI.RefreshUI();

	}

	public static void S_PingHandler(PacketSession session, IMessage packet)
	{
		C_Pong pongPacket = new C_Pong();
		Debug.Log("[Server] PingCheck");
		Managers.Network.Send(pongPacket);
	}
}


