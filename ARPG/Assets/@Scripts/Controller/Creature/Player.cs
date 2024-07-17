using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Player : Creature
{

    Vector3 _destPos = Vector3.zero;

    private EMouseState _mouseState = EMouseState.MouseUp;
    /*public EMouseState MouseState 
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
    }*/

    private EKeyState _keyState = EKeyState.None;
    /*public EKeyState KeyState
    {
        get { return _keyState; }
        set
        {
            if (_keyState != value)
            {
                _keyState = value;
                switch (value)
                {
                    case EKeyState.None:
                        break;
                    case EKeyState.Skill01:
                        break;
                    case EKeyState.Skill02:
                        break;
                    case EKeyState.Skill03:
                        break;
                    case EKeyState.Skill04:
                        break;
                }
            }
        }
    }*/


    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = ECreatureType.Player;

        Managers.Game.OnMovePosChanged -= HandleOnMovePosChanged;
        Managers.Game.OnMovePosChanged += HandleOnMovePosChanged;
        Managers.Game.OnMouseStateChanged -= HandleOnMouseStateChanged;
        Managers.Game.OnMouseStateChanged += HandleOnMouseStateChanged;
        Managers.Game.OnKeyStateChanged -= HandleOnKeyStateChanged;
        Managers.Game.OnKeyStateChanged += HandleOnKeyStateChanged;

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
        //Debug.Log(dir.sqrMagnitude);

        if(_keyState == EKeyState.Skill01)
        {
            SetRigidBodyVelocity(Vector3.zero);
        }
        else if (dir.sqrMagnitude > StopTheshold)
        {
            SetRigidBodyVelocity(dir.normalized * MoveSpeed);
            CreatureState = Define.ECreatureState.Move;
        }
        else
        {
            if (_mouseState == EMouseState.MouseUp)
            {
                _mouseState = EMouseState.None;
                SetRigidBodyVelocity(Vector3.zero);
                CreatureState = Define.ECreatureState.Idle;
            }
            
        }

    }

    private void FixedUpdate()
    {
        DrawDebugBox(this.transform.position + new Vector3(1, -0.5f), new Vector2(6, 3));

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
                _mouseState = mouseState;
                break;
            case Define.EMouseState.MouseHolding:
                _mouseState = mouseState;
                break;
            case Define.EMouseState.MouseUp:
                _mouseState = mouseState;
                break;
            default:
                break;
        }
    }
    private void HandleOnKeyStateChanged(EKeyState keyState)
    {
        switch (keyState)
        {
            case Define.EKeyState.None:
                _keyState = keyState;
                break;
            case Define.EKeyState.Skill01:
                _keyState = keyState;
                CreatureState = ECreatureState.Skill;
                break;
            case Define.EKeyState.Skill02:
                _keyState = keyState;
                break;
            case Define.EKeyState.Skill03:
                _keyState = keyState;
                break;
            case Define.EKeyState.Skill04:
                _keyState = keyState;
                break;
            default:
                break;
        }
    }

    public override void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
    {
        base.OnAnimEventHandler(trackEntry, e);

        // TODO
        CreatureState = ECreatureState.Idle;
        _keyState = EKeyState.None;


        //Skill
        Collider2D[] colliders = Physics2D.OverlapBoxAll(this.transform.position + new Vector3(1, -0.5f), new Vector2(6, 3), 0);
        foreach (Collider2D collider in colliders)
        {
            switch (collider.tag)
            {
                case "Monster":
                    Monster monster = collider.GetComponent<Monster>();

                    if (monster != null && monster.GetComponent<Rigidbody2D>() != null && monster.CreatureState != ECreatureState.Dead)
                    {
                        monster.OnDamaged(this);
                    }
                    break;
                case "Env":
                    Env env = collider.GetComponent<Env>();

                    if (env != null && env.EnvState != EEnvState.Dead)
                    {
                        env.OnDamaged(this);
                    }
                    break;
                default:
                    break;
            }
        }


    }

    
}
