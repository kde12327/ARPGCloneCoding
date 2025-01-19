using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Assets.PixelFantasy.Common.Scripts;

using static Define;
using UnityEngine.U2D.Animation;

public class Creature : BaseObject
{
    public SkillComponent Skills { get; protected set; }

    public Data.CreatureData CreatureData { get; protected set; }
    public EffectComponent Effects { get; set; }

    [SerializeField]
    public StatComponent Stats;

    protected float StopTheshold = 0.02f;

    protected Vector3 PrevPosition;



    #region Stats
    public float _hp;
    public float Hp 
    {
        get { return _hp; } 
        set
        {
            _hp = value;
            if (_hp > Stats.GetStat(Stat.Life).Value)
                _hp = Stats.GetStat(Stat.Life).Value;
            OnHpChanged?.Invoke(_hp / Stats.GetStat(Stat.Life).Value);
        } 
    }
    public event Action<float> OnHpChanged;

    protected float _energyShield;
    public float EnergyShield
    {
        get { return _energyShield; }
        set
        {
            _energyShield = value;
            if (_energyShield > Stats.GetStat(Stat.EnergySheild).Value)
                _energyShield = Stats.GetStat(Stat.EnergySheild).Value;
            if(Stats.GetStat(Stat.EnergySheild).Value == 0)
            {
                OnEnergyShieldChanged?.Invoke(0);
            }
            else
            {
                OnEnergyShieldChanged?.Invoke(_energyShield / Stats.GetStat(Stat.EnergySheild).Value);
            }
        }
    }
    public event Action<float> OnEnergyShieldChanged;

    float EnergyResetTime = 4.0f;
    float EnergyResetTimer = 0.0f;
    float EnergyRegen = 1.0f;

    #endregion

    public void OnStatChanged()
    {
        OnHpChanged?.Invoke(Hp / Stats.GetStat(Stat.Life).Value);

        if (Stats.GetStat(Stat.EnergySheild).Value == 0)
        {
            OnEnergyShieldChanged?.Invoke(0);
        }
        else
        {
            OnEnergyShieldChanged?.Invoke(_energyShield / Stats.GetStat(Stat.EnergySheild).Value);
        }
    }


    protected ECreatureState _creatureState = ECreatureState.Idle;

    //protected float Speed = 0.0f;
    public virtual ECreatureState CreatureState
    {
        get { return _creatureState; }
        set
        {
            if (_creatureState != value)
            {
                _creatureState = value;
                UpdateAnimation();
            }
        }
    }

    public event Action OnAnimAttacked;
    public event Action OnAnimAttackEnded;


    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public virtual void SetInfo(int templateID)
    {
        DataTemplateID = templateID;

        if(ObjectType == EObjectType.Monster)
        {
            CreatureData = Managers.Data.MonsterDic[templateID];
            _character.Body.GetComponent<SpriteLibrary>().spriteLibraryAsset = Managers.Resource.Load<SpriteLibraryAsset>(CreatureData.SpriteLibrary);
        }
        else if(ObjectType == EObjectType.Player)
        {
            CreatureData = Managers.Data.PlayerDic[templateID];
        }

        gameObject.name = $"{CreatureData.DataId}_{CreatureData.DescriptionTextID}";

        // Collider
        Collider.offset = new Vector2(CreatureData.ColliderOffsetX, CreatureData.ColliderOffsetY);
        Collider.radius = CreatureData.ColliderRadius;

        Stats = new();
        Stats.SetInfo(this);

        // RigidBody
        //RigidBody.mass = CreatureData.Mass;
        RigidBody.mass = 0;

        // Spine
        //SetSpineAnimation(CreatureData.SkeletonDataID, SortingLayers.CREATURE);

        // Skills
        // CreatureData.SkillIdList;

        // Stat
        Stats.SetStat(Stat.Life, CreatureData.MaxHp, this);
        Stats.SetStat(Stat.Atk, CreatureData.Atk, this);
        Stats.SetStat(Stat.AttackSpeedRate, 1, this);
        Stats.SetStat(Stat.CriRate, 0, this);

        Hp = Stats.GetStat(Stat.Life).Value;

        // State
        //CreatureState = ECreatureState.Idle;

        //Effect
        Effects = gameObject.AddComponent<EffectComponent>();
        Effects.SetInfo(this);

        // Map
        //StartCoroutine(CoLerpToCellPos());

        // HPBar
        GameObject go = Managers.Resource.Instantiate("HPBar");
        HPBar hp = go.GetComponent<HPBar>();
        hp.SetInfo(this);

        PrevPosition = transform.position;
    }

