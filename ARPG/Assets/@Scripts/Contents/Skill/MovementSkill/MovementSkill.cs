using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSkill : SkillBase
{

    public override void SetInfo(Creature owner, int skillTemplateID)
    {
        base.SetInfo(owner, skillTemplateID);
    }

    public override void DoSkill(Vector2 target)
    {
        base.DoSkill(target);

        if (Owner.CreatureState != Define.ECreatureState.Skill)
            return;

    }

    protected override void OnAttackEvent()
    {

        
    }
}
