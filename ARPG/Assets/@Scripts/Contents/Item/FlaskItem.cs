using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


public class FlaskItem : ItemBase
{
	public FlaskItemBaseData FlaskItemData;


	public float MaximumCharge { get; protected set; }
	public float Charge { get; protected set; }

	public UI_Item FlaskUIItem { get; protected set; }


	public FlaskItem(int itemDataId) : base(itemDataId)
	{
		FlaskItemData = Managers.Data.FlaskItemBaseDic[itemDataId];

		MaximumCharge = FlaskItemData.MaximumCharge;
		Charge = MaximumCharge;
		Init();

	}


	public override bool Init()
	{

		return true;
	}

	public void UseFlask()
    {
		Debug.Log("UseFlask");
		if(Charge >= FlaskItemData.ChargePerUse)
        {
			Debug.Log("UseFlask Success");
			Charge -= FlaskItemData.ChargePerUse;
		}

		// todo effect
		if (FlaskItemData.FlaskType == EFlaskType.Life)
			Managers.Object.Player.Hp += 100;
		else if (FlaskItemData.FlaskType == EFlaskType.Mana)
			Managers.Object.Player.Mp += 100;

		UIItem.UpdateFlaskFill();
		FlaskUIItem.UpdateFlaskFill();
	}

	public void GetCharge(ERarity rarity)
    {
		float chargeIncome = 0;
        switch (rarity)
        {
			case ERarity.Normal:
				chargeIncome = 1;
				break;
			case ERarity.Magic:
				chargeIncome = 3;
				break;
			case ERarity.Rare:
				chargeIncome = 7;
				break;
			case ERarity.Unique:
				chargeIncome = 7;
				break;
		}

		Charge += chargeIncome;
		if (Charge > MaximumCharge)
			Charge = MaximumCharge;

		UIItem.UpdateFlaskFill();
		FlaskUIItem.UpdateFlaskFill();
	}

	public void SetFlaskUIItem(UI_Item uiItem)
    {
		FlaskUIItem = uiItem;
	}

	public static FlaskItem MakeFlaskItem(int itemDataId)
	{
		var item = Managers.Inventory.MakeItem(itemDataId) as FlaskItem;

		return item;
	}
}
