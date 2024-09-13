using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Discription : UI_Base
{
    enum Texts
    {
        Text
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
        GetText((int)Texts.Text).text = str;
    }
}
