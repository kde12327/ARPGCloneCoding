using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillComponent : InitBase
{
	public List<SkillBase> SkillList { get; } = new List<SkillBase>();
	public List<SkillBase> ActiveSkills { get; set; } = new List<SkillBase>();

	Dictionary<UI_GameScene.UISkills, SkillBase> SkillMacro { get; set; } = new Dictionary<UI_GameScene.UISkills, SkillBase>();


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

	public void AddSkill(SkillBase skill)
	{
		SkillList.Add(skill);
		ActiveSkills.Add(skill);
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

	public SkillBase GetSkill(int skillIndex)
	{
		if (SkillList.Count <= skillIndex)
			return null;
		return SkillList[skillIndex];
	}

	public void ResetSkill()
    {
		SkillList.Clear();

	}

}
