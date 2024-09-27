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
     * �Ҹ� �����ۿ� ó�� ��Ŭ�� ���� ��, ����Ǵ� �Լ�
     * ��Ż ��ũ�� ���� ��ü�� ��� �Ǵ� �������� ���⼭ ��� �� true ��ȯ.
     * �ƴ� ��� false.
     */
    public bool UseItem()
    {
        switch (ConsumableItemData.EffectType)
        {
            case EConsumableEffectType.CreatesPortalToTown:
                Debug.Log("��Ż");
                GameObject go = Managers.Resource.Instantiate("TownPortal");
                go.name = "����";
                go.transform.position = Managers.Object.Player.transform.position + new Vector3(0, 1, 0);

                return true;
            default:
                return false;
        }
    }

    /**
     * �Ҹ� �������� ��� �� ��Ŭ�� ���� ��, ����Ǵ� �Լ�
     * ��Ż ��ũ�� ���� ��ü�� ��� �Ǵ� �������� ��������� �ʰ�,
     * ������ ��� ���� �� true, ���� �� false.
     */
    public bool UseOnItem(ItemBase item)
    {
        bool result = false;

        switch(ConsumableItemData.EffectType)
        {
            case EConsumableEffectType.IdentifiesItem:
                Debug.Log("����");
                break;
            case EConsumableEffectType.UpgradeMagicToRare:
                Debug.Log("����");
                result = item.UpgradeRarity(Define.ERarity.Magic, Define.ERarity.Rare);
                break;
            case EConsumableEffectType.UpgradeNormalToMagic:
                Debug.Log("��ȸ");
                result = item.UpgradeRarity(Define.ERarity.Normal, Define.ERarity.Magic);
                break;
            case EConsumableEffectType.UpgradeNormalToRare:
                Debug.Log("����");
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
