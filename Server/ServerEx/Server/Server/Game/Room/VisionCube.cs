using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game.Room
{
	public class VisionCube
	{
		public Player Owner { get; private set; }
		public HashSet<GameObject> PreviousObjects { get; private set; } = new HashSet<GameObject>();

		public IJob _job;

		public VisionCube(Player owner)
		{
			Owner = owner;
		}

		public HashSet<GameObject> GatherObjects()
		{
			if (Owner == null || Owner.Room == null)
            {

				return null;
			}

			HashSet<GameObject> objects = new HashSet<GameObject>();

			List<Zone> zones = Owner.Room.GetAdjacentZones(Owner.CellPos);

			Vector2Int cellPos = Owner.CellPos;
			foreach (Zone zone in zones)
			{
				foreach (Player player in zone.Players)
				{
					int dx = player.CellPos.x - cellPos.x;
					int dy = player.CellPos.y - cellPos.y;
					if (Math.Abs(dx) > GameRoom.VisionCells)
						continue;
					if (Math.Abs(dy) > GameRoom.VisionCells)
						continue;
					objects.Add(player);
				}

				foreach (Monster monster in zone.Monsters)
				{
					int dx = monster.CellPos.x - cellPos.x;
					int dy = monster.CellPos.y - cellPos.y;
					if (Math.Abs(dx) > GameRoom.VisionCells)
						continue;
					if (Math.Abs(dy) > GameRoom.VisionCells)
						continue;
					objects.Add(monster);
				}

				foreach (Projectile projectile in zone.Projectiles)
				{
					int dx = projectile.CellPos.x - cellPos.x;
					int dy = projectile.CellPos.y - cellPos.y;
					if (Math.Abs(dx) > GameRoom.VisionCells)
						continue;
					if (Math.Abs(dy) > GameRoom.VisionCells)
						continue;
					objects.Add(projectile);
				}

				foreach (Npc npc in zone.Npcs)
				{
					int dx = npc.CellPos.x - cellPos.x;
					int dy = npc.CellPos.y - cellPos.y;
					if (Math.Abs(dx) > GameRoom.VisionCells)
						continue;
					if (Math.Abs(dy) > GameRoom.VisionCells)
						continue;
					objects.Add(npc);
				}
			}

			return objects;
		}

		public void Update()
		{
			if (Owner == null || Owner.Room == null)
			{

				return;
			}

			HashSet<GameObject> currentObjects = GatherObjects();

			// 기존엔 없었는데 새로 생긴 애들 Spawn 처리
			// 이전오브젝트 제외하니까 없는데 데미지 입는 현상있음..ㅠㅠ
			//List<GameObject> added = currentObjects.Except(PreviousObjects).ToList();

			List<GameObject> added = currentObjects.ToList();
			if (added.Count > 0)
			{
				S_Spawn spawnPacket = new S_Spawn();
				foreach (GameObject gameObject in added)
				{
					ObjectInfo info = new ObjectInfo();
					info.MergeFrom(gameObject.Info);
					spawnPacket.Objects.Add(info);
				}

				Owner.Session.Send(spawnPacket);
			}

			// 기존엔 있었는데 사라진 애들 Despawn 처리
			List<GameObject> removed = PreviousObjects.Except(currentObjects).ToList();
			if (removed.Count > 0)
			{
				S_Despawn despawnPacket = new S_Despawn();

				foreach (GameObject gameObject in removed)
				{
					despawnPacket.ObjectIds.Add(gameObject.Id);
				}

				Owner.Session.Send(despawnPacket);
			}

			// 교체
			PreviousObjects = currentObjects;

			_job = Owner.Room.PushAfter(100, Update);
		}
	}
}
