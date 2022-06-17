﻿using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{
	private static float offset_x = 0.5f;
	private static float offset_y = 0.3f;

	protected GameObject cover;
	protected GameObject attack;

	public int Id { get; set; }

	StatInfo _stat = new StatInfo();
	public virtual StatInfo Stat
	{
		get { return _stat; }
		set
		{
			if (_stat.Equals(value))
				return;

			_stat.MergeFrom(value);
		}
	}

	public float Speed
	{
		get { return Stat.Speed; }
		set { Stat.Speed = value; }
	}

	public virtual int Hp
	{
		get { return Stat.Hp; }
		set
		{
			Stat.Hp = value;
		}
	}


	protected bool _updated = false;

	PositionInfo _positionInfo = new PositionInfo();
	public PositionInfo PosInfo
	{
		get { return _positionInfo; }
		set
		{
			if (_positionInfo.Equals(value))
				return;

			CellPos = new Vector3Int(value.PosX, value.PosY, 0);
			State = value.State;
			Dir = value.MoveDir;
		}
	}

	public virtual void SyncPos()
	{
		Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(offset_x, offset_y);
		transform.position = destPos;
	}


	public Vector3Int CellPos
	{
		get
		{
			return new Vector3Int(PosInfo.PosX, PosInfo.PosY, 0);
		}

		set
		{
			if (PosInfo.PosX == value.x && PosInfo.PosY == value.y)
				return;

			PosInfo.PosX = value.x;
			PosInfo.PosY = value.y;
			_updated = true;
		}
	}


	protected Animator _animator;
	protected Animator attack_animator;
	protected Animator coverattack_animator;

	protected SpriteRenderer _sprite;
	protected SpriteRenderer attack_sprite;
	protected SpriteRenderer cover_sprite;
	public virtual CreatureState State
	{
		get { return PosInfo.State; }
		set
		{
			if (PosInfo.State == value)
				return;

			PosInfo.State = value;
			UpdateAnimation();
			_updated = true;
		}
	}

	public MoveDir Dir
	{
		get { return PosInfo.MoveDir; }
		set
		{
			if (PosInfo.MoveDir == value)
				return;

			PosInfo.MoveDir = value;

			UpdateAnimation();
			_updated = true;
		}
	}

	public MoveDir GetDirFromVec(Vector3Int dir)
	{
		if (dir.x > 0)
			return MoveDir.Right;
		else if (dir.x < 0)
			return MoveDir.Left;
		else if (dir.y > 0)
			return MoveDir.Up;
		else
			return MoveDir.Down;
	}

	public Vector3Int GetFrontCellPos()
	{
		Vector3Int cellPos = CellPos;

		switch (Dir)
		{
			case MoveDir.Up:
				cellPos += Vector3Int.up;
				break;
			case MoveDir.Down:
				cellPos += Vector3Int.down;
				break;
			case MoveDir.Left:
				cellPos += Vector3Int.left;
				break;
			case MoveDir.Right:
				cellPos += Vector3Int.right;
				break;
		}

		return cellPos;
	}

	protected virtual void UpdateAnimation()
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
					if(cover != null)
						cover.SetActive(false);
					attack.SetActive(false);
					break;
				case MoveDir.Down:
					_animator.Play("idle_down");
					_sprite.flipX = false;
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					attack_sprite.flipX = false;
					if (cover != null)
						cover.SetActive(false);
					attack.SetActive(false);
					break;
				case MoveDir.Left:
					_animator.Play("idle_side");
					_sprite.flipX = false;
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					attack_sprite.flipX = false;
					if (cover != null)
						cover.SetActive(false);
					attack.SetActive(false);
					break;
				case MoveDir.Right:
					_animator.Play("idle_side");
					_sprite.flipX = true;
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					attack_sprite.flipX = true;
					if (cover != null)
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
					_sprite.flipX = false;
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 3;
					attack_sprite.flipX = false;
					if (cover != null)
						cover.SetActive(false);
					attack.SetActive(false);
					break;
				case MoveDir.Down:
					_animator.Play("run_down");
					_sprite.flipX = false;
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					attack_sprite.flipX = false;
					if (cover != null)
						cover.SetActive(false);
					attack.SetActive(false);
					break;
				case MoveDir.Left:
					_animator.Play("run_side");
					_sprite.flipX = false;
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					attack_sprite.flipX = false;
					if (cover != null)
						cover.SetActive(false);
					attack.SetActive(false);
					break;
				case MoveDir.Right:
					_animator.Play("run_side");
					_sprite.flipX = true;
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					attack_sprite.flipX = true;
					if (cover != null)
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
					_animator.Play("sword_up");
					_sprite.flipX = false;
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 3;
					attack_sprite.flipX = false;
					if (cover != null)
						cover.SetActive(false);
					attack.SetActive(true);
					attack_animator.Play("weapon_sword_up");
					break;
				case MoveDir.Down:
					_animator.Play("sword_down");
					_sprite.flipX = false;
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					attack_sprite.flipX = false;
					if (cover != null)
						cover.SetActive(true);
					attack.SetActive(true);
					if(coverattack_animator != null)
						coverattack_animator.Play("sword_down_cover");
					attack_animator.Play("weapon_sword_down");
					break;
				case MoveDir.Left:
					_animator.Play("sword_side");
					_sprite.flipX = false;
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					attack_sprite.flipX = false;
					if (cover != null)
						cover.SetActive(false);
					attack.SetActive(true);
					attack_animator.Play("weapon_sword_side");
					break;
				case MoveDir.Right:
					_animator.Play("sword_side");
					_sprite.flipX = true;
					_sprite.sortingOrder = 4;
					attack_sprite.sortingOrder = 6;
					attack_sprite.flipX = true;
					if (cover != null)
						cover.SetActive(false);
					attack.SetActive(true);
					attack_animator.Play("weapon_sword_side");
					break;
			}
		}
		else
		{

		}
	}

	void Start()
	{
		Init();
	}

	void Update()
	{
		UpdateController();
	}

	protected virtual void Init()
	{
		_animator = GetComponent<Animator>();
		_sprite = GetComponent<SpriteRenderer>();

		cover = gameObject.transform.Find("coverAttack").gameObject;
		attack = gameObject.transform.Find("Attack").gameObject;


		attack_animator = attack.GetComponent<Animator>();
		if(cover != null)
			coverattack_animator = cover.GetComponent<Animator>();

		attack_sprite = attack.GetComponent<SpriteRenderer>();
		cover_sprite = cover.GetComponent<SpriteRenderer>();

		Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(offset_x, offset_y);
        transform.position = pos;
		UpdateAnimation();

	}

	protected virtual void UpdateController()
	{
		switch (State)
		{
			case CreatureState.Idle:
				UpdateIdle();
				break;
			case CreatureState.Moving:
				UpdateMoving();
				break;
			case CreatureState.Skill:
				UpdateSkill();
				break;
			case CreatureState.Dead:
				UpdateDead();
				break;
		}
	}

	protected virtual void UpdateIdle()
	{
	}

	// 스르륵 이동하는 것을 처리
	protected virtual void UpdateMoving()
	{
		Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(offset_x, offset_y);
		Vector3 moveDir = destPos - transform.position;

		// 도착 여부 체크
		float dist = moveDir.magnitude;
		if (dist < Speed * Time.deltaTime)
		{
			transform.position = destPos;
			MoveToNextPos();
		}
		else
		{
			transform.position += moveDir.normalized * Speed * Time.deltaTime;
			State = CreatureState.Moving;
		}
	}

	protected virtual void MoveToNextPos()
	{

	}

	protected virtual void UpdateSkill()
	{

	}

	protected virtual void UpdateDead()
	{

	}
}