using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{

    public enum EMouseState
    {
        None,
        MouseDown,
        MouseUp,
        MouseHolding,
    }
    public enum EKeyState
    {
        None = -1,
        Skill00 = 0,
        Skill01 = 1,
        Skill02 = 2,
        Skill03 = 3,
        Skill04 = 4,
        Skill05 = 5,
    }

    public enum EScene
    {
        Unknown,
        TitleScene,
        GameScene,
    }

    public enum EUIEvent
    {
        Click,
        PointerDown,
        PointerUp,
        Drag,
        PointerEnter,
        PointerExit,
    }

    public enum ESound
    {
        Bgm,
        Effect,
        Max,
    }

    public enum EObjectType
    {
        None,
        Player,
        Monster,
        Npc,
        Projectile,
        Env,
        Effect,
        Interactive,
        Portal,
        ItemHolder
    }


    public enum ECreatureState
    {
        None,
        Idle,
        Move,
        Skill,
		OnDamaged,
        Dead,
    }

    public enum EEnvState
    {
        Idle,
        OnDamaged,
        Dead,
    }

    public enum ENpcState
    {
        Idle,
        OnDamaged,
        Dead,
    }


    public enum ELayer
    {
        Default = 0,
        TransparentFX = 1,
        IgnoreRaycast = 2,
        Dummy1 = 3,
        Water = 4,
        UI = 5,
        Player = 6,
        Monster = 7,
        Env = 8,
        Obstacle = 9,
        Projectile = 10,
    }

    public enum EColliderSize
    {
        Small,
        Normal,
        Big
    }

    public enum EFindPathResult
    {
        Fail_LerpCell,
        Fail_NoPath,
        Fail_MoveTo,
        Success,
    }

    public enum ECellCollisionType
    {
        None,
        SemiWall,
        Wall,
    }

    public enum EIndicatorType
    {
        None,
        Cone,
        Rectangle,
    }

    public enum EEffectSize
    {
        CircleSmall,
        CircleNormal,
        CircleBig,
        ConeSmall,
        ConeNormal,
        ConeBig,
    }

    public enum EStatModType
    {
        Add,
        PercentAdd,
        PercentMult,
    }

    public enum EEffectType
    {
        Buff,
        Debuff,
        CrowdControl,
    }

    public enum EEffectSpawnType
    {
        Skill, // 지속시간이 있는 기본적인 이펙트 
        External, // 외부(장판스킬)에서 이펙트를 관리(지속시간에 영향을 받지않음)
    }

    public enum EEffectClearType
    {
        TimeOut, // 시간초과로 인한 Effect 종료
        ClearSkill, // 정화 스킬로 인한 Effect 종료
        TriggerOutAoE, // AoE스킬을 벗어난 종료
        EndOfAirborne, // 에어본이 끝난 경우 호출되는 종료
    }

    public enum EEffectClassName
    {
        Bleeding,
        Poison,
        Ignite,
        Heal,
        AttackBuff,
        MoveSpeedBuff,
        AttackSpeedBuff,
        LifeStealBuff,
        ReduceDmgBuff,
        ThornsBuff,
        Knockback,
        Airborne,
        PullEffect,
        Stun,
        Freeze,
        CleanDebuff,
    }

    public enum EBroadcastEventType
    {
        None,
        KillMonster,
        LevelUp,
    }

    public const float EFFECT_SMALL_RADIUS = 2.5f;
    public const float EFFECT_NORMAL_RADIUS = 4.5f;
    public const float EFFECT_BIG_RADIUS = 5.5f;

    public const int CAMERA_PROJECTION_SIZE = 12;

    public enum EInteractionType
    {
        Warehouse,
        Vendor
    }



    #region Mod
    public enum EGenerateType
    {
        None,
        Prefix,
        Suffix,
        Implicit,

    }


    #endregion

    #region Item

    public enum EItemType
    {
        None,
        Equipment,
        Consumable,
        SkillGem,
        Flask
    }

    public enum EItemSubType
    {
        None,
        Weapon,
        Helmet,
        Gloves,
        Boots,
        BodyArmour,
        Amulet,
        Ring,
        Belt,
        Length
    }

    public enum EEquipSlotType
    {
        None,
        Weapon = 1,
        Helmet = 2,
        Gloves = 3,
        Boots = 4,
        BodyArmour = 5,
        Amulet = 6,
        Ring = 7,
        Belt = 8,
        Flask1 = 9,
        Flask2 = 10,
        Flask3 = 11,
        Flask4 = 12,
        Flask5 = 13,
        EquipMax,

        PlayerInventory = 100,
        WarehouseInventory = 200,
        VendorInventory = 300,
    }

    public enum ERarity
    {
        Normal,
        Magic,
        Rare,
        Unique,
    }

    public enum ESlotState
    {
        Enable,
        Error,
        None
    }

    public enum EVendorType
    {
        Equipment,
        Weapon,
        Accessory,
        Consumable,
    }

    public enum EConsumableEffectType
    {
        IdentifiesItem,
        CreatesPortalToTown,
        UpgradeNormalToMagic,
        UpgradeMagicToRare,
        UpgradeNormalToRare,
    }

    public enum ESkillGemType
    {
        SkillGem,
        SupportGem
    }

    public enum EFlaskType
    {
        Life,
        Mana,
        Utility,
    }

    #endregion

    #region HARD CODING
    public const float PLAYER_ATTACK_POINTX = 1.0f;
    public const float PLAYER_ATTACK_POINTY = -0.5f;
    public const float PLAYER_ATTACK_WIDTH = 6.0f;
    public const float PLAYER_ATTACK_HEIGHT = 3.0f;
    public const float PLAYER_ATTACK_DEBUG_DURATION = 0.25f;

	public const float MONSTER_SEARCH_DISTANCE = 8.0f;
    public const int MONSTER_DEFAULT_MELEE_ATTACK_RANGE = 1;
    public const int MONSTER_DEFAULT_RANGED_ATTACK_RANGE = 5;

    public const int PLAYER_DEFAULT_MOVE_DEPTH = 5;
    public const int MONSTER_DEFAULT_MOVE_DEPTH = 3;


    public const int PLAYER_WIZARD_ID = 201000;
    public const int PLAYER_KNIGHT_ID = 201001;

    public const int MONSTER_SLIME_ID = 202001;
    public const int MONSTER_SPIDER_COMMON_ID = 202002;
    public const int MONSTER_WOOD_COMMON_ID = 202004;
    public const int MONSTER_GOBLIN_ARCHER_ID = 202005;
    public const int MONSTER_BEAR_ID = 202006;

    public const int ENV_TREE1_ID = 300001;
    public const int ENV_TREE2_ID = 301000;

    public const char MAP_TOOL_WALL = '0';
    public const char MAP_TOOL_NONE = '1';
    public const char MAP_TOOL_SEMI_WALL = '2';

    #endregion

}

