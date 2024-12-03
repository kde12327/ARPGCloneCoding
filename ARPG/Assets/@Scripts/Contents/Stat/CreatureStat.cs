using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;




[Serializable]
public class CreatureStat
{
	public float BaseValue { get; private set; }


	private bool _isDirty = true;

	private List<CreatureStat> ProportionStats = new();

	[SerializeField]
	private float _value;
	public virtual float Value
	{
		get
		{
			if (_isDirty)
			{
				_value = CalculateFinalValue();
				_isDirty = false;
				ProportionStats.ForEach(val => val.SetDirty());
			}
			return _value;
		}

		private set { _value = value; }
	}

	public List<StatModifier> StatModifiers = new List<StatModifier>();

	public List<ProportionalStatModifier> ProportionalStatModifiers = new List<ProportionalStatModifier>();

	public CreatureStat()
	{
	}

	public CreatureStat(string statName) : this()
    {
        switch (statName)
        {
			case Stat.Life:
				BaseValue = 1000;
				break;
			case Stat.Mana:
				BaseValue = 100;
				break;
			case Stat.EnergySheild:
				BaseValue = 0;
				break;
			case Stat.Atk:
				BaseValue = 25;
				break;
			case Stat.CriRate:
				BaseValue = 0;
				break;
			case Stat.AttackSpeedRate:
				BaseValue = 1;
				break;
			case Stat.Str:
				BaseValue = 10;
				break;
			case Stat.Dex:
				BaseValue = 10;
				break;
			case Stat.Int:
				BaseValue = 10;
				break;
			case Stat.Damage:
				BaseValue = 100;
				break;
			case Stat.MeleePhysicalDamagePercent:
				BaseValue = 100;
				break;
			

		}
    }

	public CreatureStat(float baseValue) : this()
	{
		BaseValue = baseValue;
		_isDirty = true;
	}

	public void SetDirty()
    {
		_isDirty = true;
    }

	public virtual void AddModifier(StatModifier modifier)
	{
		_isDirty = true;
		StatModifiers.Add(modifier);
	}

	public virtual bool RemoveModifier(StatModifier modifier)
	{
		if (StatModifiers.Remove(modifier))
		{
			_isDirty = true;
			return true;
		}

		return false;
	}

	public virtual bool ClearModifiersFromSource(object source)
	{
		int numRemovals = StatModifiers.RemoveAll(mod => mod.Source == source);

		numRemovals += ProportionalStatModifiers.RemoveAll(mod => mod.Source == source);

		if (numRemovals > 0)
		{
			_isDirty = true;
			return true;
		}
		return false;
	}

	private int CompareOrder(StatModifier a, StatModifier b)
	{
		if (a.Order == b.Order)
			return 0;

		return (a.Order < b.Order) ? -1 : 1;
	}

	public virtual void AddModifier(ProportionalStatModifier modifier)
	{
		_isDirty = true;
		ProportionalStatModifiers.Add(modifier);
		modifier.SourceStat.ProportionStats.Add(this);
	}

	public virtual bool RemoveModifier(ProportionalStatModifier modifier)
	{
		if (ProportionalStatModifiers.Remove(modifier))
		{
			_isDirty = true;
			return true;
		}

		return false;
	}


	private int CompareOrder(ProportionalStatModifier a, ProportionalStatModifier b)
	{
		if (a.Order == b.Order)
			return 0;

		return (a.Order < b.Order) ? -1 : 1;
	}

	private float CalculateFinalValue()
	{
		float finalValue = BaseValue;
		float sumAddValue = 0;
		float sumPercentAdd = 0;
		float sumPercentMult = 1;

		StatModifiers.Sort(CompareOrder);

		for (int i = 0; i < StatModifiers.Count; i++)
		{
			StatModifier modifier = StatModifiers[i];

			switch (modifier.Type)
			{
				case EStatModType.Add:
					sumAddValue += modifier.Value;
					break;
				case EStatModType.PercentAdd:
					sumPercentAdd += modifier.Value / 100; ;
					/*if (i == StatModifiers.Count - 1 || StatModifiers[i + 1].Type != EStatModType.PercentAdd)
					{
						sumAddValue *= 1 + sumPercentAdd;
						sumPercentAdd = 0;
					}*/
					break;
				case EStatModType.PercentMult:
					sumPercentMult *= 1 + modifier.Value / 100;
					break;
			}
		}

		ProportionalStatModifiers.Sort(CompareOrder);

		for (int i = 0; i < ProportionalStatModifiers.Count; i++)
		{
			ProportionalStatModifier modifier = ProportionalStatModifiers[i];

			var sourceVal = modifier.SourceStat.Value;
			var val = (int)(sourceVal / modifier.SourValuePer) * modifier.DestValue;

			switch (modifier.Type)
			{
				case EStatModType.Add:
					sumAddValue += val;
					break;
				case EStatModType.PercentAdd:
					sumPercentAdd += val / 100;
					/*if (i == StatModifiers.Count - 1 || StatModifiers[i + 1].Type != EStatModType.PercentAdd)
					{
						finalValue *= 1 + sumPercentAdd;
						sumPercentAdd = 0;
					}*/
					break;
				case EStatModType.PercentMult:
					sumPercentMult *= 1 + val / 100; ;
					break;
			}
		}
		finalValue = (BaseValue + sumAddValue) * (1 + sumPercentAdd) * sumPercentMult;
		Debug.Log("stats: " + finalValue + ", " + sumAddValue + ", " + sumPercentAdd + ", " + sumPercentMult);
		Debug.Log((float)Math.Round(finalValue, 4));
		return (float)Math.Round(finalValue, 4);
	}
}
