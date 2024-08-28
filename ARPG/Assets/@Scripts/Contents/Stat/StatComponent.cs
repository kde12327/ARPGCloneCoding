using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatComponent
{
    public Dictionary<string, CreatureStat> Stats = new ();
    private Creature _owner;

    public void SetInfo(Creature Owner)
    {
        _owner = Owner;
    }



    public void AddModifier(string statName, StatModifier modifier)
    {

        if (!Stats.ContainsKey(statName))
        {
            Stats.Add(statName, new());
        }
        Stats.TryGetValue(statName, out CreatureStat stat);
        stat.AddModifier(modifier);
    }

    public void ClearModifierFromSource(object source)
    {
        foreach(var stat in Stats)
        {
            stat.Value.ClearModifiersFromSource(source);
        }
    }

    public CreatureStat SetStat(string statName, float value, object source)
    {
        if (!Stats.ContainsKey(statName))
        {
            Stats.Add(statName, new());
        }
        Stats.TryGetValue(statName, out CreatureStat stat);
        stat.AddModifier(new(value, Define.EStatModType.Add, 0, source));

        return stat;
    }

    public CreatureStat GetStat(string statName)
    {
        if (!Stats.ContainsKey(statName))
        {
            return null;
        }
        Stats.TryGetValue(statName, out CreatureStat stat);

        return stat;
    }

    public void EquipItem(EquipmentItem item)
    {
        List<Option> options = item.GetOptions();

        foreach (var option in options)
        {

            switch (option.Stat)
            {
                case "base_maximum_life":
                    AddModifier(Stat.Life, new StatModifier(option.Value, Define.EStatModType.Add, 0, item));
                    break;
                case "base_maximum_mana":
                    AddModifier(Stat.Mana, new StatModifier(option.Value, Define.EStatModType.Add, 0, item));
                    break;
                case "base_maximum_energy_sheild":
                    AddModifier(Stat.EnergySheild, new StatModifier(option.Value, Define.EStatModType.Add, 0, item));
                    break;
                case "additional_strength":
                    AddModifier(Stat.Str, new StatModifier(option.Value, Define.EStatModType.Add, 0, item));
                    break;
                case "additional_dexterity":
                    AddModifier(Stat.Dex, new StatModifier(option.Value, Define.EStatModType.Add, 0, item));
                    break;
                case "additional_intelligence":
                    AddModifier(Stat.Int, new StatModifier(option.Value, Define.EStatModType.Add, 0, item));
                    break;
                default:
                    AddModifier(option.Stat, new StatModifier(option.Value, Define.EStatModType.Add, 0, item));
                    break;
            }
        }
    }

    public void UnEquipItem(EquipmentItem item)
    {
        ClearModifierFromSource(item);
    }

}
