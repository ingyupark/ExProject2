using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.DB
{
	public partial class DbTransaction : JobSerializer
	{
		public static DbTransaction Instance { get; } = new DbTransaction();

		// Me (GameRoom) -> You (Db) -> Me (GameRoom)
		public static void SavePlayerStatus_AllInOne(Player player, GameRoom room)
		{
			if (player == null || room == null)
				return;

			// Me (GameRoom)
			PlayerDb playerDb = new PlayerDb();
			playerDb.PlayerDbId = player.PlayerDbId;
			playerDb.Hp = player.Stat.Hp;

			// You
			Instance.Push(() =>
			{
				using (AppDbContext db = new AppDbContext())
				{
					db.Entry(playerDb).State = EntityState.Unchanged;
					db.Entry(playerDb).Property(nameof(PlayerDb.Hp)).IsModified = true;
					bool success = db.SaveChangesEx();
					if (success)
					{
						// Me
						room.Push(() => Console.WriteLine($"Hp Saved({playerDb.Hp})"));
					}
				}
			});			
		}

		// Me (GameRoom)
		public static void SavePlayerStatus_Step1(Player player, GameRoom room)
		{
			if (player == null || room == null)
				return;

			// Me (GameRoom)
			PlayerDb playerDb = new PlayerDb();
			playerDb.PlayerDbId = player.PlayerDbId;
			playerDb.Hp = player.Stat.Hp;
			Instance.Push<PlayerDb, GameRoom>(SavePlayerStatus_Step2, playerDb, room);
		}

		// You (Db)
		public static void SavePlayerStatus_Step2(PlayerDb playerDb, GameRoom room)
		{
			using (AppDbContext db = new AppDbContext())
			{
				db.Entry(playerDb).State = EntityState.Unchanged;
				db.Entry(playerDb).Property(nameof(PlayerDb.Hp)).IsModified = true;
				bool success = db.SaveChangesEx();
				if (success)
				{
					room.Push(SavePlayerStatus_Step3, playerDb.Hp);
				}
			}
		}

		// Me
		public static void SavePlayerStatus_Step3(int hp)
		{
			Console.WriteLine($"Hp Saved({hp})");
		}

		public static void LevelUpPlayer(Player player, GameRoom room)
        {

			StatInfo _statData = null;
			DataManager.StatDict.TryGetValue(player.Stat.Level, out _statData);

  			player.Stat.Level = _statData.Level;
			player.Stat.MaxHp = _statData.MaxHp;
			player.Stat.Maxsp = _statData.Maxsp;
			player.Stat.Hp = _statData.MaxHp;
			player.Stat.Sp = _statData.Maxsp;
			player.Stat.Hprecovery = _statData.Hprecovery;
			player.Stat.Sprecovery = _statData.Sprecovery;
			player.Stat.Attack = _statData.Attack;
			player.Stat.Speed = _statData.Speed;
			player.Stat.TotalExp = _statData.TotalExp;

			// You
			Instance.Push(() =>
			{
				using (AppDbContext db = new AppDbContext())
				{
					PlayerDb playerDb = new PlayerDb()
					{
						PlayerDbId = player.PlayerDbId,
						Level = _statData.Level,
						MaxHp = _statData.MaxHp,
						MaxSp = _statData.Maxsp,
						Hp = _statData.MaxHp,
						Sp = _statData.Maxsp,
						Hrecovery = _statData.Hprecovery,
						Sprecovery = _statData.Sprecovery,
						Attack = _statData.Attack,
						Speed = _statData.Speed,
						TotalExp = _statData.TotalExp,
						Exp = player.Stat.Exp,
					};

                    //레벨업 안됨..
                    var items = db.Players
					   .FromSqlRaw("SELECT * FROM dbo.Player")
					   .Where(b => b.PlayerDbId == player.PlayerDbId)
					   .FirstOrDefault();
					if (items != default)
                    {
						items.Level = playerDb.Level;
						items.MaxHp = playerDb.MaxHp;
						items.MaxSp = playerDb.MaxSp;
						items.Hp = playerDb.Hp;
						items.Hp = playerDb.Hp;
						items.Hrecovery = playerDb.Hrecovery;
						items.Sprecovery = playerDb.Sprecovery;
						items.Attack = playerDb.Attack;
						items.Speed = playerDb.Speed;
						items.TotalExp = playerDb.TotalExp;
						items.Exp = playerDb.Exp;

					}

					bool success = db.SaveChangesEx();
					if (success)
					{
						// Me
						room.Push(() => Console.WriteLine($"Exp Saved({playerDb.Exp})"));
						// Client Noti
						{
							S_ChangeStat statPacket = new S_ChangeStat();
							StatInfo StatInfo = new StatInfo();

							// itenInfo에 newItem.Info 정보 합치기.
							StatInfo.MergeFrom(player.Stat);
							statPacket.StatInfo.Add(StatInfo);
							player.Session.Send(statPacket);
						}
					}
				}
			});
		}
		public static void RewardGoldPlayer(Player player, GameRoom room, int c = 0)
        {
			if (player == null || room == null)
				return;

			player.Gold += c;
			player.Stat.Gold += c;

			if (player.Stat.Gold >= 1000000)
			{
				player.Stat.Gold = 1000000;
			}

			// You
			Instance.Push(() =>
			{
				using (AppDbContext db = new AppDbContext())
				{

					PlayerDb playerDb = new PlayerDb()
					{
						PlayerDbId = player.PlayerDbId,
						Gold = player.Stat.Gold,
					};

					db.Entry(playerDb).State = EntityState.Unchanged;
					db.Entry(playerDb).Property(nameof(PlayerDb.Gold)).IsModified = true;
					bool success = db.SaveChangesEx();
					if (success)
					{
						// Client Noti
						{
							// 
							S_ChangeStat statPacket = new S_ChangeStat();
							StatInfo StatInfo = new StatInfo();

							// itenInfo에 newItem.Info 정보 합치기.
							StatInfo.MergeFrom(player.Stat);
							statPacket.StatInfo.Add(StatInfo);
							player.Session.Send(statPacket);
						}
					}
				}
			});
		}

		public static void RewardExpPlayer(Player player, GameRoom room,int c = 0)
        {
			if (player == null || room == null)
				return;

			player.Exp += c;
			player.Stat.Exp += c;

			if(player.Stat.Exp >= player.Stat.TotalExp)
            {
				player.Stat.Exp = player.Stat.TotalExp;
				player.Exp = player.Stat.TotalExp;
			}

			if(player.Stat.Exp >= player.Stat.TotalExp && player.Stat.Level < 16)
            {
				player.Stat.Level += 1;
				LevelUpPlayer(player,room);
			}
			else
            {
				// You
				Instance.Push(() =>
				{
					using (AppDbContext db = new AppDbContext())
					{

						PlayerDb playerDb = new PlayerDb()
						{
							PlayerDbId = player.PlayerDbId,
							Exp = player.Stat.Exp,
						};

						db.Entry(playerDb).State = EntityState.Unchanged;
						db.Entry(playerDb).Property(nameof(PlayerDb.Exp)).IsModified = true;
						bool success = db.SaveChangesEx();
						if (success)
						{
							// Me
							room.Push(() => Console.WriteLine($"Exp Saved({playerDb.Exp})"));
							// Client Noti
							{
								S_ChangeStat statPacket = new S_ChangeStat();
								StatInfo StatInfo = new StatInfo();

								// itenInfo에 newItem.Info 정보 합치기.
								StatInfo.MergeFrom(player.Stat);
								statPacket.StatInfo.Add(StatInfo);
								player.Session.Send(statPacket);
							}
						}
					}
				});
			}

		}
		public static void DisGoldPlayer(Player player, ItemlistData rewardData, GameRoom room)
        {
			if (player == null || rewardData == null || room == null)
				return;

			player.Gold -= rewardData.goldcount;
			player.Stat.Gold -= rewardData.goldcount;
			if (player.Stat.Gold <= 0)
			{
				player.Stat.Gold = 0;
			}

			// You
			Instance.Push(() =>
			{
				using (AppDbContext db = new AppDbContext())
				{

					PlayerDb playerDb = new PlayerDb()
					{
						PlayerDbId = player.PlayerDbId,
						Gold = player.Stat.Gold,
					};

					db.Entry(playerDb).State = EntityState.Unchanged;
					db.Entry(playerDb).Property(nameof(PlayerDb.Gold)).IsModified = true;
					bool success = db.SaveChangesEx();
					if (success)
					{
						// Client Noti
						{
							// 
							S_ChangeStat statPacket = new S_ChangeStat();
							StatInfo StatInfo = new StatInfo();

							// itenInfo에 newItem.Info 정보 합치기.
							StatInfo.MergeFrom(player.Stat);
							statPacket.StatInfo.Add(StatInfo);
							player.Session.Send(statPacket);
						}
					}
				}
			});
		}
		public static void Alarm(Player player, string a)
        {
			// Client Noti
			{
				S_Alarm _Alarm = new S_Alarm();
				_Alarm.Alarm = a;
				player.Session.Send(_Alarm);

			}
		}

		public static void RewardPlayer(Player player, ItemlistData rewardData, GameRoom room)
        {
			if (player == null || rewardData == null || room == null)
				return;
			// TODO : 살짝 문제가 있긴 하다...
			// 1) DB에다가 저장 요청
			// 2) DB 저장 OK
			// 3) 메모리에 적용

			ItemDb itemDb = new ItemDb();



			int? slot = player.Inven.GetEmptySlot();


			ItemData itemData = null;
			DataManager.ItemDict.TryGetValue(rewardData.itemId, out itemData);

			//이부분 좀 다르게 해야함 ㅠ
			//itemDb는 그냥 데이터 보내기 위해 만든 임시.. 
			if (rewardData.itemId >= 200 && rewardData.itemId <= 300)
			{
				//아이템 갯수 제한..
				itemDb.TemplateId = rewardData.itemId;
				itemDb.Count = rewardData.count;
				itemDb.OwnerDbId = player.PlayerDbId;
				itemDb.Damage = ((ConsumableData)itemData).damage;
				if (slot != null)
					itemDb.Slot = slot.Value;
			}
			else
			{
				if (slot == null)
					return;

				if(itemData.itemType == ItemType.Weapon)
                {
					itemDb.Damage = ((WeaponData)itemData).damage;
				}
				else if (itemData.itemType == ItemType.Armor)
                {
					itemDb.Defence = ((ArmorData)itemData).defence;

				}

				itemDb.TemplateId = rewardData.itemId;
				itemDb.Count = rewardData.count;
				itemDb.Slot = slot.Value;
				itemDb.OwnerDbId = player.PlayerDbId;
			}

			// You
			Instance.Push(() =>
			{
				using (AppDbContext db = new AppDbContext())
				{
					if (itemDb.TemplateId >= 200 && itemDb.TemplateId <= 300 && itemDb.Count > 0)
					{
						var items = db.Items
					   .FromSqlRaw("SELECT * FROM dbo.Item")
					   .Where(b => b.TemplateId == itemDb.TemplateId)
					   .FirstOrDefault();

						if (items == default && slot != null)
						{
							db.Items.Add(itemDb);
						}
						else if (items != default)
						{

							items.TemplateId = itemDb.TemplateId;
							items.Count += itemDb.Count;


							if (items.Count >= 100)
                            {
								items.Count = 100;
								DbTransaction.Alarm(player, "빈공간이 없습니다.");
								return;
							}

							items.Slot = items.Slot;
							items.OwnerDbId = itemDb.OwnerDbId;
							items.IsSetCount = true;

							itemDb.ItemDbId = items.ItemDbId;
							itemDb.IsSetCount = true;
							itemDb.Slot = items.Slot;
							itemDb.Count = items.Count;
						}
						else
							return;
					}
					else
						db.Items.Add(itemDb);

					bool success = db.SaveChangesEx();

					if (success)
					{
						// Me
						room.Push(() =>
						{
							Item newItem = Item.MakeItem(itemDb);

							if (newItem.IsSetCount && itemDb.TemplateId == newItem.TemplateId)
								player.Inven.Items[newItem.ItemDbId] = newItem;
							else if (!newItem.IsSetCount)
								player.Inven.Add(newItem);

							// Client Noti
							{
								S_AddItem itemPacket = new S_AddItem();
								ItemInfo itemInfo = new ItemInfo();

								// itenInfo에 newItem.Info 정보 합치기.
								itemInfo.MergeFrom(newItem.Info);

								if (!newItem.Info.Issetcount)
								{
									//itemPacket.Items에 추가.
									itemPacket.Items.Add(itemInfo);
								}
								else
								{
									//itemPacket.Items를 수정.
									itemPacket.Items[newItem.ItemDbId] = itemInfo;
								}

								player.Session.Send(itemPacket);
							}
						});
					}
				}
			});

		}

		public static void RewardPlayer(Player player, RewardData rewardData, GameRoom room)
		{
			if (player == null || rewardData == null || room == null)
				return;
			// TODO : 살짝 문제가 있긴 하다...
			// 1) DB에다가 저장 요청
			// 2) DB 저장 OK
			// 3) 메모리에 적용

			ItemDb itemDb = new ItemDb();

			int? slot = player.Inven.GetEmptySlot();
			
			//이부분 좀 다르게 해야함 ㅠ
			//itemDb는 그냥 데이터 보내기 위해 만든 임시.. 
			if (rewardData.itemId >= 200 && rewardData.itemId <= 300)
			{
				//아이템 갯수 제한..
				itemDb.TemplateId = rewardData.itemId;
				itemDb.Count = rewardData.count;
				itemDb.OwnerDbId = player.PlayerDbId;
				if (slot != null)
					itemDb.Slot = slot.Value;
			}
			else
            {
				if (slot == null)
					return;

				itemDb.TemplateId = rewardData.itemId;
				itemDb.Count = rewardData.count;
				itemDb.Slot = slot.Value;
				itemDb.OwnerDbId = player.PlayerDbId;
			}

			// You
			Instance.Push(() =>
			{
				using (AppDbContext db = new AppDbContext())
				{
					if (itemDb.TemplateId >= 200 && itemDb.TemplateId <= 300 && itemDb.Count > 0)
                    {
						var items = db.Items
					   .FromSqlRaw("SELECT * FROM dbo.Item")
					   .Where(b => b.TemplateId == itemDb.TemplateId)
					   .FirstOrDefault();

						if (items == default && slot != null)
						{
							db.Items.Add(itemDb);
						}
						else if (items != default)
						{
							if (items.Count >= 100)
								items.Count = 100;

							items.TemplateId = itemDb.TemplateId;
							items.Count += itemDb.Count;

							items.Slot = items.Slot;
							items.OwnerDbId = itemDb.OwnerDbId;
							items.IsSetCount = true;

							itemDb.ItemDbId  = items.ItemDbId;
							itemDb.IsSetCount = true;
							itemDb.Slot = items.Slot;
							itemDb.Count = items.Count;
						}
						else
							return;
					}
					else
						db.Items.Add(itemDb);

					bool success = db.SaveChangesEx();

					if (success)
					{
						// Me
						room.Push(() =>
						{
							Item newItem = Item.MakeItem(itemDb);

							if (newItem.IsSetCount && itemDb.TemplateId == newItem.TemplateId)
								player.Inven.Items[newItem.ItemDbId] = newItem;
							else if(!newItem.IsSetCount)
								player.Inven.Add(newItem);

							// Client Noti
							{
								S_AddItem itemPacket = new S_AddItem();
								ItemInfo itemInfo = new ItemInfo();

								// itenInfo에 newItem.Info 정보 합치기.
								itemInfo.MergeFrom(newItem.Info);

								if (!newItem.Info.Issetcount)
                                {
									//itemPacket.Items에 추가.
									itemPacket.Items.Add(itemInfo);
								}
								else
								{
									//itemPacket.Items를 수정.
									itemPacket.Items[newItem.ItemDbId] = itemInfo;
								}

								player.Session.Send(itemPacket);
							}
						});
					}
				}
			});
		}
	}
}
