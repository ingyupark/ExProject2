using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Server.Game
{
    public class Npc : GameObject
    {
        public int TemplateId { get; private set; }

		public Npc()
		{
			ObjectType = GameObjectType.Npc;
		}

		public void Init(int templateId)
		{
			TemplateId = templateId;

			NpcData npcData = null;
			DataManager.NpcDict.TryGetValue(TemplateId, out npcData);
			Info.Name = npcData.name;
			State = CreatureState.Idle;

		}
		public override void OnDamaged(GameObject attacker, int damage)
		{
        }

		IJob _job;
		public override void Update()
		{

			switch (State)
			{
				case CreatureState.Idle:
					UpdateIdle();
					break;
				
			}
			// 몬스터는 HP 리커버리 설정 안됫다는 가정..
			//if (Room != null)
			//	_Hpjob = Room.PushAfter(5000, UpdateHp);
			// 5프레임 (0.2초마다 한번씩 Update)
			if (Room != null)
				_job = Room.PushAfter(200, Update);
		}

		long _nextSearchTick = 0;

		protected virtual void UpdateIdle()
		{
			if (_nextSearchTick > Environment.TickCount64)
				return;
			_nextSearchTick = Environment.TickCount64 + 2000;

			State = CreatureState.Idle;
			Room.Map.ApplyMove(this, new Vector2Int(PosInfo.PosX, PosInfo.PosY));
		}
		public void OnBuy(Player player)
        {
		}

		//요기서 아이템 스펙 올려줘야함..
		//item을 패킷으로 수정 필요..
		public void LevelUpItem(Player player,Item item)
        {
			
		}

		//이건 언제?? 패킷 받아야겟지?? (playerid 받고 itemid 받SellectId)
		public override void OnSell(Player player , C_Buyitem buyPacket)
		{
			if (_job != null)
			{
				_job.Cancel = true;
				_job = null;
			}

			if (player.ObjectType == GameObjectType.Player)
			{
				//임시로 넣은 값 수정필요.	
				int Sellct = buyPacket.ItemDbId;
				ItemlistData rewardData = GetReward(Sellct);
				if (player.Stat.Gold < rewardData.goldcount)
                {
					DbTransaction.Alarm(player,"돈이 모자랍니다.");
					return;

				}
				if (rewardData != null)
				{
					DbTransaction.RewardPlayer(player, rewardData, Room);
					DbTransaction.DisGoldPlayer(player, rewardData, Room);
				}

			}
		}

		// 나중에는 이거 필요 없다..
		ItemlistData GetReward(int SellectId)
		{
			NpcData npcData = null;
			//npc 1 2
			if(SellectId >= 200)
				DataManager.NpcDict.TryGetValue(1, out npcData);
			else
				DataManager.NpcDict.TryGetValue(2, out npcData);

			foreach (ItemlistData itemData in npcData.itemlist)
			{

				if (itemData.itemId == SellectId)
				{
					return itemData;
				}
			}

			return null;
		}


	}
}
