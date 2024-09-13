using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_NpcInteraction : UI_Base
{

    enum Texts
    {
        InteractionText
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));


        return true;
    }



    public void SetText(string str)
    {
        GetText((int)Texts.InteractionText).text = str;
    }
}
