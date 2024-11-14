using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_QuestDescriptionView : UI_Base
{
    enum Texts
    {
        QuestTitleText,
        QuestContentText,
    }

    public int QuestId { get; protected set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));

                
        return true;
    }

    public void SetInfo(int questId)
    {
        QuestId = questId;
        Data.QuestData questData = Managers.Data.QuestDic[questId];

        GetText((int)Texts.QuestTitleText).text = questData.NameDescriptionTextID;
        GetText((int)Texts.QuestContentText).text = questData.ContentDescriptionTextID;
    }

}
