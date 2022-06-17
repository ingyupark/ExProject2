using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MyPlayerController : PlayerController
{
	private Camera MiniMap;
	bool _moveKeyPressed = false;
	public bool isBuy = false;
	public bool isAlarm = false;


	public int WeaponDamage { get; private set; }
	public int ArmorDefence { get; private set; }

	SpBar _spBar;

	public override StatInfo Stat
	{
		get { return base.Stat; }
		set { base.Stat = value; UpdateHpBar(); UpdateSpBar(); }
	}

	public override int Sp
	{
		get { return Stat.Sp; }
		set { Stat.Sp = value; UpdateSpBar(); }
	}

	protected void AddSpBar()
	{
		GameObject go = Managers.Resource.Instantiate("UI/SpBar", transform);
		go.transform.localPosition = new Vector3(0, 1.4f, 0);
		go.name = "SpBar";
		_spBar = go.GetComponent<SpBar>();
		UpdateSpBar();
	}
	public void UpdateSpBar()
	{
		if (_spBar == null)
			return;

		float ratio = 0.0f;
		if (Stat.Maxsp > 0)
			ratio = ((float)Sp) / Stat.Maxsp;

		_spBar.SetSpBar(ratio);
	}

	protected override void Init()
	{
		base.Init();
		RefreshAdditionalStat();
		AddSpBar();
		MiniMap = GameObject.Find("MiniMapCamera").GetComponent<Camera>();
	}
	protected override void UpdateController()
	{
		GetUIKeyInput();
		GetSlotInput();

		switch (State)
		{
			case CreatureState.Idle:
				GetDirInput();
				break;
			case CreatureState.Moving:
				GetDirInput();
				break;
		}

		base.UpdateController();
	}

	protected override void UpdateIdle()
	{
		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		
        UI_Chat chatUI = gameSceneUI.chatUI;

		if (chatUI.gameObject.activeSelf)
			return;

		// 이동 상태로 갈지 확인
		if (_moveKeyPressed)
		{
			State = CreatureState.Moving;
			return;
		}

		if (_coSkillCooltime == null && Input.GetKey(KeyCode.Space))
		{
			if (isBuy)
				return;

			C_Skill skill = new C_Skill() { Info = new SkillInfo() };
			// 스킬 아이디 설정 요기서 할게 아니라 아이템쪽에서 하자..
			skill.Info.SkillId = P_Skill;
			Managers.Network.Send(skill);

			_coSkillCooltime = StartCoroutine("CoInputCooltime", 0.2f);
		}

		if (_coSkilloneCooltime == null && Input.GetKey(KeyCode.Z))
		{
			if (isBuy)
				return;

			C_Skill skill = new C_Skill() { Info = new SkillInfo() };
			// 스킬 아이디 설정 요기서 할게 아니라 아이템쪽에서 하자..
			skill.Info.SkillId = 5;
			Managers.Network.Send(skill);
			SkillEffectAnimation();
			_coSkilloneCooltime = StartCoroutine("CoInputoneCooltime", 5.0f);
		}
	}

	Coroutine _coSkillCooltime;
	IEnumerator CoInputCooltime(float time)
	{
		yield return new WaitForSeconds(time);
		_coSkillCooltime = null;
	}



	void LateUpdate()
	{
		Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
		MiniMap.transform.position = new Vector3(transform.position.x, transform.position.y, -376.8492f);
	}
	public bool isDelay;
	public float delatTime = 1.0f;
	public float accumTime;

	IEnumerator Drink()
    {
		yield return new WaitForSeconds(delatTime);
		isDelay = false;
    }
	// 키보드 입력 (물약 먹는거..)
	void GetSlotInput()
    {
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{

			UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
			UI_Chat chatUI = gameSceneUI.chatUI;
			if (chatUI.gameObject.activeSelf)
				return;

			//if (!isDelay)
   //         {
			//	isDelay = true;
			//	//this.Hp += 10;?
			//	// HP 의 패킷을 바로 보낼게 아니라.. 먹엇다는거만 보낸다..
			//	// 먹엇다는 상태 패킷을 받은 서버에서 HP량을 조절 하고 HP량 업데이트를 패킷으로 보낸다.
			//	//Delay

			//	StartCoroutine(Drink());

			//}
			//else
   //         {

			//}
		}
	}

	void GetUIKeyInput()
	{
		if (Input.GetKeyDown(KeyCode.I))
		{
			UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
			UI_Chat chatUI = gameSceneUI.chatUI;
			if (chatUI.gameObject.activeSelf)
				return;

			UI_Inventory invenUI = gameSceneUI.InvenUI;
			if (invenUI.gameObject.activeSelf)
			{
				invenUI.gameObject.SetActive(false);
				invenUI.RefreshUI();
			}
			else
			{
				invenUI.gameObject.SetActive(true);
				invenUI.RefreshUI();
			}
		}
		else if (Input.GetKeyDown(KeyCode.C))
		{
			UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
			UI_Chat chatUI = gameSceneUI.chatUI;
			if (chatUI.gameObject.activeSelf)
				return;

			UI_Stat statUI = gameSceneUI.StatUI;

			if (statUI.gameObject.activeSelf)
			{
				statUI.gameObject.SetActive(false);
			}
			else
			{
				statUI.gameObject.SetActive(true);
				gameSceneUI.InvenUI.RefreshUI();
			}
		}
		else if (Input.GetKeyDown(KeyCode.RightShift))
		{
			UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
			UI_Chat chatUI = gameSceneUI.chatUI;

			if (chatUI.gameObject.activeSelf)
			{
				gameSceneUI.mainUI.RefreshUI();
				chatUI.gameObject.SetActive(false);
			}
			else
			{
				chatUI.gameObject.SetActive(true);
				gameSceneUI.mainUI.RefreshUI();
				chatUI.input.ActivateInputField();
			}
		}
	}

	// 키보드 입력
	void GetDirInput()
	{
		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		UI_Chat chatUI = gameSceneUI.chatUI;

		if (chatUI.gameObject.activeSelf)
        {
			_moveKeyPressed = false;
			return;
		}
		if (isBuy)
		{
			_moveKeyPressed = false;
			return;
		}

		_moveKeyPressed = true;

		if (Input.GetKey(KeyCode.W))
		{
			Dir = MoveDir.Up;
		}
		else if (Input.GetKey(KeyCode.S))
		{
			Dir = MoveDir.Down;
		}
		else if (Input.GetKey(KeyCode.A))
		{
			Dir = MoveDir.Left;
		}
		else if (Input.GetKey(KeyCode.D))
		{
			Dir = MoveDir.Right;
		}
		else
		{
			_moveKeyPressed = false;
		}
	}

	Coroutine _coSkilloneCooltime;
	IEnumerator CoInputoneCooltime(float time)
	{
		yield return new WaitForSeconds(time);
		_coSkilloneCooltime = null;
	}

	public void GetSkillOne()
    {
		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		UI_Chat chatUI = gameSceneUI.chatUI;
		if (chatUI.gameObject.activeSelf)
			return;

		if (_coSkilloneCooltime == null && Input.GetKey(KeyCode.Z))
		{
			if (isBuy)
				return;

			C_Skill skill = new C_Skill() { Info = new SkillInfo() };
			// 스킬 아이디 설정 요기서 할게 아니라 아이템쪽에서 하자..
			skill.Info.SkillId = P_Skill;
			Managers.Network.Send(skill);

			_coSkilloneCooltime = StartCoroutine("CoInputoneCooltime", 5.0f);
		}
	}

	protected override void MoveToNextPos()
	{
		if (_moveKeyPressed == false)
		{
			State = CreatureState.Idle;
			CheckUpdatedFlag();
			return;
		}

		Vector3Int destPos = CellPos;

		switch (Dir)
		{
			case MoveDir.Up:
				destPos += Vector3Int.up;
				break;
			case MoveDir.Down:
				destPos += Vector3Int.down;
				break;
			case MoveDir.Left:
				destPos += Vector3Int.left;
				break;
			case MoveDir.Right:
				destPos += Vector3Int.right;
				break;
		}

		if (Managers.Map.potalGo(destPos))
		{
			Managers.Map.LoadMap(2);
			C_Potal potalPacket = new C_Potal();
			potalPacket.Roomid = 2;
			Managers.Network.Send(potalPacket);
		}

		if (Managers.Map.potal2Go(destPos))
		{
			Managers.Map.LoadMap(1);
			C_Potal potalPacket = new C_Potal();
			potalPacket.Roomid = 1;
			Managers.Network.Send(potalPacket);
		}

		if (Managers.Map.CanGo(destPos))
		{
			if (Managers.Object.FindCreature(destPos) == null)
			{
				CellPos = destPos;
			}
		}

		

		CheckUpdatedFlag();
	}

	protected override void CheckUpdatedFlag()
	{
		if (_updated)
		{
			C_Move movePacket = new C_Move();
			movePacket.PosInfo = PosInfo;
			Managers.Network.Send(movePacket);
			_updated = false;
		}
	}

	public void RefreshAdditionalStat()
	{
		WeaponDamage = 0;
		ArmorDefence = 0;

		foreach (Item item in Managers.Inven.Items.Values)
		{
			if (item.Equipped == false)
				continue;

			switch (item.ItemType)
			{
				case ItemType.Weapon:
					WeaponDamage += ((Weapon)item).Damage;
					break;
				case ItemType.Armor:
					ArmorDefence += ((Armor)item).Defence;
					break;
			}
		}
	}
}
