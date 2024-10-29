using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PassiveSkillLink : UI_Base
{
    List<Image> linkImages;

    List<string> linkImagesNames = new();

    List<UI_PassiveSkill> uiPassiveSkills = new();

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Image[] imageArr = GetComponentsInChildren<Image>();

        linkImages = new(imageArr);

        foreach (var image in linkImages)
        {
            linkImagesNames.Add(image.name);
        }

        BindByNames<Image>(linkImagesNames);

        return true;
    }

    public void SetPassvieNode(UI_PassiveSkill passiveSkill)
    {
        uiPassiveSkills.Add(passiveSkill);
    }

    public UI_PassiveSkill GetOtherNode(UI_PassiveSkill passiveSkill)
    {
        return uiPassiveSkills[0] == passiveSkill ? uiPassiveSkills[1] : uiPassiveSkills[0];
    }

    public void SetSelected(bool isSelected)
    {
        string spriteStr = isSelected ? "PassiveSkillLinkSelected.sprite" : "PassiveSkillLink.sprite";

        for (int i = 0; i < linkImagesNames.Count; i++)
        {
            var idx = i;
            var image = Get<Image>(idx);
            image.sprite = Managers.Resource.Load<Sprite>(spriteStr);
        }
    }
}
