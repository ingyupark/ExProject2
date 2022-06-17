using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game
{
	public partial class GameRoom : JobSerializer
	{
		public const int VisionCells = 10;

		public int RoomId { get; set; }

		Dictionary<int, Player> _players = new Dictionary<int, Player>();

		Dictionary<int, Monster> _monsters = new Dictionary<int, Monster>();
		Dictionary<int, Npc> _npc = new Dictionary<int, Npc>();
		public int npcID;

		Dictionary<int, Potal> _potal = new Dictionary<int, Potal>();
		Dictionary<int, Projectile> _projectiles = new Dictionary<int, Projectile>();

		public Zone[,] Zones { get; private set; }
		public int ZoneCells { get; private set; }


		public Map Map { get; private set; } = new Map();

		// ㅁㅁㅁ
		// ㅁㅁㅁ
		// ㅁㅁㅁ
		public Zone GetZone(Vector2Int cellPos)
		{
			int x = (cellPos.x - Map.MinX) / ZoneCells;
			int y = (Map.MaxY - cellPos.y) / ZoneCells;
			return GetZone(y, x);
		}

		public Zone GetZone(int indexY, int indexX)
		{
			if (indexX < 0 || indexX >= Zones.GetLength(1))
				return null;
			if (indexY < 0 || indexY >= Zones.GetLength(0))
				return null;

			return Zones[indexY, indexX];
		}

		//ㅇㅓㄸㅓㄴㄱㅕㅇㅇㅜㅇㅔ Map002ㄹㅗ 이동하는지?? 화깅ㄴ?
        public void Init(int mapId, int zoneCells)
		{
			Map.LoadMap(mapId);


			// Zone
			ZoneCells = zoneCells; // 10
			// 1~10 칸 = 1존
			// 11~20칸 = 2존
			// 21~30칸 = 3존
			int countY = (Map.SizeY + zoneCells - 1) / zoneCells;
			int countX = (Map.SizeX + zoneCells - 1) / zoneCells;
			Zones = new Zone[countY, countX];
			for (int y = 0; y < countY; y++)
			{
				for (int x = 0; x < countX; x++)
				{
					Zones[y, x] = new Zone(y, x);
				}
			}

			// TEMP
			if(mapId != 2)
            {
				for (int i = 0; i < 3; i++)
				{
					Monster monster = ObjectManager.Instance.Add<Monster>();
					monster.Init(1);
					EnterGame(monster, randomPos: true);
				}
				for (int i = 0; i < 20; i++)
				{
					Monster monster = ObjectManager.Instance.Add<Monster>();
					monster.Init(2);
					EnterGame(monster, randomPos: true);
				}
				for (int i = 0; i < 5; i++)
				{
					Monster monster = ObjectManager.Instance.Add<Monster>();
					monster.Init(3);
					EnterGame(monster, randomPos: true);
				}
				for (int i = 0; i < 5; i++)
				{
					Monster monster = ObjectManager.Instance.Add<Monster>();
					monster.Init(4);
					EnterGame(monster, randomPos: true);
				}

			}
			else
            {
				Npc npc1 = ObjectManager.Instance.Add<Npc>();
				npc1.Init(1);
				npc1.PosInfo.PosX = -5;
				npc1.PosInfo.PosY = 3;
				EnterGame(npc1, randomPos: false);

				Npc npc2 = ObjectManager.Instance.Add<Npc>();
				npc2.Init(2);
				npc2.PosInfo.PosX = -4;
				npc2.PosInfo.PosY = 3;
				EnterGame(npc2, randomPos: false);

			}


		}

		// 누군가 주기적으로 호출해줘야 한다
		public void Update()
		{
			Flush();
		}
		public void PotalGame(GameObject gameObject, bool randomPos)
        {

			if (gameObject == null)
				return;

			if (randomPos)
			{
				Vector2Int respawnPos;
				while (true)
				{
					respawnPos.x = _rand.Next(Map.MinX, Map.MaxX);
					respawnPos.y = _rand.Next(Map.MinY, Map.MaxY);
					if (Map.Find(respawnPos) == null && Map.CanGo(new Vector2Int(respawnPos.x, respawnPos.y)))
					{
						gameObject.CellPos = respawnPos;
						break;
					}

				}

			}

			GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

			if (type == GameObjectType.Player)
			{
				Player player = null;
				_players.TryGetValue(gameObject.Id, out player);
				player.Room = this;

				player.RefreshAdditionalStat();

				Map.ApplyMove(player, new Vector2Int(player.CellPos.x, player.CellPos.y));
				GetZone(player.CellPos).Players.Add(player);

				// 본인한테 정보 전송
				{
					player.Vision.Update();
					player.Update();
				}

				// 타인한테 정보 전송
				{
					S_Spawn spawnPacket = new S_Spawn();
					spawnPacket.Objects.Add(gameObject.Info);
					Broadcast(gameObject.CellPos, spawnPacket);
				}
			}
		}

		Random _rand = new Random();
		public void EnterGame(GameObject gameObject, bool randomPos )
		{


			if (gameObject == null)
				return;

			if (randomPos)
			{
				Vector2Int respawnPos;
				while (true)
				{
					respawnPos.x = _rand.Next(Map.MinX, Map.MaxX);
					respawnPos.y = _rand.Next(Map.MinY, Map.MaxY);
					if (Map.Find(respawnPos) == null && Map.CanGo(new Vector2Int(respawnPos.x,respawnPos.y)))
					{
						gameObject.CellPos = respawnPos;
						break;
					}
					
				}
			}

			GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

			if (type == GameObjectType.Player)
			{
				Player player = gameObject as Player;
				if (!_players.ContainsKey(gameObject.Id))
					_players.Add(gameObject.Id, player);
				player.Room = this;

				player.RefreshAdditionalStat();

				Map.ApplyMove(player, new Vector2Int(player.CellPos.x, player.CellPos.y));
				GetZone(player.CellPos).Players.Add(player);

				// 본인한테 정보 전송
				{
					S_EnterGame enterPacket = new S_EnterGame();
					enterPacket.Player = player.Info;
					player.Session.Send(enterPacket);
					player.Vision.Update();
					player.Update();
				}
			}
			else if (type == GameObjectType.Monster)
			{
				Monster monster = gameObject as Monster;
				_monsters.Add(gameObject.Id, monster);
				monster.Room = this;

				GetZone(monster.CellPos).Monsters.Add(monster);
				Map.ApplyMove(monster, new Vector2Int(monster.CellPos.x, monster.CellPos.y));

				monster.Update();
			}
			else if (type == GameObjectType.Projectile)
			{
				Projectile projectile = gameObject as Projectile;
				_projectiles.Add(gameObject.Id, projectile);
				projectile.Room = this;

				GetZone(projectile.CellPos).Projectiles.Add(projectile);
				projectile.Update();
			}
			else if(type == GameObjectType.Npc)
            {
				Npc npc = gameObject as Npc;
				//일단 NPC 하나만이니까
				npcID = gameObject.Id;
				_npc.Add(gameObject.Id, npc);
				npc.Room = this;

				GetZone(npc.CellPos).Npcs.Add(npc);

				npc.Update();
			}
			

			// 타인한테 정보 전송
			{
				S_Spawn spawnPacket = new S_Spawn();
				spawnPacket.Objects.Add(gameObject.Info);
				Broadcast(gameObject.CellPos, spawnPacket);
			}
		}

		public void LeaveGame(int objectId)
		{
			GameObjectType type = ObjectManager.GetObjectTypeById(objectId);

			Vector2Int cellPos;

			if (type == GameObjectType.Player)
			{
				Player player = null;
				if (_players.Remove(objectId, out player) == false)
					return;

				cellPos = player.CellPos;

				player.OnLeaveGame();
				Map.ApplyLeave(player);
				player.Room = null;

				// 본인한테 정보 전송
				{
					S_LeaveGame leavePacket = new S_LeaveGame();
					player.Session.Send(leavePacket);
				}
			}
			else if (type == GameObjectType.Monster)
			{
				Monster monster = null;
				if (_monsters.Remove(objectId, out monster) == false)
					return;

				cellPos = monster.CellPos;
				Map.ApplyLeave(monster);
				monster.Room = null;
			}
			else if (type == GameObjectType.Projectile)
			{
				Projectile projectile = null;
				if (_projectiles.Remove(objectId, out projectile) == false)
					return;

				cellPos = projectile.CellPos;
				Map.ApplyLeave(projectile);
				projectile.Room = null;
			}
			else if (type == GameObjectType.Npc)
			{
				Npc npc = null;
				if (_npc.Remove(objectId, out npc) == false)
					return;

				cellPos = npc.CellPos;
				Map.ApplyLeave(npc);
				npc.Room = null;
			}
			else
			{
				return;
			}

			// 타인한테 정보 전송
			{
				S_Despawn despawnPacket = new S_Despawn();
				despawnPacket.ObjectIds.Add(objectId);
				Broadcast(cellPos, despawnPacket);
			}
		}

		public Player FindPlayer(Func<GameObject, bool> condition)
		{
			foreach (Player player in _players.Values)
			{
				if (condition.Invoke(player))
					return player;
			}

			return null;
		}
		// 살짝 부담스러운 함수
		public Player FindClosestPlayer(Vector2Int pos, int range)
		{
			List<Player> players = GetAdjacentPlayers(pos, range);

			players.Sort((left, right) =>
			{
				int leftDist = (left.CellPos - pos).cellDistFromZero;
				int rightDist = (right.CellPos - pos).cellDistFromZero;
				return leftDist - rightDist;
			});

			foreach (Player player in players)
			{
				List<Vector2Int> path = Map.FindPath(pos, player.CellPos, checkObjects: true);
				if (path.Count < 2 || path.Count > range)
					continue;

				return player;
			}

			return null;
		}

		public void Broadcast(Vector2Int pos, IMessage packet)
		{
			List<Zone> zones = GetAdjacentZones(pos);

			foreach (Player p in zones.SelectMany(z => z.Players))
			{
				int dx = p.CellPos.x - pos.x;
				int dy = p.CellPos.y - pos.y;
				if (Math.Abs(dx) > GameRoom.VisionCells)
					continue;
				if (Math.Abs(dy) > GameRoom.VisionCells)
					continue;

				p.Session.Send(packet);
			}
		}

		public void ChatBroadcast(Vector2Int pos, IMessage packet)
		{
			List<Zone> zones = GetAdjacentZones(pos);

			foreach (KeyValuePair<int,Player> p in _players)
			{
				p.Value.Session.Send(packet);
			}
		}

		public List<Player> GetAdjacentPlayers(Vector2Int pos, int range)
		{
			List<Zone> zones = GetAdjacentZones(pos, range);
			return zones.SelectMany(z => z.Players).ToList();
		}

		// ㅁㅁㅁㅁㅁㅁ
		// ㅁㅁㅁㅁㅁㅁ
		// ㅁㅁㅁㅁㅁㅁ
		// ㅁㅁㅁㅁㅁㅁ

		public List<Zone> GetAdjacentZones(Vector2Int cellPos, int range = GameRoom.VisionCells)
		{
			HashSet<Zone> zones = new HashSet<Zone>();

			int maxY = cellPos.y + range;
			int minY = cellPos.y - range;
			int maxX = cellPos.x + range;
			int minX = cellPos.x - range;

			// 좌측 상단
			Vector2Int leftTop = new Vector2Int(minX, maxY);
			int minIndexY = (Map.MaxY - leftTop.y) / ZoneCells;
			int minIndexX = (leftTop.x - Map.MinX) / ZoneCells;

			// 우측 하단
			Vector2Int rightBot = new Vector2Int(maxX, minY);
			int maxIndexY = (Map.MaxY - rightBot.y) / ZoneCells;
			int maxIndexX = (rightBot.x - Map.MinX) / ZoneCells;

			for (int x = minIndexX; x <= maxIndexX; x++)
			{
				for (int y = minIndexY; y <= maxIndexY; y++)
				{
					Zone zone = GetZone(y, x);
					if (zone == null)
						continue;

					zones.Add(zone);
				}
			}

			return zones.ToList();
		}
	}
}
