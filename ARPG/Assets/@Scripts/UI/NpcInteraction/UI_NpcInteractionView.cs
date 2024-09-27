using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_NpcInteractionView : UI_Base
{
    enum GameObjects
    {
        InteractionVertical
    }

    enum Texts
    {
        NpcNameText
    }

    public Npc Npc { get; protected set; }

    List<GameObject> InteractionList { get; set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));

        InteractionList = new();

        return true;
    }

    public void SetInfo(Npc npc)
    {
        Npc = npc;

        GetText((int)Texts.NpcNameText).text = npc.NpcData.DescriptionTextID;

        GameObject interactionParent = GetObject((int)GameObjects.InteractionVertical);

        int count = InteractionList.Count;
        GameObject[] gos = InteractionList.ToArray();
        for(int i = 0; i < count; i++)
        {
            Managers.Resource.Destroy(gos[i]);
        }
        InteractionList.Clear();

        foreach(var interaction in Npc.NpcData.InteractionTypes)
        {
            switch (interaction)
            {
                case Define.EInteractionType.Warehouse:
                    {
                        GameObject go = Managers.Resource.Instantiate("UI_NpcInteraction", interactionParent.transform);
                        go.GetComponent<UI_NpcInteraction>().SetText("창고");
                        go.BindEvent(evt =>
                        {
                            Managers.UI.GetSceneUI<UI_GameScene>().SetActiveWarehouseInventory(true);
                            Managers.UI.GetSceneUI<UI_GameScene>().EnableNpcInteraction();
                        });
                        InteractionList.Add(go);
                    }
                    

                    break;

            }
        }

        if(npc.NpcData.VendorList.Count != 0)
        {
            GameObject go = Managers.Resource.Instantiate("UI_NpcInteraction", interactionParent.transform);
            go.GetComponent<UI_NpcInteraction>().SetText("아이템 구입");
            go.BindEvent(evt =>
            {
                npc.MakeSaleList();
                Managers.UI.GetSceneUI<UI_GameScene>().SetActiveVendorInventory(npc, true);
                Managers.UI.GetSceneUI<UI_GameScene>().EnableNpcInteraction();
            });
            InteractionList.Add(go);
        }

        
        GameObject divider = Managers.Resource.Instantiate("UI_InteractionDivider", interactionParent.transform);
        InteractionList.Add(divider);
        GameObject leave = Managers.Resource.Instantiate("UI_NpcInteraction", interactionParent.transform);
        leave.BindEvent(evt => 
        {
            Managers.UI.GetSceneUI<UI_GameScene>().EnableNpcInteraction();
        });
        InteractionList.Add(leave);

    }
}
