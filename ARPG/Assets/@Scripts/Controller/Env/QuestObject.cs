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

		/*if (!Managers.Quest.HasTargetQuest(Define.EQuestType.Interact, DataTemplateID))
		{
			gameObject.SetActive(false);
		}*/
	}

	public override void Interact(Player player)
	{
		if (!Managers.Quest.HasTargetQuest(Define.EQuestType.Interact, DataTemplateID))
			return;

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

					var skillGem1 = SkillGemItem.MakeSkillGemItem(40000001);

					var skillGemHolder1 = Managers.Object.Spawn<ItemHolder>(transform.position, 0);
					skillGemHolder1.SetInfo(0, skillGem1, transform.position);

					var skillGem2 = SkillGemItem.MakeSkillGemItem(40000003);

					var skillGemHolder2 = Managers.Object.Spawn<ItemHolder>(transform.position, 0);
					skillGemHolder2.SetInfo(0, skillGem2, transform.position);
				}
				break;
        }

		
		Managers.Quest.ClearTargetQuest(Define.EQuestType.Interact, DataTemplateID);

		Destroy(this.gameObject);
	}
}
