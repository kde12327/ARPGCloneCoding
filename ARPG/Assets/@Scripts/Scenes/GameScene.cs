using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class GameScene : BaseScene
{
	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		SceneType = EScene.GameScene;

		

		Managers.UI.ShowSceneUI<UI_GameScene>();

		Managers.Map.LoadMap(Managers.Scene.CurrentMapName);

		/*if (Managers.Scene.MapInit)
        {
			Managers.Scene.MapInit = false;
		}*/

		if(Managers.Object.Player == null)
        {
			Player player = Managers.Object.Spawn<Player>(new Vector3Int(-10, -5, 0), PLAYER_KNIGHT_ID);
			player.SetCellPos(new Vector3Int(-20, -8, 0), true);


			PlayerStartingpoint startingpoint = Managers.Object.EnvRoot.GetComponentInChildren<PlayerStartingpoint>();
			if (startingpoint != null)
            {
				player.SetCellPos(Managers.Map.World2Cell(startingpoint.transform.position) , true);

			}

			CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
			camera.Target = player;
			Camera.main.cullingMask = ~(1 << LayerMask.NameToLayer("Minimap"));
		}




		return true;
	}

	public override void Clear()
	{

	}
}
