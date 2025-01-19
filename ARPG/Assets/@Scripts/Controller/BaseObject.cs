using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.CharacterScripts;
using static Define;
using Assets.PixelFantasy.PixelHeroes.Common.Scripts.ExampleScripts;

public class BaseObject : InitBase
{
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;
    public CircleCollider2D Collider { get; private set; }
    public SkeletonAnimation SkeletonAnim { get; private set; }
    public Rigidbody2D RigidBody { get; private set; }

	public Character _character;
	public CharacterAnimation _animation;


	public float ColliderRadius { get { return Collider != null ? Collider.radius : 0.0f; } }
	//public float ColliderRadius { get { return Collider?.radius ?? 0.0f; } }
	public Vector3 CenterPosition { get { return transform.position + Vector3.up * ColliderRadius; } }

	public Animator Anim { get; private set; }

	int direction = 0;

	public int DataTemplateID { get; set; }


	bool _lookLeft = true;
	public bool LookLeft
	{
		get { return _lookLeft; }
		set
		{
			_lookLeft = value;
			Flip(!value);
		}
	}

	public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
        SkeletonAnim = GetComponent<SkeletonAnimation>();
        RigidBody = GetComponent<Rigidbody2D>();
		Anim = GetComponent<Animator>();
		_character = GetComponent<Character>();
		_animation = GetComponent<CharacterAnimation>();

		return true;
    }
	protected virtual void OnDisable()
	{
		if (SkeletonAnim == null)
			return;
		if (SkeletonAnim.AnimationState == null)
			return;

		SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
	}

	public void LookAtTarget(BaseObject target)
    {
		Vector2 dir = target.transform.position - transform.position;
		if (dir.x < 0)
			LookLeft = true;
		else
			LookLeft = false;
    }
	public void LookAtPosition(Vector3 target)
	{
		Vector2 dir = target - transform.position;
		if (dir.x < 0)
			LookLeft = true;
		else
			LookLeft = false;
	}

	public static Vector3 GetLookAtRotation(Vector3 dir)
	{
		// Mathf.Atan2�� ����� ������ ����ϰ�, ���ȿ��� ���� ��ȯ
		float angle = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;

		// Z���� �������� ȸ���ϴ� Vector3 ���� ����
		return new Vector3(0, 0, angle);
	}

	#region Battle
	public virtual void OnDamaged(BaseObject attacker, SkillBase skill)
    {

    }

	public virtual void OnDead(BaseObject attacker, SkillBase skill)
    {

    }
    #endregion

    #region Spine

    protected virtual void SetSpineAnimation(string dataLabel, int sortingOrder)
	{
		if (SkeletonAnim == null)
			return;

		SkeletonAnim.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(dataLabel);
		SkeletonAnim.Initialize(true);

		// Register AnimEvent
		if (SkeletonAnim.AnimationState != null)
		{
			SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
			SkeletonAnim.AnimationState.Event += OnAnimEventHandler;
		}

		// Spine SkeletonAnimation�� SpriteRenderer �� ������� �ʰ� MeshRenderer�� �����
		// �׷��⋚���� 2D Sort Axis�� �ȸ����� �Ǵµ� SortingGroup�� SpriteRenderer,MeshRenderer�� ���� �����.
		SortingGroup sg = Util.GetOrAddComponent<SortingGroup>(gameObject);
		sg.sortingOrder = sortingOrder;
	}
	protected virtual void UpdateAnimation()
	{
	}


	public TrackEntry PlayAnimation(int trackIndex, string animName, bool loop)
	{
		if (SkeletonAnim == null)
			return null;
		if (SkeletonAnim.AnimationState == null)
			return null;
		TrackEntry entry = SkeletonAnim.AnimationState.SetAnimation(trackIndex, animName, loop);

		if (animName == AnimName.DEAD)
			entry.MixDuration = 0;
		else
			entry.MixDuration = 0.2f;

		return entry;
	}

	public void AddAnimation(int trackIndex, string AnimName, bool loop, float delay)
	{
		if (SkeletonAnim == null)
			return;

		SkeletonAnim.AnimationState.AddAnimation(trackIndex, AnimName, loop, delay);
	}

	public void Flip(bool flag)
	{
		if (SkeletonAnim != null)
			SkeletonAnim.Skeleton.ScaleX = flag ? -1 : 1;
		if (_character != null)
			_character.Body.transform.localScale = new Vector3(flag ? Mathf.Abs(_character.transform.localScale.x) : -Mathf.Abs(_character.transform.localScale.x), _character.transform.localScale.y, _character.transform.localScale.z);
	}

	public virtual void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
    {
		//Debug.Log("OnAnimEventHandler");
    }
	#endregion


	#region Map
	public bool LerpCellPosCompleted { get; protected set; }

	Vector3Int _cellPos;
	public Vector3Int CellPos
	{
		get { return _cellPos; }
		protected set
		{
			_cellPos = value;
			LerpCellPosCompleted = false;
		}
	}

	public virtual void SetCellPos(Vector3Int cellPos, bool forceMove = false)
	{
		CellPos = cellPos;
		LerpCellPosCompleted = false;

		if (forceMove)
		{
			transform.position = Managers.Map.Cell2World(CellPos);
			LerpCellPosCompleted = true;
		}
	}

	public void LerpToCellPos(float moveSpeed)
	{
		if (LerpCellPosCompleted)
			return;

		Vector3 destPos = Managers.Map.Cell2World(CellPos);
		Vector3 dir = destPos - transform.position;
		if(Anim != null)
        {
			//Debug.Log("Lerp: " + destPos + ", " + transform.position);
		}


		if (dir.x < 0)
			LookLeft = true;

		else
			LookLeft = false;

		if (dir.magnitude < 0.01f)
		{
			transform.position = destPos;
			LerpCellPosCompleted = true;
			return;
		}

		float moveDist = Mathf.Min(dir.magnitude, moveSpeed * Time.deltaTime);
		transform.position += dir.normalized * moveDist;
	}



	#endregion
}
