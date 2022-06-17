using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public partial class GameRoom : JobSerializer
	{
		public void HandleMove(Player player, C_Move movePacket)
		{
			if (player == null)
				return;

			// TODO : 검증
			PositionInfo movePosInfo = movePacket.PosInfo;
			ObjectInfo info = player.Info;

			

			// 다른 좌표로 이동할 경우, 갈 수 있는지 체크
			if (movePosInfo.PosX != info.PosInfo.PosX || movePosInfo.PosY != info.PosInfo.PosY)
			{
				if (Map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)) == false)
					return;
			}

			

				info.PosInfo.State = movePosInfo.State;
			info.PosInfo.MoveDir = movePosInfo.MoveDir;
			Map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY));

			// 다른 플레이어한테도 알려준다
			S_Move resMovePacket = new S_Move();
			resMovePacket.ObjectId = player.Info.ObjectId;
			resMovePacket.PosInfo = movePacket.PosInfo;

			Broadcast(player.CellPos, resMovePacket);
		}
		public void HandleChat(Player player, C_Chat chatPacket)
        {
			if (player == null)
				return;

			S_Chat chat = new S_Chat();
			chat.ObjectId = chatPacket.ObjectId;
			chat.Sending = chatPacket.Sending;

			ChatBroadcast(player.CellPos, chat);
		}

		public void HandleSkill(Player player, C_Skill skillPacket)
		{
			if (player == null)
				return;

			ObjectInfo info = player.Info;
			Vector2Int BuyPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
			Npc npctarget = Map.Find(BuyPos) as Npc;

			if (npctarget != null && npctarget.ObjectType == GameObjectType.Npc)
			{
				S_Buynpc BuyNpc = new S_Buynpc();
				BuyNpc.NpcId = npctarget.TemplateId;
				player.Session.Send(BuyNpc);
			}

			if (info.PosInfo.State != CreatureState.Idle)
				return;
			
			if (player.Stat.Sp <= 0 && player.P_SkillType == 2)
				return;
			if (player.Stat.Sp <= 0 && player.P_SkillType == 3)
				return;

			// TODO : 스킬 사용 가능 여부 체크
			// SP 깍는 량 바꾸어 줘야함 나중에는..

			info.PosInfo.State = CreatureState.Skill;

			if(skillPacket.Info.SkillId == 1 || skillPacket.Info.SkillId == 4)
			{
				player.Stat.Sp -= 0;
				if (player.Stat.Sp <= 0)
					player.Stat.Sp = 0;
                // 평타는 Sp 소모 없다.

			}
			else if (skillPacket.Info.SkillId == 2)
			{
				player.Stat.Sp -= 1;

				if (player.Stat.Sp <= 0)
					player.Stat.Sp = 0;
			}
			else if (skillPacket.Info.SkillId == 3)
			{
				player.Stat.Sp -= 2;

				if (player.Stat.Sp <= 0)
					player.Stat.Sp = 0;
			}
			else if (skillPacket.Info.SkillId == 5)
			{
				//범위 공격
				player.Stat.Sp -= 20;

				if (player.Stat.Sp <= 0)
					player.Stat.Sp = 0;
			}
			else
            {

            }

			S_ChangeSp _ChangeSp = new S_ChangeSp();
			_ChangeSp.ObjectId = player.PlayerDbId;
			_ChangeSp.Sp = player.Stat.Sp;
			player.Session.Send(_ChangeSp);

			S_Skill skill = new S_Skill() { Info = new SkillInfo() };
			skill.ObjectId = info.ObjectId;
			skill.Info.SkillId = skillPacket.Info.SkillId;

			Broadcast(player.CellPos, skill);

			Data.Skill skillData = null;
			if (DataManager.SkillDict.TryGetValue(skillPacket.Info.SkillId, out skillData) == false)
				return;

			switch (skillData.skillType)
			{
				case SkillType.SkillAuto:
					{
						Vector2Int skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
						GameObject target = Map.Find(skillPos);
						if (target != null)
						{
							//타겟을 찾았다..
							target.OnDamaged(player, skillData.damage);

						}
					}
					break;
				case SkillType.SkillProjectile:
					{
						Arrow arrow = ObjectManager.Instance.Add<Arrow>();
						if (arrow == null)
							return;

						arrow.Owner = player;
						arrow.Data = skillData;
						arrow.PosInfo.State = CreatureState.Moving;
						arrow.PosInfo.MoveDir = player.PosInfo.MoveDir;
						arrow.PosInfo.PosX = player.PosInfo.PosX;
						arrow.PosInfo.PosY = player.PosInfo.PosY;
						arrow.Speed = skillData.projectile.speed;
						Push(EnterGame, arrow, false);
					}
					break;
				case SkillType.SkillGun:
					{
						Pistol pistol = ObjectManager.Instance.Add<Pistol>();
						if (pistol == null)
							return;

						pistol.Owner = player;
						pistol.Data = skillData;
						pistol.Data.name = skillData.name;
						pistol.PosInfo.State = CreatureState.Moving;
						pistol.PosInfo.MoveDir = player.PosInfo.MoveDir;
						pistol.PosInfo.PosX = player.PosInfo.PosX;
						pistol.PosInfo.PosY = player.PosInfo.PosY;
						pistol.Speed = skillData.projectile.speed;
						Push(EnterGame, pistol, false);
					}
					break;
				case SkillType.SkillSickle:
					{
						Vector2Int skillPos = player.GetFrontCellPos(info.PosInfo.MoveDir);
						GameObject target = Map.Find(skillPos);
						if (target != null)
						{
							//타겟을 찾았다..
							target.OnDamaged(player, skillData.damage);

						}
					}
					break;
				case SkillType.SkillRange:
					{
						// GameObject를 배열로 받는다.
						// 그냥 내주변 4방향으로 쓰는거..
						Vector2Int[] skillPos = new Vector2Int[4];
						GameObject[] target = new GameObject[4];
						skillPos[0] = player.GetFrontCellPos(MoveDir.Up);
						skillPos[1] = player.GetFrontCellPos(MoveDir.Down);
						skillPos[2] = player.GetFrontCellPos(MoveDir.Right);
						skillPos[3] = player.GetFrontCellPos(MoveDir.Left);

						for(int i = 0;i < skillPos.Length;i++)
                        {
							target[i] = Map.Find(skillPos[i]);

							if (target[i] != null)
							{
								//타겟을 찾았다..
								target[i].OnDamaged(player, skillData.damage);
							}
						}

					}
					break;

			}
		}

	}
}
