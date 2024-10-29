using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PassiveSkillManager
{
    Dictionary<int, bool> PassiveSkills = new();

    /*public PassiveSkillManager()
    {
        
    }*/
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
            }
            else
            {
                Stats.ClearModifierFromSource(Managers.Data.PassiveSkillDic[dataId]);
            }
        }
/*
        if (PassiveSkills[dataId] == false)
        {// 안찍힌 노드를 찍을때
            var data = Managers.Data.PassiveSkillDic[dataId];
            if (data.StartNode != 1)
            {
                // start Node 가 아닐때 노드를 찍을 수 있는지 체크

                bool HasSelectNodeFlag = false;

                foreach (int targetId in Managers.Data.PassiveSkillLInkDic[dataId])
                {
                    Debug.Log(targetId + ":" + PassiveSkills[targetId]);
                    if (PassiveSkills[targetId])
                    {
                        HasSelectNodeFlag = true;
                    }
                }

                if (HasSelectNodeFlag == false) return false;
            }

            for (int i = 0; i < data.StatNames.Count; i++)
            {
                Stats.AddStat(data.StatNames[i], data.StatValues[i], Managers.Data.PassiveSkillDic[dataId]);
            }
        }
        else
        {// 찍힌 노드를 지울 때
            var data = Managers.Data.PassiveSkillDic[dataId];
            if (data.StartNode == 1)
            {// 스타트 노드일 때
                bool HasSelectNodeFlag = false;

                foreach (int targetId in Managers.Data.PassiveSkillLInkDic[dataId])
                {
                    Debug.Log(targetId + ":" + PassiveSkills[targetId]);
                    if (PassiveSkills[targetId] && Managers.Data.PassiveSkillDic[targetId].StartNode != 1)
                    {// 주위 노드가 전부 비활성화거나, 활성화여도 스타트 노드인 경우만 지움.
                        HasSelectNodeFlag = true;
                    }
                }

                if (HasSelectNodeFlag == true) return true;
            }
            else
            {// 스타트 노드가 아닐 때, 주위에 활성화 노드가 한개인 경우만 지우기 가능
                int selectedNodeCount = 0;
                int selectedStartNodeCount = 0;

                foreach (int targetId in Managers.Data.PassiveSkillLInkDic[dataId])
                {
                    Debug.Log(targetId + ":" + PassiveSkills[targetId]);
                    if (PassiveSkills[targetId])
                    {// 주위 노드가 전부 비활성화거나, 활성화여도 스타트 노드인 경우만 지움.
                        selectedNodeCount++;
                        if (Managers.Data.PassiveSkillDic[targetId].StartNode == 1)
                        {
                            selectedStartNodeCount++;
                        }
                    }
                }

                if (selectedNodeCount != 1) return true;
            }


            Stats.ClearModifierFromSource(Managers.Data.PassiveSkillDic[dataId]);
        }

        PassiveSkills[dataId] = !PassiveSkills[dataId];
*/
        return PassiveSkills[dataId];
    }

    public bool GetPassiveSkillSelected(int dataId)
    {
        return PassiveSkills[dataId];
    }
}
