using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DB
{
	public partial class DbTransaction : JobSerializer
	{
		public static void EquipTypeNoti(Player player)
        {
			if (player == null)
				return;

			PlayerDb playerDb = new PlayerDb();
			{
				//PlayerID를 줘야지.. Player정보를 찾아서 변경시키는거가틈.. ㅠㅠ
				playerDb.PlayerDbId = player.PlayerDbId;
				playerDb.P_SkillType = player.P_SkillType;
				player.Info.StatInfo.SkillType = player.P_SkillType;
			};
            //Console.WriteLine(playerDb.P_SkillType); 이까지는 잘 온다..

            Instance.Push(() =>
			{
				using (AppDbContext db = new AppDbContext())
				{
					db.Entry(playerDb).State = EntityState.Unchanged;
					db.Entry(playerDb).Property(nameof(PlayerDb.P_SkillType)).IsModified = true;

					bool success = db.SaveChangesEx();
					if (!success)
					{
						// 실패했으면 Kick
					}
				}
			});
		}
		public static void EquipItemNoti(Player player, Item item)
		{
			if (player == null || item == null)
				return;

			if(item.ItemType == ItemType.Weapon && item.Level > 0)
            {

				WeaponsStatData _statData = null;
				DataManager.WeaponsStatDict.TryGetValue($"{item.TemplateId}{item.Level}", out _statData);
				((Weapon)item).SetDamage(_statData.damage);
			}

			ItemDb itemDb = new ItemDb()
			{
				ItemDbId = item.ItemDbId,
				Equipped = item.Equipped,
			};


			// You
			Instance.Push(() =>
			{
				using (AppDbContext db = new AppDbContext())
				{
					db.Entry(itemDb).State = EntityState.Unchanged;
					db.Entry(itemDb).Property(nameof(ItemDb.Equipped)).IsModified = true;

					bool success = db.SaveChangesEx();
					if (!success)
					{
						// 실패했으면 Kick
					}
				}

			});
			if(item.ItemType == ItemType.Weapon)
				EquipTypeNoti(player);
		}

		public static void CountItemNoti(Player player, Item item)
        {
			if (player == null || item == null)
				return;

			ItemDb itemDb = new ItemDb()
			{
				ItemDbId = item.ItemDbId,
				Count = item.Count
			};

			if(itemDb.Count <= 0)
            {
				Instance.Push(() =>
				{
					using (AppDbContext db = new AppDbContext())
					{

						//이거 작동안하면 수정 필요..
						db.Entry(itemDb).State = EntityState.Unchanged;
						db.Remove(itemDb).Property(nameof(ItemDb.Count)).IsModified = true;
						bool success = db.SaveChangesEx();
						if (!success)
						{
							// 실패했으면 Kick
						}
					}
				});
			}
			else
			{ 
				// You
				Instance.Push(() =>
				{
					using (AppDbContext db = new AppDbContext())
					{
						db.Entry(itemDb).State = EntityState.Unchanged;
						db.Entry(itemDb).Property(nameof(ItemDb.Count)).IsModified = true;

						bool success = db.SaveChangesEx();
						if (!success)
						{
							// 실패했으면 Kick
						}
					}
				});
			}
		}

		public static void DrunkItemNoti(Player player, Item item)
        {

			if (player == null || item == null)
				return;

			ItemDb itemDb = new ItemDb()
			{
				ItemDbId = item.ItemDbId,
				Count = item.Count
			};
			// Me (GameRoom)
			PlayerDb playerDb = new PlayerDb()
			{
				PlayerDbId = player.PlayerDbId,
				Hp = player.Stat.Hp,
				Sp = player.Stat.Sp
			};

			if(itemDb.Count <= 0)
            {
				// You
				Instance.Push(() =>
				{
					using (AppDbContext db = new AppDbContext())
					{
						db.Entry(itemDb).State = EntityState.Unchanged;

						//ItemDB Slot이 수정됨으로 변경..
						db.Remove(itemDb).Property(nameof(ItemDb.Count)).IsModified = true;
						bool success = db.SaveChangesEx();
						if (!success)
						{
							// 실패했으면 Kick
						}
						else
						{
							//성공 후 Player 의 HP 변경 저장..
							db.Entry(playerDb).State = EntityState.Unchanged;
							db.Entry(playerDb).Property(nameof(PlayerDb.Hp)).IsModified = true;
							db.Entry(playerDb).Property(nameof(PlayerDb.Sp)).IsModified = true;
							bool successhp = db.SaveChangesEx();
							if (!success)
							{
								// 실패했으면 Kick
							}
						}
					}
				});
			}
			else
            {
				// You
				Instance.Push(() =>
				{
					using (AppDbContext db = new AppDbContext())
					{
						db.Entry(itemDb).State = EntityState.Unchanged;

						//ItemDB Slot이 수정됨으로 변경..
						db.Entry(itemDb).Property(nameof(ItemDb.Count)).IsModified = true;

						bool success = db.SaveChangesEx();
						if (!success)
						{
							// 실패했으면 Kick
						}
						else
						{
							//성공 후 Player 의 HP 변경 저장..
							db.Entry(playerDb).State = EntityState.Unchanged;
							db.Entry(playerDb).Property(nameof(PlayerDb.Hp)).IsModified = true;
							db.Entry(playerDb).Property(nameof(PlayerDb.Sp)).IsModified = true;

							bool successhp = db.SaveChangesEx();
							if (!success)
							{
								// 실패했으면 Kick
							}
						}
					}
				});
			}
			

		}
		public static void SlotItemNoti(Player player, Item item)
		{
			if (player == null || item == null)
				return;

			ItemDb itemDb = new ItemDb()
			{
				ItemDbId = item.ItemDbId,
				Slot = item.Slot
			};

			// You
			Instance.Push(() =>
			{
				using (AppDbContext db = new AppDbContext())
				{
					db.Entry(itemDb).State = EntityState.Unchanged;

					//ItemDB Slot이 수정됨으로 변경..
					db.Entry(itemDb).Property(nameof(ItemDb.Slot)).IsModified = true;

					bool success = db.SaveChangesEx();
					if (!success)
					{
						// 실패했으면 Kick
					}
				}
			});
		}
	}
}
