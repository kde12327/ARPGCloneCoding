using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : Env
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

		GameObject nameTagObject = Managers.Resource.Instantiate("NameTag");
		NameTag nameTag = nameTagObject.GetComponent<NameTag>();
		nameTag.SetInfo(this, "웨이포인트");
	}

	public override void Interact(Player player)
	{
		base.Interact(player);

		Debug.Log("Waypoint");


		Managers.Map.ActivateWaypoint(Managers.Map.MapName);

		var gamescene = Managers.UI.GetSceneUI<UI_GameScene>();
		gamescene.SetActiveWaypointView(true);
	}
}