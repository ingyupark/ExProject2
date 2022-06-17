using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.DB;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class PacketHandler
{
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

		//Console.WriteLine($"C_Move ({movePacket.PosInfo.PosX}, {movePacket.PosInfo.PosY})");

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleMove, player, movePacket);
	}
	public static void C_BuyItemHandler(PacketSession session, IMessage packet)
    {
		C_Buyitem buyPacket = packet as C_Buyitem;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		//if(player.Gold < )

		GameRoom room = player.Room;
		if (room == null)
			return;
		room.Push(room.HandleBuyItem, player, buyPacket);
	}

	public static void C_ChatHandler(PacketSession session, IMessage packet)
    {
		C_Chat chatPacket = packet as C_Chat;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleChat, player, chatPacket);
	}

	public static void C_SkillHandler(PacketSession session, IMessage packet)
	{
		C_Skill skillPacket = packet as C_Skill;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleSkill, player, skillPacket);
	}

	public static void C_LoginHandler(PacketSession session, IMessage packet)
	{
		C_Login loginPacket = packet as C_Login;
		ClientSession clientSession = session as ClientSession;
		clientSession.HandleLogin(loginPacket);
	}
	public static void C_PotalHandler(PacketSession session, IMessage packet)
    {
		C_Potal potalPacket = (C_Potal)packet;
		ClientSession clientSession = (ClientSession)session;
		clientSession.Handlepotal(potalPacket);
		//
	}

	public static void C_EnterGameHandler(PacketSession session, IMessage packet)
	{
		C_EnterGame enterGamePacket = (C_EnterGame)packet;
		ClientSession clientSession = (ClientSession)session;
		clientSession.HandleEnterGame(enterGamePacket);
	}

	public static void C_CreatePlayerHandler(PacketSession session, IMessage packet)
	{
		C_CreatePlayer createPlayerPacket = (C_CreatePlayer)packet;
		ClientSession clientSession = (ClientSession)session;
		clientSession.HandleCreatePlayer(createPlayerPacket);
	}

	public static void C_EquipItemHandler(PacketSession session, IMessage packet)
	{
		C_EquipItem equipPacket = (C_EquipItem)packet;
		ClientSession clientSession = (ClientSession)session;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleEquipItem, player, equipPacket);
	}

	public static void C_LevelupItemHandler(PacketSession session, IMessage packet)
    {
		C_Levelup_Item c_Levelup = (C_Levelup_Item)packet;
		ClientSession clientSession = (ClientSession)session;
		Player player = clientSession.MyPlayer;

		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;
		room.Push(room.Handlelevelupitem, player, c_Levelup);
	}
	//C_Drunk_ItemHandler
	public static void C_Drunk_ItemHandler(PacketSession session, IMessage packet)
    {

		C_Drunk_Item drunkpacket = (C_Drunk_Item)packet;
		ClientSession clientSession = (ClientSession)session;
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.Handledrunk, player, drunkpacket);
	}
	public static void C_SlotCountHandler(PacketSession session, IMessage packet)
    {
		C_Slot_Count slotpacket = (C_Slot_Count)packet;
		ClientSession clientSession = (ClientSession)session;
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleSlot, player, slotpacket);
	}

	//해당 패킷은 -해주는건 아직 없다.. 그낭 count만 주고받기가됨.. 이대로 연결시..
	//받은 count만큼 빼주는 걸로 디자인 하자..
	public static void C_ChangeIcountHandler(PacketSession session, IMessage packet)
	{
		C_Change_Icount countpacket = (C_Change_Icount)packet;
		ClientSession clientSession = (ClientSession)session;
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleCount, player, countpacket);
	}

	public static void C_PongHandler(PacketSession session, IMessage packet)
	{
		ClientSession clientSession = (ClientSession)session;
		clientSession.HandlePong();
	}
}
