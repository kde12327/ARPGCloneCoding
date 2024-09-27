using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UI_Item : UI_Base
{
    public ItemBase Item { get; set; }

    [SerializeField]
    bool IsItemHolding = false;

    [SerializeField]
    bool IsItemUsing = false;

    bool IsShowSocket = false;

    enum Texts
    {
        StackSizeText
    }
    enum Images
    {
        ItemImage
    }
    enum GameObjects
    {
        SocketPanel
    }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        BindTexts(typeof(Texts));
        BindObjects(typeof(GameObjects));

        /*GetObject((int)GameObjects.ItemImage).BindEvent((evt) =>
        {
            Debug.Log(Data.Name + "Item Clicked");
            transform.SetParent(Managers.UI.GetSceneUI<UI_GameScene>().transform);
            IsItemHolding = true;
            Managers.Inventory.HoldingItem = this;
        }, Define.EUIEvent.Click);*/

        GetText((int)Texts.StackSizeText).gameObject.SetActive(false);
        GetObject((int)GameObjects.SocketPanel).SetActive(false);

        

        return true;
    }
    public void SetInfo(ItemBase item)
    {
        Item = item;
        GetImage((int)Images.ItemImage).sprite = Managers.Resource.Load<Sprite>(Item.ItemData.Icon);
        item.UIItem = this;
        GetImage((int)Images.ItemImage).GetComponent<RectTransform>().sizeDelta *= Item.ItemSize;
        if(item.ItemType == Define.EItemType.Consumable)
        {
            GetText((int)Texts.StackSizeText).gameObject.SetActive(true);
        }
    }

    public void ItemHold()
    {
        IsItemHolding = true;
        GetComponent<Image>().raycastTarget = false;
        GetImage((int)Images.ItemImage).raycastTarget = false;
    }

    public void ItemUsing()
    {
        IsItemUsing = true;
    }

    public void ItemLetGo()
    {
        IsItemHolding = false;
        GetComponent<Image>().raycastTarget = true;
        GetImage((int)Images.ItemImage).raycastTarget = true;
    }

    public void SetActiveSocket(bool value)
    {
        IsShowSocket = true;
    }

    private void Update()
    {
        if(IsItemHolding)
        {
            transform.position = Input.mousePosition;
        }

        if(IsItemUsing)
        {

        }

        if(Item.ItemType == Define.EItemType.Consumable)
        {
            ConsumableItem item = Item as ConsumableItem;
            GetText((int)Texts.StackSizeText).text = item.StackSize.ToString();
        }

        if (Item.EquipSlot > (int)Define.EEquipSlotType.PlayerInventory && IsShowSocket && Item.ItemType == Define.EItemType.Equipment)
        {
            GetObject((int)GameObjects.SocketPanel).SetActive(true);
            IsShowSocket = false;
        }
        else
        {
            GetObject((int)GameObjects.SocketPanel).SetActive(false);
        }
    }
}
