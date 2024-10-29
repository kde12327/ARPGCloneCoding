using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_TitleScene : UI_Scene
{
    enum GameObjects
    {
        StartImage
    }

    enum Texts
    {
        StartText
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));

		GetObject((int)GameObjects.StartImage).BindEvent((evt) =>
		{
			Debug.Log("ChangeScene");
			Managers.Scene.CreateOrLoadGameSceneByName("ACT01_01_Map");
			//Managers.Scene.LoadScene(EScene.GameScene);
		});

		GetObject((int)GameObjects.StartImage).gameObject.SetActive(false);
		GetText((int)Texts.StartText).text = $"";

		StartLoadAssets();

		return true;
    }

	void StartLoadAssets()
	{
		Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
		{
			Debug.Log($"{key} {count}/{totalCount}");

			if (count == totalCount)
			{
				Managers.Data.Init();
				Managers.Passive.Init();

				GetObject((int)GameObjects.StartImage).gameObject.SetActive(true);
				GetText((int)Texts.StartText).text = "Touch Screen To Start";


			}
		});
	}
}
