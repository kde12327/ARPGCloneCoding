using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

public abstract class SkillBase : InitBase
{
	public Creature Owner { get; protected set; }
	public float RemainCoolTime { get; set; }

	public Data.SkillData SkillData { get; private set; }

	public List<SupportBase> SupportList { get; set; } = new List<SupportBase>();

	public Vector3 Target { get; set; } = Vector3.zero;

	SkillGemItem _skillGemItem;


	#region Stats
	public float DamageMultiplier { get
		{
			float result = SkillData.DamageMultiplier;
			foreach (var support in SupportList)
            {
				result *= support.DamageMultiplier;
			}

			return result;
		}
	}
	public float ManaMultiplier
	{
		get
		{
			float result = 1.0f;
			foreach (var support in SupportList)
			{
				result *= support.ManaMultiplier;
			}

			return result;
		}
	}
	public float AttackSpeedMultiplier
	{
		get
		{
			float result = 1.0f;
			foreach (var support in SupportList)
			{
				result *= support.AttackSpeedMultiplier;
			}

			return result;
		}
	}
	public float AreaOfEffectIncreased
	{
		get
		{
			float result = 1.0f;
			foreach (var support in SupportList)
			{
				result += support.AreaOfEffectIncreased;
			}

			return result;
		}
	}

	public float CoolTime { get; set; }
    #endregion

    public override bool Init()
	{
		if (base.Init() == false)
			return false;

		return true;
	}

	public virtual void SetInfo(Creature owner, SkillGemItem skillGemItem)
	{
		Owner = owner;
		SkillData = Managers.Data.SkillDic[skillGemItem.SkillGemItemData.SkillId];

		skillGemItem.SkillBase = this;
		_skillGemItem = skillGemItem;

		// Register AnimEvent
		if (Owner.SkeletonAnim != null && Owner.SkeletonAnim.AnimationState != null)
		{
			Owner.SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
			/*Owner.SkeletonAnim.AnimationState.Event += OnAnimEventHandler;
			Owner.SkeletonAnim.AnimationState.Complete += OnAnimCompleteHandler;*/
		}

		if(Owner.Anim != null)
        {
			Owner.OnAnimAttacked -= OnAttackEvent;
		}

		// Stat
		CoolTime = SkillData.CoolTime;
	}

	public virtual void SetInfo(Creature owner, int skillTemplateID)
	{
		Owner = owner;
		SkillData = Managers.Data.SkillDic[skillTemplateID];


		// Register AnimEvent
		if (Owner.SkeletonAnim != null && Owner.SkeletonAnim.AnimationState != null)
		{
			Owner.SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
			/*Owner.SkeletonAnim.AnimationState.Event += OnAnimEventHandler;
			Owner.SkeletonAnim.AnimationState.Complete += OnAnimCompleteHandler;*/
		}

		if (Owner.Anim != null)
		{
			Owner.OnAnimAttacked -= OnAttackEvent;
		}

		// Stat
		CoolTime = SkillData.CoolTime;
	}

	private void OnDisable()
	{
		if (Managers.Game == null)
			return;
		if (Owner.IsValid() == false)
			return;
		if (Owner.SkeletonAnim != null && Owner.SkeletonAnim.AnimationState != null)
        {
			Owner.SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;

		}

		if (Owner.Anim != null)
		{
			Owner.OnAnimAttacked -= OnAttackEvent;
			Owner.OnAnimAttackEnded -= OnAttackEndEvent;
		}
	}
	public virtual void DoSkill(Vector2 target)
	{
		Target = target;
		float timeScale = 1.0f * AttackSpeedMultiplier;
		Owner.CreatureState = Define.ECreatureState.Skill;

		if (Owner.SkeletonAnim != null)
        {
			Owner.SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
			Owner.SkeletonAnim.AnimationState.Event += OnAnimEventHandler;
			
			Owner.PlayAnimation(0, SkillData.AnimName, false).TimeScale = timeScale;

		}

		if (Owner.Anim != null)
		{
			Owner.OnAnimAttacked -= OnAttackEvent;
			Owner.OnAnimAttacked += OnAttackEvent;
			Owner.OnAnimAttackEnded -= OnAttackEndEvent;
			Owner.OnAnimAttackEnded += OnAttackEndEvent;
			//Owner.Anim.SetTrigger(SkillData.ClassName);
		}





		//TODO
		Owner.LookAtPosition(target);

		// 쿨타임 있는 스킬만 쿨타임 계산
		if (CoolTime != 0)
		{
			StartCoroutine(CoCountdownCooldown());
			Debug.Log(SkillData.Name + ": " + CoolTime);
		}

		if (Owner.ObjectType == Define.EObjectType.Player)
        {
			Player player = Owner as Player;
			player.Mp --;
		}
	}

	private IEnumerator CoCountdownCooldown()
	{
		if(OnCooldownStarted != null)
        {
			OnCooldownStarted.Invoke(SkillData.CoolTime);
		}

		// 준비된 스킬에서 제거
		if (Owner.Skills != null)
			Owner.Skills.ActiveSkills.Remove(this);
		RemainCoolTime = SkillData.CoolTime;
		yield return new WaitForSeconds(SkillData.CoolTime);
		RemainCoolTime = 0;

		// 준비된 스킬에 추가
		if (Owner.Skills != null)
			Owner.Skills.ActiveSkills.Add(this);
	}

	public virtual void CancelSkill()
    {

    }
	protected virtual void GenerateProjectile(Creature owner, Vector3 spawnPos)
    {
        Projectile projectile = Managers.Object.Spawn<Projectile>(spawnPos, SkillData.ProjectileId);

        LayerMask excludeMask = 0;
        excludeMask.AddLayer(Define.ELayer.Default);
        excludeMask.AddLayer(Define.ELayer.Projectile);
        excludeMask.AddLayer(Define.ELayer.Env);
        excludeMask.AddLayer(Define.ELayer.Obstacle);

        switch (owner.ObjectType)
        {
            case Define.EObjectType.Player:
                excludeMask.AddLayer(Define.ELayer.Player);
                break;
            case Define.EObjectType.Monster:
                excludeMask.AddLayer(Define.ELayer.Monster);
                break;
        }

		//TODO: 투사체 최종 위치 계산
		Vector3 targetPosition = transform.position + (Target - transform.position).normalized * SkillData.SkillRange;



		projectile.SetSpawnInfo(Owner, this, excludeMask, targetPosition);
    }

    public void AddSupports(List<SupportBase> supports)
	{
		SupportList = supports;

	}

	protected virtual void OnAnimEventHandler(TrackEntry trackEntry, Event e)
    {
		if(Owner.SkeletonAnim != null)
        {
			Owner.SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
		}

		if (trackEntry.Animation.Name == SkillData.AnimName)
			OnAttackEvent();
	}

	public virtual bool CanSkill()
    {
		return RemainCoolTime == 0;
	}

	protected abstract void OnAttackEvent();
	protected abstract void OnAttackEndEvent();

	public event Action<float> OnCooldownStarted;
}
