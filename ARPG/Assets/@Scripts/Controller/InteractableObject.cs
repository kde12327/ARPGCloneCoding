using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : BaseObject
{
    public virtual void Interact(Player player)
    {
        Debug.Log("Interact");
        player.InteractTarget = null;

    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }
    }

    public void OnClick()
    {
        Managers.Object.Player.InteractTarget = this;
    }

}
