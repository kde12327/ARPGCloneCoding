using Data;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : InteractableObject
{
	
	private SpriteRenderer _currentSprite;
	private ParabolaMotion _parabolaMotion;
	public ItemBase _item;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		ObjectType = Define.EObjectType.ItemHolder;
		_currentSprite = gameObject.GetOrAddComponent<SpriteRenderer>();
		_parabolaMotion = gameObject.GetOrAddComponent<ParabolaMotion>();
		Collider.isTrigger = true;

		return true;
	}

	public void SetInfo(int itemHolderId, ItemBase item, Vector2 pos)
	{
		_item = item;
		//_data = Managers.Data.ItemDic[itemDataId];
		_currentSprite.sprite = Managers.Resource.Load<Sprite>(_item.ItemData.Icon); // TODO
		//_parabolaMotion.SetInfo(0, transform.position, pos, endCallback: Arrived);
		GameObject nameTagObject = Managers.Resource.Instantiate("NameTag");
		
		NameTag nameTag = nameTagObject.GetComponent<NameTag>();
		nameTag.SetInfo(this, _item.ItemData.Name);
    }

    void Arrived()
    {
        _currentSprite.DOFade(0, 1f).OnComplete(() =>
        {
            if (_item != null)
            {
                // Acquire Item
            }

            Managers.Object.Despawn(this);
        });
    }

    public override void Interact(Player player)
    {
        base.Interact(player);

		if(Managers.Inventory.PickUpItem(_item))
        {
			Managers.Object.Despawn<ItemHolder>(this);
        }
	}
}
