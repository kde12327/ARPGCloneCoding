using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

[RequireComponent(typeof(RectTransform))]
public class UI_Item : UI_Base
{
    public ItemBase Item { get; set; }

    [SerializeField]
    bool IsItemHolding = false;

    [SerializeField]
    bool IsItemUsing = false;

    bool SETLINKFLAG = false;

    [SerializeField]
    bool IsFlaskViewer = false;

    public bool IsReward = false;

    enum Texts
    {
        StackSizeText
    }
    enum Images
    {
        ItemImage,
        ItemPanel,
        FlaskFill
    }
    enum GameObjects
    {
        SocketPanel,
        LinkPanel,
        FlaskSlider
    }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));
        BindTexts(typeof(Texts));
        BindObjects(typeof(GameObjects));

        GetImage((int)Images.ItemPanel).gameObject.BindEvent((evt) =>
        {
            
            if(IsReward && evt.button == PointerEventData.InputButton.Left)
            {
                Debug.Log("RewardClick");
                if (Managers.Inventory.HoldingItem != null) return;
                Managers.Inventory.HoldingItem = this;
                IsReward = false;
                Managers.Quest.ChoiceReward();
                return;
            }

            if (evt.button == PointerEventData.InputButton.Left)
            {
                
                Debug.Log("UIItem" + Item + ", " + (EEquipSlotType)Item.EquipSlot + ", " + Item.EquipPos);
                
                
                if (Item.IsEquippedItem())
                {
                    Managers.Inventory.ClickSlot((EEquipSlotType)Item.EquipSlot);
                }
                else
                {
                    Managers.Inventory.ClickInventory((EEquipSlotType)Item.EquipSlot, Item.EquipPos + Item.ItemSize / 2 );
                }
            }
            else if (evt.button == PointerEventData.InputButton.Right)
            {
                if (Item.IsEquippedItem())
                {
                    Debug.Log("장착한 아이템에는 사용 불가");
                }
                else
                {
                    Managers.Inventory.RightClickInventory((EEquipSlotType)Item.EquipSlot, Item.EquipPos + Item.ItemSize / 2);
                }
            }
        }, Define.EUIEvent.Click);
        GetImage((int)Images.ItemPanel).gameObject.BindEvent((evt) =>
        {
            var rect = GetImage((int)Images.ItemPanel).GetComponent<RectTransform>();

            Managers.UI.GetSceneUI<UI_GameScene>().SetDiscription(Item, rect);
        }, Define.EUIEvent.PointerEnter);
        GetImage((int)Images.ItemPanel).gameObject.BindEvent((evt) =>
        {

            Managers.UI.GetSceneUI<UI_GameScene>().EnableDiscription();
        }, Define.EUIEvent.PointerExit);



        /*
                GetImage((int)Images.ItemImage).gameObject.BindEvent((evt) =>
                {
                    SetActiveSocket(true);
                }, Define.EUIEvent.PointerUp);

                GetImage((int)Images.ItemImage).gameObject.BindEvent((evt) =>
                {
                    SetActiveSocket(false);
                }, Define.EUIEvent.PointerExit);*/

        GetImage((int)Images.ItemPanel).gameObject.SetActive(false);
        GetImage((int)Images.ItemImage).gameObject.SetActive(false);
        GetText((int)Texts.StackSizeText).gameObject.SetActive(false);

        GetObject((int)GameObjects.SocketPanel).SetActive(false);
        GetObject((int)GameObjects.LinkPanel).SetActive(false);
        GetObject((int)GameObjects.FlaskSlider).SetActive(false);


        return true;
    }
    public void SetInfo(ItemBase item)
    {
        GetImage((int)Images.ItemPanel).gameObject.SetActive(true);
        GetImage((int)Images.ItemImage).gameObject.SetActive(true);
        Item = item;
        if(Item == null)
        {
            GetImage((int)Images.ItemPanel).gameObject.SetActive(false);
            GetImage((int)Images.ItemImage).gameObject.SetActive(false);
            GetText((int)Texts.StackSizeText).gameObject.SetActive(false);

            GetObject((int)GameObjects.SocketPanel).SetActive(false);
            GetObject((int)GameObjects.LinkPanel).SetActive(false);
            GetObject((int)GameObjects.FlaskSlider).SetActive(false);
            return;
        }

        GetImage((int)Images.ItemImage).sprite = Managers.Resource.Load<Sprite>(Item.ItemData.Icon);
        
        if(!IsFlaskViewer)
            item.UIItem = this;

        // 이미지 사이즈 조정
        GetImage((int)Images.ItemPanel).GetComponent<RectTransform>().sizeDelta *= Item.ItemSize;
        GetImage((int)Images.ItemImage).GetComponent<RectTransform>().sizeDelta *= Mathf.Min(Item.ItemSize.x, Item.ItemSize.y);

        // 아이템 타입별 ui 활성화
        if(item.ItemType == Define.EItemType.Consumable)
        {
            GetText((int)Texts.StackSizeText).gameObject.SetActive(true);
        }
        if(item.ItemType == Define.EItemType.Equipment)
        {
            var eItem = item as EquipmentItem;
            GetObject((int)GameObjects.SocketPanel).SetActive(true);
            GetObject((int)GameObjects.LinkPanel).SetActive(true);
            SetSocket();
            SETLINKFLAG = true;
        }
        if(item.ItemType == Define.EItemType.Flask)
        {
            var fItem = item as FlaskItem;
            GetObject((int)GameObjects.FlaskSlider).SetActive(true);

            GetImage((int)Images.ItemImage).sprite = Managers.Resource.Load<Sprite>(fItem.FlaskItemData.FlaskSprite);
            GetImage((int)Images.FlaskFill).sprite = Managers.Resource.Load<Sprite>(fItem.FlaskItemData.FlaskFillSprite);
        }

    }

    void SetSocket()
    {
        var item = Item as EquipmentItem;

        List<ESocketColor> sockets = item.Socket;
        


        GameObject SocketPanel = GetObject((int)GameObjects.SocketPanel);

        if(Item.ItemSize.x == 1 || sockets.Count == 1)
        {
            SocketPanel.GetComponent<CustomGridLayoutGroup>().constraintCount = 1;
        }
        else
        {
            SocketPanel.GetComponent<CustomGridLayoutGroup>().constraintCount = 2;
        }

        for (int i = 0; i < sockets.Count; i++)
        {
            GameObject socket = Managers.Resource.Instantiate("UI_Socket", SocketPanel.transform);
            socket.GetComponent<UI_Socket>().SetInfo(sockets[i], Item as EquipmentItem, i);
        }
    }

    void SetLink()
    {
        var item = Item as EquipmentItem;

        List<bool> links = item.Link;

        string str = "";
        for (int i = 0; i < links.Count; i++)
        {
            str += links[i];
        }
        Debug.Log(str);

        GameObject SocketPanel = GetObject((int)GameObjects.SocketPanel);
        RectTransform SocketRect = SocketPanel.GetComponent<RectTransform>();
        GameObject LinkPanel = GetObject((int)GameObjects.LinkPanel);
        LinkPanel.GetComponent<RectTransform>().sizeDelta = SocketRect.sizeDelta;

        List<UI_Socket> sockets = new List<UI_Socket>(SocketPanel.GetComponentsInChildren<UI_Socket>());

        for (int i = 0; i < links.Count; i++)
        {
            if (links[i] == false) continue;


            GameObject link = Managers.Resource.Instantiate("UI_Link", LinkPanel.transform);
            link.GetComponent<RectTransform>().localPosition = (sockets[i].GetComponent<RectTransform>().localPosition + sockets[i + 1].GetComponent<RectTransform>().localPosition) / 2;
            if(i % 2 == 1 || item.ItemSize.x == 1) 
            {
                link.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 90);
            }
        }

    }

    public void UpdateFlaskFill()
    {
        FlaskItem flaskItem = Item as FlaskItem;

        GetObject((int)GameObjects.FlaskSlider).GetComponent<Slider>().value = flaskItem.Charge / flaskItem.MaximumCharge * 0.64f;
    }

    public void ItemHold()
    {
        IsItemHolding = true;
        GetComponent<Image>().raycastTarget = false;
        GetImage((int)Images.ItemPanel).raycastTarget = false;
    }

    public void ItemUsing()
    {
        IsItemUsing = true;
    }

    public void ItemLetGo()
    {
        IsItemHolding = false;
        GetComponent<Image>().raycastTarget = true;
        GetImage((int)Images.ItemPanel).raycastTarget = true;
    }

/*    public void SetActiveSocket(bool value)
    {
        GetObject((int)GameObjects.SocketPanel).SetActive(value);
    }*/

    public void SetInSocket(bool value)
    {
        Debug.Log("SetInSocket: " + value);
        if(value == true)
        {
            GetComponent<Image>().raycastTarget = false;
            GetImage((int)Images.ItemPanel).raycastTarget = false;
        }
        else
        {
            GetComponent<Image>().raycastTarget = true;
            GetImage((int)Images.ItemPanel).raycastTarget = true;
        }
    }

    private void Update()
    {
        if (Item == null) return;

        if(SETLINKFLAG)
        {
            SETLINKFLAG = false;
            SetLink();
        }

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

        /*if (Item.EquipSlot >= (int)Define.EEquipSlotType.PlayerInventory && IsShowSocket && Item.ItemType == Define.EItemType.Equipment)
        {
            GetObject((int)GameObjects.SocketPanel).SetActive(true);
            IsShowSocket = false;
        }
        else
        {
            GetObject((int)GameObjects.SocketPanel).SetActive(false);
        }*/
    }


}
