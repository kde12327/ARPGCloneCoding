using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Monster : Creature
{

    public override ECreatureState CreatureState {
        get { return base.CreatureState; }
        set
        {
            if (_creatureState != value)
            {
                base.CreatureState = value;
                switch (value)
                {
                    case ECreatureState.Idle:
                        SetRigidBodyVelocity(Vector3.zero);
                        UpdateAITick = 0.5f;
                        break;
                    case ECreatureState.Move:
                        UpdateAITick = 0.0f;
                        break;
                    case ECreatureState.Skill:
                        SetRigidBodyVelocity(Vector3.zero);
                        UpdateAITick = 0.0f;
                        break;
                    case ECreatureState.Dead:
                        UpdateAITick = 1.0f;
                        break;
                }
            }
        } 
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = ECreatureType.Monster;

        StartCoroutine(CoUpdateAI());

        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        // State
        CreatureState = ECreatureState.Idle;
    }

    private void Start()
    {
        _initPos = transform.position;
    }

    #region AI

    public float SearchDistance { get; private set; } = 8.0f;
    public float AttackDistance { get; private set; } = 4.0f;
    Creature _target;
    Vector3 _destPos;
    Vector3 _initPos;

    protected override void UpdateIdle()
    {
        //Debug.Log("Idle");


        // Patrol
        {
            Debug.Log("Patrol");

            int patrolPercent = 10;
            int rand = Random.Range(0, 100);
            if (rand <= patrolPercent)
            {
                _destPos = _initPos + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
                CreatureState = ECreatureState.Move;
                return;
            }
        }

        // Search Player
        {
            Debug.Log("Search");
            float searchDistanceSqr = SearchDistance * SearchDistance;

            Player player = Managers.Object.player;
            if(player != null)
            {
                Vector3 dir = player.transform.position - transform.position;
                float distToTargetSqr = dir.sqrMagnitude;


                if (distToTargetSqr < searchDistanceSqr)
                {
                    _target = player;
                    CreatureState = ECreatureState.Move;
                }

            }
        }
    }
    protected override void UpdateMove()
    {
        //Debug.Log("Move");

        if(_target == null)
        {
            // Patrol or Return
            Vector3 dir = (_destPos - transform.position);

            if (dir.sqrMagnitude <= StopTheshold)
            {
                CreatureState = ECreatureState.Idle;
                return;
            }

            SetRigidBodyVelocity(dir.normalized * MoveSpeed);
        }
        else
        {
            // Chase
            Vector3 dir = (_target.transform.position - transform.position);
            float distToTargetSqr = dir.sqrMagnitude;
            float attackDistanceSqr = AttackDistance * AttackDistance;

            if(distToTargetSqr < attackDistanceSqr)
            {
                // Attack
                Debug.Log("Attack");

                CreatureState = ECreatureState.Skill;
                LookLeft = dir.x < 0; 
                StartWait(1.0f);
            }
            else
            {
                // Chase
                Debug.Log("Chase");
                SetRigidBodyVelocity(dir.normalized * MoveSpeed);

                // Give up
                float searchDistanceSqr = SearchDistance * SearchDistance;
                if(distToTargetSqr > searchDistanceSqr)
                {
                    _destPos = _initPos;
                    _target = null;
                    CreatureState = ECreatureState.Move;
                }

            }
        }
    }
    protected override void UpdateSkill()
    {
        //Debug.Log("Skill");

        if (_coWait != null)
            return;

        CreatureState = ECreatureState.Move;

    }
    protected override void UpdateDead()
    {
        //Debug.Log("Dead");


    }


    #endregion

    #region Battle
    public override void OnDamaged(BaseObject attacker)
    {
        base.OnDamaged(attacker);
    }

    public override void OnDead(BaseObject attacker)
    {
        base.OnDead(attacker);

        // TODO : Drop Item

        Managers.Object.Despawn(this);
    }
    #endregion

}
