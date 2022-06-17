using System;
using Google.Protobuf.Protocol;
using Server.Data;

namespace Server.Game
{
    public class Potal : GameObject
    {
		public int TemplateId { get; private set; }

		public Potal()
		{
			ObjectType = GameObjectType.Potal;
		}

		public void Init(int templateId)
		{
			TemplateId = templateId;
			ObjectType = GameObjectType.Potal;
			PotalData potalData = null;
			DataManager.PotalDict.TryGetValue(TemplateId, out potalData);
			State = CreatureState.Idle;

		}

		IJob _job;
		public override void Update()
		{

			
			// 몬스터는 HP 리커버리 설정 안됫다는 가정..
			//if (Room != null)
			//	_Hpjob = Room.PushAfter(5000, UpdateHp);
			// 5프레임 (0.2초마다 한번씩 Update)
			if (Room != null)
				_job = Room.PushAfter(200, Update);
		}

		long _nextSearchTick = 0;

		protected virtual void UpdateIdle()
		{
			if (_nextSearchTick > Environment.TickCount64)
				return;
			_nextSearchTick = Environment.TickCount64 + 2000;

		}
	}
}
