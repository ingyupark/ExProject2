using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DB;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
	public partial class ClientSession : PacketSession
	{
		public int AccountDbId { get; private set; }
		public List<LobbyPlayerInfo> LobbyPlayers { get; set; } = new List<LobbyPlayerInfo>();

		public void HandleLogin(C_Login loginPacket)
		{
			// TODO : 이런 저런 보안 체크
			if (ServerState != PlayerServerState.ServerStateLogin)
				return;

			// TODO : 문제가 있긴 있다
			// - 동시에 다른 사람이 같은 UniqueId을 보낸다면?
			// - 악의적으로 여러번 보낸다면
			// - 쌩뚱맞은 타이밍에 그냥 이 패킷을 보낸다면?

			LobbyPlayers.Clear();

			using (AppDbContext db = new AppDbContext())
			{
				AccountDb findAccount = db.Accounts
					.Include(a => a.Players)
					.Where(a => a.AccountName == loginPacket.UniqueId).FirstOrDefault();

				if (findAccount != null)
				{
					// AccountDbId 메모리에 기억
					AccountDbId = findAccount.AccountDbId;

					S_Login loginOk = new S_Login() { LoginOk = 1 };
					foreach (PlayerDb playerDb in findAccount.Players)
					{
						LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
						{
							PlayerDbId = playerDb.PlayerDbId,
							Name = playerDb.PlayerName,
							StatInfo = new StatInfo()
							{
								Level = playerDb.Level,
								Hp = playerDb.Hp,
								Sp = playerDb.Sp,
								MaxHp = playerDb.MaxHp,
								Maxsp = playerDb.MaxSp,
								Hprecovery = playerDb.Hrecovery,
								Sprecovery = playerDb.Sprecovery,
								Attack = playerDb.Attack,
								Speed = playerDb.Speed,
								TotalExp = playerDb.TotalExp,
								SkillType = playerDb.P_SkillType,
								Exp = playerDb.Exp,
								Gold = playerDb.Gold
							}
						};

						// 메모리에도 들고 있다
						LobbyPlayers.Add(lobbyPlayer);

						// 패킷에 넣어준다
						loginOk.Players.Add(lobbyPlayer);
					}

					Send(loginOk);
					// 로비로 이동
					ServerState = PlayerServerState.ServerStateLobby;
				}
				else
				{
					AccountDb newAccount = new AccountDb() { AccountName = loginPacket.UniqueId };
					db.Accounts.Add(newAccount);
					bool success = db.SaveChangesEx();
					if (success == false)
						return;

					// AccountDbId 메모리에 기억
					AccountDbId = newAccount.AccountDbId;

					S_Login loginOk = new S_Login() { LoginOk = 1 };
					Send(loginOk);
					// 로비로 이동
					ServerState = PlayerServerState.ServerStateLobby;
				}
			}
		}

		public void Handlepotal(C_Potal potal)
        {
			GameLogic.Instance.Push(() =>
			{
				MyPlayer.Potals();
				//MyPlayer.Room
				//일단 디스폰 안되던거 해결.
				GameRoom room = GameLogic.Instance.Find(potal.Roomid);
				room.Push(room.EnterGame, MyPlayer, true);
			});
		}

		//EnterGame Packet받아서 포탈이동? 가능할거같은데?
		public void HandleEnterGame(C_EnterGame enterGamePacket)
		{
			if (ServerState != PlayerServerState.ServerStateLobby)
				return;

			LobbyPlayerInfo playerInfo = LobbyPlayers.Find(p => p.Name == enterGamePacket.Name);
			if (playerInfo == null)
				return;
			MyPlayer = ObjectManager.Instance.Add<Player>();
			{
				MyPlayer.PlayerDbId = playerInfo.PlayerDbId;
				MyPlayer.Info.Name = playerInfo.Name;
				MyPlayer.Info.PosInfo.State = CreatureState.Idle;
				MyPlayer.Info.PosInfo.MoveDir = MoveDir.Down;
				MyPlayer.Info.PosInfo.PosX = 0;
				MyPlayer.Info.PosInfo.PosY = 0;
				MyPlayer.Info.StatInfo.SkillType = playerInfo.StatInfo.SkillType;
				MyPlayer.P_SkillType = playerInfo.StatInfo.SkillType;
				MyPlayer.Stat.MergeFrom(playerInfo.StatInfo);
				MyPlayer.Session = this;
				MyPlayer.isUpdateHp = false;
				MyPlayer.isUpdateHp = false;
				S_ItemList itemListPacket = new S_ItemList();

				// 아이템 목록을 갖고 온다
				using (AppDbContext db = new AppDbContext())
				{
					List<ItemDb> items = db.Items
						.Where(i => i.OwnerDbId == playerInfo.PlayerDbId)
						.ToList();

					foreach (ItemDb itemDb in items)
					{
						Item item = Item.MakeItem(itemDb);
						
						if (item != null)
						{
							MyPlayer.Inven.Add(item);

							ItemInfo info = new ItemInfo();
							info.MergeFrom(item.Info);
							itemListPacket.Items.Add(info);
						}
					}
				}


				Send(itemListPacket);
			}

			ServerState = PlayerServerState.ServerStateGame;
			//어디선가 초기화 한번이 필요한데.. 초기화가 안된다..ㅠㅠ

			GameLogic.Instance.Push(() =>
			{
				GameRoom room = GameLogic.Instance.Find(2);
				//ㅅㅣㅈㅏㄱㅎㅏㅁㅕㄴ ㅁㅏㅇㅡㄹㄹㅗ ㄹㅣㅅㅡㅍㅗㄴ
				room.Push(room.EnterGame, MyPlayer, true);
			});
		}

		public void HandleCreatePlayer(C_CreatePlayer createPacket)
		{
			// TODO : 이런 저런 보안 체크
			if (ServerState != PlayerServerState.ServerStateLobby)
				return;

			using (AppDbContext db = new AppDbContext())
			{
				PlayerDb findPlayer = db.Players
					.Where(p => p.PlayerName == createPacket.Name).FirstOrDefault();

				if (findPlayer != null)
				{
					// 이름이 겹친다
					Send(new S_CreatePlayer());
				}
				else
				{
					// 1레벨 스탯 정보 추출
					StatInfo stat = null;
					DataManager.StatDict.TryGetValue(1, out stat);

					// DB에 플레이어 만들어줘야 함
					PlayerDb newPlayerDb = new PlayerDb()
					{
						PlayerName = createPacket.Name,
						Level = stat.Level,
						Hp = stat.Hp,
						Sp = stat.Sp,
						MaxHp = stat.MaxHp,
						MaxSp = stat.Maxsp,
						Hrecovery = stat.Hprecovery,
						Sprecovery = stat.Sprecovery,
						Attack = stat.Attack,
						Speed = stat.Speed,
						TotalExp = stat.TotalExp,
						Exp = 0,
						Gold = stat.Gold,
						AccountDbId = AccountDbId,
						P_SkillType = 1
						//최초 공격 타입 몽둥이..
					};

					db.Players.Add(newPlayerDb);
					bool success = db.SaveChangesEx();
					if (success == false)
						return;

					// 메모리에 추가
					LobbyPlayerInfo lobbyPlayer = new LobbyPlayerInfo()
					{
						PlayerDbId = newPlayerDb.PlayerDbId,
						Name = createPacket.Name,
						StatInfo = new StatInfo()
						{
							Level = newPlayerDb.Level,
							Hp = newPlayerDb.Hp,
							Sp = newPlayerDb.Sp,
							MaxHp = newPlayerDb.MaxHp,
							Maxsp = newPlayerDb.MaxSp,
							Hprecovery = newPlayerDb.Hrecovery,
							Sprecovery = newPlayerDb.Sprecovery,
							Attack = newPlayerDb.Attack,
							Speed = newPlayerDb.Speed,
							TotalExp = newPlayerDb.TotalExp,
							SkillType = newPlayerDb.P_SkillType,
							Exp = newPlayerDb.Exp,
							Gold = newPlayerDb.Gold,

						}
						
					};

					// 메모리에도 들고 있다
					LobbyPlayers.Add(lobbyPlayer);

					// 클라에 전송
					S_CreatePlayer newPlayer = new S_CreatePlayer() { Player = new LobbyPlayerInfo() };
					newPlayer.Player.MergeFrom(lobbyPlayer);

					Send(newPlayer);
				}
			}
		}
	}
}
