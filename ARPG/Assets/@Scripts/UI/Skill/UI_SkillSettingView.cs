using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class UI_SkillSettingView : UI_Base
{
    enum GameObjects
    {
        GridView
    }

    EKeyState CurrnetKey;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));

        return true;
    }

    public void SetKey(EKeyState currnetKey)
    {
        CurrnetKey = currnetKey;
    }

    public void SetSkillList(List<SkillBase> skills)
    {
        GameObject gridView = GetObject((int)GameObjects.GridView);

        gridView.DestroyChilds();

        foreach(var skill in skills)
        {
            UI_Skill skillui = Managers.Resource.Instantiate("UI_Skill").GetComponent<UI_Skill>();
            skillui.SetAcitceKeyText(false);
            Debug.Log(skill.name + (skillui == null));
            skillui.SetSkill(skill);
            skillui.transform.parent = gridView.transform;

            skillui.gameObject.BindEvent(evt =>
            {
                Managers.Object.Player.Skills.SetSkillKey(CurrnetKey, skill);
                Managers.UI.GetSceneUI<UI_GameScene>().DisableSkillView();
            }, EUIEvent.Click);
        }
        
    }


}
