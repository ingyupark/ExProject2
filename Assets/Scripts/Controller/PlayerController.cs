using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
	[SerializeField]
	public GameObject gunEffect;
	protected Coroutine _coSkill;
	protected bool _rangedSkill = false;

	protected GameObject _skilleffect;

	protected enum N_rangedSkill
    {
		Sword,Bow,Gun,Sickle
	}

	protected N_rangedSkill n_Ranged = N_rangedSkill.Sword;

	public int P_Skill;

	public virtual int Sp
	{
		get { return Stat.Sp; }
		set { Stat.Sp = value; }
	}

	protected override void Init()
	{
		base.Init();

		if (_SkillEffect == null)
			_SkillEffect = gameObject.transform.Find("SkillEffect").gameObject;

		if (_SkillAnim == null)
			_SkillAnim = _SkillEffect.GetComponent<Animator>();

		_Skillsprte = _SkillEffect.GetComponent<SpriteRenderer>();

		_SkillEffect.SetActive(false);

	}

	GameObject _SkillEffect = null;
	Animator _SkillAnim = null;
	SpriteRenderer _Skillsprte = null;

	IEnumerator CoInputCooltimex(float time)
	{
		yield return new WaitForSeconds(time);

		_SkillAnim.Play("electric");
		_SkillEffect.SetActive(false);
	}

	protected void SkillEffectAnimation()
    {
		if(_SkillEffect == null)
			_SkillEffect = gameObject.transform.Find("SkillEffect").gameObject;

		if (_SkillAnim == null)
			_SkillAnim = _SkillEffect.GetComponent<Animator>();

		_SkillAnim.Play("electric");
		_Skillsprte.sortingOrder = 1;
		_SkillEffect.SetActive(true);

		StartCoroutine("CoInputCooltimex", 0.5f);

	}

	protected override void UpdateAnimation()
	{
		if (_animator == null || _sprite == null)
			return;

		if (State == CreatureState.Idle)
		{
			switch (Dir)
			{
				case MoveDir.Up:
					_animator.Play("idle_up");
					_sprite.flipX = false;
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 3;
					attack_sprite.flipX = false;
					cover_sprite.flipX = false;
					cover.SetActive(false);
					attack.SetActive(false);
					break;
				case MoveDir.Down:
					_animator.Play("idle_down");
					_sprite.flipX = false;
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					attack_sprite.flipX = false;
					cover_sprite.flipX = false;
					cover.SetActive(false);
					attack.SetActive(false);
					break;
				case MoveDir.Left:
					_animator.Play("idle_side");
					_sprite.flipX = false;
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					attack_sprite.flipX = false;
					cover_sprite.flipX = false;
					cover.SetActive(false);
					attack.SetActive(false);
					break;
				case MoveDir.Right:
					_animator.Play("idle_side");
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					_sprite.flipX = true;
					attack_sprite.flipX = true;
					cover_sprite.flipX = true;

					cover.SetActive(false);
					attack.SetActive(false);
					break;
			}
		}
		else if (State == CreatureState.Moving)
		{
			switch (Dir)
			{
				case MoveDir.Up:
					_animator.Play("run_up");
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 3;
					_sprite.flipX = false;
					cover_sprite.flipX = false;
					attack_sprite.flipX = false;
					cover.SetActive(false);
					attack.SetActive(false);
					break;
				case MoveDir.Down:
					_animator.Play("run_down");
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					_sprite.flipX = false;
					cover_sprite.flipX = false;
					attack_sprite.flipX = false;
					cover.SetActive(false);
					attack.SetActive(false);
					break;
				case MoveDir.Left:
					_animator.Play("run_side");
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					_sprite.flipX = false;
					cover_sprite.flipX = false;
					attack_sprite.flipX = false;
					cover.SetActive(false);
					attack.SetActive(false);
					break;
				case MoveDir.Right:
					_animator.Play("run_side");
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					cover_sprite.flipX = true;
					_sprite.flipX = true;
					attack_sprite.flipX = true;
					cover.SetActive(false);
					attack.SetActive(false);
					break;
			}
		}
		else if (State == CreatureState.Skill)
		{
			switch (Dir)
			{
				case MoveDir.Up:
					_animator.Play(SkillType_animator(n_Ranged));
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 3;
					_sprite.flipX = false;
					cover_sprite.flipX = false;
					attack_sprite.flipX = false;
					cover.SetActive(false);
					attack.SetActive(true);
					attack_animator.Play(SkillType_attack_animator(n_Ranged));
					break;
				case MoveDir.Down:
					_animator.Play(SkillType_animator(n_Ranged));
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					cover_sprite.flipX = false;
					_sprite.flipX = false;
					attack_sprite.flipX = false;
					cover.SetActive(true);
					attack.SetActive(true);
					attack_animator.Play(SkillType_attack_animator(n_Ranged));
					if(SkillType_coverattack_animator(n_Ranged) != null)
						coverattack_animator.Play(SkillType_coverattack_animator(n_Ranged));
					else
						cover.SetActive(false);
					break;
				case MoveDir.Left:
					_animator.Play(SkillType_animator(n_Ranged));
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					cover_sprite.flipX = false;
					_sprite.flipX = false;
					attack_sprite.flipX = false;
					cover.SetActive(true);
					if (SkillType_coverattack_animator(n_Ranged) != null)
						coverattack_animator.Play(SkillType_coverattack_animator(n_Ranged));
					else
						cover.SetActive(false);
					attack.SetActive(true);
					attack_animator.Play(SkillType_attack_animator(n_Ranged));
					break;
				case MoveDir.Right:
					_animator.Play(SkillType_animator(n_Ranged));
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					cover_sprite.flipX = true;
					_sprite.flipX = true;
					attack_sprite.flipX = true;
					cover.SetActive(true);
					if (SkillType_coverattack_animator(n_Ranged) != null)
						coverattack_animator.Play(SkillType_coverattack_animator(n_Ranged));
					else
						cover.SetActive(false);
					attack.SetActive(true);
					attack_animator.Play(SkillType_attack_animator(n_Ranged));
					break;
			}
		}
		else
		{

		}
	}

	protected string SkillType_animator(N_rangedSkill skill)
    {
		switch(skill)
        {
			case N_rangedSkill.Sword:
				if (Dir == MoveDir.Up)
					return "sword_up";
				else if (Dir == MoveDir.Down)
					return "sword_down";
				else if (Dir == MoveDir.Left)
					return "sword_side";
				else if (Dir == MoveDir.Right)
					return "sword_side";
				else
					return null;
			case N_rangedSkill.Bow:
				if (Dir == MoveDir.Up)
					return "bow_up";
				else if (Dir == MoveDir.Down)
					return "bow_down";
				else if (Dir == MoveDir.Left)
					return "bow_side";
				else if (Dir == MoveDir.Right)
					return "bow_side";
				else
					return null;
			case N_rangedSkill.Gun:
				if (Dir == MoveDir.Up)
					return "gun_up";
				else if (Dir == MoveDir.Down)
					return "gun_down";
				else if (Dir == MoveDir.Left)
					return "gun_side";
				else if (Dir == MoveDir.Right)
					return "gun_side";
				else
					return null;
			case N_rangedSkill.Sickle:
				if (Dir == MoveDir.Up)
					return "sycthe_up";
				else if (Dir == MoveDir.Down)
					return "sycthe_down";
				else if (Dir == MoveDir.Left)
					return "sycthe_side";
				else if (Dir == MoveDir.Right)
					return "sycthe_side";
				else
					return null;
		}
		return null;
    }

	protected string SkillType_attack_animator(N_rangedSkill skill)
	{
		switch (skill)
		{
			case N_rangedSkill.Sword:
				gunEffect.SetActive(false);
				if (Dir == MoveDir.Up)
					return "weapon_sword_up";
				else if (Dir == MoveDir.Down)
					return "weapon_sword_down";
				else if (Dir == MoveDir.Left)
					return "weapon_sword_side";
				else if (Dir == MoveDir.Right)
					return "weapon_sword_side";
				else
					return null;
			case N_rangedSkill.Bow:
				gunEffect.SetActive(false);
				if (Dir == MoveDir.Up)
					return "weapon_bow_up";
				else if (Dir == MoveDir.Down)
					return "weapon_bow_down";
				else if (Dir == MoveDir.Left)
					return "weapon_bow_side";
				else if (Dir == MoveDir.Right)
					return "weapon_bow_side";
				else
					return null;
			case N_rangedSkill.Gun:
				gunEffect.SetActive(true);
				if (Dir == MoveDir.Up)
				{
					gunEffect.GetComponent<SpriteRenderer>().sortingOrder = 4;
					gunEffect.GetComponent<SpriteRenderer>().flipX = false;
					gunEffect.transform.localRotation = Quaternion.Euler(0f,0f,-90f);
					gunEffect.transform.localPosition = new Vector3(0f, 1.7f, 0f);
					return "weapon_gun_up";
				}
				else if (Dir == MoveDir.Down)
				{
					gunEffect.GetComponent<SpriteRenderer>().sortingOrder = 7;
					gunEffect.GetComponent<SpriteRenderer>().flipX = false;
					gunEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
					gunEffect.transform.localPosition = new Vector3(0f, 0f, 0f);
					return "weapon_gun_down";
				}
				else if (Dir == MoveDir.Left)
				{
					gunEffect.GetComponent<SpriteRenderer>().sortingOrder = 7;
					gunEffect.GetComponent<SpriteRenderer>().flipX = false;
					gunEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
					gunEffect.transform.localPosition = new Vector3(-1.0f, 0.5f, 0f);
					return "weapon_gun_side";
				}
				else if (Dir == MoveDir.Right)
				{
					gunEffect.GetComponent<SpriteRenderer>().sortingOrder = 7;
					gunEffect.GetComponent<SpriteRenderer>().flipX = true;
					gunEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
					gunEffect.transform.localPosition = new Vector3(1.0f, 0.5f, 0f);
					return "weapon_gun_side";
				}
				else
				{
					gunEffect.SetActive(false);
					return null;
				}
			case N_rangedSkill.Sickle:
				gunEffect.SetActive(false);
				if (Dir == MoveDir.Up)
					return "weapon_sycthe_up";
				else if (Dir == MoveDir.Down)
					return "weapon_sycthe_down";
				else if (Dir == MoveDir.Left)
					return "weapon_sycthe_side";
				else if (Dir == MoveDir.Right)
					return "weapon_sycthe_side";
				else
					return null;
		}
		return null;
	}


	protected string SkillType_coverattack_animator(N_rangedSkill skill)
	{
		switch (skill)
		{
			case N_rangedSkill.Sword:
				if (Dir == MoveDir.Up)
					return null;
				else if (Dir == MoveDir.Down)
					return "sword_down_cover";
				else if (Dir == MoveDir.Left)
					return null;
				else if (Dir == MoveDir.Right)
					return null;
				else
					return null;
			case N_rangedSkill.Bow:
				if (Dir == MoveDir.Up)
					return null;
				else if (Dir == MoveDir.Down)
					return "bow_down_cover";
				else if (Dir == MoveDir.Left)
					return null;
				else if (Dir == MoveDir.Right)
					return null;
				else
					return null;
			case N_rangedSkill.Gun:
				if (Dir == MoveDir.Up)
					return null;
				else if (Dir == MoveDir.Down)
					return null;
				else if (Dir == MoveDir.Left)
					return "gun_side_cover";
				else if (Dir == MoveDir.Right)
					return "gun_side_cover";
				else
					return null;
			case N_rangedSkill.Sickle:
				if (Dir == MoveDir.Up)
					return null;
				else if (Dir == MoveDir.Down)
					return "sycthe_down_cover";
				else if (Dir == MoveDir.Left)
					return null;
				else if (Dir == MoveDir.Right)
					return null;
				else
					return null;
		}
		return null;
	}

	protected override void UpdateController()
	{		
		base.UpdateController();
	}

	public override void UseSkill(int skillId)
	{
		if (skillId == 1)
		{
			_coSkill = StartCoroutine("CoStartPunch");
		}
		else if (skillId == 2)
		{
			_coSkill = StartCoroutine("CoStartShootArrow");
		}
		else if (skillId == 3)
		{
			_coSkill = StartCoroutine("CoStartShootGun");
		}
		else if (skillId == 4)
		{
			_coSkill = StartCoroutine("SickleCoStartPunch");
		}
	}

	protected virtual void CheckUpdatedFlag()
	{

	}

	IEnumerator CoStartPunch()
	{
		// 대기 시간
		_rangedSkill = false;
		n_Ranged = N_rangedSkill.Sword;
		State = CreatureState.Skill;
		yield return new WaitForSeconds(0.5f);
		State = CreatureState.Idle;
		_coSkill = null;
		CheckUpdatedFlag();
	}
	IEnumerator SickleCoStartPunch()
	{
		// 대기 시간
		_rangedSkill = false;
		n_Ranged = N_rangedSkill.Sickle;
		State = CreatureState.Skill;
		yield return new WaitForSeconds(0.5f);
		State = CreatureState.Idle;
		_coSkill = null;
		CheckUpdatedFlag();
	}

	IEnumerator CoStartShootArrow()
	{
		// 대기 시간
		_rangedSkill = true;
		n_Ranged = N_rangedSkill.Bow;
		State = CreatureState.Skill;
		yield return new WaitForSeconds(0.3f);
		State = CreatureState.Idle;
		_coSkill = null;
		CheckUpdatedFlag();
	}
	IEnumerator CoStartShootGun()
	{
		// 대기 시간
		_rangedSkill = true;
		n_Ranged = N_rangedSkill.Gun;
		State = CreatureState.Skill;
		yield return new WaitForSeconds(0.3f);
		State = CreatureState.Idle;
		_coSkill = null;
		CheckUpdatedFlag();
	}

	public override void OnDamaged()
	{
		Debug.Log("Player HIT !");
	}
}
