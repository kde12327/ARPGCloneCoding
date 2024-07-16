using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, -Camera.main.transform.position.z));

            Managers.Game.MouseState = Define.EMouseState.MouseDown;
            Managers.Game.MovePos = point;
        }
        if (Input.GetMouseButton(1))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, -Camera.main.transform.position.z));

            Managers.Game.MouseState = Define.EMouseState.MouseHolding;
            Managers.Game.MovePos = point;
        }
        if (Input.GetMouseButtonUp(1))
        {
            Managers.Game.MouseState = Define.EMouseState.MouseUp;
        }
    }
}