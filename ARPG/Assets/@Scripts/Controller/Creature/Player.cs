using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Player : Creature
{

    Vector3 _destPos = Vector3.zero;

    private EMouseState _mouseState = EMouseState.MouseUp;
    public EMouseState MouseState 
    { 
        get { return _mouseState; } 
        set 
        {
            if (_mouseState != value)
            {
                _mouseState = value;
                switch (value)
                {
                    case EMouseState.MouseDown:
                        break;
                    case EMouseState.MouseHolding:
                        break;
                    case EMouseState.MouseUp:
                        break;
                }
            }
        } 
    }

    float StopTheshold = 0.02f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = ECreatureType.Player;

        Managers.Game.OnMovePosChanged -= HandleOnMovePosChanged;
        Managers.Game.OnMovePosChanged += HandleOnMovePosChanged;
        Managers.Game.OnMouseStateChanged -= HandleOnMouseStateChanged;
        Managers.Game.OnMouseStateChanged += HandleOnMouseStateChanged;

        Collider.includeLayers = (1 << (int)Define.ELayer.Obstacle);
        Collider.excludeLayers = (1 << (int)Define.ELayer.Monster) | (1 << (int)Define.ELayer.Player);


        return true;
    }
    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        // State
        CreatureState = ECreatureState.Idle;
    }

    private void Start()
    {
        _destPos = transform.position;
    }

    private void Update()
    {
        Vector3 dir = (_destPos - transform.position);
        Debug.Log(dir.sqrMagnitude);
        if (dir.sqrMagnitude > StopTheshold)
        {
            //float moveDist = Mathf.Min(dir.magnitude, Time.deltaTime * MoveSpeed);
            SetRigidBodyVelocity(dir.normalized * MoveSpeed);
            //transform.TranslateEx(dir.normalized * moveDist);
            CreatureState = Define.ECreatureState.Move;
        }
        else
        {
            if (MouseState == EMouseState.MouseUp)
            {
                SetRigidBodyVelocity(Vector3.zero);
                CreatureState = Define.ECreatureState.Idle;
            }
            
        }
    }

    protected override void UpdateIdle() 
    {

    }
    protected override void UpdateMove() 
    { 

    }
    protected override void UpdateSkill() 
    {
    
    }

    protected override void UpdateDead() 
    {
    
    }

    private void HandleOnMovePosChanged(Vector3 pos)
    {
        _destPos = pos;
        Debug.Log(_destPos);
        
    }

    private void HandleOnMouseStateChanged(EMouseState mouseState)
    {
        switch (mouseState)
        {
            case Define.EMouseState.MouseDown:
                MouseState = mouseState;
                break;
            case Define.EMouseState.MouseHolding:
                MouseState = mouseState;
                break;
            case Define.EMouseState.MouseUp:
                MouseState = mouseState;
                break;
            default:
                break;
        }
    }
}
