using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class CreatureController : BaseController
{
	protected HpBar _hpBar;
	public HpText _hpText;
	[SerializeField]
	public Text NameText;

	public override StatInfo Stat
	{
		get { return base.Stat; }
		set { base.Stat = value; UpdateHpBar(); }
	}

	public override int Hp
	{
		get { return Stat.Hp; }
		set { base.Hp = value; UpdateHpBar();  }
	}


	protected void AddHpBar()
	{
		GameObject go = Managers.Resource.Instantiate("UI/HpBar", transform);
		go.transform.localPosition = new Vector3(0, 1.5f, 0);
		go.name = "HpBar";
		_hpBar = go.GetComponent<HpBar>();
		UpdateHpBar();
	}
	public void missText()
    {
		GameObject go = Managers.Resource.Instantiate("UI/HpText", transform);
		go.transform.localPosition = new Vector3(0, 1.8f, 0);
		go.name = "HpText";
		_hpText = go.GetComponent<HpText>();
		_hpText.MissText();
		Destroy(go, 1f);
	}


	public void AddText(int newText = 0, int oldText = 0)
    {
		GameObject go = Managers.Resource.Instantiate("UI/HpText", transform);
		go.transform.localPosition = new Vector3(0, 1.8f, 0);
		go.name = "HpText";
		_hpText = go.GetComponent<HpText>();
		_hpText.SetHpText(newText, oldText);
		Destroy(go,1f);
	}

	

	public void UpdateHpBar()
	{
		if (_hpBar == null)
			return;

		float ratio = 0.0f;
		if (Stat.MaxHp > 0)
			ratio = ((float)Hp) / Stat.MaxHp;

		_hpBar.SetHpBar(ratio);
	}

	protected override void Init()
	{
		base.Init();
		AddHpBar();
	}
	public override void SyncPos()
	{
		base.SyncPos();
	}

	public virtual void OnDamaged()
	{

	}

	public virtual void OnDead()
	{
		State = CreatureState.Dead;

		GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
		effect.transform.position = transform.position;
		effect.GetComponent<Animator>().Play("vanish");
		GameObject.Destroy(effect, 0.5f);
	}

	public virtual void UseSkill(int skillId)
	{

	}
}
