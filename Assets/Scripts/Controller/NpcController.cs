using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class NpcController : CreatureController
{
	protected override void Init()
	{
		base.Init();
		if(_hpBar != null)
			_hpBar.gameObject.SetActive(false);
		if(_hpText != null)
			_hpText.gameObject.SetActive(false);
	}

	protected override void UpdateIdle()
	{
		base.UpdateIdle();

	}

	bool isSet = false;
	protected override void UpdateAnimation()
	{
		if (_animator == null || _sprite == null)
			return;
		if (isSet)
			return;
		_animator.Play("idle_down");
		_sprite.flipX = false;
		_sprite.sortingOrder = 4;
		attack_sprite.sortingOrder = 6;
		attack_sprite.flipX = false;
		cover.SetActive(false);
		attack.SetActive(false);
		isSet = true;
	}

}
