using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : Env
{
	public Data.QuestObjectData QuestObjectData { get; protected set; }


	public override void SetInfo(int templateID)
	{
		base.SetInfo(templateID);

		DataTemplateID = templateID;
		QuestObjectData = Managers.Data.QuestObjectDic[templateID];

        


		GameObject nameTagObject = Managers.Resource.Instantiate("NameTag");
		NameTag nameTag = nameTagObject.GetComponent<NameTag>();
		nameTag.SetInfo(this, QuestObjectData.DescriptionTextId);

		if (!Managers.Quest.HasTargetQuest(Define.EQuestType.Interact, DataTemplateID))
		{
			gameObject.SetActive(false);
		}
	}

	public override void Interact(Player player)
	{
		base.Interact(player);

		Debug.Log("QuestObject");

        switch (QuestObjectData.QuestObjectItemType)
        {
			case Define.EQuestObjectItemType.GetDefaultItems:
                {
					List<ESocketColor> socket = new();
					socket.Add(ESocketColor.Green);
					socket.Add(ESocketColor.Green);

					List<bool> link = new();
					link.Add(true);

					var defaultWeapon = EquipmentItem.MakeEquipmentItem(20100001, Define.ERarity.Normal, socket, link);

					var defaultWeaponHolder = Managers.Object.Spawn<ItemHolder>(transform.position, 0);
					defaultWeaponHolder.SetInfo(0, defaultWeapon, transform.position);

					var skillGem = SkillGemItem.MakeSkillGemItem(40000001);

					var skillGemHolder = Managers.Object.Spawn<ItemHolder>(transform.position, 0);
					skillGemHolder.SetInfo(0, skillGem, transform.position);
				}
				break;
        }

		if (Managers.Quest.HasTargetQuest(Define.EQuestType.Interact, DataTemplateID))
		{
			Managers.Quest.ClearTargetQuest(Define.EQuestType.Interact, DataTemplateID);
		}

		Destroy(this.gameObject);
	}
}
