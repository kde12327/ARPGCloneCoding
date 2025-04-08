using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_GameScene : UI_Scene
{
    enum GameObjects
    {
        PlayerInventoryPanel,
        WarehouseInventoryPanel,
        VendorInventoryPanel,
        UI_PlayerStatusView,
        UI_PassiveSkillView,
        UI_NpcInteractionView,
        UI_DiscriptionView,
        UI_WaypointView,
        UI_ItemFlask1,
        UI_ItemFlask2,
        UI_ItemFlask3,
        UI_ItemFlask4,
        UI_ItemFlask5,
        UI_QuestView,
        UI_RewardView,
        UI_GameMenuView,
        UI_SkillSettingView,
        UI_ScriptView
    }

    enum Images
    {
        UsingItemImage,
        PassiveSkillButton
    }

    enum Texts
    {
        
    }

    enum Sliders
    {
        HPSlider,
        MPSlider,
        EXPSlider,
        EnergyShieldSlider
    }

    public enum UISkills
    {
        UI_SkillLeftClick,
        UI_SkillSpaceBar,
        UI_SkillRightClick,
        UI_SkillQ,
        UI_SkillW,
        UI_SkillE,
        UI_SkillR,
        UI_SkillT,
    }

    public enum UIItemSlots
    {
        UI_ItemSlotWeapon,
        UI_ItemSlotOffHand,
        UI_ItemSlotHelmet,
        UI_ItemSlotGloves,
        UI_ItemSlotBoots,
        UI_ItemSlotBodyArmour,
        UI_ItemSlotFlask1,
        UI_ItemSlotFlask2,
        UI_ItemSlotFlask3,
        UI_ItemSlotFlask4,
        UI_ItemSlotFlask5
    }

    public enum UI_InventoryGrids
    {
        PlayerInventory,
        WarehouseInventory,
        VendorInventory,
    }

    bool TEMPOPENPLAYERINVENTORYFLAG = false;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        BindImages(typeof(Images));
        Bind<UI_Skill>(typeof(UISkills));

        

        Bind<UI_ItemSlot>(typeof(UIItemSlots));
        Bind<UI_InventoryGrid>(typeof(UI_InventoryGrids));


        BindItemSlotEvent();
        
        InitSkillImage();


        var playerInven = GetObject((int)GameObjects.PlayerInventoryPanel);
        playerInven.SetActive(false);
        
        var warehouseInven = GetObject((int)GameObjects.WarehouseInventoryPanel);
        warehouseInven.SetActive(false);
        
        var vendorInven = GetObject((int)GameObjects.VendorInventoryPanel);
        vendorInven.SetActive(false);

        var npcInteractinoView = GetObject((int)GameObjects.UI_NpcInteractionView);
        npcInteractinoView.SetActive(false);
        
        var itemDiscriptionView = GetObject((int)GameObjects.UI_DiscriptionView);
        itemDiscriptionView.SetActive(false);
        
        var waypointView = GetObject((int)GameObjects.UI_WaypointView);
        waypointView.SetActive(false);
        
        var playerStatusView= GetObject((int)GameObjects.UI_PlayerStatusView);
        playerStatusView.SetActive(false);

        var passiveSkillView = GetObject((int)GameObjects.UI_PassiveSkillView);
        passiveSkillView.SetActive(false);
        
        var rewardView = GetObject((int)GameObjects.UI_RewardView);
        rewardView.SetActive(false);
        
        var gameMenuView = GetObject((int)GameObjects.UI_GameMenuView);
        gameMenuView.SetActive(false);
        
        var skillSettingView = GetObject((int)GameObjects.UI_SkillSettingView);
        skillSettingView.SetActive(false);

        var scriptView = GetObject((int)GameObjects.UI_ScriptView);
        scriptView.SetActive(false);
        

        var usingItemImage = GetImage((int)Images.UsingItemImage);
        usingItemImage.gameObject.SetActive(false);
        usingItemImage.raycastTarget = false;
        
        var passiveSkillButton = GetImage((int)Images.PassiveSkillButton);
        passiveSkillButton.gameObject.SetActive(false);
        passiveSkillButton.gameObject.BindEvent(evt =>
        {
            var passiveSkillView = GetObject((int)GameObjects.UI_PassiveSkillView);
            passiveSkillView.SetActive(true);
        });

        Get<UI_Skill>((int)UISkills.UI_SkillQ).gameObject.BindEvent(evt =>
        {
            OnClickSkill(EKeyState.Skill00);
        }, EUIEvent.Click);
        
        Get<UI_Skill>((int)UISkills.UI_SkillW).gameObject.BindEvent(evt =>
        {
            OnClickSkill(EKeyState.Skill01);
        }, EUIEvent.Click);
        
        Get<UI_Skill>((int)UISkills.UI_SkillE).gameObject.BindEvent(evt =>
        {
            OnClickSkill(EKeyState.Skill02);
        }, EUIEvent.Click);
        
        Get<UI_Skill>((int)UISkills.UI_SkillR).gameObject.BindEvent(evt =>
        {
            OnClickSkill(EKeyState.Skill03);
        }, EUIEvent.Click);
        
        Get<UI_Skill>((int)UISkills.UI_SkillT).gameObject.BindEvent(evt =>
        {
            OnClickSkill(EKeyState.Skill04);
        }, EUIEvent.Click);
        Get<UI_Skill>((int)UISkills.UI_SkillSpaceBar).gameObject.BindEvent(evt =>
        {
            OnClickSkill(EKeyState.Skill05);
        }, EUIEvent.Click);



        return true;
    }

    private void Update()
    {
        var usingItemImage = GetImage((int)Images.UsingItemImage);
        if(usingItemImage.IsActive())
        {
            usingItemImage.transform.position = Input.mousePosition;
        }
    }

    public void OnClickSkill(EKeyState keyState)
    {
        var skillSettingView = GetObject((int)GameObjects.UI_SkillSettingView);

        skillSettingView.SetActive(true);
        skillSettingView.GetComponent<UI_SkillSettingView>().SetKey(keyState);
    }

    public void DisableSkillView()
    {
        var skillSettingView = GetObject((int)GameObjects.UI_SkillSettingView);

        skillSettingView.SetActive(false);
    }

    public void SetScriptView(int scriptId, int npcId, int questId)
    {
        var scriptView = GetObject((int)GameObjects.UI_ScriptView);
        scriptView.SetActive(true);
        scriptView.GetComponent<UI_ScriptView>().SetInfo(scriptId, npcId, questId);
    }

    public void DisableScriptView()
    {
        var scriptView = GetObject((int)GameObjects.UI_ScriptView);
        scriptView.SetActive(false);
    }


    public void SetWaypoints(Dictionary<string, bool> waypoints)
    {
        var waypointView = GetObject((int)GameObjects.UI_WaypointView);
        waypointView.GetComponent<UI_WaypointView>().SetInfo(waypoints);
    }

    public void HasPassiveSkillPoint(bool hasPoint)
    {
        var passiveSkillButton = GetImage((int)Images.PassiveSkillButton);
        passiveSkillButton.gameObject.SetActive(hasPoint);
    }

    public void SetQuests(List<int> questIds)
    {
        GetObject((int)GameObjects.UI_QuestView).GetComponent<UI_QuestView>().SetInfo(questIds);
    }

    public void SetFlask(FlaskItem item, EEquipSlotType itemSlot)
    {
        int objectIndex = (int)GameObjects.UI_ItemFlask1;
        switch (itemSlot)
        {
            case EEquipSlotType.Flask1:
                objectIndex = (int)GameObjects.UI_ItemFlask1;
                break;
            case EEquipSlotType.Flask2:
                objectIndex = (int)GameObjects.UI_ItemFlask2;
                break;
            case EEquipSlotType.Flask3:
                objectIndex = (int)GameObjects.UI_ItemFlask3;
                break;
            case EEquipSlotType.Flask4:
                objectIndex = (int)GameObjects.UI_ItemFlask4;
                break;
            case EEquipSlotType.Flask5:
                objectIndex = (int)GameObjects.UI_ItemFlask5;
                break;
        }

        var uiItem = GetObject(objectIndex).GetComponent<UI_Item>();
        uiItem.SetInfo(item);
        if(item != null)
            item.SetFlaskUIItem(uiItem);
    }

    public void SetUsingItem(ItemBase item)
    {
        var usingItemImage = GetImage((int)Images.UsingItemImage);
        if(item != null)
        {
            usingItemImage.sprite = Managers.Resource.Load<Sprite>(item.ItemData.Icon);
            usingItemImage.gameObject.SetActive(true);
        }
        else
        {
            usingItemImage.gameObject.SetActive(false);
        }
    }

    public void SetActiveWaypointView(bool isActive)
    {
        var waypointView = GetObject((int)GameObjects.UI_WaypointView);
        waypointView.SetActive(isActive);
    }



    public void PassiveSkillToggle()
    {
        var passiveSkillView = GetObject((int)GameObjects.UI_PassiveSkillView);
        passiveSkillView.SetActive(!passiveSkillView.activeSelf);

        if (passiveSkillView.activeSelf == false)
        {
            DisableDiscription();
        }

        /*if (passiveSkillView.activeSelf)
        {
            passiveSkillView.GetComponent<UI_PassiveSkillView>().UpdateStatusView();
        }*/

    }

    public void PlayerStatusToggle()
    {
        var playerStatus = GetObject((int)GameObjects.UI_PlayerStatusView);
        playerStatus.SetActive(!playerStatus.activeSelf);
        if (playerStatus.activeSelf)
        {
            playerStatus.GetComponent<UI_PlayerStatusView>().UpdateStatusView();
        }

    }

    public void GameMenuToggle()
    {
        var gameMenuView = GetObject((int)GameObjects.UI_GameMenuView);
        gameMenuView.SetActive(!gameMenuView.activeSelf);
    }

    public void SetRewardItems(List<UI_Item> items)
    {
        var rewardView = GetObject((int)GameObjects.UI_RewardView);
        rewardView.GetComponent<UI_RewardView>().SetInfo(items);
        rewardView.SetActive(true);
    }
    
    public void ClearRewardItems()
    {
        var rewardView = GetObject((int)GameObjects.UI_RewardView);
        rewardView.GetComponent<UI_RewardView>().ClearGridView();
        rewardView.SetActive(false);
    }

    public void DisableRewardView()
    {
        var rewardView = GetObject((int)GameObjects.UI_RewardView);
        rewardView.SetActive(false);
    }

    public void SetNpcInteraction(Npc npc)
    {
        var npcInteractinoView = GetObject((int)GameObjects.UI_NpcInteractionView);
        npcInteractinoView.SetActive(true);
        npcInteractinoView.GetComponent<UI_NpcInteractionView>().SetInfo(npc);
    }
    
    

    public void DisableNpcInteraction()
    {
        var npcInteractinoView = GetObject((int)GameObjects.UI_NpcInteractionView);
        npcInteractinoView.SetActive(false);
    }



    public void SetDiscription<T>(T obj, RectTransform rect)
    {

        var discriptionView = GetObject((int)GameObjects.UI_DiscriptionView);
        discriptionView.SetActive(true);
        if(obj is ItemBase)
        {
            discriptionView.GetComponent<UI_DiscriptionView>().SetInfo(obj as ItemBase);
        }
        else if(obj is Data.PassiveSkillData)
        {
            discriptionView.GetComponent<UI_DiscriptionView>().SetInfo(obj as Data.PassiveSkillData);
        }


        Vector3[] worldCorners = new Vector3[4];
        rect.GetWorldCorners(worldCorners);

        float left = worldCorners[0].x;
        float right = worldCorners[3].x;
        float top = worldCorners[1].y;


        // ��ġ
        // �켱����: ��, ��, ��
        RectTransform discriptionRect = discriptionView.GetComponent<RectTransform>();

        discriptionRect.GetWorldCorners(worldCorners);

        // worldCorners[0]�� ���ϴ�, worldCorners[1]�� �»��, worldCorners[2]�� ����, worldCorners[3]�� ���ϴ� ��ǥ�� ����

        // �»���� y���� yMin����, ���ϴ��� x���� xMin����, ���ϴ��� x���� xMax�� ���
        float dTop = worldCorners[1].y;
        float dBottom = worldCorners[0].y;
        float dLeft = worldCorners[0].x;
        float dRight = worldCorners[3].x;

        /*float discriptionWidth = discriptionRect.rect.width;
        float discriptionHeight = discriptionRect.rect.height;*/
        float discriptionWidth = MathF.Abs(dRight - dLeft);
        float discriptionHeight = MathF.Abs(dTop - dBottom);

        //Debug.Log(top + ", " + left + ", " + right);

        Vector2 newPosition = Vector2.zero;



        newPosition = new Vector2((right + left) / 2, top);
        //Debug.Log(newPosition.x + ", " + newPosition.y + ", " + discriptionWidth + ", " + discriptionHeight);
        // 1920 1080
        //Debug.Log(Screen.width + ", " + Screen.height );


        // �켱����: ����, ����, ������
        if (top + discriptionHeight < Screen.height)  // ȭ�� �ȿ��� ���ʿ� ��ġ ����
        {
            // �߾ӿ� ��ġ (xMin�� xMax�� �߰������� ����)
            newPosition = new Vector2((right + left) / 2, top);

        }
        else if (left - discriptionWidth >= 0)  // ȭ�� ���ʿ� ��ġ ����
        {
            // �������� �ٿ��� ��ġ
            newPosition = new Vector2(left - discriptionWidth / 2, top - discriptionHeight);
        }
        else  // �����ʿ� ��ġ
        {
            // ���������� �ٿ��� ��ġ
            newPosition = new Vector2(right + discriptionWidth / 2, top - discriptionHeight);
        }

        // �θ� ĵ���� ��ǥ�� ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gameObject.GetComponent<RectTransform>(),
            newPosition,
            gameObject.GetComponent<Canvas>().worldCamera,
            out Vector2 localPoint);

        // RectTransform�� ��ġ ����
        discriptionRect.anchoredPosition = localPoint;
    }

    public void DisableDiscription()
    {
        var discriptionView = GetObject((int)GameObjects.UI_DiscriptionView);
        discriptionView.SetActive(false);
    }

    public void DisableItemSlot()
    {
        for(int i = 0; i < Enum.GetNames(typeof(UIItemSlots)).Length; i++)
        {
            Get<UI_ItemSlot>((int)i).SetSlotState(ESlotState.None);
        }

    }

    public void InventoryToggle()
    {
        var playerInven = GetObject((int)GameObjects.PlayerInventoryPanel);
        playerInven.SetActive(!playerInven.activeSelf);
        if (!playerInven.activeSelf)
        {
            DisableDiscription();
            DisableItemSlot();
        }
    }

    public void SetActiveWarehouseInventory(bool active)
    {
        var warehouseInven = GetObject((int)GameObjects.WarehouseInventoryPanel);
        warehouseInven.SetActive(active);

        var playerInven = GetObject((int)GameObjects.PlayerInventoryPanel);

        // �κ��丮 �ȿ��� ���¿��� â�� ������ ��,
        // �κ��丮�� ���� ��������.
        if (active && !playerInven.activeSelf)
        {
            TEMPOPENPLAYERINVENTORYFLAG = true;
            playerInven.SetActive(true);
        }

        // â�� ���� ���� ���ȴ� �κ��丮��
        // â�� ���� �� �κ��丮�� �ݵ���.
        if (!active && TEMPOPENPLAYERINVENTORYFLAG)
        {
            TEMPOPENPLAYERINVENTORYFLAG = false;
            DisableItemSlot();
            playerInven.SetActive(false);
        }

        if (!warehouseInven.activeSelf)
        {
            DisableDiscription();
        }


    }

    public void SetActiveVendorInventory(Npc npc, bool active)
    {
        var vendorInven = GetObject((int)GameObjects.VendorInventoryPanel);
        vendorInven.SetActive(active);

        Get<UI_InventoryGrid>((int)UI_InventoryGrids.VendorInventory).GetComponent<UI_VendorInventoryGrid>().SetInfo(npc);

        var playerInven = GetObject((int)GameObjects.PlayerInventoryPanel);

        // �κ��丮 �ȿ��� ���¿��� â�� ������ ��,
        // �κ��丮�� ���� ��������.
        if (active && !playerInven.activeSelf)
        {
            TEMPOPENPLAYERINVENTORYFLAG = true;
            playerInven.SetActive(true);
        }

        // â�� ���� ���� ���ȴ� �κ��丮��
        // â�� ���� �� �κ��丮�� �ݵ���.
        if (!active && TEMPOPENPLAYERINVENTORYFLAG)
        {
            TEMPOPENPLAYERINVENTORYFLAG = false;
            DisableItemSlot();
            playerInven.SetActive(false);

        }
    }


    public void EquipItem(EEquipSlotType type, UI_Item item)
    {
        switch(type)
        {
            case EEquipSlotType.Weapon:
                Get<UI_ItemSlot>((int)UIItemSlots.UI_ItemSlotWeapon).SetItem(item);
                break;
            case EEquipSlotType.Helmet:
                Get<UI_ItemSlot>((int)UIItemSlots.UI_ItemSlotHelmet).SetItem(item);
                break;
            case EEquipSlotType.Boots:
                Get<UI_ItemSlot>((int)UIItemSlots.UI_ItemSlotBoots).SetItem(item);
                break;
            case EEquipSlotType.BodyArmour:
                Get<UI_ItemSlot>((int)UIItemSlots.UI_ItemSlotBodyArmour).SetItem(item);
                break;
            case EEquipSlotType.Gloves:
                Get<UI_ItemSlot>((int)UIItemSlots.UI_ItemSlotGloves).SetItem(item);
                break;
            case EEquipSlotType.Flask1:
                Get<UI_ItemSlot>((int)UIItemSlots.UI_ItemSlotFlask1).SetItem(item);
                break;
            case EEquipSlotType.Flask2:
                Get<UI_ItemSlot>((int)UIItemSlots.UI_ItemSlotFlask2).SetItem(item);
                break;
            case EEquipSlotType.Flask3:
                Get<UI_ItemSlot>((int)UIItemSlots.UI_ItemSlotFlask3).SetItem(item);
                break;
            case EEquipSlotType.Flask4:
                Get<UI_ItemSlot>((int)UIItemSlots.UI_ItemSlotFlask4).SetItem(item);
                break;
            case EEquipSlotType.Flask5:
                Get<UI_ItemSlot>((int)UIItemSlots.UI_ItemSlotFlask5).SetItem(item);
                break;
        }
    }

    public void PutItem(EEquipSlotType type, UI_Item item, Vector2 itemPos)
    {
        switch (type)
        {
            case EEquipSlotType.PlayerInventory:
                Get<UI_InventoryGrid>((int)UI_InventoryGrids.PlayerInventory).PutItem(item, itemPos);
                break;
            case EEquipSlotType.WarehouseInventory:
                Get<UI_InventoryGrid>((int)UI_InventoryGrids.WarehouseInventory).PutItem(item, itemPos);
                break;
            case EEquipSlotType.VendorInventory:
                Get<UI_InventoryGrid>((int)UI_InventoryGrids.VendorInventory).PutItem(item, itemPos);
                break;
        }
    }


    /**
    �̵� �����ϸ� true, interact � ���� �̵� �Ұ��� false
    �̵� �Ұ��� �ش� �κ� ó��
     */
    public bool MoveOrNext()
    {
        SetActiveWarehouseInventory(false);
        SetActiveVendorInventory(null, false);
        DisableNpcInteraction();
        SetActiveWaypointView(false);
        DisableSkillView();
        // ��ũ��Ʈ ���ų� �ϴ� ��� false ��ȯ

        return true;
    }

    public void BindItemSlotEvent()
    {
        foreach(UIItemSlots uitype in Enum.GetValues(typeof(UIItemSlots)))
        {
            EEquipSlotType slotType = EEquipSlotType.None;
            switch(uitype)
            {
                case UIItemSlots.UI_ItemSlotWeapon:
                    slotType = EEquipSlotType.Weapon;
                    break;
                case UIItemSlots.UI_ItemSlotHelmet:
                    slotType = EEquipSlotType.Helmet;
                    break;
                case UIItemSlots.UI_ItemSlotGloves:
                    slotType = EEquipSlotType.Gloves;
                    break;
                case UIItemSlots.UI_ItemSlotBoots:
                    slotType = EEquipSlotType.Boots;
                    break;
                case UIItemSlots.UI_ItemSlotBodyArmour:
                    slotType = EEquipSlotType.BodyArmour;
                    break;
                case UIItemSlots.UI_ItemSlotFlask1:
                    slotType = EEquipSlotType.Flask1;
                    break;
                case UIItemSlots.UI_ItemSlotFlask2:
                    slotType = EEquipSlotType.Flask2;
                    break;
                case UIItemSlots.UI_ItemSlotFlask3:
                    slotType = EEquipSlotType.Flask3;
                    break;
                case UIItemSlots.UI_ItemSlotFlask4:
                    slotType = EEquipSlotType.Flask4;
                    break;
                case UIItemSlots.UI_ItemSlotFlask5:
                    slotType = EEquipSlotType.Flask5;
                    break;
            }

            if (slotType == EEquipSlotType.None) continue;

            Get<UI_ItemSlot>((int)uitype).gameObject.BindEvent(evt =>
            {
                Managers.Inventory.ClickSlot(slotType);

            });

            Get<UI_ItemSlot>((int)uitype).gameObject.BindEvent(evt =>
            {
                ItemBase item = Managers.Inventory.GetEquippedItem(slotType);

                if (item != null)
                {
                    Get<UI_ItemSlot>((int)uitype).SetSlotState(ESlotState.Enable);

                    var rect = Get<UI_ItemSlot>((int)uitype).GetComponent<RectTransform>();

                    // RectTransform�� World ��ǥ�� ���ؼ� ȭ�� �� ��ġ�� ��ȯ
                    Vector3[] worldCorners = new Vector3[4];
                    rect.GetWorldCorners(worldCorners);

                    // worldCorners[0]�� ���ϴ�, worldCorners[1]�� �»��, worldCorners[2]�� ����, worldCorners[3]�� ���ϴ� ��ǥ�� ����

                    // �»���� y���� yMin����, ���ϴ��� x���� xMin����, ���ϴ��� x���� xMax�� ���
                    float top = worldCorners[1].y;
                    float left = worldCorners[0].x;
                    float right = worldCorners[3].x;

                    // SetDiscription ȣ��
                    //SetDiscription(item, top, left, right);
                    /*if (item.ItemType == EItemType.Equipment)
                    {
                        var eItem = item as EquipmentItem;
                        eItem.UIItem.SetActiveSocket(true);
                    }*/
                }
                else
                {
                    //DisableDiscription();
                }

                if (Managers.Inventory.HoldingItem != null)
                {
                    if (Managers.Inventory.CheckHoldingItemCanEquip(slotType))
                    {
                        Get<UI_ItemSlot>((int)uitype).SetSlotState(ESlotState.Enable);
                    }
                    else
                    {
                        Get<UI_ItemSlot>((int)uitype).SetSlotState(ESlotState.Error);
                    }
                }



            }, EUIEvent.PointerEnter);

            Get<UI_ItemSlot>((int)uitype).gameObject.BindEvent(evt =>
            {
                ItemBase item = Managers.Inventory.GetEquippedItem(slotType);

                //DisableDiscription();
                /*if (item != null && item.ItemType == EItemType.Equipment)
                {
                    var eItem = item as EquipmentItem;
                    eItem.UIItem.SetActiveSocket(false);
                }*/
                Get<UI_ItemSlot>((int)uitype).SetSlotState(ESlotState.None);
            }, EUIEvent.PointerExit);
        }

    }

    public void SetHpBarValue(float hpRatio)
    {
        hpRatio = Mathf.Max(0, hpRatio);
        hpRatio = Mathf.Min(1, hpRatio);
        GetSliders((int)Sliders.HPSlider).value = hpRatio;
    }
    public void SetMpBarValue(float mpRatio)
    {
        mpRatio = Mathf.Max(0, mpRatio);
        mpRatio = Mathf.Min(1, mpRatio);
        GetSliders((int)Sliders.MPSlider).value = mpRatio;
    }

    public void SetEnergyShieldBarValue(float esRatio)
    {
        esRatio = Mathf.Max(0, esRatio);
        esRatio = Mathf.Min(1, esRatio);
        GetSliders((int)Sliders.EnergyShieldSlider).value = esRatio;
    }
    public void SetExpBarValue(float expRatio)
    {
        expRatio = Mathf.Max(0, expRatio);
        expRatio = Mathf.Min(1, expRatio);
        GetSliders((int)Sliders.EXPSlider).value = expRatio;
    }

    public void SetSkillImage(UISkills key, string skillIconName = null)
    {
        UI_Skill uiskill = Get<UI_Skill>((int)key);

        uiskill.SetSkillIcon(skillIconName);
    }

    public void SetSkill(UISkills key, SkillBase skill)
    {
        //Debug.Log(skill.name);

        UI_Skill uiskill = Get<UI_Skill>((int)key);

        uiskill.SetSkill(skill);
    }

    public void SetSkills(Dictionary<EKeyState, SkillBase> skillMacro)
    {

        foreach( var pair in skillMacro)
        {
            EKeyState key = pair.Key;
            SkillBase skill = pair.Value;
            Debug.Log(pair.Key + ", " + skill.SkillData.Name);

            switch (key)
            {
                case EKeyState.Skill00:
                    { 
                        SetSkill(UISkills.UI_SkillQ, skill);
                    }
                    break;
                case EKeyState.Skill01:
                    { 
                        SetSkill(UISkills.UI_SkillW, skill);
                    }
                    break;
                case EKeyState.Skill02:
                    { 
                        SetSkill(UISkills.UI_SkillE, skill);
                    }
                    break;
                case EKeyState.Skill03:
                    { 
                        SetSkill(UISkills.UI_SkillR, skill);
                    }
                    break;
                case EKeyState.Skill04:
                    { 
                        SetSkill(UISkills.UI_SkillT, skill);
                    }
                    break;
                case EKeyState.Skill05:
                    { 
                        SetSkill(UISkills.UI_SkillSpaceBar, skill);
                    }
                    break;
            }
        }

        
    }

    public void SetSkillList(List<SkillBase> skills)
    {

        var skillSettingView = GetObject((int)GameObjects.UI_SkillSettingView);
        skillSettingView.GetComponent<UI_SkillSettingView>().SetSkillList(skills);

    }

    public void SetSkillCooldown(UISkills key, float cooldownRatio)
    {
        UI_Skill skill = Get<UI_Skill>((int)key);
        skill.SetSkillCooldown(cooldownRatio);
    }

    public void InitSkillImage()
    {
        foreach (int key in Enum.GetValues(typeof(UISkills)))
        {
            UI_Skill skill = Get<UI_Skill>(key);
            skill.SetSkillIcon(null);
            skill.SetSkillCooldown(0.0f);
        }

        UI_Skill interact = Get<UI_Skill>(0);
        interact.SetSkillIcon("interactionicon.sprite");
        interact.SetSkillCooldown(0.0f);

        UI_Skill move = Get<UI_Skill>(2);
        move.SetSkillIcon("moveicon.sprite");
        move.SetSkillCooldown(0.0f);
    }
    public Vector2Int GetInventorySize(Define.EEquipSlotType invenType)
    {
        switch (invenType)
        {
            case EEquipSlotType.PlayerInventory:
                return Get<UI_InventoryGrid>((int)UI_InventoryGrids.PlayerInventory).GridSize;

            case EEquipSlotType.WarehouseInventory:
                return Get<UI_InventoryGrid>((int)UI_InventoryGrids.WarehouseInventory).GridSize;

            case EEquipSlotType.VendorInventory:
                return Get<UI_InventoryGrid>((int)UI_InventoryGrids.VendorInventory).GridSize;
        }

        return new();
    }
}
