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

	int[,] PlayerInventoryItemGrid = new int[12, 5];
	int[,] WarehouseInventoryItemGrid = new int[12, 12];

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
				return EquipItem(uiitem.Item, uiitem.Item.GetEquipItemEquipSlot());
            }
            else
            {
				// 인벤토리에 바로

				Vector2 pos = GetInventoryEmptyPosition(Define.EEquipSlotType.PlayerInventory, item.ItemSize);
				if (pos.x != -1)
				{
					return AddItemInInventory(Define.EEquipSlotType.PlayerInventory, pos, item);
				}
				else
				{
					return false;
				}


			}
			
        }

		AllItems.Add(item);

		return true;
	}

	public SlotState GetCellList(Define.EEquipSlotType invenType, Vector2 pos,  out List<Vector2Int> list)
    {

		list = new();


		if (HoldingItem == null)
        {
			int id = 0;

			switch(invenType)
            {
				case EEquipSlotType.PlayerInventory:
					id = PlayerInventoryItemGrid[(int)pos.x, (int)pos.y];
					break;
				case EEquipSlotType.WarehouseInventory:
					id = WarehouseInventoryItemGrid[(int)pos.x, (int)pos.y];
					break;
			}

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

			Vector2Int sizeMax = GetInventorySize(invenType);


			xStart = Mathf.Max(xStart, 0);
			yStart = Mathf.Max(yStart, 0);
			xStart = Mathf.Min(xStart, sizeMax.x - xSize);
			yStart = Mathf.Min(yStart, sizeMax.y - ySize);

			for (int x = 0; x < xSize; x++)
			{
				for (int y = 0; y < ySize; y++)
				{
					list.Add(new Vector2Int(xStart + x, yStart + y));
				}
			}


			List<int> ids = GetIdsOnPos(invenType, pos,  size);

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
			EquipSlot = (int)EEquipSlotType.PlayerInventory,
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


	// 아이템 주웠을 때.
	public bool AddItem(ItemBase item)
	{
		if (item == null)
			return false;

		// 장착 가능한 경우에 바로 착용
		if(item.ItemType == EItemType.Equipment && GetEquippedItemBySubType(item.ItemSubType) == null)
        {
			EquipItem(item, item.GetEquipItemEquipSlot());
		}
        else
        {
			Vector2 pos = GetInventoryEmptyPosition(Define.EEquipSlotType.PlayerInventory, item.ItemSize);
			if (pos.x != -1)
			{
				return AddItemInInventory(Define.EEquipSlotType.PlayerInventory, pos, item);
			}
			else
			{
				return false;
			}
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

	public bool ClickInventory(EEquipSlotType invenType, Vector2 pos)
    {
		if(HoldingItem == null)
        {
			ItemBase item = GetItemByPosInInventory(invenType, pos);

			if (item == null)
            {
				// nothing

			}
			else
            {
				RemoveItemInInventory(invenType, item.InstanceId);
				HoldingItem = item.UIItem;
			}
        }
        else
        {
			return AddItemInInventory(invenType, pos,  HoldingItem.Item, true);
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

	public ItemBase GetItemByPosInInventory(Define.EEquipSlotType invenType, Vector2 pos)
    {

		int id = 0;
		switch (invenType)
		{
			case EEquipSlotType.PlayerInventory:
				id = PlayerInventoryItemGrid[(int)pos.x, (int)pos.y];
				break;
			case EEquipSlotType.WarehouseInventory:
				id = WarehouseInventoryItemGrid[(int)pos.x, (int)pos.y];
				break;
		}
		

		if(id == 0)
        {
			return null;
        }
        else
        {
			return GetItem(id);
		}
	}
	public bool AddItemInInventory(Define.EEquipSlotType invenType, Vector2 pos, ItemBase item, bool holdingItemFlag = false)
    {

		Vector2Int size = item.ItemSize;


		int xStart = (size.x % 2 == 1) ?
			(Mathf.FloorToInt(pos.x) - (size.x / 2)) :
			(Mathf.RoundToInt(pos.x) - (size.x / 2));
		int yStart = (size.y % 2 == 1) ?
			(Mathf.FloorToInt(pos.y) - (size.y / 2)) :
			(Mathf.RoundToInt(pos.y) - (size.y / 2));

		

		int xSize = size.x;
		int ySize = size.y;

		Vector2Int sizeMax = GetInventorySize(invenType);


		xStart = Mathf.Max(xStart, 0);
		yStart = Mathf.Max(yStart, 0);
		xStart = Mathf.Min(xStart, sizeMax.x - xSize);
		yStart = Mathf.Min(yStart, sizeMax.y - ySize);


		List<int> ids = GetIdsOnPos(invenType, pos, size);

		
		if(ids.Count > 1)
        {// 놓으려는 위치에 아이템이 여러개일 때
			return false;
        }



		if (ids.Count == 0)
		{
			for (int x = 0; x < xSize; x++)
			{
				for (int y = 0; y < ySize; y++)
				{
					switch (invenType)
                    {
						case EEquipSlotType.PlayerInventory:
							PlayerInventoryItemGrid[xStart + x, yStart + y] = item.InstanceId;
							break;
						case EEquipSlotType.WarehouseInventory:
							WarehouseInventoryItemGrid[xStart + x, yStart + y] = item.InstanceId;
							break;
                    }
				}
			}

			Managers.UI.GetSceneUI<UI_GameScene>().PutItem(invenType, item.UIItem, new Vector2(xStart + (float)xSize/2, yStart + (float)ySize / 2));
			switch (invenType)
			{
				case EEquipSlotType.PlayerInventory:
					InventoryItems.Add(item);
					break;
				case EEquipSlotType.WarehouseInventory:
					WarehouseItems.Add(item);
					break;
			}
			item.EquipPos = new(xStart, yStart);

			// 홀딩 아이템 처리 부분
			if (holdingItemFlag)
            {
				HoldingItem = null;
            }

		}
		else if (ids.Count == 1)
		{	
			// 아이템 두려는 곳에 이미 아이템이 있는 경우.
			// 아이템을 들고 있는 경우만 여기로 옴.
			ItemBase removeItem = RemoveItemInInventory(invenType, ids[0]);

			for (int x = 0; x < xSize; x++)
			{
				for (int y = 0; y < ySize; y++)
				{
					switch (invenType)
                    {
						case EEquipSlotType.PlayerInventory:
							PlayerInventoryItemGrid[xStart + x, yStart + y] = item.InstanceId;
							break;
						case EEquipSlotType.WarehouseInventory:
							WarehouseInventoryItemGrid[xStart + x, yStart + y] = item.InstanceId;
							break;
                    }
				}
			}

			Managers.UI.GetSceneUI<UI_GameScene>().PutItem(invenType, item.UIItem, new Vector2(xStart + (float)xSize/2, yStart + (float)ySize / 2));
			
			switch (invenType)
			{
				case EEquipSlotType.PlayerInventory:
					InventoryItems.Add(item);
					break;
				case EEquipSlotType.WarehouseInventory:
					WarehouseItems.Add(item);
					break;
			}
			item.EquipPos = new(xStart, yStart);

			// 홀딩 아이템 처리 부분
			if (holdingItemFlag)
			{
				HoldingItem = removeItem.UIItem;
			}
		}


		return true;
	}

	public List<int> GetIdsOnPos(Define.EEquipSlotType invenType, Vector2 pos, Vector2Int? size = null)
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

		Vector2Int sizeMax = GetInventorySize(invenType);


		xStart = Mathf.Max(xStart, 0);
		yStart = Mathf.Max(yStart, 0);
		xStart = Mathf.Min(xStart, sizeMax.x - xSize);
		yStart = Mathf.Min(yStart, sizeMax.y - ySize);

		List<int> list = new();

		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				int val = 0;
				switch(invenType)
                {
					case EEquipSlotType.PlayerInventory:
						val = PlayerInventoryItemGrid[xStart + x, yStart + y];
						break;
					case EEquipSlotType.WarehouseInventory:
						val = WarehouseInventoryItemGrid[xStart + x, yStart + y];
						break;
                }
				if (val != 0 && list.Find(i => i == val) == 0)
				{
					list.Add(val);
				}
			}
		}

		return list;
    }

	public ItemBase RemoveItemInInventory(Define.EEquipSlotType invenType, int instanceId)
    {
		ItemBase item = GetItemInInventory(invenType, instanceId);

		for(int x = item.EquipPos.x; x < item.EquipPos.x + item.ItemSize.x; x++)
        {
			for (int y = item.EquipPos.y; y < item.EquipPos.y + item.ItemSize.y; y++)
			{
				switch(invenType)
                {
					case EEquipSlotType.PlayerInventory:
						PlayerInventoryItemGrid[x, y] = 0;
						break;
					case EEquipSlotType.WarehouseInventory:
						WarehouseInventoryItemGrid[x, y] = 0;
						break;
                }
			}
		}
		switch (invenType)
		{
			case EEquipSlotType.PlayerInventory:
				InventoryItems.Remove(item);
				break;
			case EEquipSlotType.WarehouseInventory:
				WarehouseItems.Remove(item);
				break;
		}

		return item;
	}

	public Vector2Int GetInventorySize(Define.EEquipSlotType invenType)
    {
		return Managers.UI.GetSceneUI<UI_GameScene>().GetInventorySize(invenType);
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

	public ItemBase GetItemInInventory(Define.EEquipSlotType invenType, int instanceId)
	{
		switch (invenType)
		{
			case EEquipSlotType.PlayerInventory:
				return InventoryItems.Find(x => x.SaveData.InstanceId == instanceId);
			case EEquipSlotType.WarehouseInventory:
				return WarehouseItems.Find(x => x.SaveData.InstanceId == instanceId);
			default:
				return null;
		}
	}


	/**
	 * 공간 없을 시 (-1, -1) 반환
	 * 아이템 둘 공간의 중앙 위치 반환
	 */
	public Vector2 GetInventoryEmptyPosition(Define.EEquipSlotType invenType, Vector2Int size)
	{

		Vector2Int invenSize = GetInventorySize(invenType);

		int[,] inven;
		Vector2 result = new(-1, -1);

		switch (invenType)
		{
			case EEquipSlotType.PlayerInventory:
				inven = PlayerInventoryItemGrid;
				break;
			case EEquipSlotType.WarehouseInventory:
				inven = WarehouseInventoryItemGrid;
				break;
			default:
				return result;
		}

		bool FINDFLAG = false;

		for (int x = 0; x < invenSize.x - size.x + 1 && !FINDFLAG; x++)
        {
			for (int y = 0; y < invenSize.y - size.y + 1 && !FINDFLAG; y++)
			{

				// 아이템 사이즈만큼 체크
				bool HASITEMFLAG = false;

				for (int sizex = 0; sizex < size.x && !HASITEMFLAG; sizex++)
				{
					for (int sizey = 0; sizey < size.y && !HASITEMFLAG; sizey++)
					{
						if(inven[x + sizex, y + sizey] != 0)
                        {
							HASITEMFLAG = true;
						}
					}
				}

				if(HASITEMFLAG == false)
                {
					FINDFLAG = true;
					result.x = x + size.x / 2.0f;
					result.y = y + size.y / 2.0f;
				}
			}
		}

		return result;
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
