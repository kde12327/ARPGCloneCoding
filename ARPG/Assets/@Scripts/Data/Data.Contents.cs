using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

namespace Data
{
	#region CreatureData
	[Serializable]
	public class CreatureData
	{
		public int DataId;
		public string DescriptionTextID;
		public string PrefabLabel;
		public float ColliderOffsetX;
		public float ColliderOffsetY;
		public float ColliderRadius;
		public float MaxHp;
		public float UpMaxHpBonus;
		public float Atk;
		public float AtkRange;
		public float AtkBonus;
		public float MoveSpeed;
		public float CriRate;
		public float CriDamage;
		public string IconImage;
		public string SkeletonDataID;
		public List<int> SkillIdList = new List<int>();
	}

	#endregion

	#region MonsterData
	[Serializable]
	public class MonsterData : CreatureData
    {
		public int DropItemId;
	}

	[Serializable]
	public class MonsterDataLoader : ILoader<int, MonsterData>
    {
		public List<MonsterData> monsters = new List<MonsterData>();

		public Dictionary<int, MonsterData> MakeDict()
		{
			Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
			foreach (MonsterData monster in monsters)
				dict.Add(monster.DataId, monster);
			return dict;
		}
	}

	#endregion

	#region PlayerData
	[Serializable]
	public class PlayerData : CreatureData
	{
	}

	[Serializable]
	public class PlayerDataLoader : ILoader<int, PlayerData>
	{
		public List<PlayerData> players = new List<PlayerData>();

		public Dictionary<int, PlayerData> MakeDict()
		{
			Dictionary<int, PlayerData> dict = new Dictionary<int, PlayerData>();
			foreach (PlayerData player in players)
				dict.Add(player.DataId, player);
			return dict;
		}
	}

	#endregion


	#region SkillData
	[Serializable]
	public class SkillData
	{
		public int DataId;
		public string Name;
		public string ClassName;
		public string Description;
		public int ProjectileId;
		public string PrefabLabel;
		public string IconLabel;
		public string AnimName;
		public float CoolTime;
		public float DamageMultiplier;
		public float Duration;
		public float AnimImpactDuration;
		public string CastingSound;
		public float SkillRange;
		public float ScaleMultiplier;
		public int TargetCount;
		public List<int> EffectIds = new List<int>();
		public int NextLevelId;
		public int AoEId;
		public EEffectSize EffectSize;
	}

	[Serializable]
	public class SkillDataLoader : ILoader<int, SkillData>
	{
		public List<SkillData> skills = new List<SkillData>();

		public Dictionary<int, SkillData> MakeDict()
		{
			Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
			foreach (SkillData skill in skills)
				dict.Add(skill.DataId, skill);
			return dict;
		}
	}
	#endregion

	#region SupportData
	[Serializable]
	public class SupportData
	{
		public int DataId;
		public string Name;
		public List<String> Tag;
		public float ManaMultiplier;
		public float DamageMultiplier;
		public float AttackSpeedMultiplier;
		public float AreaOfEffectIncreased;
	}

	[Serializable]
	public class SupportDataLoader : ILoader<int, SupportData>
	{
		public List<SupportData> supports = new List<SupportData>();

		public Dictionary<int, SupportData> MakeDict()
		{
			Dictionary<int, SupportData> dict = new Dictionary<int, SupportData>();
			foreach (SupportData support in supports)
				dict.Add(support.DataId, support);
			return dict;
		}
	}

	#endregion

	#region ProjectileData
	[Serializable]
	public class ProjectileData
	{
		public int DataId;
		public string Name;
		public string ClassName;
		public string ComponentName;
		public string ProjectileSpriteName;
		public string PrefabLabel;
		public float Duration;
		public float HitSound;
		public float ProjRange;
		public float ProjSpeed;
	}

	[Serializable]
	public class ProjectileDataLoader : ILoader<int, ProjectileData>
	{
		public List<ProjectileData> projectiles = new List<ProjectileData>();

		public Dictionary<int, ProjectileData> MakeDict()
		{
			Dictionary<int, ProjectileData> dict = new Dictionary<int, ProjectileData>();
			foreach (ProjectileData projectile in projectiles)
				dict.Add(projectile.DataId, projectile);
			return dict;
		}
	}
	#endregion

	#region Modifier
	[Serializable]
	public class ModData : IComparable<ModData>
	{
		public int DataId;
		public string ModId;
		public string DescriptionTextID;
		public string Family;
		public EItemType ItemType;
		public EGenerateType GenerationType;
		public int ReqLevel;
		public List<String> Stats = new();
		public List<float> MinMaxValues = new();
		public List<EItemSubType> SpawnTags = new();
		public int Weight = new();

