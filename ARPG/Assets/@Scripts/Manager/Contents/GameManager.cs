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
    #endregion

    #region Action
    public event Action<Vector3> OnMovePosChanged;
    public event Action<EMouseState> OnMouseStateChanged;

    #endregion
}
