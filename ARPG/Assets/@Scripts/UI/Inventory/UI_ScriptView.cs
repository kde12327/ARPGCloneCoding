using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ScriptView : UI_Base
{
    enum Texts
    {
        ScriptTitleText,
        ScriptContentText
    }

    enum GameObjects
    {
        ScriptNextImage,
        ScriptTitleImage
    }

    int QuestId;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));
        BindObjects(typeof(GameObjects));

        gameObject.BindEvent(evt =>
        {
            Debug.Log("Click");
            var gamescene = Managers.UI.GetSceneUI<UI_GameScene>();
            var rewardItems = Managers.Quest.GetReward(QuestId);
            if (rewardItems.Count != 0)
            {
                gamescene.SetRewardItems(rewardItems);
            }

            gamescene.DisableScriptView();

        }, Define.EUIEvent.Click);

/*
        GetObject((int)GameObjects.ScriptNextImage).BindEvent(evt =>
        {
            Debug.Log("Click");
            var gamescene = Managers.UI.GetSceneUI<UI_GameScene>();

            var rewardItems = Managers.Quest.GetReward(QuestId);
            if (rewardItems.Count != 0)
            {
                gamescene.SetRewardItems(rewardItems);
            }
        }, Define.EUIEvent.Click);

        GetObject((int)GameObjects.ScriptTitleImage).BindEvent(evt =>
        {
            Debug.Log("Click");
        }, Define.EUIEvent.Click);*/


        return true;
    }

    public void SetInfo(int scriptId, int npcId, int questId)
    {
        Data.NpcData npcData = Managers.Data.NpcDic[npcId];
        Data.ScriptData scriptData = Managers.Data.ScriptDic[scriptId];
        GetText((int)Texts.ScriptTitleText).text = npcData.DescriptionTextID;
        GetText((int)Texts.ScriptContentText).text = scriptData.ScriptText;

        QuestId = questId;
    }
}
