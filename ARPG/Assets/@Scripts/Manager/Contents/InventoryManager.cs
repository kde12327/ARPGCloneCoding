using Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class InventoryManager
{

	public readonly int DEFAULT_INVENTORY_SLOT_COUNT = 30;

	public List<ItemBase> AllItems { get; } = new List<ItemBase>();

	// Cache
	public Dictionary<int /*EquipSlot*/, ItemBase> EquippedItems = new Dictionary<int, ItemBase>(); // 장비 인벤
	List<ItemBase> InventoryItems = new List<ItemBase>(); // 가방 인벤
	List<ItemBase> WarehouseItems = new List<ItemBase>(); // 창고

	int[,] InventoryItemGrid = new int[12, 5];

	UI_Item _holdingItem;
	public UI_Item HoldingItem 
	{ 
		get { return _holdingItem; } 
		set 
		{
			_holdingItem?.ItemLetGo();
			_holdingItem = value;
			if(_holdingItem != null)
            {
				_holdingItem.ItemHold();
				_holdingItem.transform.SetParent(Managers.UI.GetSceneUI<UI_GameScene>().transform);
			}
		} 
	}

	public bool PickUpItem(ItemBase item)
    {
		UI_Item uiitem = Managers.Resource.Instantiate("UI_Item").GetComponent<UI_Item>();
		uiitem.SetInfo(item);

		if(item.ItemType == EItemType.Equipment)
        {
			ItemBase equippedItem = null;
			switch(item.ItemSubType)
            {
				case EItemSubType.Weapon:
					equippedItem = GetEquippedItem(EEquipSlotType.Weapon);
					break;
				case EItemSubType.Helmet:
					equippedItem = GetEquippedItem(EEquipSlotType.Helmet);
					break;
				case EItemSubType.Gloves:
					equippedItem = GetEquippedItem(EEquipSlotType.Gloves);
					break;
				case EItemSubType.Boots:
					equippedItem = GetEquippedItem(EEquipSlotType.Boots);
					break;
				case EItemSubType.BodyArmour:
					equippedItem = GetEquippedItem(EEquipSlotType.BodyArmour);
					break;
			}

			if(equippedItem == null)
            {
				// 장비 비었을 때 장착
				EquipItem(uiitem.Item, uiitem.Item.GetEquipItemEquipSlot());
            }
            else
            {
				//인벤토리에 들어가기
            }
			
        }

		return true;
	}

	public SlotState GetCellList(Vector2 pos, out List<Vector2Int> list)
    {

		list = new();


		if (HoldingItem == null)
        {
			int id = InventoryItemGrid[(int)pos.x, (int)pos.y];

			if (id != 0)
			{
				var item = GetItem(id);

				Vector2Int equipPos = item.EquipPos;
				Vector2Int itemSize = item.ItemSize;

				for (int x = equipPos.x; x < equipPos.x + itemSize.x; x++)
				{
					for (int y = equipPos.y; y < equipPos.y + itemSize.y; y++)
					{
						list.Add(new Vector2Int(x, y));
					}
				}

				return SlotState.Enable;
			}
            else
            {
				return SlotState.None;
			}
		}
        else
        {
			Vector2Int size = HoldingItem.Item.ItemSize;

			int xStart = (size.x % 2 == 1) ?
				(Mathf.FloorToInt(pos.x) - (size.x / 2)) :
				(Mathf.RoundToInt(pos.x) - (size.x / 2));
			int yStart = (size.y % 2 == 1) ?
				(Mathf.FloorToInt(pos.y) - (size.y / 2)) :
				(Mathf.RoundToInt(pos.y) - (size.y / 2));



			int xSize = size.x;
			int ySize = size.y;

			xStart = Mathf.Max(xStart, 0);
			yStart = Mathf.Max(yStart, 0);
			xStart = Mathf.Min(xStart, 12 - xSize);
			yStart = Mathf.Min(yStart, 5 - ySize);

			for (int x = xStart; x < xStart + xSize; x++)
			{
				for (int y = yStart; y < yStart + ySize; y++)
				{
					list.Add(new Vector2Int(x, y));
				}
			}


			List<int> ids = GetIdsOnPos(pos);

			if(ids.Count == 0)
            {
				return SlotState.Enable;
			}
			else if (ids.Count == 1)
            {
				return SlotState.Enable;
			}
            else
            {
				return SlotState.Error;
			}
		}

		
	}

	public ItemBase MakeItem(int itemTemplateId, int count = 1)
	{
		int itemDbId = Managers.Game.GenerateItemDbId();

		if (Managers.Data.ItemDic.TryGetValue(itemTemplateId, out ItemData itemData) == false)
			return null;

		ItemSaveData saveData = new ItemSaveData()
		{
			InstanceId = itemDbId,
			DbId = itemDbId,
			TemplateId = itemTemplateId,
			Count = count,
			EquipSlot = (int)EEquipSlotType.Inventory,
			EnchantCount = 0,
		};

		return AddItem(saveData);
	}

	public ItemBase AddItem(ItemSaveData itemInfo)
	{
		ItemBase item = ItemBase.MakeItem(itemInfo);
		if (item == null)
			return null;

		if (item.IsEquippedItem())
		{
			EquippedItems.Add(item.EquipSlot, item);
		}
		else if (item.IsInInventory())
		{
			InventoryItems.Add(item);
		}
		else if (item.IsInWarehouse())
		{
			WarehouseItems.Add(item);
		}

		AllItems.Add(item);

		return item;
	}

	public bool AddItem(ItemBase item)
	{
		if (item == null)
			return false;

		switch(item.ItemType)
        {
			case EItemType.Equipment:
				if(GetEquippedItemBySubType(item.ItemSubType) == null)
                {
					EquipItem(item, item.GetEquipItemEquipSlot());

					//EquippedItems.Add((int)item.ItemSubType, item);
				}
				else
                {
					if (!IsInventoryFull())
					{
						InventoryItems.Add(item);
					}
                    else
                    {
						return false;
                    }
				}
				break;
			case EItemType.Consumable:
				if(!IsInventoryFull())
                {
					InventoryItems.Add(item);
				}
                else
                {
					return false;
                }
				break;
			case EItemType.None:
				return false;
        }

		AllItems.Add(item);

		return true;
	}

	public void RemoveItem(int instanceId)
	{
		ItemBase item = AllItems.Find(x => x.SaveData.InstanceId == instanceId);
		if (item == null)
			return;

		if (item.IsEquippedItem())
		{
			EquippedItems.Remove(item.SaveData.EquipSlot);
		}
		else if (item.IsInInventory())
		{
			InventoryItems.Remove(item);
		}
		else if (item.IsInWarehouse())
		{
			WarehouseItems.Remove(item);
		}

		AllItems.Remove(item);
	}


	/**
	 * 아이템을 해당 슬롯에 장착
	 */

	public bool EquipItem(ItemBase item, EEquipSlotType itemSlot)
	{

		EquipmentItem eitem = item as EquipmentItem;

		EEquipSlotType equipSlotType = eitem.GetEquipItemEquipSlot();
		if (equipSlotType != itemSlot)
			return false;

		// 아이템 장착
		eitem.EquipSlot = (int)equipSlotType;
		EquippedItems[(int)equipSlotType] = eitem;

		Managers.Object.Player.Stats.EquipItem(eitem);
		Managers.UI.GetSceneUI<UI_GameScene>().EquipItem(equipSlotType, eitem.UIItem);

		return true;
	}

	/**
	 * 슬롯에 장착된 아이템 해제
	 */
	public void UnEquipItem(EEquipSlotType slotType)
	{
		var item = GetEquippedItem(slotType) as EquipmentItem;
		if (item == null)
			return;

		EquippedItems.Remove((int)item.EquipSlot);


		Managers.Object.Player.Stats.UnEquipItem(item);
		item.EquipSlot = (int)EEquipSlotType.None;
	}

	/**
	 * 아이템 슬롯 클릭시 실행
	 */
	public bool ClickSlot(EEquipSlotType slotType)
    {
		

		if (HoldingItem == null)
		{// 마우스에 아이템을 안들고 있을 때
			ItemBase equippedItem = GetEquippedItem(slotType);
			if (equippedItem == null)
			{
			}
            else
            {// 장착한 아이템이 있을 때
				UnEquipItem(slotType);
				HoldingItem = equippedItem.UIItem;
			}
		}
        else
		{// 마우스에 아이템을 들고 있을 때

			if (HoldingItem.Item.GetEquipItemEquipSlot() != slotType)
				return false;

			ItemBase equippedItem = GetEquippedItem(slotType);
			if (equippedItem == null)
			{// 장착한 아이템이 없을 때
				EquipItem(HoldingItem.Item, slotType);
				HoldingItem = null;
			}
			else
			{// 장착한 아이템이 있을 때
				UnEquipItem(slotType);
				EquipItem(HoldingItem.Item, slotType);
				HoldingItem = equippedItem.UIItem;
			}
		}

		return true;
	}

	public bool ClickInventory(EEquipSlotType slotType, Vector2 pos)
    {
		string str = "";
		for(int y = 0; y < 5; y++)
        {
			for(int x = 0; x < 12; x++)
            {
				str += InventoryItemGrid[x, y];

			}
			str += "\n";
        }
		Debug.Log(str);
		if(HoldingItem == null)
        {
			ItemBase item = GetItemByPosInInventory(pos);

			if (item == null)
            {
				// nothing

			}
			else
            {
				RemoveItemInInventory(item.InstanceId);
				HoldingItem = item.UIItem;
			}
        }
        else
        {

			return AddHoldingItemInInventory(pos, HoldingItem.Item.ItemSize);
		}

		

		return true;
    }

	public bool CheckHoldingItemCanEquip(EEquipSlotType slotType)
    {
		if (HoldingItem == null) return false;

		if(HoldingItem.Item.GetEquipItemEquipSlot() == slotType)
        {
			return true;
        }
        else
        {
			return false;
        }
    }

	public void Clear()
	{
		AllItems.Clear();

		EquippedItems.Clear();
		InventoryItems.Clear();
		WarehouseItems.Clear();
	}

	#region Helper
	public ItemBase GetItem(int instanceId)
	{
		return AllItems.Find(item => item.InstanceId == instanceId);
	}

	public ItemBase GetItemByPosInInventory(Vector2 pos)
    {

		int id = InventoryItemGrid[(int)pos.x, (int)pos.y];

		if(id == 0)
        {
			return null;
        }
        else
        {
			return GetItem(id);
		}
	}
	public bool AddHoldingItemInInventory(Vector2 pos, Vector2Int? size =  null)
    {

		if (!size.HasValue) size = new Vector2Int(1, 1);

		int xStart = (size.Value.x % 2 == 1) ?
			(Mathf.FloorToInt(pos.x) - (size.Value.x / 2)) :
			(Mathf.RoundToInt(pos.x) - (size.Value.x / 2));
		int yStart = (size.Value.y % 2 == 1) ?
			(Mathf.FloorToInt(pos.y) - (size.Value.y / 2)) :
			(Mathf.RoundToInt(pos.y) - (size.Value.y / 2));

		

		int xSize = size.Value.x;
		int ySize = size.Value.y;

		xStart = Mathf.Max(xStart, 0);
		yStart = Mathf.Max(yStart, 0);
		xStart = Mathf.Min(xStart, 12 - xSize);
		yStart = Mathf.Min(yStart, 5 - ySize);


		List<int> ids = GetIdsOnPos(pos, size);

		
		if(ids.Count > 1)
        {// 놓으려는 위치에 아이템이 여러개일 때
			return false;
        }



		if (ids.Count == 0)
		{
			for (int x = xStart; x < xStart + xSize; x++)
			{
				for (int y = yStart; y < yStart + ySize; y++)
				{
					InventoryItemGrid[x, y] = HoldingItem.Item.InstanceId;
				}
			}

			Managers.UI.GetSceneUI<UI_GameScene>().PutItem(EEquipSlotType.Inventory, HoldingItem, new Vector2(xStart + (float)xSize/2, yStart + (float)ySize / 2));
			InventoryItems.Add(HoldingItem.Item);
			HoldingItem.Item.EquipPos = new(xStart, yStart);
			HoldingItem = null;

		}
		else if (ids.Count == 1)
		{
			ItemBase item = RemoveItemInInventory(ids[0]);

			for (int x = xStart; x < xStart + xSize; x++)
			{
				for (int y = yStart; y < yStart + ySize; y++)
				{
					InventoryItemGrid[x, y] = HoldingItem.Item.InstanceId;
				}
			}

			Managers.UI.GetSceneUI<UI_GameScene>().PutItem(EEquipSlotType.Inventory, HoldingItem, new Vector2(xStart + (float)xSize/2, yStart + (float)ySize / 2));
			InventoryItems.Add(HoldingItem.Item);
			HoldingItem.Item.EquipPos = new(xStart, yStart);
			HoldingItem = item.UIItem;
		}


		return true;
	}

	public List<int> GetIdsOnPos(Vector2 pos, Vector2Int? size = null)
    {

		if (!size.HasValue) size = new Vector2Int(1, 1);

		int xStart = (size.Value.x % 2 == 1) ?
			(Mathf.FloorToInt(pos.x) - (size.Value.x / 2)) :
			(Mathf.RoundToInt(pos.x) - (size.Value.x / 2));
		int yStart = (size.Value.y % 2 == 1) ?
			(Mathf.FloorToInt(pos.y) - (size.Value.y / 2)) :
			(Mathf.RoundToInt(pos.y) - (size.Value.y / 2));



		int xSize = size.Value.x;
		int ySize = size.Value.y;

		xStart = Mathf.Max(xStart, 0);
		yStart = Mathf.Max(yStart, 0);
		xStart = Mathf.Min(xStart, 12 - xSize);
		yStart = Mathf.Min(yStart, 5 - ySize);

		List<int> list = new();

		for (int x = xStart; x < xStart + xSize; x++)
		{
			for (int y = yStart; y < yStart + ySize; y++)
			{
				int val = InventoryItemGrid[x, y];
				if (val != 0 && list.Find(i => i == val) == 0)
				{
					list.Add(val);
				}
			}
		}

		return list;
    }

	public ItemBase RemoveItemInInventory(int instanceId)
    {
		ItemBase item = GetItemInInventory(instanceId);

		for(int x = item.EquipPos.x; x < item.EquipPos.x + item.ItemSize.x; x++)
        {
			for (int y = item.EquipPos.y; y < item.EquipPos.y + item.ItemSize.y; y++)
			{
				InventoryItemGrid[x, y] = 0;
			}
		}

		InventoryItems.Remove(item);

		return item;
	}

	public ItemBase GetEquippedItem(EEquipSlotType equipSlotType)
	{
		EquippedItems.TryGetValue((int)equipSlotType, out ItemBase item);

		return item;
	}

	public ItemBase GetEquippedItem(int instanceId)
	{
		return EquippedItems.Values.Where(x => x.InstanceId == instanceId).FirstOrDefault();
	}

	public ItemBase GetEquippedItemBySubType(EItemSubType subType)
	{
		return EquippedItems.Values.Where(x => x.ItemSubType == subType).FirstOrDefault();
	}

	public ItemBase GetItemInInventory(int instanceId)
	{
		return InventoryItems.Find(x => x.SaveData.InstanceId == instanceId);
	}

	public bool IsInventoryFull()
	{
		return InventoryItems.Count >= InventorySlotCount();
	}

	public int InventorySlotCount()
	{
		return DEFAULT_INVENTORY_SLOT_COUNT;
	}

	public List<ItemBase> GetEquippedItems()
	{
		return EquippedItems.Values.ToList();
	}

	public List<ItemSaveData> GetEquippedItemInfos()
	{
		return EquippedItems.Values.Select(x => x.SaveData).ToList();
	}

	public List<ItemBase> GetInventoryItems()
	{
		return InventoryItems.ToList();
	}

	public List<ItemSaveData> GetInventoryItemInfos()
	{
		return InventoryItems.Select(x => x.SaveData).ToList();
	}

	/*public List<ItemSaveData> GetInventoryItemInfosOrderbyGrade()
	{
		return InventoryItems.OrderByDescending(y => (int)y.ItemData.Grade)
						.ThenBy(y => (int)y.TemplateId)
						.Select(x => x.SaveData)
						.ToList();
	}*/

	public List<ItemSaveData> GetWarehouseItemInfos()
	{
		return WarehouseItems.Select(x => x.SaveData).ToList();
	}

	/*public List<ItemSaveData> GetWarehouseItemInfosOrderbyGrade()
	{
		return WarehouseItems.OrderByDescending(y => (int)y.ItemData.Grade)
									.ThenBy(y => (int)y.TemplateId)
									.Select(x => x.SaveData)
									.ToList();
	}*/
	#endregion
}
