using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Monster : Creature
{
    public Data.MonsterData MonsterData { get { return (Data.MonsterData)CreatureData; } }

    public override ECreatureState CreatureState {
        get { return base.CreatureState; }
        set
        {
            if (_creatureState != value)
            {
                base.CreatureState = value;
                state = (int)value;
                switch (value)
                {
                    case ECreatureState.Idle:
                        UpdateAITick = 0.5f;
                        break;
                    case ECreatureState.Move:
                        UpdateAITick = 0.0f;
                        break;
                    case ECreatureState.Skill:
                        UpdateAITick = 0.0f;
                        break;
                    case ECreatureState.Dead:
                        UpdateAITick = 1.0f;
                        break;
                }
            }
        } 
    }

    [SerializeField]
    int state;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Monster;


        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        // State
        CreatureState = ECreatureState.Idle;

        // Skill
        Skills = gameObject.GetOrAddComponent<SkillComponent>();
        Skills.SetInfo(this, CreatureData.SkillIdList);
    }

    private void Start()
    {
        _initPos = transform.position;
    }

    protected override void Update()
    {
        base.Update();

        UpdateAITick -= Time.deltaTime;
        if (UpdateAITick < 0)
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
        }
        
    }
    /*private void OnEnable()
    {
        Debug.Log("d");
        CreatureState = ECreatureState.Idle;
        //transform.position = _destPos;
        //LerpCellPosCompleted = true;
    }*/

    #region AI

    public float SearchDistance { get; private set; } = 8.0f;
    public float AttackDistance { get; private set; } = 4.0f;
    public BaseObject Target { get; protected set; }
    Vector3 _destPos;
    Vector3 _initPos;

    protected override void UpdateIdle()
    {
        /*if(Hp != MaxHp.Value)
        Debug.Log("Idle");*/


        // Patrol
        {
            //Debug.Log("Patrol");

            int patrolPercent = 10;
            int rand = UnityEngine.Random.Range(0, 100);
            if (rand <= patrolPercent)
            {
                _destPos = _initPos + new Vector3(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-2, 2));
                CreatureState = ECreatureState.Move;
                return;
            }
        }

        // Search Player
        {
            //Debug.Log("Search");
            Creature creature = FindClosestInRange(MONSTER_SEARCH_DISTANCE, Managers.Object.Player, func: IsValid) as Creature;
            if (creature != null)
            {
                Target = creature;
                CreatureState = ECreatureState.Move;
                return;
            }

        }
    }
    protected override void UpdateMove()
    {
        /*if(Hp != MaxHp.Value)
            Debug.Log("Move");*/

        if(Target.IsValid() == false)
        {

            Creature creature = FindClosestInRange(MONSTER_SEARCH_DISTANCE, Managers.Object.Player, func: IsValid) as Creature;
            if (creature != null)
            {
                Target = creature;
                CreatureState = ECreatureState.Move;
                return;
            }

            EFindPathResult result = FindPathAndMoveToCellPos(_destPos, MONSTER_DEFAULT_MOVE_DEPTH);
            if(result != EFindPathResult.Success)
            {
                //Debug.Log(result);
            }

            if (LerpCellPosCompleted)
            {
                CreatureState = ECreatureState.Idle;
                return;
            }
        }
        else
        {
            // Chase
            SkillBase skill = Skills.CurrentSkill;
            ChaseOrAttackTarget(MONSTER_SEARCH_DISTANCE, skill);

            // 너무 멀어지면 포기.
            if (Target.IsValid() == false)
            {
                Target = null;
                _destPos = _initPos;
                return;
            }
        }
    }
    protected override void UpdateSkill()
    {
        /*if (Hp != MaxHp.Value)
            Debug.Log("Skill");*/

        if (Target.IsValid() == false)
        {
            Target = null;
            _destPos = _initPos;
            CreatureState = ECreatureState.Move;
            return;
        }

    }

    protected override void UpdateOnDamaged()
    {
        /*if (Hp != MaxHp.Value)
            Debug.Log("UpdateOnDamaged");*/


    }

    protected override void UpdateDead()
    {
        //Debug.Log("Dead");


    }


    #endregion

    #region Battle
    public override void OnDamaged(BaseObject attacker, SkillBase skill)
    {
        base.OnDamaged(attacker, skill);
    }

    public override void OnDead(BaseObject attacker, SkillBase skill)
    {
        base.OnDead(attacker, skill);

        // TODO : Drop Item
        if(attacker.ObjectType == EObjectType.Player)
        {
            Player player = attacker as Player;
            player.Exp++;

            {
                var item =  EquipmentItem.MakeRandomEquipmentItem();

                var itemHolder = Managers.Object.Spawn<ItemHolder>(transform.position, 0);
                itemHolder.SetInfo(0, item, transform.position);
            }
        }


        Managers.Object.Despawn(this);
    }

    protected BaseObject FindClosestInRange(float range, BaseObject obj, Func<BaseObject, bool> func = null)
    {
        if (obj == null) return null;

        BaseObject target = null;
        float searchDistanceSqr = range * range;

        Vector3 dir = obj.transform.position - transform.position;
        float distToTargetSqr = dir.sqrMagnitude;

        // 서치 범위보다 멀리 있으면 스킵.
        if (distToTargetSqr > searchDistanceSqr)
            return null;

        // 추가 조건
        if (func != null && func.Invoke(obj) == false)
            return null;

        target = obj;


        return target;
    }

    protected void ChaseOrAttackTarget(float chaseRange, SkillBase skill)
    {
        Vector3 dir = (Target.transform.position - transform.position);
        float distToTargetSqr = dir.sqrMagnitude;

        // TEMP
        float attackRange = MONSTER_DEFAULT_MELEE_ATTACK_RANGE;
        if (skill.SkillData.ProjectileId != 0)
            attackRange = MONSTER_DEFAULT_RANGED_ATTACK_RANGE;

        float finalAttackRange = attackRange + Target.ColliderRadius + ColliderRadius;
        float attackDistanceSqr = finalAttackRange * finalAttackRange;

        if (distToTargetSqr <= attackDistanceSqr)
        {
            // 공격 범위 이내로 들어왔다면 공격.
            CreatureState = ECreatureState.Skill;
            skill.DoSkill(Target.transform.position);
            return;
        }
        else
        {
            // 공격 범위 밖이라면 추적.
            FindPathAndMoveToCellPos(Target.transform.position, MONSTER_DEFAULT_MOVE_DEPTH);

            // 너무 멀어지면 포기.
            float searchDistanceSqr = chaseRange * chaseRange;
            if (distToTargetSqr > searchDistanceSqr)
            {
                Target = null;
                CreatureState = ECreatureState.Move;
            }
            return;
        }
    }
    #endregion

}
