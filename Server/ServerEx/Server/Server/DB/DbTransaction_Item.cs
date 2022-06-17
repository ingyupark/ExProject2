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
		public static void LevelUpItem(Player player,Item item)
		{
			if (player == null || item == null)
				return;

			

			ItemDb itemDb = new ItemDb()
			{
				ItemDbId = item.ItemDbId,
				Level = item.Level
			};


			// You
			Instance.Push(() =>
			{
				using (AppDbContext db = new AppDbContext())
				{
					db.Entry(itemDb).State = EntityState.Unchanged;
					db.Entry(itemDb).Property(nameof(ItemDb.Level)).IsModified = true;

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
