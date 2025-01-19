using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PassiveSkillManager
{
    Dictionary<int, bool> PassiveSkills = new();

    /*public PassiveSkillManager()
    {
        
    }*/
    int _skillPoint = 20;
    public int skillPoint { get { return _skillPoint; } 
        set{
            _skillPoint = value;
            Managers.UI.GetSceneUI<UI_GameScene>().HasPassiveSkillPoint(true);
        } 
    }
    int usedSkillPoint = 0;

    public void Init()
    {
        foreach (var skill in Managers.Data.PassiveSkillDic)
        {
            PassiveSkills[skill.Key] = false;
        }
    }


    public bool TogglePassiveSkill(int dataId)
    {
        var Stats = Managers.Object.Player.Stats;

        // 스킬 포인트 체크
        if(PassiveSkills[dataId] == false)
        {
            if(skillPoint <= usedSkillPoint)
            {
                return false;
            }
        }

        // 해당 노드가 토글이 가능한지 테스트

        Dictionary<int, bool> TmpPassiveSkills = new(PassiveSkills);
        List<int> CheckPassiveSkills = new();
        TmpPassiveSkills[dataId] = !TmpPassiveSkills[dataId];

        Stack<int> stack = new();
        int selectedNodeCount = 0;
        foreach(var skill in Managers.Data.PassiveSkillDic)
        {
            int id = skill.Key;
            var data = skill.Value;

            if(data.StartNode == 1 && TmpPassiveSkills[id] == true)
            {
                stack.Push(id);
            }

            if(TmpPassiveSkills[id] == true)
            {
                selectedNodeCount++;
            }
        }

        while(stack.Count > 0)
        {
            int id = stack.Pop();

            if(!CheckPassiveSkills.Contains(id))
                CheckPassiveSkills.Add(id);

            var linkIds = Managers.Data.PassiveSkillLInkDic[id];

            foreach(var linkId in linkIds)
            {
                if (TmpPassiveSkills[linkId] && !CheckPassiveSkills.Contains(linkId))
                {
                    stack.Push(linkId);
                }
            }
        }
        if(selectedNodeCount == CheckPassiveSkills.Count)
        {
            PassiveSkills[dataId] = !PassiveSkills[dataId];
            if (PassiveSkills[dataId])
            {
                var data = Managers.Data.PassiveSkillDic[dataId];

                for (int i = 0; i < data.StatNames.Count; i++)
                {
                    Stats.AddStat(data.StatNames[i], data.StatValues[i], Managers.Data.PassiveSkillDic[dataId]);
                }

                usedSkillPoint++;
            }
            else
            {
                Stats.ClearModifierFromSource(Managers.Data.PassiveSkillDic[dataId]);
                usedSkillPoint--;
            }
        }

        Managers.UI.GetSceneUI<UI_GameScene>().HasPassiveSkillPoint(skillPoint > usedSkillPoint);
        

        return PassiveSkills[dataId];
    }

    public bool GetPassiveSkillSelected(int dataId)
    {
        return PassiveSkills[dataId];
    }
}
