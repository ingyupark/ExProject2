﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DB
{
	public class AppDbContext : DbContext
	{
		public DbSet<AccountDb> Accounts { get; set; }
		public DbSet<PlayerDb> Players { get; set; }
		public DbSet<ItemDb> Items { get; set; }
		public DbSet<SkillDb> Skills { get; set; }

		static readonly ILoggerFactory _logger = LoggerFactory.Create(builder => { builder.AddConsole(); });

		string _connectionString = @"Server=localhost,1433; Database=Master;Initial Catalog=GameDB; User Id = SA; Password=<Ingyu@0013>";

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options
				.UseLoggerFactory(_logger)
				.UseSqlServer(ConfigManager.Config == null ? _connectionString : ConfigManager.Config.connectionString);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<AccountDb>()
				.HasIndex(a => a.AccountName)
				.IsUnique();

			builder.Entity<PlayerDb>()
				.HasIndex(p => p.PlayerName)
				.IsUnique();
		}
	}
}