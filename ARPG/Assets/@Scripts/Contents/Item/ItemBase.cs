using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ItemBase
{

	public ItemData ItemData;
	public EItemType ItemType;
	public EItemSubType ItemSubType { get; private set; }

	public bool IsDiscriptionDirty { get; set; }

	public ItemSaveData SaveData { get; set; }

	public UI_Item UIItem { get; set; }

	public ERarity Rarity { get; set; }

	public int InstanceId
	{
		get { return SaveData.InstanceId; }
		set { SaveData.InstanceId = value; }
	}

	public long DbId
	{
		get { return SaveData.DbId; }
	}

	public int TemplateId
	{
		get { return SaveData.TemplateId; }
		set { SaveData.TemplateId = value; }
	}

	public int Count
	{
		get { return SaveData.Count; }
		set { SaveData.Count = value; }
	}

	public int EquipSlot
	{
		get { return SaveData.EquipSlot; }
		set { SaveData.EquipSlot = value; }
	}

	public Vector2Int EquipPos
    {
		get; set;
    }

	public Vector2Int ItemSize
    {
        get 
		{ 
			switch(ItemData.ItemSubType)
            {
				case EItemSubType.Weapon:
					return new Vector2Int(1, 3);
				case EItemSubType.Helmet:
					return new Vector2Int(2, 2);
				case EItemSubType.Gloves:
					return new Vector2Int(2, 2);
				case EItemSubType.Boots:
					return new Vector2Int(2, 2);
				case EItemSubType.BodyArmour:
					return new Vector2Int(2, 3);

				default:
					return new Vector2Int(1, 1);
			}
		}
    }

	public ItemBase(int itemDataId)
	{
		ItemData = Managers.Data.ItemDic[itemDataId];
		ItemType = ItemData.ItemType;
		ItemSubType = ItemData.ItemSubType;
		Init();
	}

	public virtual bool Init()
	{
		return true;
	}


	public static ItemBase MakeItem(ItemSaveData itemInfo)
    {
		if (Managers.Data.ItemDic.TryGetValue(itemInfo.TemplateId, out ItemData itemData) == false)
			return null;

		ItemBase item = null;

		switch (itemData.ItemType)
		{
			case EItemType.Equipment:
				item = new EquipmentItem(itemInfo.TemplateId);
				break;
			case EItemType.Consumable:
				item = new ConsumableItem(itemInfo.TemplateId);
				break;
			case EItemType.SkillGem:
				item = new SkillGemItem(itemInfo.TemplateId);
				break;
		}

		if (item != null)
		{
			item.SaveData = itemInfo;
			item.InstanceId = itemInfo.InstanceId;
			item.Count = itemInfo.Count;
		}

		return item;
	}

	public void Destroy()
    {
		if (UIItem != null)
		{
			Managers.Resource.Destroy(UIItem.gameObject);
		}
	}

	~ItemBase()
	{
		if (UIItem != null)
		{
			Managers.Resource.Destroy(UIItem.gameObject);
		}
	}

	public virtual bool UpgradeRarity(Define.ERarity curRarity, Define.ERarity destRarity)
    {
		if(curRarity != Rarity)
        {
			return false;
        }

		return HasRarity();
    }

	#region Helpers

	public bool HasRarity()
    {
        switch (ItemType)
        {
			case EItemType.Equipment:
				return true;
			case EItemType.Consumable:
				return false;
			default:
				return false;
        }
    }

	public bool IsEquippable()
	{
		return GetEquipItemEquipSlot() != EEquipSlotType.None;
	}

	public EEquipSlotType GetEquipItemEquipSlot()
	{
		

		return (EEquipSlotType)this.ItemData.ItemSubType;

	}

	public bool IsEquippedItem()
	{
		
		return this.EquipSlot > (int)EEquipSlotType.None && this.EquipSlot < (int)EEquipSlotType.EquipMax;

	}

	public bool IsInInventory()
	{
		return this.EquipSlot == (int)EEquipSlotType.PlayerInventory;

	}

	public bool IsInWarehouse()
	{
		return this.EquipSlot == (int)EEquipSlotType.WarehouseInventory;
	}
	
	public bool IsInVendor()
	{
		return this.EquipSlot == (int)EEquipSlotType.VendorInventory;
	}
	#endregion
}



public class Option
{
	public ModData Data { get; set; }
	public string Stat { get; set; }
	public float Value { get; set; }

	public Option(ModData data, string stat, float value)
	{
		Data = data;
		Stat = stat;
		Value = value;

	}
}


public class Modifier
{
	public List<Option> Options = new();

	public ModData data;

	public static Modifier MakeModifier(int modDataId)
	{
		Modifier mod = new();

		var data = Managers.Data.ModDic[modDataId];
		mod.data = data;

		if (data == null) return null;

		for (int i = 0; i < data.Stats.Count; i++)
		{
			string stat = data.Stats[i];
			float min = data.MinMaxValues[i];
			float max = data.MinMaxValues[i + 1];

			float value = Random.Range(min, max);

			mod.AddOption(data, stat, value);
		}


		return mod;

	}
	public static Modifier MakeModifier(List<string> stats, List<float> minmax)
	{
		if (stats == null)
			return null;

		Modifier mod = new();

		for (int i = 0; i < stats.Count; i++)
		{
			string stat = stats[i];
			float min = minmax[i];
			float max = minmax[i + 1];

			float value = Random.Range((int)min, (int)max);

			mod.AddOption(null, stat, value);
		}

		return mod;

	}

	public void AddOption(ModData data, string stat, float value)
	{
		Options.Add(new Option(data, stat, value));
	}

}