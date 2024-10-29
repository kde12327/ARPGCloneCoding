using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class TitleScene : BaseScene
{
	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		SceneType = Define.EScene.TitleScene;

        //StartLoadAssets();


        // �� ī�޶� ���� ������Ʈ ����
        GameObject cameraObject = new GameObject("Main Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        cameraObject.tag = "MainCamera";
        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.Skybox; // ����� Skybox�� ����
        camera.orthographicSize = 12;
        DontDestroyOnLoad(camera);



        return true;
	}

	public override void Clear()
	{

	}
}
