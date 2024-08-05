using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager
{

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

    #endregion
}
