using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class BaseObject : InitBase
{
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;
    public CircleCollider2D Collider { get; private set; }
    public SkeletonAnimation SkeletonAnim { get; private set; }
    public Rigidbody2D RigidBody { get; private set; }

	public float ColliderRadius { get { return Collider != null ? Collider.radius : 0.0f; } }
	//public float ColliderRadius { get { return Collider?.radius ?? 0.0f; } }
	public Vector3 CenterPosition { get { return transform.position + Vector3.up * ColliderRadius; } }

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

        return true;
    }

	#region Battle
	public virtual void OnDamaged(BaseObject attacker)
    {

    }

	public virtual void OnDead(BaseObject attacker)
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

		// Spine SkeletonAnimation은 SpriteRenderer 를 사용하지 않고 MeshRenderer을 사용함
		// 그렇기떄문에 2D Sort Axis가 안먹히게 되는데 SortingGroup을 SpriteRenderer,MeshRenderer을 같이 계산함.
		SortingGroup sg = Util.GetOrAddComponent<SortingGroup>(gameObject);
		sg.sortingOrder = sortingOrder;
	}
	protected virtual void UpdateAnimation()
	{
	}

	public void SetRigidBodyVelocity(Vector2 velocity)
	{
		if (RigidBody == null)
			return;

		RigidBody.velocity = velocity;

		if (velocity.x < 0)
			LookLeft = true;
		else if (velocity.x > 0)
			LookLeft = false;
	}

	public void PlayAnimation(int trackIndex, string AnimName, bool loop)
	{
		if (SkeletonAnim == null)
			return;

		SkeletonAnim.AnimationState.SetAnimation(trackIndex, AnimName, loop);
	}

	public void AddAnimation(int trackIndex, string AnimName, bool loop, float delay)
	{
		if (SkeletonAnim == null)
			return;

		SkeletonAnim.AnimationState.AddAnimation(trackIndex, AnimName, loop, delay);
	}

	public void Flip(bool flag)
	{
		if (SkeletonAnim == null)
			return;

		SkeletonAnim.Skeleton.ScaleX = flag ? -1 : 1;
	}

	public virtual void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
    {
		Debug.Log("OnAnimEventHandler");
    }
	#endregion

	#region Debug
	protected void DrawDebugBox(Vector2 point, Vector2 size)
	{
		float dx = size.x / 2;
		float dy = size.y / 2;

		Vector2 leftTop = new Vector2(point.x - dx, point.y + dy);
		Vector2 leftBottom = new Vector2(point.x - dx, point.y - dy);
		Vector2 rightTop = new Vector2(point.x + dx, point.y + dy);
		Vector2 rightBottom = new Vector2(point.x + dx, point.y - dy);

		Debug.DrawLine(leftTop, leftBottom, Color.red, 1000);
		Debug.DrawLine(leftTop, rightTop, Color.red, 1000);
		Debug.DrawLine(rightBottom, leftBottom, Color.red, 1000);
		Debug.DrawLine(rightTop, rightBottom, Color.red, 1000);
	}
	#endregion
}
