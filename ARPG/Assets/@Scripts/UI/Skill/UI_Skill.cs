using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Skill : UI_Base
{

    enum Texts
    {
        TextSkillKey
    }

    enum Images
    {
        ImageSkillIcon
    }

    enum Sliders
    {
        CooldownSlider
    }

    SkillBase Skill { get; set; }

    float Cooldown { get; set; }
    float MaxCooldown { get; set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        BindTexts(typeof(Texts));
        BindSliders(typeof(Sliders));

        GetImage((int)Images.ImageSkillIcon).gameObject.BindEvent((evt) =>
        {
            Debug.Log("ClickSkillIcon");
        });

        GetImage((int)Images.ImageSkillIcon).sprite = null;

        return true;
    }

    public void InitState()
    {
        Cooldown = 0;
        GetSliders((int)Sliders.CooldownSlider).value = 0;

    }

    private void Update()
    {
        if(Cooldown > 0)
        {
            Cooldown -= Time.deltaTime;
            Cooldown = Mathf.Max(0, Cooldown);
            GetSliders((int)Sliders.CooldownSlider).value = Cooldown / MaxCooldown;
        }


    }

    public void SetSkillIcon(string skillIconName)
    {
        if(skillIconName == null)
        {
            Image image = GetImage((int)Images.ImageSkillIcon);
            image.sprite = null;
            image.color = new Color(255, 255, 255, 0);
        }
        else
        {
            Image image = GetImage((int)Images.ImageSkillIcon);
            image.sprite = Managers.Resource.Load<Sprite>(skillIconName);
            image.color = new Color(255, 255, 255, 1);
        }
    }

    public void SetSkillCooldown(float cooldownRatio)
    {
        Slider slider = GetSliders((int)Sliders.CooldownSlider);
        slider.value = cooldownRatio;
    }

    public void SetSkill(SkillBase skill)
    {
        if (skill == null) return;

        if(Skill != null)
        {
            Skill.OnCooldownStarted -= OnCooldownCount;
        }

        InitState();

        Skill = skill;
        Skill.OnCooldownStarted += OnCooldownCount;
        if (Skill.SkillData.IconLabel == "")
        {
            SetSkillIcon(null);
        }
        else
        {
            SetSkillIcon(Skill.SkillData.IconLabel);
        }
    }

    public void OnCooldownCount(float cooldown)
    {
        MaxCooldown = cooldown;
        Cooldown = MaxCooldown;
    }

}
