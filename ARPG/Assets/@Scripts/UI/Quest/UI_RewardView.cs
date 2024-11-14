using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RewardView : UI_Base
{
    enum GameObjects
    {
        GridLayout
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));

        return true;
    }

    public void SetInfo(List<UI_Item> items)
    {
        foreach(UI_Item item in items)
        {
            item.gameObject.transform.parent = GetObject((int)GameObjects.GridLayout).transform;
        }
    }

    public void ClearGridView()
    {
        var items = GetObject((int)GameObjects.GridLayout).GetComponentsInChildren<UI_Item>();
        foreach(var item in items)
        {
            item.Item.Destroy();
        }
    }
}
