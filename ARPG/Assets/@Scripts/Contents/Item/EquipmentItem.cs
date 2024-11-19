using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


public enum ESocketColor
{
	Red,
	Green,
	Blue,
	White
}

public class EquipmentItem : ItemBase
{
	public EquipmentItemBaseData EquipmentItemBaseData;

	public List<Modifier> DefaultMod = new();
	public List<Modifier> ImplicitMod = new();
	public List<Modifier> PrefixMod = new();
	public List<Modifier> SuffixMod = new();

	public List<ESocketColor> Socket { get; set; }
	public List<bool> Link { get; set; }

	public SkillGemItem[] SkillGems = new SkillGemItem[6];

	public int MaxSocket 
	{
        get 
		{
			if(ItemSize.x == 1 && ItemSize.y == 3)
            {
				return 3;
            }
			else if(ItemSize.x == 2 && ItemSize.y == 2)
            {
				return 4;
			}
			else if(ItemSize.x == 2 && ItemSize.y == 3)
            {
				return 6;
			}
            else
            {
				return 0;
            }
		}
	}

	public EquipmentItem(int itemDataId) : base(itemDataId)
	{
		EquipmentItemBaseData = Managers.Data.EquipmentItemBaseDic[itemDataId];
		Init();
	}


	public override bool Init()
	{

		return true;
	}

	public void SetSkillGem(SkillGemItem item, int socketNumber)
    {
		SkillGems[socketNumber] = item;

        if (IsEquippedItem())
        {
			Managers.Object.Player.OnChangeSkillSetting();
		}
	}


	public List<List<SkillGemItem>> GetSkills()
    {
		List<List<SkillGemItem>> skills = new();

		for(int i = 0; i < 6; i++)
        {
			if (SkillGems[i] == null) continue;

			List<SkillGemItem> skill = new();
			if(SkillGems[i].SkillGemItemData.SkillGemType == ESkillGemType.SkillGem)
            {
				skill.Add(SkillGems[i]);

				List<int> linkedGemIdx = new();
				
				for(int idx = i - 1; idx >= 0; idx--)
                {
					if (!Link[idx]) break;

                    if (SkillGems[idx] != null
						&& SkillGems[idx].SkillGemItemData.SkillGemType == ESkillGemType.SupportGem)
                    {
						// TODO: 서폿젬 태그 관련 로직 추가
						skill.Add(SkillGems[idx]);
					}
                }
				for (int idx = i + 1; idx < Link.Count; idx++)
				{
					if (!Link[idx]) break;

					if (SkillGems[idx] != null
						&& SkillGems[idx].SkillGemItemData.SkillGemType == ESkillGemType.SupportGem)
					{
						// TODO: 서폿젬 태그 관련 로직 추가
						skill.Add(SkillGems[idx]);
					}
				}

			}
			if(skill.Count != 0)
				skills.Add(skill);
		}
		return skills;
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

	public override bool UpgradeRarity(Define.ERarity curRarity, Define.ERarity destRarity)
	{
		if(base.UpgradeRarity(curRarity, destRarity) == false)
        {
			return false;
        }

		Rarity = destRarity;

		int prefixNum = 0;
		int suffixNum = 0;

		Debug.Log(destRarity);

        switch (destRarity)
        {
			case ERarity.Normal:
				break;
			case ERarity.Magic:
                {
					int rand = Random.Range(0, 3);
					switch(rand)
                    {
						case 0:
							prefixNum = 1;
							suffixNum = 0;
							break;
						case 1:
							prefixNum = 0;
							suffixNum = 1;
							break;
						case 2:
							prefixNum = 1;
							suffixNum = 1;
							break;
                    }
				}
				break;
			case ERarity.Rare:
                {
					int affixNum = Random.Range(4, 7);

					while(prefixNum + suffixNum < affixNum)
                    {
						if(prefixNum == 3)
                        {
							suffixNum++;
							continue;
                        }
						else if(suffixNum == 3)
                        {
							prefixNum++;
							continue;
						}


						int rand = Random.Range(0, 2);
						if(rand == 0)
                        {
							prefixNum++;
						}
                        else
                        {
							suffixNum++;
						}
					}

				}
				break;
			case ERarity.Unique:
				break;
		}

		List<ModData> prefixList = Managers.Data.PrefixModifierData[ItemSubType];
		List<ModData> suffixList = Managers.Data.SuffixModifierData[ItemSubType];


		Debug.Log(prefixNum +", "+  suffixNum);
		MakeModifiers(PrefixMod, prefixNum - PrefixMod.Count, prefixList);
		MakeModifiers(SuffixMod, suffixNum - SuffixMod.Count, suffixList);

		return true;
	}


	public static EquipmentItem MakeRandomEquipmentItem()
    {
		int itemRand = UnityEngine.Random.Range(0, Managers.Data.EquipmentItemBaseDic.Count);
		float rarityRand = UnityEngine.Random.Range(0, 10);

		ERarity rarity = ERarity.Normal;
		if (rarityRand < 4)
		{
			rarity = ERarity.Normal;
		}
		else if (rarityRand < 8)
		{
			rarity = ERarity.Magic;
		}
		else if (rarityRand < 10)
		{
			rarity = ERarity.Rare;
		}

		List<int> keys = new List<int>(Managers.Data.EquipmentItemBaseDic.Keys);
		var key = keys[itemRand];


		



		return EquipmentItem.MakeEquipmentItem(Managers.Data.EquipmentItemBaseDic[key].DataId, rarity);
	}

	public static EquipmentItem MakeEquipmentItem(int itemDataId, ERarity rarity, List<ESocketColor> socket = null, List<bool> link = null)
    {
		

		var item = Managers.Inventory.MakeItem(itemDataId) as EquipmentItem;

		if(socket == null)
        {
			socket = new();

			int socketRand = UnityEngine.Random.Range(1, item.MaxSocket + 1);

			while (0 < socketRand--)
			{
				float socketColorRnad = UnityEngine.Random.Range(0, 3);
				socket.Add((ESocketColor)socketColorRnad);
			}
		}


		

		item.Socket = socket;

		if(link == null)
        {
			link = new();

			for (int i = 0; i < socket.Count - 1; i++)
			{
				int linkRand = UnityEngine.Random.Range(0, 2);

				link.Add(linkRand == 0);
			}
		}

		
		item.Link = link;


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
