using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Socket : UI_Base
{
    enum Images
    {
        SocketImage
    }

    EquipmentItem EquipedItem;

    SkillGemItem _skillGemItem;

    public ESocketColor SocketColor { get; protected set; }
 
    public int SocketNumber { get; set; }

    public SkillGemItem SkillGemItem 
    {
        get 
        {
            return _skillGemItem;
        }
        set 
        {
            _skillGemItem = value;
            if(value == null)
            {
                SkillGemUnequiped();
            }
            else
            {
                SkillGemEquiped(value);
            }
        } 
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));

        GetImage((int)Images.SocketImage).gameObject.BindEvent(evt =>
        {
            if (Managers.Inventory.HoldingItem != null && Managers.Inventory.HoldingItem.Item.ItemType == Define.EItemType.SkillGem)
            {
                UI_Item skillGemUI = Managers.Inventory.HoldingItem;
                SkillGemItem = skillGemUI.Item as SkillGemItem;

                if (SocketColor != SkillGemItem.SkillGemItemData.SkillGemColor) return;

                // 들고 있는 스킬 젬 놓기
                Managers.Inventory.HoldingItem = null;

                SkillGemItem.IsInSocket = true;
                skillGemUI.transform.SetParent(GetImage((int)Images.SocketImage).gameObject.transform);
                skillGemUI.transform.localPosition = new Vector3();
            }
            else if (Managers.Inventory.HoldingItem == null)
            {
                // 장착된 스킬 젬 들기
                SkillGemItem.IsInSocket = false;
                Managers.Inventory.HoldingItem = SkillGemItem.UIItem;
                SkillGemItem = null;
            }
            
            
        }, Define.EUIEvent.Click);

        return true;
    }

    private void Update()
    {
        if ((Managers.Inventory.HoldingItem != null && Managers.Inventory.HoldingItem.Item.ItemType == Define.EItemType.SkillGem)
            || (Managers.Inventory.HoldingItem == null && SkillGemItem != null))
        {
            GetImage((int)Images.SocketImage).raycastTarget = true;
        }
        else
        {
            GetImage((int)Images.SocketImage).raycastTarget = false;
        }
    }

    public void SetInfo(ESocketColor socketColor, EquipmentItem equipedItem, int socketNumber)
    {
        SocketColor = socketColor;
        SocketNumber = socketNumber;
        switch (socketColor)
        {
            case ESocketColor.Red:
                GetImage((int)Images.SocketImage).sprite = Managers.Resource.Load<Sprite>("RedSocket.sprite");
                break;
            case ESocketColor.Green:
                GetImage((int)Images.SocketImage).sprite = Managers.Resource.Load<Sprite>("GreenSocket.sprite");
                break;
            case ESocketColor.Blue:
                GetImage((int)Images.SocketImage).sprite = Managers.Resource.Load<Sprite>("BlueSocket.sprite");
                break;
            case ESocketColor.White:
                GetImage((int)Images.SocketImage).sprite = Managers.Resource.Load<Sprite>("WhiteSocket.sprite");
                break;
        }
        EquipedItem = equipedItem;
    }

    void SkillGemEquiped(SkillGemItem item)
    {
        EquipedItem.SetSkillGem(item, SocketNumber);
    }

    void SkillGemUnequiped()
    {
        EquipedItem.SkillGems[SocketNumber] = null;
    }
}
