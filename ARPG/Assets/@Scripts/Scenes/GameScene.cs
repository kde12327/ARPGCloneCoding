using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		SceneType = EScene.GameScene;

		GameObject map = Managers.Resource.Instantiate("BaseMap");
		map.transform.position = Vector3.zero;
		map.name = "@BaseMap";

		Player player = Managers.Object.Spawn<Player>(new Vector3Int(-10, -5, 0), HERO_KNIGHT_ID);

		CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
		camera.Target = player;

        {
			Managers.Object.Spawn<Monster>(new Vector3Int(0, 1, 0), MONSTER_BEAR_ID);
			//Managers.Object.Spawn<Monster>(new Vector3Int(1, 1, 0), MONSTER_SLIME_ID);
        }

		{
			Env env = Managers.Object.Spawn<Env>(new Vector3Int(0, 2, 0), ENV_TREE1_ID);
		}

		// TODO

		return true;
	}

	public override void Clear()
	{

	}
}
