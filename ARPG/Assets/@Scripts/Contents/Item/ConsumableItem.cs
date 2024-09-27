using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ConsumableItem : ItemBase
{
    public Data.ConsumableItemData ConsumableItemData;

    public int StackSize = 0;

    public ConsumableItem(int itemDataId) : base(itemDataId)
    {
        ConsumableItemData = Managers.Data.ConsumableItemDic[itemDataId];

        Init();
    }

    public override bool Init()
    {
        if (!base.Init()) return false;

        return true;
    }

    /**
     * 소모 아이템에 처음 우클릭 했을 때, 실행되는 함수
     * 포탈 스크롤 같이 자체로 사용 되는 아이템은 여기서 사용 후 true 반환.
     * 아닌 경우 false.
     */
    public bool UseItem()
    {
        switch (ConsumableItemData.EffectType)
        {
            case EConsumableEffectType.CreatesPortalToTown:
                Debug.Log("포탈");
                GameObject go = Managers.Resource.Instantiate("TownPortal");
                go.name = "마을";
                go.transform.position = Managers.Object.Player.transform.position + new Vector3(0, 1, 0);

                return true;
            default:
                return false;
        }
    }

    /**
     * 소모 아이템을 사용 중 우클릭 했을 때, 실행되는 함수
     * 포탈 스크롤 같이 자체로 사용 되는 아이템은 여기들어오지 않고,
     * 아이템 사용 성공 시 true, 실패 시 false.
     */
    public bool UseOnItem(ItemBase item)
    {
        bool result = false;

        switch(ConsumableItemData.EffectType)
        {
            case EConsumableEffectType.IdentifiesItem:
                Debug.Log("감정");
                break;
            case EConsumableEffectType.UpgradeMagicToRare:
                Debug.Log("제왕");
                result = item.UpgradeRarity(Define.ERarity.Magic, Define.ERarity.Rare);
                break;
            case EConsumableEffectType.UpgradeNormalToMagic:
                Debug.Log("기회");
                result = item.UpgradeRarity(Define.ERarity.Normal, Define.ERarity.Magic);
                break;
            case EConsumableEffectType.UpgradeNormalToRare:
                Debug.Log("연금");
                result = item.UpgradeRarity(Define.ERarity.Normal, Define.ERarity.Rare);
                break;
            default:
                break;
        }

        if(result == true)
        {
            StackSize--;
            if(StackSize == 0)
            {
                Managers.Inventory.RemoveItem(this);
            }
        }

        return result;
    }

	public static ConsumableItem MakeConsumableItem(int itemDataId, int stackSize)
	{
        Debug.Log(itemDataId + " , "+ stackSize);
		var item = Managers.Inventory.MakeItem(itemDataId) as ConsumableItem;
        item.StackSize = stackSize;

        return item;
	}
}
