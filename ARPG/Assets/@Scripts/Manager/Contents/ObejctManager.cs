using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public Player player;
    public HashSet<Monster> Monsters { get; } = new HashSet<Monster>();

    #region Roots
    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject { name = name };

        return root.transform;
    }

    public Transform PlayerRoot { get { return GetRootTransform("@Players"); } }
    public Transform MonsterRoot { get { return GetRootTransform("@Monsters"); } }

    #endregion

    public T Spawn<T>(Vector3 position) where T : BaseObject
    {
        string prefabName = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName;
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();

        if(obj.ObjectType == Define.EObjectType.Creature)
        {
            Creature creature = go.GetComponent<Creature>();
            switch (creature.CreatureType)
            {
                case ECreatureType.Player:
                    obj.transform.parent = PlayerRoot;
                    player = creature as Player;
                    break;

                case ECreatureType.Monster:
                    obj.transform.parent = MonsterRoot;
                    Monster monster = creature as Monster;
                    Monsters.Add(monster);
                    break;
            }
        }
        else if (obj.ObjectType == EObjectType.Projectile)
        {
            // TODO
        }
        else if (obj.ObjectType == EObjectType.Env)
        {
            // TODO
        }

        return obj as T;
    }

    public void Despawn<T>(T obj) where T : BaseObject
    {
        EObjectType objectType = obj.ObjectType;

        if(obj.ObjectType == EObjectType.Creature)
        {
            Creature creature = obj.GetComponent<Creature>();
            switch (creature.CreatureType)
            {
                case ECreatureType.Player:
                    player = null;
                    break;

                case ECreatureType.Monster:
                    Monster monster = creature as Monster;
                    Monsters.Remove(monster);
                    break;
            }
        }
        else if(obj.ObjectType == EObjectType.Projectile)
        {
            //TODO
        }
        else if(obj.ObjectType == EObjectType.Env)
        {
            //TODO
        }

        Managers.Resource.Destroy(obj.gameObject);
    }
}
