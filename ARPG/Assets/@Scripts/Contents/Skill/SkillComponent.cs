using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SkillComponent : InitBase
{
	public List<SkillBase> SkillList { get; } = new List<SkillBase>();
	public List<SkillBase> ActiveSkills { get; set; } = new List<SkillBase>();

	public Dictionary<EKeyState, SkillBase> SkillMacro { get; protected set; } = new Dictionary<EKeyState, SkillBase>();
	
	// <key, id> to memory skill
	Dictionary<EKeyState, SkillBase> SkillMacroCache { get; set; } = new Dictionary<EKeyState, SkillBase>();

	Creature _owner;

	public SkillBase CurrentSkill
	{
		get
		{
			int randomIndex = UnityEngine.Random.Range(0, ActiveSkills.Count);
			if(randomIndex >= ActiveSkills.Count)
            {
				Debug.Log(_owner.CreatureData.DescriptionTextID + " : " +  randomIndex);
				return null;
			}
			return ActiveSkills[randomIndex];
		}
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		return true;
	}

	public void SetInfo(Creature owner)
	{
		_owner = owner;
	}

	public void SetInfo(Creature owner, List<int> skillTemplateIDs)
	{
		SetInfo(owner);

		foreach (int skillTemplateID in skillTemplateIDs)
			AddSkill(skillTemplateID);
	}
	
	public void SetSkillKey(EKeyState key, SkillBase skill)
    {
		SkillMacro[key] = skill;
		SkillMacroCache[key] = skill;
		Managers.UI.GetSceneUI<UI_GameScene>().SetSkills(SkillMacro);

	}

	public void AddSkill(SkillBase skill)
	{

		SkillList.Add(skill);
		ActiveSkills.Add(skill);


		// ������ ��ų�� ����Ǿ� �ִ���
		bool Flag = false;

		// ����Ǿ� �ִ��� Ȯ��.
		// ����Ǿ� ������ �ش� ��ġ�� Ű ����.

		List<EKeyState> keys = new();
		foreach(var pair in SkillMacroCache)
        {
			if(pair.Value == skill)
            {
				Flag = true;
				keys.Add(pair.Key);
			}
        }
		
		// �ٸ� ��ų�� ��ũ�� ��ϵǾ� �ִ��� Ȯ���� �ʿ� ����.
		// ��ų ��Ͻ� ��ũ�ΰ� ����Ǳ� ����.
		foreach(var key in keys)
        {
			SetSkillKey(key, skill);
		}



		// ���� �ȵǾ� ������ �� ������ Ű ����
		if(Flag == false)
        {
			for (int i = 0; i < Enum.GetNames(typeof(EKeyState)).Length; i++)
			{
				if (!SkillMacro.ContainsKey((EKeyState)i))
				{
					SetSkillKey((EKeyState)i, skill);
					break;
				}
			}
		}


	}

	public void AddSkill(int skillTemplateID = 0)
	{
		string className = Managers.Data.SkillDic[skillTemplateID].ClassName;

		SkillBase skill = gameObject.AddComponent(Type.GetType(className)) as SkillBase;
		if (skill == null)
			return;

		skill.SetInfo(_owner, skillTemplateID);

		AddSkill(skill);
	}

	public SkillBase GetSkill(EKeyState key)
	{
		return SkillMacro[key];
	}

	public void ResetSkill()
    {
		SkillList.Clear();
		SkillMacro.Clear();

	}

}
