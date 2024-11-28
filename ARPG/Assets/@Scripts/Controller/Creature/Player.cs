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
            if (_mp > Stats.GetStat("Mana").Value)
                _mp = Stats.GetStat("Mana").Value;
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

            if (MaxExp <= _exp)
            {
                Level++;
                Managers.Passive.skillPoint++;
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

    public int? MapArriveId { get; set; }

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

        MaxMp = Stats.GetStat(Stat.Mana);

        MaxExp = 10;
        Exp = 0;

        Stats.GetStat(Stat.Life).AddModifier(new ProportionalStatModifier(Stats.GetStat(Stat.Str), 1, 20, EStatModType.Add, 0, this));
        Stats.GetStat(Stat.MeleePhysicalDamagePercent).AddModifier(new ProportionalStatModifier(Stats.GetStat(Stat.Str), 1, 10, EStatModType.Add, 0, this));

        Stats.GetStat(Stat.AttackSpeedRate).AddModifier(new ProportionalStatModifier(Stats.GetStat(Stat.Dex), 10, 1, EStatModType.Add, 0, this));
        Stats.GetStat(Stat.CriRate).AddModifier(new ProportionalStatModifier(Stats.GetStat(Stat.Dex), 1, 1, EStatModType.PercentAdd, 0, this));

        Stats.GetStat(Stat.Mana).AddModifier(new ProportionalStatModifier(Stats.GetStat(Stat.Int), 1, 5, EStatModType.Add, 0, this));
        Stats.GetStat(Stat.EnergySheild).AddModifier(new ProportionalStatModifier(Stats.GetStat(Stat.Int), 1, 5, EStatModType.Add, 0, this));


        Stats.OnPlayerStatusChanged -= OnPlayerStatChanged;
        Stats.OnPlayerStatusChanged += OnPlayerStatChanged;
        OnPlayerStatChanged();

        Hp = Stats.GetStat(Stat.Life).Value;
        Mp = MaxMp.Value;

    }

    public void OnPlayerStatChanged()
    {
        


        OnStatChanged();
        OnMpChanged?.Invoke(Mp / Stats.GetStat(Stat.Mana).Value);
    }


    /**
     * 토글 적용 후 노드의 상태(bool 활성화)를 반환
     */
    

    public void OnChangeSkillSetting()
    {
        // 아이템 제거/장착에 의한 스킬 재 세팅
        // TODO: 스킬 완전 초기화 방식 -> 변경된 스킬만 제거/추가
        Skills.ResetSkill();
        Managers.UI.GetSceneUI<UI_GameScene>().InitSkillImage();

        List<ItemBase> items = Managers.Inventory.GetEquippedItems();
        List<List<SkillGemItem>> skills = new();
        for (int i = 0; i < items.Count; i++)
        {
            EquipmentItem eItem = items[i] as EquipmentItem;
            if (eItem == null) continue;
            List<List<SkillGemItem>> _skills = eItem.GetSkills();
            if(_skills.Count != 0)
                skills.AddRange(_skills);
        }

        for (int i = 0; i < skills.Count; i++)
        {
            int skillId = skills[i][0].SkillGemItemData.SkillId;
            
            string className = Managers.Data.SkillDic[skillId].ClassName;
            SkillBase skillBase = gameObject.AddComponent(Type.GetType(className)) as SkillBase;
            skillBase.SetInfo(this, skillId);

            for (int j = 1; j < skills[i].Count; j++)
            {
                Debug.Log(skills[i][j].SkillGemItemData.Name);

                SupportBase supportBase = new SupportBase();
                supportBase.SetInfo(skills[i][j].SkillGemItemData.SkillId);

                skillBase.AddSupport(supportBase);
            }

            Skills.AddSkill(skillBase);

            Managers.UI.GetSceneUI<UI_GameScene>().SetSkill((UI_GameScene.UISkills)(i+3), skillBase);


        }
    }

    public void OnMapChange()
    {

        InteractTarget = null;

        if (MapArriveId != null)
        {
            Env NextPoint = null;

            Debug.Log("target: " + MapArriveId);

            List<Env> Envs = new(Managers.Object.EnvRoot.GetComponentsInChildren<Env>());

            foreach (Env env in Envs)
            {
                Debug.Log("check: " + env.DataTemplateID);
                if (env.DataTemplateID == MapArriveId)
                {
                    NextPoint = env;
                    Debug.Log(transform.position);
                    Debug.Log(NextPoint.transform.position);

                }
            }
            MapArriveId = null;
            if (NextPoint != null)
            {
                Managers.Map.RemoveObject(this);
                SetCellPos(Managers.Map.World2Cell(NextPoint.transform.position), true);
                DestPos = transform.position;
            }

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

    protected override void Update()
    {
        base.Update();

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
                if (result != EFindPathResult.Success)
                {
                    //Debug.Log(result);
                }
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
