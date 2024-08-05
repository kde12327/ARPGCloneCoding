using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Define;

public class NormalAttack : SkillBase
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }
    public override void SetInfo(Creature owner, int skillTemplateID)
    {
        base.SetInfo(owner, skillTemplateID);
    }

    public override void DoSkill(Vector2 target)
    {
        base.DoSkill(target);

        
    }

    protected override void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
    {
        base.OnAnimEventHandler(trackEntry, e);

        //Debug.Log(e.ToString());
        
    }
    protected override void OnAttackEvent()
    {
        /*if (Owner.Target.IsValid() == false)
            return;*/

        if (SkillData.ProjectileId == 0)
        {
            // Melee
            
            /*Util.DrawDebugBox(Owner.transform.position + new Vector3(PLAYER_ATTACK_POINTX, PLAYER_ATTACK_POINTY),
            new Vector2(PLAYER_ATTACK_WIDTH, PLAYER_ATTACK_HEIGHT),
            PLAYER_ATTACK_DEBUG_DURATION);*/

            Vector3 _skillDir = (Target - Owner.transform.position).normalized;
            int _angleRange = 180;
            float radius = Util.GetEffectRadius(SkillData.EffectSize);
            List<Creature> targets = Managers.Object.FindConeRangeTargets(Owner, _skillDir, radius, _angleRange);

            foreach (var target in targets)
            {
                if (target.IsValid())
                {
                    target.OnDamaged(Owner, this);
                }
            }
        }
        else
        {
            // Ranged
            GenerateProjectile(Owner, Owner.CenterPosition);
        }

        if (Owner.CreatureState == Define.ECreatureState.Skill)
        {
            Owner.CreatureState = Define.ECreatureState.Move;
        }
    }

}
