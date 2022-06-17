using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public partial class GameRoom : JobSerializer
	{		
		public void HandleEquipItem(Player player, C_EquipItem equipPacket)
		{
			if (player == null)
				return;

			player.HandleEquipItem(equipPacket);
		}
		//Handledrunk
		public void Handledrunk(Player player, C_Drunk_Item DrunkPacket)
        {
			if (player == null)
				return;

			player.HandleDrunk(DrunkPacket);
		}

		public void Handlelevelupitem(Player player, C_Levelup_Item levelupPacket)
        {
			if (player == null)
				return;
			player.Handlelevelupitem(levelupPacket);

		}

		public void HandleSlot(Player player, C_Slot_Count SlotPacket)
		{
			if (player == null)
				return;

			player.HandleSlot(SlotPacket);
		}

		public void HandleCount(Player player, C_Change_Icount countPacket)
		{
			if (player == null)
				return;

			player.HandleCount(countPacket);
		}

		public void HandleBuyItem(Player player, C_Buyitem buyPacket)
		{
			if (player == null)
				return;

			Npc npc = null;
			_npc.TryGetValue(npcID,out npc);
			if (npc != null && npc.ObjectType == GameObjectType.Npc)
			{
				//타겟을 찾았다..
				npc.OnSell(player, buyPacket);

			}
		}
	}
}
