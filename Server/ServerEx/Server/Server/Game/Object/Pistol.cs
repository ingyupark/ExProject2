using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public class Pistol : Projectile
	{
		public GameObject Owner { get; set; }
		public Pistol()
        {
			Skill skillData = null;
			DataManager.SkillDict.TryGetValue(3, out skillData);
			Info.Name = skillData.name;
		}
		public override void Update()
		{
			if (Data == null || Data.projectile == null || Owner == null || Room == null)
				return;

			int tick = (int)(1000 / Data.projectile.speed);
			Room.PushAfter(tick, Update);

			Vector2Int destPos = GetFrontCellPos();
			if (Room.Map.ApplyMove(this, destPos, collision: false))
			{
				S_Move movePacket = new S_Move();
				movePacket.ObjectId = Id;
				movePacket.PosInfo = PosInfo;
				Room.Broadcast(CellPos, movePacket);

				Console.WriteLine("Move Pistol");
			}
			else
			{
				GameObject target = Room.Map.Find(destPos);
				if (target != null)
				{
					target.OnDamaged(this, Data.damage + Owner.TotalAttack);
				}

				// 소멸
				Room.Push(Room.LeaveGame, Id);
			}
		}

		public override GameObject GetOwner()
		{
			return Owner;
		}
	}
}
