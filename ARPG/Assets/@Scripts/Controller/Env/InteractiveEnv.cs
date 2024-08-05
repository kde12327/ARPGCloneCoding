using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveEnv : Env
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
            Managers.Object.Player.InteractTarget = this;
        }
    }
}