    protected virtual void Update()
    {
        if (CreatureState != ECreatureState.Skill)
            LerpToCellPos(CreatureData.MoveSpeed);

        
        

        if (EnergyResetTimer > 0)
        {
            EnergyResetTimer -= Time.deltaTime;
        }
        else
        {
            EnergyShield += EnergyRegen * Time.deltaTime;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (Anim != null)
        {
            //Anim.SetFloat("Speed", (transform.position - PrevPosition).magnitude * 10);
            //Debug.Log((transform.position - PrevPosition).magnitude * 10);
            PrevPosition = transform.position;
        }
    }

    protected override void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case ECreatureState.Idle:
                if(_character != null)
                {
                    _animation.Idle();
                }
                PlayAnimation(0, AnimName.IDLE, true);

                break;
            case ECreatureState.Skill:
                //PlayAnimation(0, AnimName.ATTACK_A, true);
                break;
            case ECreatureState.Move:
                if (_character != null)
                {
                    _animation.Run();

                }
                PlayAnimation(0, AnimName.MOVE, true);
                break;
            case ECreatureState.OnDamaged:
                if (_character != null)
                {
                    _animation.Hit();
                }
                PlayAnimation(0, AnimName.IDLE, true);
                if(Skills.CurrentSkill != null)
                {
                    Skills.CurrentSkill.CancelSkill();
                }
                break;
            case ECreatureState.Dead:
                if (_character != null)
                {
                    _animation.Die();
                }
                PlayAnimation(0, AnimName.DEAD, true);
                RigidBody.simulated = false;
                break;
            default:
                break;
        }
    }

    public void OnAnimAttack()
    {
        if(OnAnimAttacked != null)
        {
            OnAnimAttacked.Invoke();
        }
    }
    
    public void OnAnimAttackEnd()
    {

        if(OnAnimAttackEnded != null)
        {
            Anim.speed = 1.0f;
            OnAnimAttackEnded.Invoke();
        }
    }

    

    #region AI

    public float UpdateAITick { get; protected set; } = 0.0f;

    protected IEnumerator CoUpdateState()
    {
        while (true)
        {
            switch (CreatureState)
            {
                case ECreatureState.Idle:
                    UpdateIdle();
                    break;
                case ECreatureState.Move:
                    UpdateMove();
                    break;
                case ECreatureState.Skill:
                    UpdateSkill();
                    break;
                case ECreatureState.OnDamaged:
                    UpdateOnDamaged();
                    break;
                case ECreatureState.Dead:
                    UpdateDead();
                    break;
            }
            if (UpdateAITick > 0)
                yield return new WaitForSeconds(UpdateAITick);
            else
                yield return null;
        }

        
    }

    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMove() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateOnDamaged() { }
    protected virtual void UpdateDead() { }

    #endregion

    #region Battle
    public override void OnDamaged(BaseObject attacker, SkillBase skill)
    {
        base.OnDamaged(attacker, skill);

        if (attacker.IsValid() == false)
            return;

        Creature creature = attacker as Creature;
        if (creature == null)
            return;

        float finalDamage = creature.Stats.GetStat(Stat.Atk).Value 
            * skill.DamageMultiplier 
            * creature.Stats.GetStat(Stat.Damage).Value / 100.0f
            * creature.Stats.GetStat(Stat.MeleePhysicalDamagePercent).Value / 100.0f;
        bool isCritical = UnityEngine.Random.Range(0, 100) < creature.Stats.GetStat(Stat.CriRate).Value;

        if (isCritical)
        {
            finalDamage *= 2;
        }

        float leftDamage = finalDamage;

        if (EnergyShield > 0)
        {
            if(leftDamage > EnergyShield)
            {
                leftDamage -= EnergyShield;
                EnergyShield = 0;
            }
            else
            {
                EnergyShield -= leftDamage;
                leftDamage = 0;
            }
        }

        Hp = Mathf.Clamp(Hp - leftDamage, 0, Stats.GetStat(Stat.Life).Value);

        EnergyResetTimer = EnergyResetTime;

        Managers.Object.ShowDamageFont(CenterPosition, finalDamage, transform, isCritical);
        //Debug.Log(CreatureData.DescriptionTextID + ": " + Hp + "/" + MaxHp.Value);

        if (Hp <= 0)
        {
            OnDead(attacker, skill);
            CreatureState = ECreatureState.Dead;
        }

        // Effect 적용
        if (skill.SkillData.EffectIds != null)
            Effects.GenerateEffects(skill.SkillData.EffectIds.ToArray(), EEffectSpawnType.Skill);
    }

    public override void OnDead(BaseObject attacker, SkillBase skill)
    {
        base.OnDead(attacker, skill);

        if(attacker == Managers.Object.Player)
        {
            Managers.Quest.OnMonsterDead(CreatureData.DataId);
        }
    }
    #endregion

    #region Wait
    protected Coroutine _coWait;
    protected void StartWait(float seconds)
    {
        CancelWait();
        _coWait = StartCoroutine(CoWait(seconds));
    }
    IEnumerator CoWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coWait = null;
    }
    protected void CancelWait()
    {
        if (_coWait != null)
            StopCoroutine(_coWait);
        _coWait = null;
    }
    #endregion



    #region Map
    public EFindPathResult FindPathAndMoveToCellPos(Vector3 destWorldPos, int maxDepth, bool forceMoveCloser = false)
    {
        Vector3Int destCellPos = Managers.Map.World2Cell(destWorldPos);
        return FindPathAndMoveToCellPos(destCellPos, maxDepth, forceMoveCloser);
    }

    public EFindPathResult FindPathAndMoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
    {
        if (LerpCellPosCompleted == false)
            return EFindPathResult.Fail_LerpCell;

        // A*
        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destCellPos, maxDepth);
        if (path.Count < 2)
            return EFindPathResult.Fail_NoPath;

        // 가까운 두 좌표에서 이동이 반복되는 경우
        if (forceMoveCloser)
        {
            Vector3Int diff1 = CellPos - destCellPos;
            Vector3Int diff2 = path[1] - destCellPos;
            if (diff1.sqrMagnitude <= diff2.sqrMagnitude)
                return EFindPathResult.Fail_NoPath;
        }

        Vector3Int dirCellPos = path[1] - CellPos;
        //Vector3Int dirCellPos = destCellPos - CellPos;
        Vector3Int nextPos = CellPos + dirCellPos;

        if (Managers.Map.MoveTo(this, nextPos) == false)
            return EFindPathResult.Fail_MoveTo;

        return EFindPathResult.Success;
    }

    public bool MoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
    {
        if (LerpCellPosCompleted == false)
            return false;

        return Managers.Map.MoveTo(this, destCellPos);
    }

    protected IEnumerator CoLerpToCellPos()
    {
        while (true)
        {
            /*Player player = this as Player;
            if (player != null)
            {
                float div = 5;
                Vector3 campPos = Managers.Object.Camp.Destination.transform.position;
                Vector3Int campCellPos = Managers.Map.World2Cell(campPos);
                float ratio = Math.Max(1, (CellPos - campCellPos).magnitude / div);

                LerpToCellPos(CreatureData.MoveSpeed * ratio);
            }
            else*/
            if(CreatureState != ECreatureState.Skill)
                LerpToCellPos(CreatureData.MoveSpeed);

            yield return null;
        }
    }
    #endregion


    #region Misc
    protected bool IsValid(BaseObject bo)
    {
        return bo.IsValid();
    }
    #endregion
}
