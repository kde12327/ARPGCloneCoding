using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using static Define;

public class Env : InteractableObject
{
    public Data.EnvData EnvData { get; protected set; }

    private Define.EEnvState _envState = Define.EEnvState.Idle;
	public Define.EEnvState EnvState
	{
		get { return _envState; }
		set
		{
			_envState = value;
			UpdateAnimation();
		}
	}

	#region Stat
	public float Hp { get; set; }
	public float MaxHp { get; set; }
	#endregion

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		ObjectType = Define.EObjectType.Env;

		return true;
	}

	public virtual void SetInfo(int templateID)
	{
		Collider.isTrigger = true;

		/*DataTemplateID = templateID;
		EnvData = Managers.Data.EnvDic[templateID];*/

		// Stat
		/*Hp = EnvData.MaxHp;
		MaxHp = EnvData.MaxHp;*/

		// Spine
		/*string ranSpine = _data.SkeletonDataIDs[Random.Range(0, _data.SkeletonDataIDs.Count)];
		SetSpineAnimation(ranSpine, SortingLayers.ENV);
		*/
		//_character.Body.GetComponent<SpriteLibrary>().spriteLibraryAsset = Managers.Resource.Load<SpriteLibraryAsset>(EnvData.SpriteLibrary);

		EnvState = Define.EEnvState.Idle;

	}

	protected override void UpdateAnimation()
	{
		switch (EnvState)
		{
			case Define.EEnvState.Idle:
				PlayAnimation(0, AnimName.IDLE, true);
				break;
			case Define.EEnvState.OnDamaged:
				PlayAnimation(0, AnimName.DAMAGED, false);
				break;
			case Define.EEnvState.Dead:
				PlayAnimation(0, AnimName.DEAD, false);
				break;
			default:
				break;
		}
	}

	#region Battle
	public override void OnDamaged(BaseObject attacker, SkillBase skill)
	{
		if (EnvState == EEnvState.Dead)
			return;

		if (EnvData == null) return;
		if (EnvData.MaxHp == 0) return;

		base.OnDamaged(attacker, skill);

		float finalDamage = 1;
		EnvState = EEnvState.OnDamaged;
		// TODO : Show UI

		Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp);
		//Debug.Log(Hp + "/" + MaxHp);

		if (Hp <= 0)
			OnDead(attacker, skill);
	}

	public override void OnDead(BaseObject attacker, SkillBase skill)
	{
		base.OnDead(attacker, skill);

		EnvState = EEnvState.Dead;

		// TODO : Drop Item	

		Managers.Object.Despawn(this);
	}
	#endregion

}
