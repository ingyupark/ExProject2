using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DB;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public class Player : GameObject
	{
		public int PlayerDbId { get; set; }
		public ClientSession Session { get; set; }
		public VisionCube Vision { get; private set; }

		public Inventory Inven { get; private set; } = new Inventory();

		public int WeaponDamage { get; private set; }
		public int ArmorDefence { get; private set; }

        public override int TotalAttack { get {  return Stat.Attack + WeaponDamage; } }
		public override int TotalDefence { get { return ArmorDefence; } }

		public int Gold { get; set; }
		public int Exp { get; set; }

		public int P_SkillType { get; set; }

		public Player()
		{
			ObjectType = GameObjectType.Player;
			Vision = new VisionCube(this);
		}
		public void LevelUp(int Level)

		{


		}

		public override void OnDamaged(GameObject attacker, int damage)
		{
			base.OnDamaged(attacker, damage);
		}

		public override void OnDead(GameObject attacker)
		{

			isSpUpdate = false;
			isUpdate = false;

			if (Vision._job != null)
			{
				Vision._job.Cancel = true;
				Vision._job = null;
			}
			base.OnDead(attacker);
		}

		// FSM (Finite State Machine)
		IJob _job;
		IJob _Hpjob;
		bool isUpdate =false;
		bool isSpUpdate = false;

		public override void Update()
        {

			//해당 부분 5초 뒤에 한다는 뜻.
			if (isUpdateHp && Stat.Hp < Stat.MaxHp && !isUpdate)
			{
				
				isUpdate = true;

				if (!isUpdateHp || Stat.Hp >= Stat.MaxHp)
					return;
				if (Room != null)
					_Hpjob = Room.PushAfter(10000, UpdateHp);
			}

			if (isUpdateSp && Stat.Sp < Stat.Maxsp && !isSpUpdate)
			{
				
				isSpUpdate = true;

				if (!isUpdateSp || Stat.Sp >= Stat.Maxsp)
					return;
				if (Room != null)
					_Hpjob = Room.PushAfter(10000, UpdateSp);
			}

			// (5초마다 한번씩 Update)
			// 5프레임 (0.2초마다 한번씩 Update)
			if (Room != null)
				_job = Room.PushAfter(200, Update);
		}

		public override void UpdateHp()
		{
			if (!isUpdateHp || Stat.Hp >= Stat.MaxHp )
			{ }
			else
			{ 
				Hp += Stat.Hprecovery;

				if (Hp >= Stat.MaxHp)
				{
					isUpdateHp = false;
					isUpdate = false;
					Hp = Stat.MaxHp;
				}

				S_ChangeHp changePacket = new S_ChangeHp();
				changePacket.ObjectId = Id;
				changePacket.Hp = Stat.Hp;

				if (Room != null)
					Room.Broadcast(CellPos, changePacket);


				if (Room != null)
					_Hpjob = Room.PushAfter(5000, UpdateHp);
			}
		}

		public void UpdateSp()
		{
			if (!isUpdateSp || Stat.Sp >= Stat.Maxsp)
			{ }
			else
			{
				Sp += Stat.Sprecovery;

				if (Sp >= Stat.Maxsp)
				{
					isUpdateSp = false;
					isSpUpdate = false;
					Sp = Stat.Maxsp;
				}


				S_ChangeSp changeSPPacket = new S_ChangeSp();
				changeSPPacket.ObjectId = Id;
				changeSPPacket.Sp = Stat.Sp;

				if (Room != null)
					Room.Broadcast(CellPos, changeSPPacket);

				if (Room != null)
					_Hpjob = Room.PushAfter(5000, UpdateSp);
			}
		}

		public void OnLeaveGame()
		{
			// TODO
			// DB 연동?
			// -- 피가 깎일 때마다 DB 접근할 필요가 있을까?
			// 1) 서버 다운되면 아직 저장되지 않은 정보 날아감
			// 2) 코드 흐름을 다 막아버린다 !!!!
			// - 비동기(Async) 방법 사용?
			// - 다른 쓰레드로 DB 일감을 던져버리면 되지 않을까?
			// -- 결과를 받아서 이어서 처리를 해야 하는 경우가 많음.
			// -- 아이템 생성

			isUpdateHp = false;
			isUpdate = false;
			isUpdateSp = false;
			isSpUpdate = false;

			if (_job != null)
			{
				_job.Cancel = true;
				_job = null;
			}

			if (_Hpjob != null)
			{
				_Hpjob.Cancel = true;
				_Hpjob = null;
			}

			if (Vision._job != null)
			{
				Vision._job.Cancel = true;
				Vision._job = null;
			}
			DbTransaction.SavePlayerStatus_Step1(this, Room);
		}
		//HandleCount
		

		public void HandleCount(C_Change_Icount countPacket)
		{
			if (countPacket.Iscount)
				return;

			Item item = Inven.Get(countPacket.ItemDbId);

			if (item == null)
				return;
			{
				// 메모리 선적용
				item.ItemDbId = countPacket.ItemDbId;
				item.Count -= countPacket.Count;
				//메모리에는 그냥 0으로 저장하고 없애지 않는다? or 메모리상 지워버린다??
				//일단 그냥 0으로 날리고 클라에서 지우자..

				// DB에 Noti
				DbTransaction.CountItemNoti(this, item);

				//클라에는 클라에서 삭제처리하면됨..
				// 클라에 통보
				S_Change_Icount countItem = new S_Change_Icount();
				countItem.ItemDbId = countPacket.ItemDbId;
				countItem.Count = item.Count;
				countItem.Iscount = countPacket.Iscount;
				Session.Send(countItem);

				//메모리에 제일 마지막으로.. item을 null 처리 하는 거니까..
				if (item.Count <= 0)
					item = Inven.Remove(countPacket.ItemDbId);
			}
		}
		public void Handlelevelupitem(C_Levelup_Item c_Levelup)
        {
			if (_job != null)
			{
				_job.Cancel = true;
				_job = null;
			}

			Item unequipItem = Inven.Get(c_Levelup.ItemDbId);

			if (unequipItem.Level >= 6)
				return;

			if (unequipItem.ItemType == ItemType.Weapon)
			{
				WeaponsStatData _statData = null;

				if (unequipItem.Level < 0)
				{
					unequipItem.Level = 0;
				}
				else
				{
					_statData = null;
					DataManager.WeaponsStatDict.TryGetValue($"{unequipItem.TemplateId}{unequipItem.Level}", out _statData);

					int rand = new Random().Next(0, 101);

					//확률인듯.
					int sum = 0;
					if (unequipItem.Level == 0)
						sum += 101;
					else
						sum += _statData.probability;

					if (rand <= sum)
					{
						unequipItem.Level += 1;
						//수정완료 레벨업은 됨..
						_statData = null;
						DataManager.WeaponsStatDict.TryGetValue($"{unequipItem.TemplateId}{unequipItem.Level}", out _statData);
						((Weapon)unequipItem).SetDamage(_statData.damage);
						//데미지 적용이 안됨 ㅠㅠ 레벨업은 되지만..
					}
					else
						return;
				}

                //확률에 맞게 레벨 올려주게 수정 필요..
                //레벨은 올려줘도 될듯.

                // 강화 직후에는 강화된 데미지로 변환되어 적용 완료된다..
                // 시작할때도 강화되어 있던 Level 불러와서 적용 어떻게?
                // 레벨업의 적용은 완료.
			}
			else if (unequipItem.ItemType == ItemType.Armor)
			{
				//((Armor)item).SetDef();
				// Armor는 아니구나..
				// 아직 데이터시트가 없음..
				return;
			}
			else
				return;

			//저장하는 기분만 있음..

			RefreshAdditionalStat();
			DbTransaction.LevelUpItem(this, unequipItem);
		}

		public void HandleDrunk(C_Drunk_Item drunkPacket)
        {
			Item item = Inven.Get(drunkPacket.ItemDbId);
			
			if (item == null)
				return;
			if (item.ItemType != ItemType.Consumable)
				return;

			if (item.Count < 0)
				return;

			if (this.Stat.Hp < this.Stat.MaxHp && item.TemplateId >= 200 && item.TemplateId < 260)
			{
				this.Stat.Hp += ((Consumable)item).Damage;
                //this.Sp += ;


                if (this.Stat.Hp >= this.Stat.MaxHp)
					this.Stat.Hp = this.Stat.MaxHp;
			}
			else
            {
				//변동없이 카운트만 깍는다.
            }

			if (this.Stat.Sp < this.Stat.Maxsp && item.TemplateId >= 260 && item.TemplateId < 300)
			{
				//this.Hp += ;
				this.Stat.Sp += ((Consumable)item).Damage;
				if (this.Stat.Sp >= this.Stat.Maxsp)
					this.Stat.Sp = this.Stat.Maxsp;
			}
			else
			{
				//변동없이 카운트만 깍는다.
			}
			//물약Data있어야할듯.. 소량 중 대량이란 SP도 만들어야함.. 아니면 그냥 엘릭서 개념으로 만들자..

			{
				// 메모리 선적용
				item.ItemDbId = drunkPacket.ItemDbId;
				item.Count -= 1;
				if (item.Count <= 0)
					item.Count = 0;
				// DB에 Noti
				DbTransaction.DrunkItemNoti(this, item);
				// 클라에 통보
				S_Drunk_Item drunkItem = new S_Drunk_Item();
				drunkItem.ItemDbId = drunkPacket.ItemDbId;
				drunkItem.Count = item.Count;
				drunkItem.PlayerDbId = this.PlayerDbId;
				drunkItem.Hp = this.Stat.Hp;
				drunkItem.Sp = this.Stat.Sp;
				// S_Drunk_Item에 Hp랑 Sp PlayerID함꼐 보내야함..
				Session.Send(drunkItem);

				// 다른 애들애게 브로드 케스팅..
				S_ChangeHp changePacket = new S_ChangeHp();
				changePacket.ObjectId = Id;
				changePacket.Hp = Stat.Hp;

				if (Room != null)
					Room.Broadcast(CellPos, changePacket);

			}

			//메모리에 제일 마지막으로.. item을 null 처리 하는 거니까..
			if (item.Count <= 0)
				item = Inven.Remove(drunkPacket.ItemDbId);
		}

		public void HandleSlot(C_Slot_Count slotPacket)
        {
			Item item = Inven.Get(slotPacket.ItemDbId);

			if (item == null)
				return;

			{
				// 메모리 선적용
				item.ItemDbId = slotPacket.ItemDbId;
				item.Slot = slotPacket.Slot;

				// DB에 Noti
				DbTransaction.SlotItemNoti(this, item);

				// 클라에 통보
				S_Slot_Count SlotItem = new S_Slot_Count();
				SlotItem.ItemDbId = slotPacket.ItemDbId;
				SlotItem.Slot = slotPacket.Slot;
				Session.Send(SlotItem);
			}
		}

		public void HandleEquipItem(C_EquipItem equipPacket)
		{
			Item item = Inven.Get(equipPacket.ItemDbId);
			if (item == null)
				return;

			//if (item.ItemType == ItemType.Consumable)
			//	return;
			//포션도 장착 타입으로 바꾼다.


			// 착용 요청이라면, 겹치는 부위 해제
			if (equipPacket.Equipped)
			{
				Item unequipItem = null;

				if (item.ItemType == ItemType.Weapon)
				{
					unequipItem = Inven.Find(
						i => i.Equipped && i.ItemType == ItemType.Weapon);
				}
				else if (item.ItemType == ItemType.Armor)
				{
					ArmorType armorType = ((Armor)item).ArmorType;
					unequipItem = Inven.Find(
						i => i.Equipped && i.ItemType == ItemType.Armor
							&& ((Armor)i).ArmorType == armorType);
				}
				else if (item.ItemType == ItemType.Consumable)
                {
					unequipItem = Inven.Find(
					i => i.Equipped && i.ItemType == ItemType.Consumable);
				}

				if (unequipItem != null)
				{
					// 메모리 선적용
					unequipItem.Equipped = false;

					if (unequipItem.ItemType == ItemType.Weapon)
						unequipItem.SkillType = 1;
					// DB에 Noti
					DbTransaction.EquipItemNoti(this, unequipItem);

					// 클라에 통보
					S_EquipItem equipOkItem = new S_EquipItem();
					equipOkItem.ItemDbId = unequipItem.ItemDbId;
					equipOkItem.Equipped = unequipItem.Equipped;
					Session.Send(equipOkItem);
				}
			}

			{
				// 메모리 선적용
				item.Equipped = equipPacket.Equipped;
				if (item.ItemType == ItemType.Weapon)
                {
					Info.StatInfo.SkillType = equipPacket.SkillType;
					P_SkillType = equipPacket.SkillType;
					item.SkillType = equipPacket.SkillType;
				}

				// DB에 Noti
				DbTransaction.EquipItemNoti(this, item);

				// 클라에 통보
				S_EquipItem equipOkItem = new S_EquipItem();
				equipOkItem.ItemDbId = equipPacket.ItemDbId;
				equipOkItem.Equipped = equipPacket.Equipped;
				if(item.ItemType == ItemType.Weapon)
					equipOkItem.SkillType = equipPacket.SkillType;
				Session.Send(equipOkItem);
			}

			RefreshAdditionalStat();
		}

		public void RefreshAdditionalStat()
		{
			WeaponDamage = 0;
			ArmorDefence = 0;

			foreach (Item item in Inven.Items.Values)
			{
				if (item.Equipped == false)
					continue;

				switch (item.ItemType)
				{
					case ItemType.Weapon:

						if(item.Level > 0)
                        {
							WeaponsStatData _statData = null;
							DataManager.WeaponsStatDict.TryGetValue($"{item.TemplateId}{item.Level}", out _statData);
							((Weapon)item).SetDamage(_statData.damage);
						}
						WeaponDamage += ((Weapon)item).Damage;

						break;
					case ItemType.Armor:
						ArmorDefence += ((Armor)item).Defence;
						break;
				}
			}
		}
	}
}
