using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DiscriptionView : UI_Base
{
    enum Texts
    {
        DiscriptionNameText
    }

    enum Images
    {
        DiscriptionNameImage
    }

    enum GameObjects
    {
        DiscriptionVertical
    }

    List<GameObject> DiscriptionList { get; set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindTexts(typeof(Texts));
        BindImages(typeof(Images));
        BindObjects(typeof(GameObjects));

        DiscriptionList = new();



        return true;
    }

    public void SetInfo(ItemBase item)
    {

        GetText((int)Texts.DiscriptionNameText).text = item.ItemData.Name;

        GameObject discriptionParent = GetObject((int)GameObjects.DiscriptionVertical);


        int count = DiscriptionList.Count;
        GameObject[] gos = DiscriptionList.ToArray();
        for (int i = 0; i < count; i++)
        {
            Managers.Resource.Destroy(gos[i]);
        }
        DiscriptionList.Clear();

        if (item.ItemType == Define.EItemType.Equipment)
        {
            EquipmentItem equipmentItem = item as EquipmentItem;
            switch(equipmentItem.Rarity)
            {
                case Define.ERarity.Normal:
                    GetImage((int)Images.DiscriptionNameImage).color = UIColor.NORMAL;
                    GetText((int)Texts.DiscriptionNameText).color = UIColor.NORMALTEXT;
                    break;
                case Define.ERarity.Magic:
                    GetImage((int)Images.DiscriptionNameImage).color = UIColor.MAGIC;
                    GetText((int)Texts.DiscriptionNameText).color = UIColor.MAGICTEXT;
                    break;
                case Define.ERarity.Rare:
                    GetImage((int)Images.DiscriptionNameImage).color = UIColor.RARE;
                    GetText((int)Texts.DiscriptionNameText).color = UIColor.RARETEXT;
                    break;
                case Define.ERarity.Unique:
                    GetImage((int)Images.DiscriptionNameImage).color = UIColor.UNIQUE;
                    GetText((int)Texts.DiscriptionNameText).color = UIColor.UNIQUETEXT;
                    break;
            }

            //quality
            GameObject quality = Managers.Resource.Instantiate("UI_Discription", discriptionParent.transform);
            quality.GetComponent<UI_Discription>().SetText("퀄리티: +0%");
            DiscriptionList.Add(quality);

            //default
            string defaultOptionStr = "";
            var defaultMod = equipmentItem.DefaultMod;
            for (int i = 0; i < defaultMod.Count; i++)
            {
                var options = defaultMod[i].Options;
                for (int op = 0; op < options.Count; op++)
                {
                    var option = options[op];
                    defaultOptionStr += option.Stat + ": " + option.Value;
                }
                {
                    GameObject defaultOption = Managers.Resource.Instantiate("UI_Discription", discriptionParent.transform);
                    defaultOption.GetComponent<UI_Discription>().SetText(defaultOptionStr);
                    DiscriptionList.Add(defaultOption);
                }
                if (i == defaultMod.Count - 1)
                {
                    GameObject defaultDivider = Managers.Resource.Instantiate("UI_DiscriptionDivider", discriptionParent.transform);
                    DiscriptionList.Add(defaultDivider);
                }
                    
            }
            

            //requirement
            var itemData = equipmentItem.EquipmentItemBaseData;
            string requireStr = "요구 사항";

            for (int i = 0; i < itemData.RequireStats.Count; i++)
            {
                var stat = itemData.RequireStats[i];
                var value = itemData.RequireStatValues[i];

                requireStr += stat + ": " + value;
            }
            if(itemData.RequireStats.Count != 0)
            {
                GameObject requireOption = Managers.Resource.Instantiate("UI_Discription", discriptionParent.transform);
                requireOption.GetComponent<UI_Discription>().SetText(requireStr);
                DiscriptionList.Add(requireOption);

                GameObject requireDivider = Managers.Resource.Instantiate("UI_DiscriptionDivider", discriptionParent.transform);
                DiscriptionList.Add(requireDivider);
            }


            //implicit
            var implicitMod = equipmentItem.ImplicitMod;
            for (int i = 0; i < implicitMod.Count; i++)
            {
                string implicitOptionStr = "";

                var options = implicitMod[i].Options;
                for (int op = 0; op < options.Count; op++)
                {
                    var option = options[op];
                    implicitOptionStr += option.Stat + ": " + option.Value;
                }

                {
                    GameObject option = Managers.Resource.Instantiate("UI_Discription", discriptionParent.transform);
                    option.GetComponent<UI_Discription>().SetText(implicitOptionStr);
                    DiscriptionList.Add(option);
                }

                if (i == implicitMod.Count - 1)
                {
                    GameObject divider = Managers.Resource.Instantiate("UI_DiscriptionDivider", discriptionParent.transform);
                    DiscriptionList.Add(divider);
                }
            }

            //prefix suffix
            var prefixMod = equipmentItem.PrefixMod;
            for (int i = 0; i < prefixMod.Count; i++)
            {
                string modOptionStr = "";

                var options = prefixMod[i].Options;
                for (int op = 0; op < options.Count; op++)
                {
                    var option = options[op];
                    modOptionStr += option.Stat + ": " + option.Value;
                }


                {
                    GameObject option = Managers.Resource.Instantiate("UI_Discription", discriptionParent.transform);
                    option.GetComponent<UI_Discription>().SetText(modOptionStr);
                    DiscriptionList.Add(option);
                }
            }

            var suffixMod = equipmentItem.SuffixMod;
            for (int i = 0; i < suffixMod.Count; i++)
            {
                string modOptionStr = "";

                var options = suffixMod[i].Options;
                for (int op = 0; op < options.Count; op++)
                {
                    var option = options[op];
                    modOptionStr += option.Stat + ": " + option.Value;
                }


                {
                    GameObject option = Managers.Resource.Instantiate("UI_Discription", discriptionParent.transform);
                    option.GetComponent<UI_Discription>().SetText(modOptionStr);
                    DiscriptionList.Add(option);
                }
            }





        }
    }
}
