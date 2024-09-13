using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INpcInteraction
{
	public void SetInfo(Npc owner);
	public void HandleOnClickEvent();
	public bool CanInteract();
}

public class Npc : InteractableObject
{
    public Data.NpcData NpcData { get; protected set; }

    private Define.ENpcState _npcState = Define.ENpcState.Idle;

	public List<ItemBase> VendorList { get; set; } = new();

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		ObjectType = Define.EObjectType.Npc;

		return true;
	}

	public virtual void SetInfo(int templateID)
	{
		Collider.isTrigger = true;

		DataTemplateID = templateID;
		NpcData = Managers.Data.NpcDic[templateID];


		SetSpineAnimation(NpcData.SkeletonDataID, SortingLayers.NPC);

		GameObject nameTagObject = Managers.Resource.Instantiate("NameTag");

		NameTag nameTag = nameTagObject.GetComponent<NameTag>();
		nameTag.SetInfo(this, NpcData.DescriptionTextID);
	}

	public override void Interact(Player player)
    {
        base.Interact(player);

        Debug.Log("Interact Npc");

		var gamescene = Managers.UI.GetSceneUI<UI_GameScene>();
		gamescene.SetNpcInteraction(this);

	}
}
