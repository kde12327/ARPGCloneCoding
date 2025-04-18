using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

#if UNITY_EDITOR
using Newtonsoft.Json;
using UnityEditor;
#endif

public class MapEditor : MonoBehaviour
{
#if UNITY_EDITOR
	// % (Ctrl), # (Shift), & (Alt)
	[MenuItem("Tools/GenerateMap %#m")]
	private static void GenerateMap()
	{
		GameObject[] gameObjects = Selection.gameObjects;

		foreach (GameObject go in gameObjects)
		{
			Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);

			using (var writer = File.CreateText($"Assets/@Resources/Data/MapData/{go.name}Collision.txt"))
			{
				writer.WriteLine(tm.cellBounds.xMin);
				writer.WriteLine(tm.cellBounds.xMax);
				writer.WriteLine(tm.cellBounds.yMin);
				writer.WriteLine(tm.cellBounds.yMax);

				for (int y = tm.cellBounds.yMax; y >= tm.cellBounds.yMin; y--)
				{
					for (int x = tm.cellBounds.xMin; x <= tm.cellBounds.xMax; x++)
					{
						TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
						if (tile != null)
						{
							if (tile.name.Contains("O"))
								writer.Write(Define.MAP_TOOL_NONE);
							else
								writer.Write(Define.MAP_TOOL_SEMI_WALL);
						}
						else
							writer.Write(Define.MAP_TOOL_WALL);
					}
					writer.WriteLine();
				}
			}
		}

		Debug.Log("Map Collision Generation Complete");
	}

    [MenuItem("Tools/Create Object Tile Asset %#o")]
    public static void CreateObjectTile()
    {
        // Monster
        Dictionary<int, Data.MonsterData> MonsterDic = LoadJson<Data.MonsterDataLoader, int, Data.MonsterData>("MonsterData").MakeDict();
        foreach (var data in MonsterDic.Values)
        {
            CustomTile customTile = ScriptableObject.CreateInstance<CustomTile>();
            customTile.Name = data.DescriptionTextID;
            customTile.DataTemplateID = data.DataId;
            customTile.ObjectType = Define.EObjectType.Monster;

            string name = $"{data.DataId}_{data.DescriptionTextID}";
            string path = "Assets/@Resources/TileMaps/Tiles/Dev/Monster";
            path = Path.Combine(path, $"{name}.Asset");

            if (File.Exists(path))
                continue;

            AssetDatabase.CreateAsset(customTile, path);
        }

        // Env
        Dictionary<int, Data.EnvData> EnvDic = LoadJson<Data.EnvDataLoader, int, Data.EnvData>("EnvData").MakeDict();
        foreach (var data in EnvDic.Values)
        {

            CustomTile customTile = ScriptableObject.CreateInstance<CustomTile>();
            customTile.Name = data.DescriptionTextID;
            customTile.DataTemplateID = data.DataId;
            customTile.ObjectType = Define.EObjectType.Env;

            string name = $"{data.DataId}_{data.DescriptionTextID}";
            string path = "Assets/@Resources/TileMaps/Tiles/Dev/Env";
            path = Path.Combine(path, $"{name}.Asset");

            if (File.Exists(path))
                continue;

            AssetDatabase.CreateAsset(customTile, path);
        }

        // Portal
        Dictionary<int, Data.PortalData> PortalDic = LoadJson<Data.PortalDataLoader, int, Data.PortalData>("PortalData").MakeDict();
        foreach (var data in PortalDic.Values)
        {

            CustomTile customTile = ScriptableObject.CreateInstance<CustomTile>();
            customTile.Name = data.DescriptionTextID;
            customTile.DataTemplateID = data.DataId;
            customTile.ObjectType = Define.EObjectType.Portal;

            string name = $"{data.DataId}_{data.DescriptionTextID}";
            string path = "Assets/@Resources/TileMaps/Tiles/Dev/Portal";
            path = Path.Combine(path, $"{name}.Asset");

            if (File.Exists(path))
                continue;

            AssetDatabase.CreateAsset(customTile, path);
        }

        // Npc
        Dictionary<int, Data.NpcData> NpcDic = LoadJson<Data.NpcDataLoader, int, Data.NpcData>("NpcData").MakeDict();
        foreach (var data in NpcDic.Values)
        {

            CustomTile customTile = ScriptableObject.CreateInstance<CustomTile>();
            customTile.Name = data.DescriptionTextID;
            customTile.DataTemplateID = data.DataId;
            customTile.ObjectType = Define.EObjectType.Npc;

            string name = $"{data.DataId}_{data.DescriptionTextID}";
            string path = "Assets/@Resources/TileMaps/Tiles/Dev/Npc";
            path = Path.Combine(path, $"{name}.Asset");

            if (File.Exists(path))
                continue;

            AssetDatabase.CreateAsset(customTile, path);
        }

        // QuestObject
        Dictionary<int, Data.QuestObjectData> QuestObjectDic = LoadJson<Data.QuestObjectDataLoader, int, Data.QuestObjectData>("QuestObjectData").MakeDict();
        foreach (var data in QuestObjectDic.Values)
        {

            CustomTile customTile = ScriptableObject.CreateInstance<CustomTile>();
            customTile.Name = data.DescriptionTextId;
            customTile.DataTemplateID = data.DataId;
            customTile.ObjectType = Define.EObjectType.QuestObject;

            string name = $"{data.DataId}_{data.DescriptionTextId}";
            string path = "Assets/@Resources/TileMaps/Tiles/Dev/QuestObject";
            path = Path.Combine(path, $"{name}.Asset");

            if (File.Exists(path))
                continue;

            AssetDatabase.CreateAsset(customTile, path);
        }

        Debug.Log("Map Tile Generation Complete");

    }

    private static Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
	{
		TextAsset textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/@Resources/Data/JsonData/{path}.json");
		return JsonConvert.DeserializeObject<Loader>(textAsset.text);
	}
#endif
}
