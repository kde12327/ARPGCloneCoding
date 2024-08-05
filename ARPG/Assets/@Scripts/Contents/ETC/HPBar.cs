using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    private Slider _slider;

    public Creature Owner { get; protected set; }

    public void SetInfo(Creature owner)
    {
        _slider = GetComponentInChildren<Slider>();
        Owner = owner;
        owner.OnHpChanged -= SetValue;
        owner.OnHpChanged += SetValue;

        transform.SetParent(Owner.transform);
        GetComponent<Canvas>().sortingOrder = 300;
        transform.localPosition = new Vector2(0.2f, 2.0f);
        _slider.value = 1;
    }

    public void SetValue(float hpRatio)
    {
        hpRatio = Mathf.Max(0, hpRatio);
        hpRatio = Mathf.Min(1, hpRatio);
        _slider.value = hpRatio;
    }

    
}
