using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
	public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

	// <SceneName, Scene>
	Dictionary<string, Scene> Scenes = new();

	public string CurrentMapName { get; protected set; }
	public bool MapInit { get; set; }



	public void CreateOrLoadGameSceneByName(string name)
	{
		Scene beforeScene = SceneManager.GetActiveScene();

		Debug.Log(name);

		// 이전 씬 루트 컴포넌트 비활성화
		{ 
			GameObject[] rootObjects = beforeScene.GetRootGameObjects();

			// 각 루트 오브젝트를 순회하면서 원하는 이름의 오브젝트를 찾음
			foreach (GameObject obj in rootObjects)
			{
				obj.SetActive(false);
			}
		}

		Scene scene;
		if (!Scenes.TryGetValue(name, out scene))
		{
			SceneManager.CreateScene(name);
			scene = SceneManager.GetSceneByName(name);
			Scenes.Add(name, scene);

			MapInit = true;
		}
        else
        {// 현재 씬이 신규 인스턴스가 아닌 경우 루트 컴포넌트 활성화
			GameObject[] rootObjects = scene.GetRootGameObjects();

			// 각 루트 오브젝트를 순회하면서 원하는 이름의 오브젝트를 찾음
			foreach (GameObject obj in rootObjects)
			{
				obj.SetActive(true);
			}
		}

		CurrentMapName = name;
		SceneManager.SetActiveScene(scene);
		Managers.Resource.Instantiate("@GameScene");
		Managers.Quest.OnMapChanged();
        Managers.Object.Player?.OnMapChange();



		//if (SceneManager.GetSceneByName(name) == null)
		/*{
			Scene beforeScene = SceneManager.GetActiveScene();

			Debug.Log(SceneManager.GetActiveScene().name);
			SceneManager.CreateScene(name);
			Scene scene = SceneManager.GetSceneByName(name);
			SceneManager.SetActiveScene(scene);
			SceneManager.UnloadSceneAsync(beforeScene);

			Managers.Resource.Instantiate("@GameScene");
		}*/

	}




	public void LoadScene(Define.EScene type)
	{
		//Managers.Clear();
		SceneManager.LoadScene(GetSceneName(type));
	}

	private string GetSceneName(Define.EScene type)
	{
		string name = System.Enum.GetName(typeof(Define.EScene), type);
		return name;
	}


	public GameObject FindObjectInSpecificScene(string sceneName, string objectName)
	{
		// 씬 가져오기
		Scene scene = SceneManager.GetSceneByName(sceneName);

		// 씬이 로드되어 있는지 확인
		if (scene.isLoaded)
		{
			// 씬의 모든 루트 게임 오브젝트 가져오기
			GameObject[] rootObjects = scene.GetRootGameObjects();

			// 각 루트 오브젝트를 순회하면서 원하는 이름의 오브젝트를 찾음
			foreach (GameObject obj in rootObjects)
			{
				// 게임 오브젝트를 이름으로 찾기
				if (objectName == obj.name)
				{
					return obj; // 찾은 오브젝트 반환
				}
			}
		}
		else
		{
			Debug.LogWarning("씬이 로드되지 않았습니다: " + sceneName);
		}

		return null; // 찾지 못했을 때 null 반환
	}

	public void Clear()
	{
		//CurrentScene.Clear();
	}
}
