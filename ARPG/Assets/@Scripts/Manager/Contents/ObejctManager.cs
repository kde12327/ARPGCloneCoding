using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public Player Player;
    public HashSet<Monster> Monsters { get; } = new HashSet<Monster>();
	public HashSet<Projectile> Projectiles { get; } = new HashSet<Projectile>();
    public HashSet<Env> Envs { get; } = new HashSet<Env>();
    public HashSet<EffectBase> Effects { get; } = new HashSet<EffectBase>();
    public HashSet<ItemHolder> ItemHolders { get; } = new HashSet<ItemHolder>();


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
	public Transform ProjectileRoot { get { return GetRootTransform("@Projectiles"); } }
    public Transform EnvRoot { get { return GetRootTransform("@Env"); } }
    public Transform ItemHolderRoot { get { return GetRootTransform("@ItemHolders"); } }

    #endregion

    public void ShowDamageFont(Vector2 position, float damage, Transform parent, bool isCritical = false)
    {
        GameObject go = Managers.Resource.Instantiate("DamageFont", pooling: true);
        DamageFont damageText = go.GetComponent<DamageFont>();
        damageText.SetInfo(position, damage, parent, isCritical);
    }

    public GameObject SpawnGameObject(Vector3 position, string prefabName)
    {
        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        go.transform.position = position;
        return go;
    }

    public T Spawn<T>(Vector3Int cellPos, int templateID) where T : BaseObject
    {
        Vector3 spawnPos = Managers.Map.Cell2World(cellPos);
        return Spawn<T>(spawnPos, templateID);
    }

    public T Spawn<T>(Vector3 position, int templateID) where T : BaseObject
    {
        string prefabName = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName;
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();

        if(obj.ObjectType == Define.EObjectType.Player)
        {
            Player player = go.GetComponent<Player>();
            Player = player;
            obj.transform.parent = PlayerRoot;
            player.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.Monster)
        {
            Monster monster = go.GetComponent<Monster>();
            obj.transform.parent = MonsterRoot;
            Monsters.Add(monster);

            monster.SetInfo(templateID);

        }
        else if (obj.ObjectType == EObjectType.Projectile)
        {
            obj.transform.parent = ProjectileRoot;

            Projectile projectile = go.GetComponent<Projectile>();
            Projectiles.Add(projectile);

            projectile.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.Env)
        {
            obj.transform.parent = EnvRoot;

            Env env = go.GetComponent<Env>();
            Envs.Add(env);

            env.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.Portal)
        {
            obj.transform.parent = EnvRoot;

            Portal portal = go.GetComponent<Portal>();
            Envs.Add(portal);

            portal.SetInfo(templateID);
        }
        else if (obj.ObjectType == EObjectType.ItemHolder)
        {
            obj.transform.parent = ItemHolderRoot;

            ItemHolder itemHolder = go.GetOrAddComponent<ItemHolder>();
            ItemHolders.Add(itemHolder);
        }

        return obj as T;
    }

    public void Despawn<T>(T obj) where T : BaseObject
    {
        EObjectType objectType = obj.ObjectType;

        if(objectType == EObjectType.Player)
        {
            Player = null;
        }
        else if (objectType == EObjectType.Monster)
        {
            Monster monster = obj.GetComponent<Monster>();
            Monsters.Remove(monster);
        }
        else if(objectType == EObjectType.Projectile)
        {
            Projectile projectile = obj as Projectile;
            Projectiles.Remove(projectile);
        }
        else if(objectType == EObjectType.Env)
        {
            Env env = obj as Env;
            Envs.Remove(env);
        }
        else if (obj.ObjectType == EObjectType.Portal)
        {
            Portal portal = obj as Portal;
            Envs.Remove(portal);
        }
        else if (obj.ObjectType == EObjectType.ItemHolder)
        {
            ItemHolder itemHolder = obj as ItemHolder;
            ItemHolders.Remove(itemHolder);
        }

        Managers.Resource.Destroy(obj.gameObject);
    }

    #region Map 관련
    public void ClearObjects()
    {
        foreach (Monster monster in Monsters)
        {
            Managers.Resource.Destroy(monster.gameObject);
        }

        Monsters.Clear();

        foreach (Env env in Envs)
        {
            Managers.Resource.Destroy(env.gameObject);
        }

        Envs.Clear();
    }

    #endregion


    #region Skill 판정
    public List<Creature> FindConeRangeTargets(Creature owner, Vector3 dir, float range, int angleRange, bool isAllies = false)
    {
        List<Creature> targets = new List<Creature>();
        List<Creature> ret = new List<Creature>();

        EObjectType targetType = Util.DetermineTargetType(owner.ObjectType, isAllies);

        if (targetType == EObjectType.Monster)
        {
            var objs = Managers.Map.GatherObjects<Monster>(owner.transform.position, range, range);
            targets.AddRange(objs);
        }
        else if (targetType == EObjectType.Player)
        {
            var objs = Managers.Map.GatherObjects<Player>(owner.transform.position, range, range);
            targets.AddRange(objs);
        }

        foreach (var target in targets)
        {
            // 1. 거리안에 있는지 확인
            var targetPos = target.transform.position;
            float distance = Vector3.Distance(targetPos, owner.transform.position);

            if (distance > range)
                continue;

            // 2. 각도 확인
            if (angleRange != 360)
            {
                //BaseObject ownerTarget = (owner as Creature).Target;

                // 2. 부채꼴 모양 각도 계산
                float dot = Vector3.Dot((targetPos - owner.transform.position).normalized, dir.normalized);
                float degree = Mathf.Rad2Deg * Mathf.Acos(dot);

                if (degree > angleRange / 2f)
                    continue;
            }

            ret.Add(target);
        }

        return ret;
    }

    #endregion
}
