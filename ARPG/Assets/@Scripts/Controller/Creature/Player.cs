using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Player : Creature
{

    Vector3 _destPos = Vector3.zero;

    float StopTheshold = 0.01f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = ECreatureType.Player;
        CreatureState = ECreatureState.Idle;

        Managers.Game.OnMovePosChanged -= HandleOnMovePosChanged;
        Managers.Game.OnMovePosChanged += HandleOnMovePosChanged;
        Managers.Game.OnMouseStateChanged -= HandleOnMouseStateChanged;
        Managers.Game.OnMouseStateChanged += HandleOnMouseStateChanged;
        Speed = 2.0f;


        return true;
    }

    private void Start()
    {
        _destPos = transform.position;

    }

    private void Update()
    {
        Vector3 dir = (_destPos - transform.position);

        if (dir.sqrMagnitude > StopTheshold)
        {
            float moveDist = Mathf.Min(dir.magnitude, Time.deltaTime * Speed);
            transform.TranslateEx(dir.normalized * moveDist);
            CreatureState = Define.ECreatureState.Move;
        }
        else
        {
            CreatureState = Define.ECreatureState.Idle;
        }
    }

    private void HandleOnMovePosChanged(Vector3 pos)
    {
        _destPos = pos;
        //Debug.Log(_destPos);
    }

    private void HandleOnMouseStateChanged(EMouseState mouseState)
    {
        switch (mouseState)
        {
            case Define.EMouseState.MouseDown:
                break;
            case Define.EMouseState.MouseHolding:
                break;
            case Define.EMouseState.MouseUp:
                break;
            default:
                break;
        }
    }
}
