using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public void Update()
    {

        if (!EventSystem.current.IsPointerOverGameObject())
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
        

        if (Input.GetKey(KeyCode.Q))
        {
            Managers.Game.KeyState = Define.EKeyState.Skill00;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            Managers.Game.KeyState = Define.EKeyState.Skill01;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            Managers.Game.KeyState = Define.EKeyState.Skill02;
        }
        else if (Input.GetKey(KeyCode.R))
        {
            Managers.Game.KeyState = Define.EKeyState.Skill03;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            string str = "";
            foreach( var key in Managers.Object.Player.Stats.Stats)
            {
                str += key.Key + ", " + key.Value.Value + "\n";
            }

            Debug.Log(str);

        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            /*string str = "";
            foreach (var key in Managers.Inventory.EquippedItems)
            {
                str += key.Key + ", " + key.Value.ItemData.Name  + "\n";
            }

            Debug.Log(str);*/
            Managers.UI.GetSceneUI<UI_GameScene>().InventoryToggle();
        }


    }
}