using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WaypointView : UI_Base
{
    enum GameObjects
    {
        ACT01_Town_Map,
        ACT01_01_Map,
        ACT01_02_Map,
        ACT01_03_Map,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));

        return true;
    }

    public void SetInfo(Dictionary<string, bool> waypoints)
    {
        string[] mapNames = Enum.GetNames(typeof(GameObjects));

        for(int i = 0; i < mapNames.Length; i++)
        {
            GetObject(i).GetComponent<UI_WaypointButton>().SetActive(waypoints[mapNames[i]]);
            
        }
    }

}