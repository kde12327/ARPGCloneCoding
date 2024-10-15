using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

[RequireComponent(typeof(RectTransform))]
public class UI_InventoryGrid : UI_Base
{

    public Define.EEquipSlotType InvenType = EEquipSlotType.None;


    [Header("Grid Config")]
    public Vector2Int GridSize = new(5, 5);

    public UI_Item[,] Items { get; set; }

    bool IsPointEnter { get; set; } = false;

    Vector2? MousePosition;

    List<Vector2Int> ActiveCellList { get; set; } = new List<Vector2Int>();

    List<string> GridCellNames = new();

    enum GameObjects
    {
        InventoryBackground
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));


        Items = new UI_Item[GridSize.x, GridSize.y];

        for(int y = 0; y < GridSize.y; y++)
        {
            for(int x = 0; x < GridSize.x; x++)
            {
                GameObject cell = Managers.Resource.Instantiate("UI_GridCell", GetObject((int)GameObjects.InventoryBackground).transform);
                cell.name = GetCellName(new Vector2Int(x, y));
                GridCellNames.Add(cell.name);
            }
        }

        BindByNames<UI_GridCell>(GridCellNames);

        gameObject.BindEvent(evt =>
        {
            if(evt.button == PointerEventData.InputButton.Left)
            {
                var v = MouseToCell();
                Debug.Log(v);
                Managers.Inventory.ClickInventory(InvenType, v);
            }
            else if(evt.button == PointerEventData.InputButton.Right)
            {
                var v = MouseToCell();
                Debug.Log("rihgt click");
                Managers.Inventory.RightClickInventory(InvenType, v);
            }

        }, EUIEvent.Click);

        gameObject.BindEvent(evt =>
        {
            PointEnter();
        }, EUIEvent.PointerEnter);

        gameObject.BindEvent(evt =>
        {
            PointExit();
        }, EUIEvent.PointerExit);

        return true;
    }

    public void SetInfo(Define.EEquipSlotType invenType)
    {
        InvenType = invenType;
    }

    public void Update()
    {
        // clear grid hover state
        foreach (var cell in ActiveCellList)
        {
            int idx = GetCellIdx(cell);
            if (idx != -1)
            {
                Get<UI_GridCell>(idx).SetSlotState(Define.ESlotState.None);
            }
        }

        ActiveCellList.Clear();

        // check and change grid hover state
        if (IsPointEnter)
        {
            MousePosition = MouseToCell();
            //Debug.Log("MousePosition: " + MousePosition);

            List<Vector2Int> list;
            Define.ESlotState state = Managers.Inventory.GetCellList(InvenType, MousePosition.Value,  out list);
            ActiveCellList.AddRange(list);

            

            float left = int.MaxValue;
            float right = 0;
            float top = 0;


            foreach (var cell in ActiveCellList)
            {
                int idx = GetCellIdx(cell);
                if (idx != -1)
                {
                    Get<UI_GridCell>(GetCellIdx(cell)).SetSlotState(state);
                    var rect = Get<UI_GridCell>(GetCellIdx(cell)).GetComponent<RectTransform>();

                    // RectTransform의 World 좌표를 구해서 화면 내 위치로 변환
                    Vector3[] worldCorners = new Vector3[4];
                    rect.GetWorldCorners(worldCorners);

                    left = Mathf.Min(left, worldCorners[0].x);
                    right = Mathf.Max(right, worldCorners[3].x);
                    top = Mathf.Max(top, worldCorners[1].y);
                }
            }

            ItemBase item = Managers.Inventory.GetItemByPosInInventory(InvenType, MousePosition.Value);

            if (item != null)
            {
                Managers.UI.GetSceneUI<UI_GameScene>().SetDiscription(item, top, left, right);
                /*if(item.ItemType == EItemType.Equipment)
                {
                    var eItem = item as EquipmentItem;
                    eItem.UIItem.SetActiveSocket(true);
                }*/
            }
            else
            {
                Managers.UI.GetSceneUI<UI_GameScene>().EnableDiscription();
            }
        }
    }

    public void PutItem(UI_Item item, Vector2 itemPos)
    {
        item.transform.SetParent(this.transform, false);
        item.transform.localPosition = CellToPosition(itemPos);
    }

    public void PointEnter()
    {
        IsPointEnter = true;
    }
    
    public void PointExit()
    {
        IsPointEnter = false;
        Managers.UI.GetSceneUI<UI_GameScene>().EnableDiscription();

    }

    public Vector2 MouseToCell()
    {
        RectTransform rectTransform = GetObject((int)GameObjects.InventoryBackground).GetComponent<RectTransform>();


        Vector2 gridPosition =
            new(
                Input.mousePosition.x - rectTransform.rect.x,
                rectTransform.rect.y - Input.mousePosition.y
            );


        Vector2 result;
        Vector2 clickPosition = Input.mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, clickPosition, null, out result);
        result *= new Vector2(1, -1);

        Vector2 girdSize = new Vector2(rectTransform.rect.width / GridSize.x, rectTransform.rect.height / GridSize.y);

        Vector2 slotPosition = new((result.x / (girdSize.x)), (result.y / (girdSize.y)));

        return slotPosition;
    }

    public Vector2 CellToPosition(Vector2 pos)
    {
        RectTransform rectTransform = GetObject((int)GameObjects.InventoryBackground).GetComponent<RectTransform>();
        Vector2 girdSize = new Vector2(rectTransform.rect.width / GridSize.x, rectTransform.rect.height / GridSize.y);

        Vector2 result = new(pos.x * girdSize.x,  -1 * pos.y * girdSize.y);

        //Debug.Log(pos + ", " + result);


        return result;
    }

    public string GetCellName(Vector2Int pos)
    {
        return "UI_GridCell_" + pos.x + "_" + pos.y;
    }
    
    public int GetCellIdx(Vector2Int pos)
    {
        //Debug.Log(pos.x + ", " + pos.y);
        string str = GetCellName(pos);

        int idx = -1;

        for(int i = 0; i < GridCellNames.Count; i++)
        {
            if(GridCellNames[i] == str)
            {
                idx = i;
            }
        }

        return idx;
    }


}
