using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Stat : UI_Base
{
	enum Texts
	{
		NameText,
		AttackValueText,
		DefenceValueText,
		SpeedValueText,
		SpValueText,
		HpValueText,
		LevelValueText,
		SprecoveryValueText,
		HprecoveryValueText
	}

	bool _init = false;
	public override void Init()
	{
		Bind<Text>(typeof(Texts));

		_init = true;
		RefreshUI();
	}

	public void RefreshUI()
	{
		if (_init == false)
			return;


		// Text
		MyPlayerController player = Managers.Object.MyPlayer;
		player.RefreshAdditionalStat();

		Get<Text>((int)Texts.NameText).text = player.name;

		int totalDamage = player.Stat.Attack + player.WeaponDamage;
		Get<Text>((int)Texts.AttackValueText).text = $"{totalDamage}(+{player.WeaponDamage})";
		Get<Text>((int)Texts.DefenceValueText).text = $"{player.ArmorDefence}";
		Get<Text>((int)Texts.HprecoveryValueText).text = $"{player.Stat.Hprecovery}";
		Get<Text>((int)Texts.SprecoveryValueText).text = $"{player.Stat.Sprecovery}";
		Get<Text>((int)Texts.SpeedValueText).text = $"{player.Speed}";
		Get<Text>((int)Texts.SpValueText).text = $"{player.Stat.Maxsp} ({player.Sp})";
		Get<Text>((int)Texts.HpValueText).text = $"{player.Stat.MaxHp} ({player.Hp})";
		Get<Text>((int)Texts.LevelValueText).text = $"{player.Stat.Level}";
	}
}
