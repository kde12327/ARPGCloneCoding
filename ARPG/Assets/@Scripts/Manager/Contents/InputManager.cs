using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public void Update()
    {
        if (EventSystem.current == null) return;

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
            Managers.UI.GetSceneUI<UI_GameScene>().PlayerStatusToggle();

        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Managers.UI.GetSceneUI<UI_GameScene>().InventoryToggle();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Managers.UI.GetSceneUI<UI_GameScene>().PassiveSkillToggle();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var flaskItem = Managers.Inventory.GetEquippedItem(Define.EEquipSlotType.Flask1) as FlaskItem;
            flaskItem.UseFlask();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var flaskItem = Managers.Inventory.GetEquippedItem(Define.EEquipSlotType.Flask2) as FlaskItem;
            flaskItem.UseFlask();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            var flaskItem = Managers.Inventory.GetEquippedItem(Define.EEquipSlotType.Flask3) as FlaskItem;
            flaskItem.UseFlask();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            var flaskItem = Managers.Inventory.GetEquippedItem(Define.EEquipSlotType.Flask4) as FlaskItem;
            flaskItem.UseFlask();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            var flaskItem = Managers.Inventory.GetEquippedItem(Define.EEquipSlotType.Flask5) as FlaskItem;
            flaskItem.UseFlask();
        }

    }
}