        public int CompareTo(ModData other)
        {
            return ReqLevel - other.ReqLevel;
        }
    }

	[Serializable]
	public class ModDataLoader : ILoader<int, ModData>
	{
		public List<ModData> mods = new List<ModData>();
		public Dictionary<int, ModData> MakeDict()
		{
			Dictionary<int, ModData> dict = new Dictionary<int, ModData>();
			foreach (ModData mod in mods)
				dict.Add(mod.DataId, mod);
			return dict;
		}
	}

	#endregion

	#region Item
	// Equipment.Weapon.Dagger
	// Consumable.Potion.Hp
	// EItemGroupType.

	[Serializable]
	public class BaseData
	{
		public int DataId;
	}

	[Serializable]
	public class ItemData : BaseData
	{
		public string Name;
		public EItemType ItemType;
		public EItemSubType ItemSubType;
		public string Icon;
	}

	[Serializable]
	public class EquipmentItemBaseData:ItemData
	{
		public int DropLevel;
		public List<String> RequireStats = new();
		public List<int> RequireStatValues = new();
		public List<String> DefaultOptions = new();
		public List<float> DefaultMinMaxValues = new();
		public List<String> ImplicitOptions	 = new();
		public List<float> ImplicitMinMaxValues = new();

	}

	


	[Serializable]
	public class EquipmentItemBaseDataLoader : ILoader<int, EquipmentItemBaseData>
	{
		public List<EquipmentItemBaseData> itemBases = new List<EquipmentItemBaseData>();
		public Dictionary<int, EquipmentItemBaseData> MakeDict()
		{
			Dictionary<int, EquipmentItemBaseData> dict = new Dictionary<int, EquipmentItemBaseData>();
			foreach (EquipmentItemBaseData itemBase in itemBases)
				dict.Add(itemBase.DataId, itemBase);
			return dict;
		}
	}

	[Serializable]
	public class ConsumableItemData : ItemData
	{
		public EConsumableEffectType EffectType;
		public int StackSize;
	}

	[Serializable]
	public class ConsumableItemDataLoader : ILoader<int, ConsumableItemData>
	{
		public List<ConsumableItemData> items = new List<ConsumableItemData>();
		public Dictionary<int, ConsumableItemData> MakeDict()
		{
			Dictionary<int, ConsumableItemData> dict = new Dictionary<int, ConsumableItemData>();
			foreach (ConsumableItemData item in items)
				dict.Add(item.DataId, item);
			return dict;
		}
	}

	[Serializable]
	public class SkillGemItemData : ItemData
	{
		public ESkillGemType SkillGemType;
		public int SkillId;
		public ESocketColor SkillGemColor;
	}

	[Serializable]
	public class SkillGemItemDataLoader : ILoader<int, SkillGemItemData>
	{
		public List<SkillGemItemData> items = new List<SkillGemItemData>();
		public Dictionary<int, SkillGemItemData> MakeDict()
		{
			Dictionary<int, SkillGemItemData> dict = new Dictionary<int, SkillGemItemData>();
			foreach (SkillGemItemData item in items)
				dict.Add(item.DataId, item);
			return dict;
		}
	}


	public class FlaskItemBaseData : ItemData
	{
		public EFlaskType FlaskType;
		public string FlaskSprite;
		public string FlaskFillSprite;
		public string EffectName;
		public float EffectValue;
		public float EffectTime;
		public float MaximumCharge;
		public float ChargePerUse; 
	}

	[Serializable]
	public class FlaskItemBaseDataLoader : ILoader<int, FlaskItemBaseData>
	{
		public List<FlaskItemBaseData> items = new List<FlaskItemBaseData>();
		public Dictionary<int, FlaskItemBaseData> MakeDict()
		{
			Dictionary<int, FlaskItemBaseData> dict = new Dictionary<int, FlaskItemBaseData>();
			foreach (FlaskItemBaseData item in items)
				dict.Add(item.DataId, item);
			return dict;
		}
	}

	#endregion

	#region PassiveSkill
	[Serializable]
	public class PassiveSkillData
	{
		public int DataId;
		public string NameDescriptionTextID;
		public string ContentDescriptionTextID;
		public string Icon;
		public List<String> StatNames = new List<String>();
		public List<float> StatValues = new List<float>();
		public List<int> LinkIds = new List<int>();
		public int StartNode; // 0 - nope, 1 - startnode
	}

