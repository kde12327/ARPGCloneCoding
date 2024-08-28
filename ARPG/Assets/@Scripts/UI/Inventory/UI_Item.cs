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

    enum Images
    {
        ItemImage
    }
    enum GameObjects
    {
        ItemImage
    }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        BindObjects(typeof(GameObjects));

        /*GetObject((int)GameObjects.ItemImage).BindEvent((evt) =>
        {
            Debug.Log(Data.Name + "Item Clicked");
            transform.SetParent(Managers.UI.GetSceneUI<UI_GameScene>().transform);
            IsItemHolding = true;
            Managers.Inventory.HoldingItem = this;
        }, Define.EUIEvent.Click);*/


        return true;
    }
    public void SetInfo(ItemBase item)
    {
        Item = item;
        GetImage((int)Images.ItemImage).sprite = Managers.Resource.Load<Sprite>(Item.ItemData.Icon);
        item.UIItem = this;
        GetImage((int)Images.ItemImage).GetComponent<RectTransform>().sizeDelta *= Item.ItemSize;
    }

    public void ItemHold()
    {
        IsItemHolding = true;
        GetComponent<Image>().raycastTarget = false;
        GetImage((int)Images.ItemImage).raycastTarget = false;
    }

    public void ItemLetGo()
    {
        IsItemHolding = false;
        GetComponent<Image>().raycastTarget = true;
        GetImage((int)Images.ItemImage).raycastTarget = true;
    }

    private void Update()
    {
        if(IsItemHolding)
        {
            transform.position = Input.mousePosition;
        }
    }
}
