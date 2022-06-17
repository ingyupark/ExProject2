using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ArrowController : BaseController
{
	protected override void Init()
	{
		switch (Dir)
		{
			case MoveDir.Up:
				transform.rotation = Quaternion.Euler(0, 0, -90);
				break;
			case MoveDir.Down:
				transform.rotation = Quaternion.Euler(0, 0, 90);
				break;
			case MoveDir.Left:
				transform.rotation = Quaternion.Euler(0, 0, 0);
				break;
			case MoveDir.Right:
				transform.rotation = Quaternion.Euler(0, 0, -180);
				break;
		}

		State = CreatureState.Moving;

		_animator = GetComponent<Animator>();
		_sprite = GetComponent<SpriteRenderer>();
		Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 1f);
		transform.position = pos;
		UpdateAnimation();
	}

	public override void SyncPos()
	{
		Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 1f);
		transform.position = destPos;
	}

	protected override void UpdateMoving()
	{
		Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 1f);
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

	protected override void UpdateAnimation()
	{

	}
}
