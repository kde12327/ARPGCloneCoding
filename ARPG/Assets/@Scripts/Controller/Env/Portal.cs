using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Env
{
    public string NextStageName { get; set; }

	public PortalData PortalData { get; protected set; }

	public override void SetInfo(int templateID)
	{
		base.SetInfo(templateID);

		DataTemplateID = templateID;
		EnvData = Managers.Data.PortalDic[templateID];
		PortalData = Managers.Data.PortalDic[templateID];

		// Stat
		Hp = EnvData.MaxHp;
		MaxHp = EnvData.MaxHp;

		EnvState = Define.EEnvState.Idle;

		GameObject nameTagObject = Managers.Resource.Instantiate("NameTag");
		NameTag nameTag = nameTagObject.GetComponent<NameTag>();
		nameTag.SetInfo(this, PortalData.DescriptionTextID);
		nameTag.transform.localPosition = new Vector2(0.2f, 2.5f); ;
	}

	public override void Interact(Player player)
    {
        base.Interact(player);

		PortalData NextPortalData = Managers.Data.PortalDic[PortalData.DestPortalId];
		player.MapArriveId = PortalData.DestPortalId;

		Managers.Scene.CreateOrLoadGameSceneByName(NextPortalData.MapName);
	}
}
