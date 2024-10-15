using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportBase
{
    //public Creature Owner { get; protected set; }
    public Data.SupportData SupportData { get; private set; }

	#region Stats
	public float DamageMultiplier { get; set; }
	public float ManaMultiplier { get; set; }
	public float AttackSpeedMultiplier { get; set; }
	public float AreaOfEffectIncreased { get; set; }
	#endregion


	public virtual void SetInfo(int supportTemplateID)
	{
		SupportData = Managers.Data.SupportDic[supportTemplateID];

		DamageMultiplier = SupportData.DamageMultiplier;
		ManaMultiplier = SupportData.ManaMultiplier;
		AttackSpeedMultiplier = SupportData.AttackSpeedMultiplier;
		AreaOfEffectIncreased = SupportData.AreaOfEffectIncreased;
	}
}
