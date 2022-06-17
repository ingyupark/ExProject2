using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Server.DB
{
	[Table("Account")]
	public class AccountDb
	{
		public int AccountDbId { get; set; }
		public string AccountName { get; set; }
		public ICollection<PlayerDb> Players { get; set; }
	}

	[Table("Player")]
	public class PlayerDb
	{
		public int PlayerDbId { get; set; }
		public string PlayerName { get; set; }

		[ForeignKey("Account")]
		public int AccountDbId { get; set; }
		public AccountDb Account { get; set; }

		public ICollection<ItemDb> Items { get; set; }
		public ICollection<SkillDb> Skills { get; set; }

		public int Level { get; set; }
		public int Hp { get; set; }
		public int Sp { get; set; }
		public int Hrecovery { get; set; }
		public int Sprecovery { get; set; }
		public int MaxHp { get; set; }
		public int MaxSp { get; set; }
		public int Attack { get; set; }
		public float Speed { get; set; }
		public int TotalExp { get; set; }
		public int Exp { get; set; }
		public int Gold { get; set; }
		public int P_SkillType { get; set; }
	}

	[Table("Item")]
	public class ItemDb
	{
		public int ItemDbId { get; set; }
		public int TemplateId { get; set; }
		public int Level { get; set; }
		public int Damage { get; set; }
		public int Defence { get; set; }
		public int Count { get; set; }
		public int Slot { get; set; }
		public bool Equipped { get; set; } = false;
		public bool IsSetCount { get; set; } = false;

		[ForeignKey("Owner")]
		public int? OwnerDbId { get; set; }
		public PlayerDb Owner { get; set; }
	}

	[Table("Skill")]
	public class SkillDb
	{
		public int SkillDbId { get; set; }
		public int TemplateId { get; set; }
		public int Level { get; set; }
		public int Damage { get; set; }
		public int Spconsumption { get; set; }
		public int SkillSlot { get; set; }
		public bool SkillEquipped { get; set; } = false;

		[ForeignKey("Owner")]
		public int? OwnerDbId { get; set; }
		public PlayerDb Owner { get; set; }
	}
}
