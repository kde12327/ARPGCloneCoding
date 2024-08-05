using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    enum GameObjects
    {

    }

    enum Texts
    {
        
    }

    enum Sliders
    {
        HPSlider,
        MPSlider,
        EXPSlider,
    }

    public enum UISkills
    {
        UI_SkillLeftClick,
        UI_SkillSpaceBar,
        UI_SkillRightClick,
        UI_SkillQ,
        UI_SkillW,
        UI_SkillE,
        UI_SkillR,
        UI_SkillT,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        Bind<UI_Skill>(typeof(UISkills));

        //GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        InitSkillImage();

        return true;
    }

    public void SetHpBarValue(float hpRatio)
    {
        hpRatio = Mathf.Max(0, hpRatio);
        hpRatio = Mathf.Min(1, hpRatio);
        GetSliders((int)Sliders.HPSlider).value = hpRatio;
    }
    public void SetMpBarValue(float mpRatio)
    {
        mpRatio = Mathf.Max(0, mpRatio);
        mpRatio = Mathf.Min(1, mpRatio);
        GetSliders((int)Sliders.MPSlider).value = mpRatio;
    }
    public void SetExpBarValue(float expRatio)
    {
        expRatio = Mathf.Max(0, expRatio);
        expRatio = Mathf.Min(1, expRatio);
        GetSliders((int)Sliders.EXPSlider).value = expRatio;
    }

    public void SetSkillImage(UISkills key, string skillIconName = null)
    {
        UI_Skill uiskill = Get<UI_Skill>((int)key);

        uiskill.SetSkillIcon(skillIconName);
    }

    public void SetSkill(UISkills key, SkillBase skill)
    {
        Debug.Log(skill.name);

        UI_Skill uiskill = Get<UI_Skill>((int)key);

        uiskill.SetSkill(skill);
    }

    public void SetSkillCooldown(UISkills key, float cooldownRatio)
    {
        UI_Skill skill = Get<UI_Skill>((int)key);
        skill.SetSkillCooldown(cooldownRatio);
    }

    public void InitSkillImage()
    {
        foreach (int key in Enum.GetValues(typeof(UISkills)))
        {
            UI_Skill skill = Get<UI_Skill>(key);
            skill.SetSkillIcon(null);
            skill.SetSkillCooldown(0.0f);
        }
    }
}
