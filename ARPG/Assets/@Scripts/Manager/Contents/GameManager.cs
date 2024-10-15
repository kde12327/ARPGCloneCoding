using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


[Serializable]
public class GameSaveData
{
    /*public List<HeroSaveData> Heroes = new List<HeroSaveData>();*/

    public int ItemDbIdGenerator = 1;
    public List<ItemSaveData> Items = new List<ItemSaveData>();

    /*public List<QuestSaveData> ProcessingQuests = new List<QuestSaveData>(); // 진행중
    public List<QuestSaveData> CompletedQuests = new List<QuestSaveData>(); // 완료
    public List<QuestSaveData> RewardedQuests = new List<QuestSaveData>(); // 보상 받음*/
}

[Serializable]
public class ItemSaveData
{
    public int InstanceId;
    public int DbId;
    public int TemplateId;
    public int Count;
    public EEquipSlotType EquipSlot; // 장착 + 인벤 + 창고
                          //public int OwnerId;
    public int EnchantCount;
}

public class GameManager
{
    #region GameData
    GameSaveData _saveData = new GameSaveData();
    public GameSaveData SaveData { get { return _saveData; } set { _saveData = value; } }

    public void BroadcastEvent(EBroadcastEventType eventType, int value)
    {
        OnBroadcastEvent?.Invoke(eventType, value);
    }

    public int GenerateItemDbId()
    {
        int itemDbId = _saveData.ItemDbIdGenerator;
        _saveData.ItemDbIdGenerator++;
        return itemDbId;
    }

    #endregion

    #region Player
    private Vector3 _movePos;
    public Vector3 MovePos
    {
        get { return _movePos; }
        set
        {
            _movePos = value;
            OnMovePosChanged?.Invoke(value);
        }
    }

    private Vector3 _targetPos;
    public Vector3 TargetPos
    {
        get { return _targetPos; }
        set
        {
            _targetPos = value;
            OnTargetPosChanged?.Invoke(value);
        }
    }

    private EMouseState _mouseState;
    public EMouseState MouseState
    {
        get { return _mouseState; }
        set
        {
            _mouseState = value;
            OnMouseStateChanged?.Invoke(value);
        }
    }

    private EKeyState _keyState;
    public EKeyState KeyState
    {
        get { return _keyState; }
        set
        {
            _keyState = value;
            OnKeyStateChanged?.Invoke(value);
        }
    }
    #endregion

    #region Action
    public event Action<Vector3> OnMovePosChanged;
    public event Action<EMouseState> OnMouseStateChanged;
    public event Action<EKeyState> OnKeyStateChanged;
    public event Action<Vector3> OnTargetPosChanged;


    public event Action<EBroadcastEventType, int> OnBroadcastEvent;

    #endregion
}
