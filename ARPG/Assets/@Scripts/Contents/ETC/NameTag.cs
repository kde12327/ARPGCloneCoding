using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameTag : UI_Base
{

    public InteractableObject Owner { get; protected set; }
    enum GameObjects
    {
        Background
    }
    enum Images
    {
        Background
    }
    enum Texts
    {
        NameText
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindImages(typeof(Images));
        BindTexts(typeof(Texts));
        GetObject((int)GameObjects.Background).BindEvent((evt) =>
        {
            Owner.OnClick();
        });

        return true;
    }
    public void SetInfo(InteractableObject owner, string name)
    {
        Owner = owner;

        transform.SetParent(Owner.transform);
        GetComponent<Canvas>().sortingOrder = 500;
        transform.localPosition = new Vector2(0.2f, 2.0f);

        SetName(name);
    }

    public void SetName(string name)
    {
        GetText((int)Texts.NameText).text = name;
    }
}
