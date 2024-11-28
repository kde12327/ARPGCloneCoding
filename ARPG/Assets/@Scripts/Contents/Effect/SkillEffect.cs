using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : BaseObject
{

	public override bool Init()
	{
		if (base.Init() == false)
			return false;



		return true;
	}

	private void OnAnimationEnd()
	{
		Debug.Log(" Ended");
		Destroy(this.gameObject);
	}

}
