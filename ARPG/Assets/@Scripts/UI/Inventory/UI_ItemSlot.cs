using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_ItemSlot : UI_Base
{

    enum Images
    {
        HoverImage
    }


    UI_Item Item { get; set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));


        GetImage((int)Images.HoverImage).color = UIColor.TRANSPARENT;


        return true;
    }

    public void SetItem(UI_Item item)
    {
        Item = item;

        item.transform.SetParent(this.transform, false);
        item.transform.position = this.transform.position;

    }
    public UI_Item RemoveItem()
    {
        UI_Item item = Item;
        Item = null;

        return item;
    }

    public void SetSlotState(SlotState state)
    {
        switch(state)
        {
            case SlotState.Enable:
                GetImage((int)Images.HoverImage).color = UIColor.ENABLE;
                break;
            case SlotState.Error:
                GetImage((int)Images.HoverImage).color = UIColor.ERROR;
                break;
            case SlotState.None:
                GetImage((int)Images.HoverImage).color = UIColor.TRANSPARENT;
                break;
        }
    }




}
