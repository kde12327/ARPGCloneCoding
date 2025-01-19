using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartingpoint : Env
{

	public override void SetInfo(int templateID)
	{
		base.SetInfo(templateID);

		DataTemplateID = templateID;
		EnvData = Managers.Data.EnvDic[templateID];

		// Stat
		Hp = EnvData.MaxHp;
		MaxHp = EnvData.MaxHp;

		EnvState = Define.EEnvState.Idle;
	}

	public override void Interact(Player player)
	{

	}
}
