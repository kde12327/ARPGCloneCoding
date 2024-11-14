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
        Debug.Log("UI_QuestView Setinfo" + questIds.Count);
        UI_QuestDescriptionView[] list= GetComponentsInChildren<UI_QuestDescriptionView>();
        Debug.Log("list" + list.Length);


        List<int> resistIds = new();

        for (int i = 0; i < list.Length; i++)
        {
            var e = list[i];
            if (questIds.Contains(e.QuestId))
            {
                resistIds.Add(e.QuestId);
            }
            else
            {
                Destroy(e.gameObject);
            }
        }

        for (int i = 0; i < questIds.Count; i++)
        {
            int questId = questIds[i];
            if (!resistIds.Contains(questId))
            {
                GameObject questDisc = Managers.Resource.Instantiate("UI_QuestDescriptionView", transform);
                questDisc.name = questDisc.name + questId;
                questDisc.GetComponent<UI_QuestDescriptionView>().SetInfo(questId);
            }
            
        }

    }
}
