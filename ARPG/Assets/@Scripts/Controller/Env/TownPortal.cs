using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownPortal : InteractableObject
{

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		ObjectType = Define.EObjectType.Env;
		Collider.isTrigger = true;

		return true;
	}

	public override void Interact(Player player)
	{
		base.Interact(player);

		Managers.Scene.CreateOrLoadGameSceneByName("ACT01_Town_Map");
	}
}
