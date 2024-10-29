using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PassiveSkillView : UI_Base
{

    List<UI_PassiveSkill> passiveSkills;

    List<string> passiveSkillNames = new();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        UI_PassiveSkill[] passiveSkillsArr = GetComponentsInChildren<UI_PassiveSkill>();

        passiveSkills = new(passiveSkillsArr);

        passiveSkills.Sort((A, B) => A.PassiveNodeId.CompareTo(B.PassiveNodeId));

        foreach(var skill in passiveSkills)
        {
            passiveSkillNames.Add(skill.name);
            Debug.Log(skill.name);
        }

        BindByNames<UI_PassiveSkill>(passiveSkillNames);

        for(int i = 0; i < passiveSkillNames.Count; i++)
        {
            var idx = i;
            var skill = Get<UI_PassiveSkill>(idx);
            skill.Index = idx;
            skill.gameObject.BindEvent(evt => 
            {
                OnClickPassiveNode(idx);
            }, Define.EUIEvent.Click);
        }

        return true;
    }

    void OnClickPassiveNode(int idx)
    {
        var skill = Get<UI_PassiveSkill>(idx);

        bool result = Managers.Passive.TogglePassiveSkill(skill.PassiveNodeId);
        skill.SetFrame(result);
    }


}
