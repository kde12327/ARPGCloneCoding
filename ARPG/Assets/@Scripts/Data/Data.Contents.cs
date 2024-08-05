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
}