using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UI_PlayerStatusView : UI_Base
{

    enum Texts
    {
        LevelText,
        PlayerNameText,
        StrText,
        DexText,
        IntText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));

        return true;
    }

    private void Start()
    {
        Managers.Object.Player.Stats.OnPlayerStatusChanged += OnStatusChange;

    }

    public void OnStatusChange()
    {
        if (gameObject.activeSelf)
        {
            UpdateStatusView();
        }
    }

    public void UpdateStatusView()
    {
        Player player = Managers.Object.Player;

        GetText((int)Texts.LevelText).text = player.Level.ToString();
        GetText((int)Texts.StrText).text = player.Stats.GetStat(Stat.Str).Value.ToString();
        GetText((int)Texts.DexText).text = player.Stats.GetStat(Stat.Dex).Value.ToString();
        GetText((int)Texts.IntText).text = player.Stats.GetStat(Stat.Int).Value.ToString();
    }
}
