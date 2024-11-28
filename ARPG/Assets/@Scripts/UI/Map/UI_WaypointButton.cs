using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WaypointButton : UI_Base
{

    [SerializeField]
    string MapName = "";

    [SerializeField]
    bool IsTown = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        gameObject.BindEvent((evt) => 
        {
            if (Managers.Map.IsActiveWaypoint(MapName))
            {
                Managers.Object.Player.MapArriveId = 301001;

                Managers.Scene.CreateOrLoadGameSceneByName(MapName);
            }
        }, Define.EUIEvent.Click);

        return true;
    }

    public void SetActive(bool isActive)
    {
        if (isActive)
        {
            if (IsTown)
            {
                GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>("WayPointTownlFill.sprite");
            }
            else
            {
                GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>("WayPointNormalFill.sprite");
            }
        }
        else
        {
            GetComponent<Image>().sprite = Managers.Resource.Load<Sprite>("WayPointNormalEmpty.sprite");
        }
    }
}
