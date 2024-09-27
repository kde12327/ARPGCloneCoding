using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Player : Creature
{

    Vector3 _destPos = Vector3.zero;
    Vector3 DestPos
    {
        get { return _destPos; }
        set 
        {

            InteractTarget = null;

            if (Managers.UI.GetSceneUI<UI_GameScene>().MoveOrNext())
                _destPos = value; 
        }
    }



    #region Stats
    protected float _mp;
    public float Mp
    {
        get { return _mp; }
        set
        {
            _mp = value;
            OnMpChanged?.Invoke(_mp / MaxMp.Value);
        }
    }
    public event Action<float> OnMpChanged;

    protected float _exp;
    public float Exp
    {
        get { return _exp; }
        set
        {
            _exp = value;

            if (MaxExp > _exp)
            {
                Level++;
                _exp -= MaxExp;
            }

            OnExpChanged?.Invoke(_exp / MaxExp);
        }
    }
    public event Action<float> OnExpChanged;

    public CreatureStat MaxMp;
    public int MaxExp;

    public int Level = 1;
    
    #endregion



    private InteractableObject _interactTarget = null;
    public InteractableObject InteractTarget 
    {
        get { return _interactTarget; }
        set
        {
            if(value != null)
            {
                DestPos = value.transform.position;
            }
            _interactTarget = value;
        }
    }

    public Data.PlayerData PlayerData { get; protected set; }

    public int? NextPortalId { get; set; }

    public override ECreatureState CreatureState
    {
        get { return base.CreatureState; }
        set
        {
            if (_creatureState != value)
            {
                base.CreatureState = value;
                //Debug.Log("PlayerState: " + value + ", MouseState: " + _mouseState + ", KeyState: " + KeyState);
                switch (value)
                {
                    case ECreatureState.Idle:
                        OnStateIdle();
                        break;
                    case ECreatureState.Move:
                        OnStateMove();
                        break;
                    case ECreatureState.Skill:
                        OnStateSkill();
                        break;
                    case ECreatureState.Dead:
                        OnStateDead();
                        break;
                }
            }
        }
    }

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
    public EKeyState KeyState
    {
        get { return _keyState; }
        set
        {
            if (_keyState != value)
            {
                _keyState = value;

                if(_keyState != EKeyState.None && CreatureState != ECreatureState.Skill)
                {
                    SkillBase skill = Skills.GetSkill((int)value);
                    if(skill != null)
                    {
                        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                        Input.mousePosition.y, -Camera.main.transform.position.z));
                        if (skill.CanSkill())
                        {
                            skill.DoSkill(point);
                        }
                        else
                        {
                            _keyState = EKeyState.None;
                        }
                    }
                }
            }
        }
    }


    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = EObjectType.Player;

        Managers.Game.OnMovePosChanged -= HandleOnMovePosChanged;
        Managers.Game.OnMovePosChanged += HandleOnMovePosChanged;
        Managers.Game.OnMouseStateChanged -= HandleOnMouseStateChanged;
        Managers.Game.OnMouseStateChanged += HandleOnMouseStateChanged;
        Managers.Game.OnKeyStateChanged -= HandleOnKeyStateChanged;
        Managers.Game.OnKeyStateChanged += HandleOnKeyStateChanged;

        Collider.includeLayers = (1 << (int)Define.ELayer.Obstacle);
        Collider.excludeLayers = (1 << (int)Define.ELayer.Monster) | (1 << (int)Define.ELayer.Player);

        // Map 
        Collider.isTrigger = true;
        RigidBody.simulated = true;

        // Scene UI
        UI_GameScene gameSceneUI = Managers.UI.GetSceneUI<UI_GameScene>();
        OnHpChanged -= gameSceneUI.SetHpBarValue;
        OnHpChanged += gameSceneUI.SetHpBarValue;

        OnMpChanged -= gameSceneUI.SetMpBarValue;
        OnMpChanged += gameSceneUI.SetMpBarValue;

        OnExpChanged -= gameSceneUI.SetExpBarValue;
        OnExpChanged += gameSceneUI.SetExpBarValue;

        MaxMp = new CreatureStat(100.0f);
        Mp = MaxMp.Value;

        MaxExp = 10;
        Exp = 0;


        PlayerData = CreatureData as Data.PlayerData;
        return true;
    }
    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        // State
        CreatureState = ECreatureState.Idle;

        // Skill
        Skills = gameObject.GetOrAddComponent<SkillComponent>();
        Skills.SetInfo(this);

        int skillTemplateID = CreatureData.SkillIdList[0];

        string className = Managers.Data.SkillDic[skillTemplateID].ClassName;
        SkillBase attack1 = gameObject.AddComponent(Type.GetType(className)) as SkillBase;
        attack1.SetInfo(this, skillTemplateID);
        SkillBase attack2 = gameObject.AddComponent(Type.GetType(className)) as SkillBase;
        attack2.SetInfo(this, skillTemplateID);
        SkillBase attack3 = gameObject.AddComponent(Type.GetType(className)) as SkillBase;
        attack3.SetInfo(this, skillTemplateID);

        SupportBase support1 = gameObject.AddComponent<SupportBase>();
        support1.SetInfo(11001);

        SupportBase support2 = gameObject.AddComponent<SupportBase>();
        support2.SetInfo(11002);

        attack1.AddSupport(ref support1);
        attack2.AddSupport(ref support2);
        attack3.AddSupport(ref support1);
        attack3.AddSupport(ref support2);

        Skills.AddSkill(attack1);
        Managers.UI.GetSceneUI<UI_GameScene>().SetSkill(UI_GameScene.UISkills.UI_SkillQ, attack1);
        Skills.AddSkill(attack2);
        Managers.UI.GetSceneUI<UI_GameScene>().SetSkill(UI_GameScene.UISkills.UI_SkillW, attack2);
        Skills.AddSkill(attack3);
        Managers.UI.GetSceneUI<UI_GameScene>().SetSkill(UI_GameScene.UISkills.UI_SkillE, attack3);

        // skill cooldown test
        int skillTemplateID2 = 10004;
        string className2 = Managers.Data.SkillDic[skillTemplateID2].ClassName;
        SkillBase attack4 = gameObject.AddComponent(Type.GetType(className2)) as SkillBase;
        attack4.SetInfo(this, skillTemplateID2);
        Skills.AddSkill(attack4);
        Managers.UI.GetSceneUI<UI_GameScene>().SetSkill(UI_GameScene.UISkills.UI_SkillR, attack4);



    }

    private void Start()
    {
    }

    public void OnMapChange()
    {
        InteractTarget = null;

        if(NextPortalId != null)
        {
            Portal NextPortal = null;

            foreach (Env env in Managers.Object.Envs)
            {
                if (env.DataTemplateID == NextPortalId)
                {
                    NextPortal = env as Portal;
                }
            }
            NextPortalId = null;

            SetCellPos(Managers.Map.World2Cell(NextPortal.transform.position), true);
            DestPos = transform.position;
        }
    }

    public override void SetCellPos(Vector3Int cellPos, bool forceMove = false)
    {
        base.SetCellPos(cellPos, forceMove);
        if(forceMove == true)
        {
            DestPos = transform.position;
        }
    }

    private void Update()
    {
        Vector3 dir = (DestPos - transform.position);
        //Debug.Log(dir.sqrMagnitude);

        if(CreatureState != ECreatureState.Skill)
        {
            if (Managers.Map.World2Cell(DestPos) == Managers.Map.World2Cell(transform.position))
            {
                CreatureState = Define.ECreatureState.Idle;
                if(InteractTarget != null &&(transform.position - InteractTarget.transform.position).sqrMagnitude < 1)
                {
                    InteractTarget.Interact(this);
                    InteractTarget = null;
                }
            }
            else
            {
                CreatureState = Define.ECreatureState.Move;
                EFindPathResult result = FindPathAndMoveToCellPos(DestPos, PLAYER_DEFAULT_MOVE_DEPTH);
            }

        }

        if (_mouseState == EMouseState.MouseUp)
        {
            _mouseState = EMouseState.None;
        }

        
    }

    /*protected override void UpdateIdle() 
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
    
    }*/

    private void OnStateIdle()
    {
        //SetRigidBodyVelocity(Vector3.zero);
    }
    private void OnStateMove()
    {
        KeyState = EKeyState.None;
    }
    private void OnStateSkill()
    {
        //SetRigidBodyVelocity(Vector3.zero);
    }
    private void OnStateDead()
    {
        //SetRigidBodyVelocity(Vector3.zero);
    }

    private void HandleOnMovePosChanged(Vector3 pos)
    {
        DestPos = pos;

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
        KeyState = keyState;
    }

    public override void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
    {
        base.OnAnimEventHandler(trackEntry, e);


    }

    
}
