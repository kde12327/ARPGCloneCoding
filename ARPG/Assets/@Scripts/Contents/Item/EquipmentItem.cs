using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EquipmentItem : ItemBase
{
	public EquipmentItemBaseData EquipmentItemBaseData;

	public List<Modifier> DefaultMod = new();
	public List<Modifier> ImplicitMod = new();
	public List<Modifier> PrefixMod = new();
	public List<Modifier> SuffixMod = new();

	public ERarity Rarity { get; set; }

	public EquipmentItem(int itemDataId) : base(itemDataId)
	{
		EquipmentItemBaseData = Managers.Data.EquipmentItemBaseDic[itemDataId];
		Init();
	}


	public override bool Init()
	{
		
		
		return true;
	}

	public List<Option> GetOptions()
    {
		List<Option> options = new();
		foreach (var mod in DefaultMod)
		{
			options.AddRange(mod.Options);
		}
		foreach (var mod in ImplicitMod)
        {
			options.AddRange(mod.Options);
		}
		foreach (var mod in PrefixMod)
		{
			options.AddRange(mod.Options);
		}
		foreach (var mod in SuffixMod)
		{
			options.AddRange(mod.Options);
		}

		return options;
    }


	public static EquipmentItem MakeEquipmentItem(int itemDataId, ERarity rarity)
    {
		

		var item = Managers.Inventory.MakeItem(itemDataId) as EquipmentItem;

		var defaultMod = Modifier.MakeModifier(item.EquipmentItemBaseData.DefaultOptions, item.EquipmentItemBaseData.DefaultMinMaxValues);
		if(defaultMod != null)
			item.DefaultMod.Add(defaultMod);

		var implicitMod = Modifier.MakeModifier(item.EquipmentItemBaseData.ImplicitOptions, item.EquipmentItemBaseData.ImplicitMinMaxValues);
		if (implicitMod != null)
			item.ImplicitMod.Add(implicitMod);

		int prefixNum = 0;
		int suffixNum = 0;

		item.Rarity = rarity;

		switch (rarity)
        {
			case ERarity.Normal:
				break;
			case ERarity.Magic:
				prefixNum = Random.Range(0, 2);
				suffixNum = Random.Range(0, 2);
				if (prefixNum + suffixNum == 0)
					if (Random.Range(0, 2) == 0)
						prefixNum++;
					else
						suffixNum++;
				break;
			case ERarity.Rare:
				prefixNum = Random.Range(1, 3);
				suffixNum = Random.Range(1, 3);
				break;
			case ERarity.Unique:
				break;
		}

		// calculate mod weight

		List<ModData> prefixList = Managers.Data.PrefixModifierData[item.ItemSubType];
		List<ModData> suffixList = Managers.Data.SuffixModifierData[item.ItemSubType];

		MakeModifiers(item.PrefixMod, prefixNum, prefixList);
		MakeModifiers(item.SuffixMod, suffixNum, suffixList);

		return item;
    }

	static void MakeModifiers(List<Modifier> destList, int count, List<ModData> modList)
    {

		int maxWeight = 0;

		for (int i = 0; i < modList.Count; i++)
		{
			maxWeight += modList[i].Weight;
		}

		for (int idx = 0; idx < count; idx++)
		{
			int value = Random.Range(1, maxWeight + 1);
			ModData mod = null;
			for (int listidx = 0; listidx < modList.Count; listidx++)
			{
				if (value < modList[listidx].Weight)
				{
					mod = modList[listidx];
					break;
				}
				else
				{
					value -= modList[listidx].Weight;
				}
			}
			var _mod = Modifier.MakeModifier(mod.Stats, mod.MinMaxValues);
			if (_mod != null)
				destList.Add(_mod);
		}
	}

}
