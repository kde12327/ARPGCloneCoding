using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SkillGemItem : ItemBase
{
    public Data.SkillGemItemData SkillGemItemData;

    bool _isInSocket = false;
    public bool IsInSocket
    {
        get
        {
            return _isInSocket;
        }
        set
        {
            _isInSocket = value;
            if(UIItem != null)
            {
                UIItem.SetInSocket(value);
            }
        }
    }

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
