using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_QuestView : UI_Base
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public void SetInfo(List<int> questIds)
    {
        UI_QuestDescriptionView[] list= GetComponentsInChildren<UI_QuestDescriptionView>();



        for (int i = 0; i < list.Length; i++)
        {
            
             Destroy(list[i].gameObject);
        }

        for (int i = 0; i < questIds.Count; i++)
        {
            int questId = questIds[i];
            Debug.Log(questId);
            GameObject questDisc = Managers.Resource.Instantiate("UI_QuestDescriptionView", transform);
            questDisc.name = questDisc.name + questId;
            questDisc.GetComponent<UI_QuestDescriptionView>().SetInfo(questId);
        }

    }
}
