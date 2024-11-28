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

		// ���� �� ��Ʈ ������Ʈ ��Ȱ��ȭ
		{ 
			GameObject[] rootObjects = beforeScene.GetRootGameObjects();

			// �� ��Ʈ ������Ʈ�� ��ȸ�ϸ鼭 ���ϴ� �̸��� ������Ʈ�� ã��
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
        {// ���� ���� �ű� �ν��Ͻ��� �ƴ� ��� ��Ʈ ������Ʈ Ȱ��ȭ
			GameObject[] rootObjects = scene.GetRootGameObjects();

			// �� ��Ʈ ������Ʈ�� ��ȸ�ϸ鼭 ���ϴ� �̸��� ������Ʈ�� ã��
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
		// �� ��������
		Scene scene = SceneManager.GetSceneByName(sceneName);

		// ���� �ε�Ǿ� �ִ��� Ȯ��
		if (scene.isLoaded)
		{
			// ���� ��� ��Ʈ ���� ������Ʈ ��������
			GameObject[] rootObjects = scene.GetRootGameObjects();

			// �� ��Ʈ ������Ʈ�� ��ȸ�ϸ鼭 ���ϴ� �̸��� ������Ʈ�� ã��
			foreach (GameObject obj in rootObjects)
			{
				// ���� ������Ʈ�� �̸����� ã��
				if (objectName == obj.name)
				{
					return obj; // ã�� ������Ʈ ��ȯ
				}
			}
		}
		else
		{
			Debug.LogWarning("���� �ε���� �ʾҽ��ϴ�: " + sceneName);
		}

		return null; // ã�� ������ �� null ��ȯ
	}

	public void Clear()
	{
		//CurrentScene.Clear();
	}
}
