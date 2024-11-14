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



	/** 
	 * �÷��̾�� ���� ������ ����
	 * �÷��̾� ������ �� ���� ��� �ʱ�ȭ �ؾ���.
	 *
	 */
	public int LastPlayerLevel = 0;

	public List<ItemBase> SaleList = new();

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

		// ������� ����Ʈ�� ������ ���� â ����

		foreach (int qeustId in Managers.Quest.CurrentQuestList)
		{
			Data.QuestData questData = Managers.Data.QuestDic[qeustId];

			if (questData.QuestType == Define.EQuestType.Interact && questData.QuestTargetId == DataTemplateID)
			{
				var rewardItems = Managers.Quest.GetReward(questData.DataId);
				gamescene.SetRewardItems(rewardItems);
				gamescene.EnableNpcInteraction();

				return;
			}
		}

		// �ƴ� ��� NPC ��ȣ�ۿ� â ����
		gamescene.SetNpcInteraction(this);
	}

	

	public void MakeSaleList()
    {
		

        if (LastPlayerLevel != Managers.Object.Player.Level)
        {
			LastPlayerLevel = Managers.Object.Player.Level;

			for (int i = SaleList.Count - 1; i >= 0; i--)
			{
				ItemBase item = SaleList[i];
				SaleList.RemoveAt(i);
				item.Destroy();
			}
			SaleList.Clear();
			Managers.Inventory.ClearVendorInventory();

			for (int i = 0; i < 5; i++)
			{
				var item = EquipmentItem.MakeRandomEquipmentItem();
				UI_Item uiitem = Managers.Resource.Instantiate("UI_Item").GetComponent<UI_Item>();
				uiitem.SetInfo(item);
				Vector2 pos = Managers.Inventory.GetInventoryEmptyPosition(Define.EEquipSlotType.VendorInventory, item.ItemSize);
				item.EquipSlot = Define.EEquipSlotType.VendorInventory;

				Managers.Inventory.AddItemInInventory(Define.EEquipSlotType.VendorInventory, pos, item);

				SaleList.Add(item);
			}

			List<int> consumbleKeys = new List<int>(Managers.Data.ConsumableItemDic.Keys);

            foreach (var key in consumbleKeys)
            {
				var citem = ConsumableItem.MakeConsumableItem(Managers.Data.ConsumableItemDic[key].DataId, Managers.Data.ConsumableItemDic[key].StackSize);
				UI_Item cuiitem = Managers.Resource.Instantiate("UI_Item").GetComponent<UI_Item>();
				cuiitem.SetInfo(citem);
				citem.EquipSlot = Define.EEquipSlotType.VendorInventory;

				Vector2 cpos = Managers.Inventory.GetInventoryEmptyPosition(Define.EEquipSlotType.VendorInventory, citem.ItemSize);

				Managers.Inventory.AddItemInInventory(Define.EEquipSlotType.VendorInventory, cpos, citem);

				SaleList.Add(citem);
			}

			List<int> skillGemKeys = new List<int>(Managers.Data.SkillGemItemDic.Keys);

			foreach (var key in skillGemKeys)
			{
				var sitem = SkillGemItem.MakeSkillGemItem(Managers.Data.SkillGemItemDic[key].DataId);
				UI_Item suiitem = Managers.Resource.Instantiate("UI_Item").GetComponent<UI_Item>();
				suiitem.SetInfo(sitem);
				sitem.EquipSlot = Define.EEquipSlotType.VendorInventory;

				Vector2 spos = Managers.Inventory.GetInventoryEmptyPosition(Define.EEquipSlotType.VendorInventory, sitem.ItemSize);

				Managers.Inventory.AddItemInInventory(Define.EEquipSlotType.VendorInventory, spos, sitem);

				SaleList.Add(sitem);
			}
			
			List<int> flaskKeys = new List<int>(Managers.Data.FlaskItemBaseDic.Keys);

			foreach (var key in flaskKeys)
			{
				var fitem = FlaskItem.MakeFlaskItem(Managers.Data.FlaskItemBaseDic[key].DataId);
				UI_Item fuiitem = Managers.Resource.Instantiate("UI_Item").GetComponent<UI_Item>();
				fuiitem.SetInfo(fitem);
				fitem.EquipSlot = Define.EEquipSlotType.VendorInventory;

				Vector2 spos = Managers.Inventory.GetInventoryEmptyPosition(Define.EEquipSlotType.VendorInventory, fitem.ItemSize);

				Managers.Inventory.AddItemInInventory(Define.EEquipSlotType.VendorInventory, spos, fitem);

				SaleList.Add(fitem);
			}


			Managers.Inventory.Vendor = this;

		}
        else
        {
			Managers.Inventory.Vendor = this;
            Managers.Inventory.SetVendorSaleList(SaleList);
		}
    }
}
