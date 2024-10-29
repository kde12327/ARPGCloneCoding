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

        // �ش� ��尡 ����� �������� �׽�Ʈ

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
        {// ������ ��带 ������
            var data = Managers.Data.PassiveSkillDic[dataId];
            if (data.StartNode != 1)
            {
                // start Node �� �ƴҶ� ��带 ���� �� �ִ��� üũ

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
        {// ���� ��带 ���� ��
            var data = Managers.Data.PassiveSkillDic[dataId];
            if (data.StartNode == 1)
            {// ��ŸƮ ����� ��
                bool HasSelectNodeFlag = false;

                foreach (int targetId in Managers.Data.PassiveSkillLInkDic[dataId])
                {
                    Debug.Log(targetId + ":" + PassiveSkills[targetId]);
                    if (PassiveSkills[targetId] && Managers.Data.PassiveSkillDic[targetId].StartNode != 1)
                    {// ���� ��尡 ���� ��Ȱ��ȭ�ų�, Ȱ��ȭ���� ��ŸƮ ����� ��츸 ����.
                        HasSelectNodeFlag = true;
                    }
                }

                if (HasSelectNodeFlag == true) return true;
            }
            else
            {// ��ŸƮ ��尡 �ƴ� ��, ������ Ȱ��ȭ ��尡 �Ѱ��� ��츸 ����� ����
                int selectedNodeCount = 0;
                int selectedStartNodeCount = 0;

                foreach (int targetId in Managers.Data.PassiveSkillLInkDic[dataId])
                {
                    Debug.Log(targetId + ":" + PassiveSkills[targetId]);
                    if (PassiveSkills[targetId])
                    {// ���� ��尡 ���� ��Ȱ��ȭ�ų�, Ȱ��ȭ���� ��ŸƮ ����� ��츸 ����.
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
