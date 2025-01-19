using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager
{
    public Dictionary<int, bool> QuestClearDic = new();

    public List<int> CurrentQuestList = new();
    List<int> CurrentMapQuestList = new();

    public Dictionary<int, List<UI_Item>> RewardDic = new();

    int CurrentRewardQuestId;
    public void Init()
    {
        

        foreach (var e in Managers.Data.QuestDic)
        {
            int key = e.Key;
            Data.QuestData data = e.Value;

            QuestClearDic[key] = false;
        }
        CurrentQuestList.Add(1001);

        

    }

    public void OnMapChanged()
    {
        RefreshCurrentMapQuest();
    }

    public void RefreshCurrentMapQuest()
    {
        CurrentMapQuestList.Clear();

        string mapName = Managers.Map.MapName;
        

        foreach (int id in CurrentQuestList)
        {
            var data = Managers.Data.QuestDic[id];
            if (data.QuestMapId == mapName)
            {
                CurrentMapQuestList.Add(id);
            }
        }
        

        Managers.UI.GetSceneUI<UI_GameScene>().SetQuests(CurrentQuestList);
    }

    public void OnMonsterDead(int monsterId)
    {
        bool IsDirty = false;

        Debug.Log("OnMonsterDead: " + monsterId);
        foreach (int questId in CurrentMapQuestList)
        {
            var data = Managers.Data.QuestDic[questId];

            if (data.QuestTargetId == monsterId)
            {
                ClearQuest(questId);
                IsDirty = true;
            }
        }
        if(IsDirty)
            RefreshCurrentMapQuest();
    }

    public bool HasTargetQuest(Define.EQuestType questType, int targetId)
    {
        foreach (int id in CurrentQuestList)
        {
            var data = Managers.Data.QuestDic[id];
            if (data.QuestType == questType && data.QuestTargetId == targetId)
            {
                return true;
            }
        }
        return false;
    }

    public void ClearTargetQuest(Define.EQuestType questType, int targetId)
    {
        foreach (int id in CurrentQuestList)
        {
            var data = Managers.Data.QuestDic[id];
            if (data.QuestType == questType && data.QuestTargetId == targetId)
            {
                ClearQuest(data.DataId);
                RefreshCurrentMapQuest();
                return;
            }
        }
    }

    public bool ClearQuest(int id)
    {
        Debug.Log("ClearQuest" + id);
        if (!CurrentQuestList.Contains(id))
            return false;

        var data = Managers.Data.QuestDic[id];
        if(data.NextQuestId != 0)
        {
            List<int> keys = new (Managers.Data.QuestDic.Keys);
            if (keys.Contains(data.NextQuestId) && QuestClearDic[data.NextQuestId] == false)
            {
                Debug.Log("add " + data.NextQuestId);
                CurrentQuestList.Add(data.NextQuestId);
            } 
        }
        CurrentQuestList.Remove(id);
        QuestClearDic[id] = true;

        return true;
    }

    public List<UI_Item> GetReward(int id)
    {
        if (!RewardDic.ContainsKey(id))
        {
            var data = Managers.Data.QuestDic[id];
            List<UI_Item> items = new();

            switch (data.QuestRewardType)
            {
                case Define.EQuestRewardType.GetGloveItems:
                    int gloveItemId = 20300001;
                    for(int i = 0; i < 3; i++)
                    {
                        var item = EquipmentItem.MakeEquipmentItem(gloveItemId, Define.ERarity.Rare);
                        UI_Item uiitem = Managers.Resource.Instantiate("UI_Item", Managers.UI.GetSceneUI<UI_GameScene>().transform).GetComponent<UI_Item>();
                        uiitem.SetInfo(item);
                        uiitem.IsReward = true;
                        item.EquipSlot = Define.EEquipSlotType.RewardView;
                        items.Add(uiitem);
                    }

                    break;  
                case Define.EQuestRewardType.GetFlaskItems:
                    
                    break;  
                case Define.EQuestRewardType.GetLinkSkillGems:
                    {
                        int[] SkillGemId = {41000001, 41000002};
                        for (int i = 0; i < SkillGemId.Length; i++)
                        {
                            var item = SkillGemItem.MakeSkillGemItem(SkillGemId[i]);
                            UI_Item uiitem = Managers.Resource.Instantiate("UI_Item", Managers.UI.GetSceneUI<UI_GameScene>().transform).GetComponent<UI_Item>();
                            uiitem.SetInfo(item);
                            uiitem.IsReward = true;
                            item.EquipSlot = Define.EEquipSlotType.RewardView;
                            items.Add(uiitem);
                        }

                    }

                    break;  
                case Define.EQuestRewardType.GetMoveSkillGems:
                    {
                        int[] SkillGemId = { 40000002 };
                        for (int i = 0; i < SkillGemId.Length; i++)
                        {
                            var item = SkillGemItem.MakeSkillGemItem(SkillGemId[i]);
                            UI_Item uiitem = Managers.Resource.Instantiate("UI_Item", Managers.UI.GetSceneUI<UI_GameScene>().transform).GetComponent<UI_Item>();
                            uiitem.SetInfo(item);
                            uiitem.IsReward = true;
                            item.EquipSlot = Define.EEquipSlotType.RewardView;
                            items.Add(uiitem);
                        }

                    }
                    break;  
            }

            RewardDic[id] = items;

        }


        CurrentRewardQuestId = id;
        return RewardDic[id];
    }

    public bool ChoiceReward()
    {
        RewardDic.Remove(CurrentRewardQuestId);
        ClearQuest(CurrentRewardQuestId);
        RefreshCurrentMapQuest();
        Managers.UI.GetSceneUI<UI_GameScene>().ClearRewardItems();
        Managers.UI.GetSceneUI<UI_GameScene>().EnableRewardView();
        return true;
    }
}
