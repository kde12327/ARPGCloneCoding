using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : InitBase
{
	private BaseObject _target;
	public BaseObject Target
	{
		get { return _target; }
		set { _target = value; }
	}

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		Camera.main.orthographicSize = 7.0f;

		return true;
	}

	// 카메라는 다른 오브젝트 전부 업데이트 된 후에 해야함.
	void LateUpdate()
    {
		if (Target == null)
			return;

		Vector3 targetPosition = new Vector3(Target.CenterPosition.x, Target.CenterPosition.y, -10f);
		transform.position = targetPosition;
	}
}
