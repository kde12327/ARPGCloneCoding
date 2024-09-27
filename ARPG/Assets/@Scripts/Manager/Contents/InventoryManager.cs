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
	public Dictionary<int /*EquipSlot*/, ItemBase> EquippedItems = new Dictionary<int, ItemBase>(); // ��� �κ�
	List<ItemBase> InventoryItems = new List<ItemBase>(); // ���� �κ�
	List<ItemBase> WarehouseItems = new List<ItemBase>(); // â��
	List<ItemBase> VendorItems = new List<ItemBase>(); // ����

	int[,] PlayerInventoryItemGrid = new int[12, 5];
	int[,] WarehouseInventoryItemGrid = new int[12, 12];
	int[,] VendorInventoryItemGrid = new int[12, 12];

	public Npc Vendor { get; set; }


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

	ConsumableItem _usingItem;
	public ConsumableItem UsingItem
	{
		get { return _usingItem; }
		set
		{
			_usingItem = value;

			Managers.UI.GetSceneUI<UI_GameScene>().SetUsingItem(_usingItem);
		}
	}

	// ������ �ݱ�
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
				// ��� ����� �� ����
				return EquipItem(uiitem.Item, uiitem.Item.GetEquipItemEquipSlot());
            }
            else
            {
				// �κ��丮�� �ٷ�

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

	// Grid ui���� �� ĥ�� �� grid cell ������
	public ESlotState GetCellList(Define.EEquipSlotType invenType, Vector2 pos,  out List<Vector2Int> list)
    {

		list = new();


		if (HoldingItem == null)
        {
			int id = 0;
			id = GetInventoryGridNum(invenType, (int)pos.x, (int)pos.y);


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

				return ESlotState.Enable;
			}
            else
            {
				return ESlotState.None;
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
				return ESlotState.Enable;
			}
			else if (ids.Count == 1)
            {
				return ESlotState.Enable;
			}
            else
            {
				return ESlotState.Error;
			}
		}

		
	}


	// ������ �����
	// TODO: savedata ���� �պ����ؼ� ���� �̿ϼ�
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

	// ������ savedata�� ����ִ� ������ ���� �ش� �κ��丮�� �ֱ�
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
		else if (item.IsInVendor())
		{
			VendorItems.Add(item);
		}

		AllItems.Add(item);

		return item;
	}


	// ������ �ֿ��� ��.
	// �Ⱦ�
	public bool AddItem(ItemBase item)
	{
		if (item == null)
			return false;

		// ���� ������ ��쿡 �ٷ� ����
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

	// �������� ����Ʈ���� ����
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
		else if (item.IsInVendor())
		{
			VendorItems.Remove(item);
		}

		AllItems.Remove(item);
	}


	/**
	 * �������� �ش� ���Կ� ����
	 */

	public bool EquipItem(ItemBase item, EEquipSlotType itemSlot)
	{

		EquipmentItem eitem = item as EquipmentItem;

		EEquipSlotType equipSlotType = eitem.GetEquipItemEquipSlot();
		if (equipSlotType != itemSlot)
			return false;

		// ������ ����
		eitem.EquipSlot = (int)equipSlotType;
		EquippedItems[(int)equipSlotType] = eitem;

		Managers.Object.Player.Stats.EquipItem(eitem);
		Managers.UI.GetSceneUI<UI_GameScene>().EquipItem(equipSlotType, eitem.UIItem);

		return true;
	}

	/**
	 * ���Կ� ������ ������ ����
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
	 * ������ ���� Ŭ���� ����
	 */
	public bool ClickSlot(EEquipSlotType slotType)
    {
		

		if (HoldingItem == null)
		{// ���콺�� �������� �ȵ�� ���� ��
			ItemBase equippedItem = GetEquippedItem(slotType);
			if (equippedItem == null)
			{
			}
            else
            {// ������ �������� ���� ��
				UnEquipItem(slotType);
				HoldingItem = equippedItem.UIItem;
			}
		}
        else
		{// ���콺�� �������� ��� ���� ��

			if (HoldingItem.Item.GetEquipItemEquipSlot() != slotType)
				return false;

			ItemBase equippedItem = GetEquippedItem(slotType);
			if (equippedItem == null)
			{// ������ �������� ���� ��
				EquipItem(HoldingItem.Item, slotType);
				HoldingItem = null;
			}
			else
			{// ������ �������� ���� ��
				UnEquipItem(slotType);
				EquipItem(HoldingItem.Item, slotType);
				HoldingItem = equippedItem.UIItem;
			}
		}

		return true;
	}


	// �κ��丮 �׸��带 Ŭ������ �� ����
	public bool ClickInventory(EEquipSlotType invenType, Vector2 pos)
    {

		if(UsingItem != null)
        {// �Ҹ� �������� ��� ���� ��
			ItemBase item = GetItemByPosInInventory(invenType, pos);
			bool result = UsingItem.UseOnItem(item);
			UsingItem = null;
			return result;
		}
		else if(HoldingItem == null)
        {
			ItemBase item = GetItemByPosInInventory(invenType, pos);

			if (item == null)
            {
				// nothing

			}
			else
            {
				if (invenType == EEquipSlotType.VendorInventory)
                {
					// TODO: buy system
					Vendor.SaleList.Remove(item);
					RemoveItemInInventory(invenType, item.InstanceId);
					HoldingItem = item.UIItem;
				}
                else
                {
					RemoveItemInInventory(invenType, item.InstanceId);
					HoldingItem = item.UIItem;
				}
			}
        }
        else
        {
			// �������� ������ ���ٳ��� �Ұ�
			if (invenType == EEquipSlotType.VendorInventory)
				return false;
			return AddItemInInventory(invenType, pos,  HoldingItem.Item, true);
		}

		

		return true;
    }
	
	// �κ��丮 �׸��带 ��Ŭ������ �� ����
	public bool RightClickInventory(EEquipSlotType invenType, Vector2 pos)
    {
		if (HoldingItem != null) return false;

		if (UsingItem == null)
		{
			if (invenType != EEquipSlotType.VendorInventory)
			{
				ItemBase item = GetItemByPosInInventory(invenType, pos);

				if(item != null && item.ItemType == EItemType.Consumable)
                {
					ConsumableItem cItem = item as ConsumableItem;
					if(!cItem.UseItem())
                    {// �����ۿ� ���Ǵ� ���
						UsingItem = cItem;
					}
				}
			}
		}
		else
		{
			UsingItem = null;
		}

		return true;
    }



	// ��� �ִ� �������� �ش� ���Կ� ���� ��������
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
		id = GetInventoryGridNum(invenType, (int)pos.x, (int)pos.y);

		

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
        {// �������� ��ġ�� �������� �������� ��
			return false;
        }



		if (ids.Count == 0)
		{
			for (int x = 0; x < xSize; x++)
			{
				for (int y = 0; y < ySize; y++)
				{
					SetInventoryGridNum(invenType, xStart + x, yStart + y, item.InstanceId);
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
				case EEquipSlotType.VendorInventory:
					VendorItems.Add(item);
					break;
			}
			item.EquipPos = new(xStart, yStart);

			// Ȧ�� ������ ó�� �κ�
			if (holdingItemFlag)
            {
				HoldingItem = null;
            }

		}
		else if (ids.Count == 1)
		{	
			// ������ �η��� ���� �̹� �������� �ִ� ���.
			// �������� ��� �ִ� ��츸 ����� ��.
			ItemBase removeItem = RemoveItemInInventory(invenType, ids[0]);

			for (int x = 0; x < xSize; x++)
			{
				for (int y = 0; y < ySize; y++)
				{
					SetInventoryGridNum(invenType, xStart + x, yStart + y, item.InstanceId);

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
				case EEquipSlotType.VendorInventory:
					VendorItems.Add(item);
					break;
			}
			item.EquipPos = new(xStart, yStart);

			// Ȧ�� ������ ó�� �κ�
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
				int val = GetInventoryGridNum(invenType, xStart + x, yStart + y);
				if (val != 0 && list.Find(i => i == val) == 0)
				{
					list.Add(val);
				}
			}
		}

		return list;
    }

	public void RemoveItem(ItemBase item)
	{
		RemoveItemInInventory((Define.EEquipSlotType)item.EquipSlot, item.InstanceId);
		item.Destroy();

	}

	public ItemBase RemoveItemInInventory(Define.EEquipSlotType invenType, int instanceId)
    {
		ItemBase item = GetItemInInventory(invenType, instanceId);

		for(int x = item.EquipPos.x; x < item.EquipPos.x + item.ItemSize.x; x++)
        {
			for (int y = item.EquipPos.y; y < item.EquipPos.y + item.ItemSize.y; y++)
			{
				SetInventoryGridNum(invenType, x, y, 0);
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
			case EEquipSlotType.VendorInventory:
				VendorItems.Remove(item);
				break;
		}

		return item;
	}

	// ���� ������ ����Ʈ �ѹ��� �Է�
	public void SetVendorSaleList(List<ItemBase> items)
    {
		EEquipSlotType invenType = EEquipSlotType.VendorInventory;

		foreach (var item in items)
        {
			AddItemInInventory(invenType, item.EquipPos, item);
		}
    }

	// ���� ���� �ʱ�ȭ
	public void ClearVendorInventory()
    {
		EEquipSlotType invenType = EEquipSlotType.VendorInventory;


		VendorItems.Clear();
		Vector2Int sizeMax = GetInventorySize(invenType);


		List<int> list = new();

		for (int x = 0; x < sizeMax.x; x++)
		{
			for (int y = 0; y < sizeMax.y; y++)
			{
				SetInventoryGridNum(invenType, x, y, 0);
			}
		}
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
			case EEquipSlotType.VendorInventory:
				return VendorItems.Find(x => x.SaveData.InstanceId == instanceId);
			default:
				return null;
		}
	}


	/**
	 * ���� ���� �� (-1, -1) ��ȯ
	 * ������ �� ������ �߾� ��ġ ��ȯ
	 */
	public Vector2 GetInventoryEmptyPosition(Define.EEquipSlotType invenType, Vector2Int size)
	{

		Vector2Int invenSize = GetInventorySize(invenType);

		Vector2 result = new(-1, -1);


		bool FINDFLAG = false;

		for (int x = 0; x < invenSize.x - size.x + 1 && !FINDFLAG; x++)
        {
			for (int y = 0; y < invenSize.y - size.y + 1 && !FINDFLAG; y++)
			{

				// ������ �����ŭ üũ
				bool HASITEMFLAG = false;

				for (int sizex = 0; sizex < size.x && !HASITEMFLAG; sizex++)
				{
					for (int sizey = 0; sizey < size.y && !HASITEMFLAG; sizey++)
					{
						if (GetInventoryGridNum(invenType, x + sizex, y + sizey) != 0)
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


	public int GetInventoryGridNum(EEquipSlotType invenType, int x, int y)
    {
		int result = -1;

		switch(invenType)
        {
			case EEquipSlotType.PlayerInventory:
				result = PlayerInventoryItemGrid[x, y];
				break;
			case EEquipSlotType.WarehouseInventory:
				result = WarehouseInventoryItemGrid[x, y];
				break;
			case EEquipSlotType.VendorInventory:
				result = VendorInventoryItemGrid[x, y];
				break;
		}

		return result;
    }

	public void SetInventoryGridNum(EEquipSlotType invenType, int x, int y, int value)
	{
		switch (invenType)
		{
			case EEquipSlotType.PlayerInventory:
				PlayerInventoryItemGrid[x, y] = value;
				break;
			case EEquipSlotType.WarehouseInventory:
				WarehouseInventoryItemGrid[x, y] = value;
				break;
			case EEquipSlotType.VendorInventory:
				VendorInventoryItemGrid[x, y] = value;
				break;
		}
	}

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
