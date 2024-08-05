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

	public List<SupportBase> SupportList { get; } = new List<SupportBase>();

	public Vector3 Target { get; set; } = Vector3.zero;


	#region Stats
	public float DamageMultiplier { get; set; }
	public float ManaMultiplier { get; set; }
	public float AttackSpeedMultiplier { get; set; }
	public float AreaOfEffectIncreased { get; set; }
	public float CoolTime { get; set; }
    #endregion

    public override bool Init()
	{
		if (base.Init() == false)
			return false;

		return true;
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

		// Stat
		DamageMultiplier = SkillData.DamageMultiplier;
		ManaMultiplier = 1.0f;
		AttackSpeedMultiplier = 1.0f;
		AreaOfEffectIncreased = 1.0f;
		CoolTime = SkillData.CoolTime;
	}

	private void OnDisable()
	{
		if (Managers.Game == null)
			return;
		if (Owner.IsValid() == false)
			return;
		if (Owner.SkeletonAnim == null)
			return;
		if (Owner.SkeletonAnim.AnimationState == null)
			return;

		Owner.SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
	}
	public virtual void DoSkill(Vector2 target)
	{
		Target = target;

		Owner.SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
        Owner.SkeletonAnim.AnimationState.Event += OnAnimEventHandler;

		float timeScale = 1.0f * AttackSpeedMultiplier;

		Owner.CreatureState = Define.ECreatureState.Skill;
		Owner.PlayAnimation(0, SkillData.AnimName, false).TimeScale = timeScale;

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
		OnCooldownStarted.Invoke(SkillData.CoolTime);

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

        projectile.SetSpawnInfo(Owner, this, excludeMask, Target);
    }

    public void AddSupport(ref SupportBase support)
	{
		SupportList.Add(support);

		// Process Support
		DamageMultiplier *= support.DamageMultiplier;
		ManaMultiplier *= support.ManaMultiplier;
		AttackSpeedMultiplier *= support.AttackSpeedMultiplier;
		AreaOfEffectIncreased += support.AreaOfEffectIncreased;
		//CoolTime = SkillData.CoolTime;
	}

	protected virtual void OnAnimEventHandler(TrackEntry trackEntry, Event e)
    {
		Owner.SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;

		if (trackEntry.Animation.Name == SkillData.AnimName)
			OnAttackEvent();
	}

	public bool CanSkill()
    {
		return RemainCoolTime == 0;
	}

	protected abstract void OnAttackEvent();

	public event Action<float> OnCooldownStarted;
}