	[Serializable]
	public class PassiveSkillDataLoader : ILoader<int, PassiveSkillData>
	{
		public List<PassiveSkillData> passiveSkills = new List<PassiveSkillData>();
		public Dictionary<int, PassiveSkillData> MakeDict()
		{
			Dictionary<int, PassiveSkillData> dict = new Dictionary<int, PassiveSkillData>();
			foreach (PassiveSkillData passive in passiveSkills)
				dict.Add(passive.DataId, passive);
			return dict;
		}
	}
	#endregion

	#region Env
	[Serializable]
	public class EnvData
	{
		public int DataId;
		public string DescriptionTextID;
		public string PrefabLabel;
		public float MaxHp;
		public int IsInteractable;
		public List<String> SkeletonDataIDs = new List<String>();
	}

	[Serializable]
	public class EnvDataLoader : ILoader<int, EnvData>
	{
		public List<EnvData> envs = new List<EnvData>();
		public Dictionary<int, EnvData> MakeDict()
		{
			Dictionary<int, EnvData> dict = new Dictionary<int, EnvData>();
			foreach (EnvData env in envs)
				dict.Add(env.DataId, env);
			return dict;
		}
	}
	#endregion

	#region Npc
	[Serializable]
	public class NpcData
	{
		public int DataId;
		public string DescriptionTextID;
		public string SkeletonDataID;
		public List<EInteractionType> InteractionTypes;
		public List<int> ScriptIds;
		public List<EVendorType> VendorList;
	}

	[Serializable]
	public class NpcDataLoader : ILoader<int, NpcData>
	{
		public List<NpcData> npcs = new List<NpcData>();
		public Dictionary<int, NpcData> MakeDict()
		{
			Dictionary<int, NpcData> dict = new Dictionary<int, NpcData>();
			foreach (NpcData npc in npcs)
				dict.Add(npc.DataId, npc);
			return dict;
		}
	}


	#endregion

	#region QuestObject
	[Serializable]
	public class QuestObjectData
	{
		public int DataId;
		public string DescriptionTextId;
		public string Icon;
		public int RequiredQuestId;
		public EQuestObjectItemType QuestObjectItemType;
	}

	[Serializable]
	public class QuestObjectDataLoader : ILoader<int, QuestObjectData>
	{
		public List<QuestObjectData> questObjects = new List<QuestObjectData>();
		public Dictionary<int, QuestObjectData> MakeDict()
		{
			Dictionary<int, QuestObjectData> dict = new Dictionary<int, QuestObjectData>();
			foreach (QuestObjectData questObject in questObjects)
				dict.Add(questObject.DataId, questObject);
			return dict;
		}
	}
	#endregion

	#region EffectData
	[Serializable]
	public class EffectData
	{
		public int DataId;
		public string Name;
		public string ClassName;
		public string DescriptionTextID;
		public string SkeletonDataID;
		public string IconLabel;
		public string SoundLabel;
		public float Amount;
		public float PercentAdd;
		public float PercentMult;
		public float TickTime;
		public float TickCount;
		public EEffectType EffectType;
	}

	[Serializable]
	public class EffectDataLoader : ILoader<int, EffectData>
	{
		public List<EffectData> effects = new List<EffectData>();
		public Dictionary<int, EffectData> MakeDict()
		{
			Dictionary<int, EffectData> dict = new Dictionary<int, EffectData>();
			foreach (EffectData effect in effects)
				dict.Add(effect.DataId, effect);
			return dict;
		}
	}
	#endregion


	#region Portal
	[Serializable]
	public class PortalData: EnvData
	{
		public string MapName;
		public int DestPortalId;
	}

	[Serializable]
	public class PortalDataLoader : ILoader<int, PortalData>
	{
		public List<PortalData> portals = new List<PortalData>();
		public Dictionary<int, PortalData> MakeDict()
		{
			Dictionary<int, PortalData> dict = new Dictionary<int, PortalData>();
			foreach (PortalData portal in portals)
				dict.Add(portal.DataId, portal);
			return dict;
		}
	}
	#endregion

	#region Quest
	[Serializable]
	public class QuestData
	{
		public int DataId;
		public string NameDescriptionTextID;
		public string ContentDescriptionTextID;
		public EQuestType QuestType;
		public string QuestMapId;
		public int QuestTargetId;
		public EQuestRewardType QuestRewardType;
		public int NextQuestId;
	}

	[Serializable]
	public class QuestDataLoader : ILoader<int, QuestData>
	{
		public List<QuestData> quests = new List<QuestData>();
		public Dictionary<int, QuestData> MakeDict()
		{
			Dictionary<int, QuestData> dict = new Dictionary<int, QuestData>();
			foreach (QuestData quest in quests)
				dict.Add(quest.DataId, quest);
			return dict;
		}
	}
	#endregion
}