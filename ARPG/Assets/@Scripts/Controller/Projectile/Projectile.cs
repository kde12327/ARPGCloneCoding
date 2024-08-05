using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BaseObject
{
	public Creature Owner { get; private set; }
	public SkillBase Skill { get; private set; }
	public Data.ProjectileData ProjectileData { get; private set; }
	public ProjectileMotionBase ProjectileMotion { get; private set; }

	public Vector3 Target { get; set; } = Vector3.zero;

	private SpriteRenderer _spriteRenderer;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		ObjectType = Define.EObjectType.Projectile;
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.sortingOrder = SortingLayers.PROJECTILE;

		return true;
	}

	public void SetInfo(int dataTemplateID)
	{
		ProjectileData = Managers.Data.ProjectileDic[dataTemplateID];
		_spriteRenderer.sprite = Managers.Resource.Load<Sprite>(ProjectileData.ProjectileSpriteName);

		if (_spriteRenderer.sprite == null)
		{
			Debug.LogWarning($"Projectile Sprite Missing {ProjectileData.ProjectileSpriteName}");
			return;
		}
	}

	public void SetSpawnInfo(Creature owner, SkillBase skill, LayerMask layer, Vector2 target)
	{
		Owner = owner;
		Skill = skill;
		Target = target;

		// Rule
		Collider.excludeLayers = layer;

		if (ProjectileMotion != null)
			Destroy(ProjectileMotion);

		string componentName = ProjectileData.ComponentName;
		ProjectileMotion = gameObject.AddComponent(Type.GetType(componentName)) as ProjectileMotionBase;

		StraightMotion straightMotion = ProjectileMotion as StraightMotion;
		if (straightMotion != null)
			straightMotion.SetInfo(ProjectileData.DataId, owner.CenterPosition, Target, () => { Managers.Object.Despawn(this); });

		ParabolaMotion parabolaMotion = ProjectileMotion as ParabolaMotion;
		if (parabolaMotion != null)
			parabolaMotion.SetInfo(ProjectileData.DataId, owner.CenterPosition, Target, () => { Managers.Object.Despawn(this); });

		StartCoroutine(CoReserveDestroy(5.0f));
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log(other.name);
		BaseObject target = other.GetComponent<BaseObject>();
		if (target.IsValid() == false)
			return;
		// TODO
		target.OnDamaged(Owner, Skill);
		Managers.Object.Despawn(this);
	}

	private IEnumerator CoReserveDestroy(float lifeTime)
	{
		yield return new WaitForSeconds(lifeTime);
		Managers.Object.Despawn(this);
	}
}
