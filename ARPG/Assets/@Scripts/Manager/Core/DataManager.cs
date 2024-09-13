using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public interface ILoader<Key, Value>
{
	Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
	public Dictionary<int, Data.MonsterData> MonsterDic { get; private set; } = new Dictionary<int, Data.MonsterData>();
	public Dictionary<int, Data.PlayerData> PlayerDic { get; private set; } = new Dictionary<int, Data.PlayerData>();
	public Dictionary<int, Data.SkillData> SkillDic { get; private set; } = new Dictionary<int, Data.SkillData>();
	public Dictionary<int, Data.SupportData> SupportDic { get; private set; } = new Dictionary<int, Data.SupportData>();
	public Dictionary<int, Data.ProjectileData> ProjectileDic { get; private set; } = new Dictionary<int, Data.ProjectileData>();
	public Dictionary<int, Data.EnvData> EnvDic { get; private set; } = new Dictionary<int, Data.EnvData>();
	public Dictionary<int, Data.NpcData> NpcDic { get; private set; } = new Dictionary<int, Data.NpcData>();
	public Dictionary<int, Data.EffectData> EffectDic { get; private set; } = new Dictionary<int, Data.EffectData>();
	public Dictionary<int, Data.PortalData> PortalDic { get; private set; } = new Dictionary<int, Data.PortalData>();
	public Dictionary<int, Data.ModData> ModDic { get; private set; } = new Dictionary<int, Data.ModData>();
	public Dictionary<int, Data.ItemData> ItemDic { get; private set; } = new Dictionary<int, Data.ItemData>();
	public Dictionary<int, Data.EquipmentItemBaseData> EquipmentItemBaseDic { get; private set; } = new Dictionary<int, Data.EquipmentItemBaseData>();

	#region Mod
	// Modifier를 미리 리스트로 만들기
	public Dictionary<EItemSubType, List<Data.ModData>> PrefixModifierData { get; set; } = new Dictionary<EItemSubType, List<Data.ModData>>();
	public Dictionary<EItemSubType, List<Data.ModData>> SuffixModifierData { get; set; } = new Dictionary<EItemSubType, List<Data.ModData>>();


    #endregion


    public void Init()
	{
		MonsterDic = LoadJson<Data.MonsterDataLoader, int, Data.MonsterData>("MonsterData").MakeDict();
		PlayerDic = LoadJson<Data.PlayerDataLoader, int, Data.PlayerData>("PlayerData").MakeDict();
		SkillDic = LoadJson<Data.SkillDataLoader, int, Data.SkillData>("SkillData").MakeDict();
		SupportDic = LoadJson<Data.SupportDataLoader, int, Data.SupportData>("SupportData").MakeDict();
		ProjectileDic = LoadJson<Data.ProjectileDataLoader, int, Data.ProjectileData>("ProjectileData").MakeDict();
		EnvDic = LoadJson<Data.EnvDataLoader, int, Data.EnvData>("EnvData").MakeDict();
		NpcDic = LoadJson<Data.NpcDataLoader, int, Data.NpcData>("NpcData").MakeDict();
		EffectDic = LoadJson<Data.EffectDataLoader, int, Data.EffectData>("EffectData").MakeDict();
		PortalDic = LoadJson<Data.PortalDataLoader, int, Data.PortalData>("PortalData").MakeDict();
		ModDic = LoadJson<Data.ModDataLoader, int, Data.ModData>("ModData").MakeDict();
		EquipmentItemBaseDic = LoadJson<Data.EquipmentItemBaseDataLoader, int, Data.EquipmentItemBaseData>("EquipmentItemBaseData").MakeDict();

		ItemDic.Clear();

		foreach (var item in EquipmentItemBaseDic)
			ItemDic.Add(item.Key, item.Value);

		PrefixModifierData.Clear();
		SuffixModifierData.Clear();

		Dictionary<EItemSubType, PriorityQueue<Data.ModData>> ModQD = new Dictionary<EItemSubType, PriorityQueue<Data.ModData>>();


		// init
		for(int i = 1; i < (int)EItemSubType.Length; i++)
        {
			PriorityQueue<Data.ModData> queue = new();
			ModQD.Add((EItemSubType)i, queue);

			List<Data.ModData> list = new();
			PrefixModifierData.Add((EItemSubType)i, list);
			SuffixModifierData.Add((EItemSubType)i, list);
		}

		// push mods in queue
		foreach (var _mod in ModDic)
        {
			var mod = _mod.Value;

			for (int i = 0; i < mod.SpawnTags.Count; i++)
			{
				ModQD[mod.SpawnTags[i]].Push(mod);
			}
        }

		// queue to list
		for (int i = 1; i < (int)EItemSubType.Length; i++)
		{
			var modQueue = ModQD[(EItemSubType)i];
			var prefixList = PrefixModifierData[(EItemSubType)i];
			var suffixList = SuffixModifierData[(EItemSubType)i];
			while (modQueue.Count > 0)
            {
				var mod = modQueue.Pop();
                switch (mod.GenerationType)
                {
					case EGenerateType.Prefix:
						prefixList.Add(mod);
						break;
					case EGenerateType.Suffix:
						suffixList.Add(mod);
						break;
				}
            }
		}
	}

	private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
	{
		TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
		return JsonConvert.DeserializeObject<Loader>(textAsset.text);
	}
}
