using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class StatModifier
{
	public readonly float Value;
	public readonly EStatModType Type;
	public readonly int Order;
	public readonly object Source;

	public StatModifier(float value, EStatModType type, int order, object source)
	{
		Value = value;
		Type = type;
		Order = order;
		Source = source;
	}

	public StatModifier(float value, EStatModType type) : this(value, type, (int)type, null) { }

	public StatModifier(float value, EStatModType type, int order) : this(value, type, order, null) { }

	public StatModifier(float value, EStatModType type, object source) : this(value, type, (int)type, source) { }
}

public class ProportionalStatModifier
{
	public readonly CreatureStat SourceStat;
	public readonly float SourValuePer;
	public readonly float DestValue;
	public readonly EStatModType Type;
	public readonly int Order;
	public readonly object Source;

	public ProportionalStatModifier(CreatureStat sourceStat, float sourValuePer, float destValue, EStatModType type, int order, object source)
	{
		SourceStat = sourceStat;
		SourValuePer = sourValuePer;
		DestValue = destValue;
		Type = type;
		Order = order;
		Source = source;
	}
}