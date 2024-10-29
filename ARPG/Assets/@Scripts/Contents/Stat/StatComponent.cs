using System;
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
            Stats.Add(statName, new(statName));
        }
        Stats.TryGetValue(statName, out CreatureStat stat);
        stat.AddModifier(modifier);

        OnStatChanged();
    }

    public void ClearModifierFromSource(object source)
    {
        foreach(var stat in Stats)
        {
            stat.Value.ClearModifiersFromSource(source);
        }

        OnStatChanged();
    }

    public CreatureStat SetStat(string statName, float value, object source)
    {
        CreatureStat stat;
        if (!Stats.ContainsKey(statName))
        {
            Stats.Add(statName, new(value));
            Stats.TryGetValue(statName, out stat);
        }
        else
        {
            Stats.TryGetValue(statName, out stat);
            stat.AddModifier(new(value, Define.EStatModType.Add, 0, source));
        }

        OnStatChanged();

        return stat;
    }
    public CreatureStat AddStat(string optionName, float value, object source, int order = 0)
    {
        string statName = "";
        Define.EStatModType eStatModType = Define.EStatModType.Add;

        switch (optionName)
        {
            case "base_maximum_life":
                statName = Stat.Life;
                eStatModType = Define.EStatModType.Add;
                break;
            case "base_maximum_mana":
                statName = Stat.Mana;
                eStatModType = Define.EStatModType.Add;
                break;
            case "base_maximum_energy_sheild":
                statName = Stat.EnergySheild;
                eStatModType = Define.EStatModType.Add;
                break;
            case "additional_strength":
                statName = Stat.Str;
                eStatModType = Define.EStatModType.Add;
                break;
            case "additional_dexterity":
                statName = Stat.Dex;
                eStatModType = Define.EStatModType.Add;
                break;
            case "additional_intelligence":
                statName = Stat.Int;
                eStatModType = Define.EStatModType.Add;
                break;
            default:
                break;
        }

        CreatureStat stat;
        if (!Stats.ContainsKey(statName))
        {
            Stats.Add(statName, new(statName));

        }

        Stats.TryGetValue(statName, out stat);
        stat.AddModifier(new(value, eStatModType, order, source));


        OnStatChanged();

        return stat;

    }

    public CreatureStat GetStat(string statName)
    {
        if (!Stats.ContainsKey(statName))
        {
            Stats.Add(statName, new(statName));


            OnStatChanged();
        }
        Stats.TryGetValue(statName, out CreatureStat stat);

        return stat;
    }

    public void EquipItem(EquipmentItem item)
    {
        List<Option> options = item.GetOptions();

        foreach (var option in options)
        {
            AddStat(option.Stat, option.Value, item);
        }
    }

    public void UnEquipItem(EquipmentItem item)
    {
        ClearModifierFromSource(item);
    }

    void OnStatChanged()
    {
        if (OnPlayerStatusChanged != null)
        {
            OnPlayerStatusChanged.Invoke();
        }
    }

    public event Action OnPlayerStatusChanged;

}
