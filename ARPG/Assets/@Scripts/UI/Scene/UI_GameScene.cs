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
        UI_NpcInteractionView,
        UI_DiscriptionView,
    }

    enum Texts
    {
        
    }

    enum Sliders
    {
        HPSlider,
        MPSlider,
        EXPSlider,
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
    }

    public enum UI_InventoryGrids
    {
        PlayerInventory,
        WarehouseInventory
    }

    bool TEMPOPENPLAYERINVENTORYFLAG = false;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));
        BindSliders(typeof(Sliders));
        Bind<UI_Skill>(typeof(UISkills));
        Bind<UI_ItemSlot>(typeof(UIItemSlots));
        Bind<UI_InventoryGrid>(typeof(UI_InventoryGrids));

        UI_InventoryGrid playerInventoryGrid = Get<UI_InventoryGrid>((int)UI_InventoryGrids.PlayerInventory);
        playerInventoryGrid.SetInfo(Define.EEquipSlotType.PlayerInventory);
        playerInventoryGrid.gameObject.BindEvent(evt =>
        {
            var v = Get<UI_InventoryGrid>((int)UI_InventoryGrids.PlayerInventory).MouseToCell();
            //Debug.Log(v);
            Managers.Inventory.ClickInventory(playerInventoryGrid.InvenType, v);
        });

        playerInventoryGrid.gameObject.BindEvent(evt =>
        {
            Get<UI_InventoryGrid>((int)UI_InventoryGrids.PlayerInventory).PointEnter();
        }, EUIEvent.PointerEnter);

        playerInventoryGrid.gameObject.BindEvent(evt =>
        {
            Get<UI_InventoryGrid>((int)UI_InventoryGrids.PlayerInventory).PointExit();
        }, EUIEvent.PointerExit);



        UI_InventoryGrid warehouseInventoryGrid = Get<UI_InventoryGrid>((int)UI_InventoryGrids.WarehouseInventory);
        warehouseInventoryGrid.SetInfo(Define.EEquipSlotType.WarehouseInventory);
        warehouseInventoryGrid.gameObject.BindEvent(evt =>
        {
            var v = Get<UI_InventoryGrid>((int)UI_InventoryGrids.WarehouseInventory).MouseToCell();
            //Debug.Log(v);
            Managers.Inventory.ClickInventory(warehouseInventoryGrid.InvenType, v);
        });

        warehouseInventoryGrid.gameObject.BindEvent(evt =>
        {
            Get<UI_InventoryGrid>((int)UI_InventoryGrids.WarehouseInventory).PointEnter();
        }, EUIEvent.PointerEnter);

        warehouseInventoryGrid.gameObject.BindEvent(evt =>
        {
            Get<UI_InventoryGrid>((int)UI_InventoryGrids.WarehouseInventory).PointExit();
        }, EUIEvent.PointerExit);



        BindItemSlotEvent();
        
        InitSkillImage();


        var playerInven = GetObject((int)GameObjects.PlayerInventoryPanel);
        playerInven.SetActive(false);
        
        var warehouseInven = GetObject((int)GameObjects.WarehouseInventoryPanel);
        warehouseInven.SetActive(false);

        var npcInteractinoView = GetObject((int)GameObjects.UI_NpcInteractionView);
        npcInteractinoView.SetActive(false);
        
        var itemDiscriptionView = GetObject((int)GameObjects.UI_DiscriptionView);
        itemDiscriptionView.SetActive(false);

        return true;
    }

    
    public void SetActiveVendor(Npc npc)
    {
        Debug.Log("vendor");
    }

    public void SetNpcInteraction(Npc npc)
    {
        var npcInteractinoView = GetObject((int)GameObjects.UI_NpcInteractionView);
        npcInteractinoView.SetActive(true);
        npcInteractinoView.GetComponent<UI_NpcInteractionView>().SetInfo(npc);
    }

    public void EnableNpcInteraction()
    {
        var npcInteractinoView = GetObject((int)GameObjects.UI_NpcInteractionView);
        npcInteractinoView.SetActive(false);
    }

    public void SetDiscription(ItemBase item, float top, float left, float right)
    {
        var discriptionView = GetObject((int)GameObjects.UI_DiscriptionView);
        discriptionView.SetActive(true);
        discriptionView.GetComponent<UI_DiscriptionView>().SetInfo(item);

        // ��ġ
        // �켱����: ��, ��, ��
        RectTransform discriptionRect = discriptionView.GetComponent<RectTransform>();

        Vector3[] worldCorners = new Vector3[4];
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

        Debug.Log(top + ", " + left + ", " + right);

        Vector2 newPosition = Vector2.zero;



        newPosition = new Vector2((right + left) / 2 , top);
        Debug.Log(newPosition.x + ", " + newPosition.y + ", " + discriptionWidth + ", " + discriptionHeight);
        // 1920 1080
        //Debug.Log(Screen.width + ", " + Screen.height );


        // �켱����: ����, ����, ������
        if (top + discriptionHeight < Screen.height)  // ȭ�� �ȿ��� ���ʿ� ��ġ ����
        {
            // �߾ӿ� ��ġ (xMin�� xMax�� �߰������� ����)
            newPosition = new Vector2((right + left) / 2 , top);

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

    public void EnableDiscription()
    {
        var discriptionView = GetObject((int)GameObjects.UI_DiscriptionView);
        discriptionView.SetActive(false);
    }


    public void InventoryToggle()
    {
        var playerInven = GetObject((int)GameObjects.PlayerInventoryPanel);
        playerInven.SetActive(!playerInven.activeSelf);
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
        }
    }


    /**
    �̵� �����ϸ� true, interact � ���� �̵� �Ұ��� false
    �̵� �Ұ��� �ش� �κ� ó��
     */
    public bool MoveOrNext()
    {
        SetActiveWarehouseInventory(false);
        EnableNpcInteraction();

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
                    Get<UI_ItemSlot>((int)uitype).SetSlotState(SlotState.Enable);

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
                    SetDiscription(item, top, left, right);
                }
                else
                {
                    EnableDiscription();
                }

                if (Managers.Inventory.HoldingItem != null)
                {
                    if (Managers.Inventory.CheckHoldingItemCanEquip(slotType))
                    {
                        Get<UI_ItemSlot>((int)uitype).SetSlotState(SlotState.Enable);
                    }
                    else
                    {
                        Get<UI_ItemSlot>((int)uitype).SetSlotState(SlotState.Error);
                    }
                }



            }, EUIEvent.PointerEnter);

            Get<UI_ItemSlot>((int)uitype).gameObject.BindEvent(evt =>
            {
                EnableDiscription();
                Get<UI_ItemSlot>((int)uitype).SetSlotState(SlotState.None);
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
    }
    public Vector2Int GetInventorySize(Define.EEquipSlotType invenType)
    {
        switch (invenType)
        {
            case EEquipSlotType.PlayerInventory:
                return Get<UI_InventoryGrid>((int)UI_InventoryGrids.PlayerInventory).GridSize;

            case EEquipSlotType.WarehouseInventory:
                return Get<UI_InventoryGrid>((int)UI_InventoryGrids.WarehouseInventory).GridSize;
        }

        return new();
    }
}