public static class AnimName
{
    public const string ATTACK_A = "attack";
    public const string ATTACK_B = "attack";
    public const string SKILL_A = "skill";
    public const string SKILL_B = "skill";
    public const string IDLE = "idle";
    public const string MOVE = "move";
    public const string DAMAGED = "hit";
    public const string DEAD = "dead";
    public const string EVENT_ATTACK_A = "event_attack";
    public const string EVENT_ATTACK_B = "event_attack";
    public const string EVENT_SKILL_A = "event_attack";
    public const string EVENT_SKILL_B = "event_attack";
}

public static class SortingLayers
{
    public const int SPELL_INDICATOR = 200;
    public const int CREATURE = 300;
    public const int ENV = 300;
    public const int NPC = 300;
    public const int PROJECTILE = 310;
    public const int SKILL_EFFECT = 310;
    public const int UI_DEFAULT = 400;
    public const int DAMAGE_FONT = 410;

}

public static class Stat
{
    public const string Life = "Life";
    public const string Mana = "Mana";
    public const string EnergySheild = "EnergySheild";
    public const string Atk = "Atk";
    public const string CriRate = "CriRate";
    public const string AttackSpeedRate = "AttackSpeedRate";
    public const string Str = "Str";
    public const string Dex = "Dex";
    public const string Int = "Int";
}

public static class UIColor
{
    public static Color ENABLE = new Color32(17, 132, 52, 156);
    public static Color ERROR = new Color32(132, 23, 17, 156);
    public static Color TRANSPARENT = new Color32(17, 132, 52, 0);

    public static Color NORMAL = new Color32(56, 56, 58, 156);
    public static Color MAGIC = new Color32(23, 23, 38, 156);
    public static Color RARE = new Color32(254, 183, 64, 156);
    public static Color UNIQUE = new Color32(57, 27, 12, 156);

    public static Color NORMALTEXT = new Color32(187, 187, 187, 255);
    public static Color MAGICTEXT = new Color32(135, 135, 254, 255);
    public static Color RARETEXT = new Color32(254, 254, 118, 255);
    public static Color UNIQUETEXT = new Color32(173, 94, 28, 255);
}