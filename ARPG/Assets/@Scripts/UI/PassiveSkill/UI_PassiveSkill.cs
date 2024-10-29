using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PassiveSkill : UI_Base
{

    [SerializeField]
    public int PassiveNodeId;

    public int Index;

    Data.PassiveSkillData PassiveSkillData;

    [SerializeField]
    List<UI_PassiveSkillLink> UILinks = new();

    enum Images
    {
        PassiveSkillImage,
        PassiveSkillFrameImage
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));

        UpdatePassiveNode();

        for(int i = 0; i < UILinks.Count; i++)
        {
            UILinks[i].SetPassvieNode(this);
        }
        

        return true;
    }


    void UpdatePassiveNode()
    {
        PassiveSkillData = Managers.Data.PassiveSkillDic[PassiveNodeId];

        GetImage((int)Images.PassiveSkillImage).sprite = Managers.Resource.Load<Sprite>(PassiveSkillData.Icon);
    }

    public void SetFrame(bool isSelected)
    {
        if (isSelected)
        {
            GetImage((int)Images.PassiveSkillFrameImage).sprite = Managers.Resource.Load<Sprite>("PassiveIconFrameSelected.sprite");
            for (int i = 0; i < UILinks.Count; i++)
            {
                UI_PassiveSkill node = UILinks[i].GetOtherNode(this);
                bool result = Managers.Passive.GetPassiveSkillSelected(node.PassiveNodeId);
                if (result)
                {
                    UILinks[i].SetSelected(isSelected);
                }
            }
        }
        else
        {
            GetImage((int)Images.PassiveSkillFrameImage).sprite = Managers.Resource.Load<Sprite>("PassiveIconFrame.sprite");
            for (int i = 0; i < UILinks.Count; i++)
            {
                UILinks[i].SetSelected(isSelected);
            }
        }
    }
}
