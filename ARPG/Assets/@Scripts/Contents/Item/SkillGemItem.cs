using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SkillGemItem : ItemBase
{
    public Data.SkillGemItemData SkillGemItemData;

    public SkillGemItem(int itemDataId) : base(itemDataId)
    {
        SkillGemItemData = Managers.Data.SkillGemItemDic[itemDataId];

        Init();
    }

    public override bool Init()
    {
        if (!base.Init()) return false;

        return true;
    }


    public static SkillGemItem MakeSkillGemItem(int itemDataId)
    {
        var item = Managers.Inventory.MakeItem(itemDataId) as SkillGemItem;

        return item;
    }
}
