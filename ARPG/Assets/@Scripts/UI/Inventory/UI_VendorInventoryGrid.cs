using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

[RequireComponent(typeof(RectTransform))]
public class UI_VendorInventoryGrid : UI_InventoryGrid
{

    Npc _npc;
    Npc Npc {
        get { return _npc; }
        set 
        {
            _npc = value;

            if(_npc != null)
            {
            }
        }
    }


    public void SetInfo(Npc npc)
    {
        Npc = npc;
    }


}
