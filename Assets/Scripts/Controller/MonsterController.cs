using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
	Coroutine _coSkill;

	protected override void Init()
	{
		base.Init();
	}

	protected override void UpdateIdle()
	{
		base.UpdateIdle();
	}

	public override void OnDamaged()
	{
		//Managers.Object.Remove(Id);
		//Managers.Resource.Destroy(gameObject);
	}
	public override void OnDead()
	{
		State = CreatureState.Dead;
		if(NameText.text == "좀비")
        {
			GameObject effect = Managers.Resource.Instantiate("Effect/ZombieDieEffect");
			effect.transform.position = transform.position;
			effect.GetComponent<Animator>().Play("vanish");
			GameObject.Destroy(effect, 0.5f);
		}
		else if (NameText.text == "도플갱어")
        {
			GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
			effect.transform.position = transform.position;
			effect.GetComponent<Animator>().Play("vanish");
			GameObject.Destroy(effect, 0.5f);
		}
		else if (NameText.text == "나무괴물")
		{
			GameObject effect = Managers.Resource.Instantiate("Effect/TreeDieEffect");
			effect.transform.position = transform.position;
			effect.GetComponent<Animator>().Play("vanish");
			GameObject.Destroy(effect, 0.5f);
		}
	}

	public override void UseSkill(int skillId)
	{
		if (skillId == 1)
		{
			State = CreatureState.Skill;
		}
	}
}
