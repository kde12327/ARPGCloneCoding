using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WaypointButton : UI_Base
{

    [SerializeField]
    string MapName = "";

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        gameObject.BindEvent((evt) => 
        {
            Managers.Object.Player.MapArriveId = 301001;

            Managers.Scene.CreateOrLoadGameSceneByName(MapName);

        }, Define.EUIEvent.Click);

        return true;
    }
}
