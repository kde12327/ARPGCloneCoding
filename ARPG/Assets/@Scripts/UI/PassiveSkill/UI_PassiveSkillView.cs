using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_PassiveSkillView : UI_Base, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    List<UI_PassiveSkill> passiveSkills;

    List<string> passiveSkillNames = new();

    Vector3 dragStartPos;


    enum Images
    {
        PassiveSkillBackgroundImage
    }


    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImages(typeof(Images));


        UI_PassiveSkill[] passiveSkillsArr = GetComponentsInChildren<UI_PassiveSkill>();

        passiveSkills = new(passiveSkillsArr);

        passiveSkills.Sort((A, B) => A.PassiveNodeId.CompareTo(B.PassiveNodeId));

        foreach(var skill in passiveSkills)
        {
            passiveSkillNames.Add(skill.name);
        }

        BindByNames<UI_PassiveSkill>(passiveSkillNames);

        for(int i = 0; i < passiveSkillNames.Count; i++)
        {
            var idx = i;
            var skill = Get<UI_PassiveSkill>(idx);
            skill.Index = idx;
            skill.gameObject.BindEvent(evt => 
            {
                OnClickPassiveNode(idx);
            }, Define.EUIEvent.Click);
        }

        return true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {

        GetImage((int)Images.PassiveSkillBackgroundImage).transform.localPosition += (Input.mousePosition - dragStartPos);
        dragStartPos = Input.mousePosition;

        Rect viewRect = GetComponent<RectTransform>().rect;
        Rect imageRect = GetImage((int)Images.PassiveSkillBackgroundImage).rectTransform.rect;
        var lpos = GetImage((int)Images.PassiveSkillBackgroundImage).transform.localPosition;


        if(imageRect.xMax < lpos.x)
        {
            GetImage((int)Images.PassiveSkillBackgroundImage).transform.localPosition = new Vector3(imageRect.xMax, lpos.y, lpos.z);
        }
        if (viewRect.xMax - imageRect.xMax > lpos.x)
        {
            GetImage((int)Images.PassiveSkillBackgroundImage).transform.localPosition = new Vector3(viewRect.xMax - imageRect.xMax, lpos.y, lpos.z);
        }
        if (imageRect.yMax / 2 < lpos.y)
        {
            GetImage((int)Images.PassiveSkillBackgroundImage).transform.localPosition = new Vector3(lpos.x, imageRect.yMax / 2, lpos.z);
        }
        if (viewRect.yMax - imageRect.yMax > lpos.y)
        {
            GetImage((int)Images.PassiveSkillBackgroundImage).transform.localPosition = new Vector3(lpos.x, viewRect.yMax - imageRect.yMax, lpos.z);
        }


        /*Debug.Log("v : "+ viewRect.xMin + ", " + viewRect.xMax + ", " + viewRect.yMin + ", " + viewRect.yMax);
        Debug.Log("i : " + imageRect.xMin + ", " + imageRect.xMax + ", " + imageRect.yMin + ", " + imageRect.yMax);
        Debug.Log(GetImage((int)Images.PassiveSkillBackgroundImage).transform.position);
        Debug.Log(GetImage((int)Images.PassiveSkillBackgroundImage).transform.localPosition);*/
        
        //Debug.Log((imageRect.xMax < pos.x) + ", " + (viewRect.xMax - imageRect.xMax > pos.x) + ", " + (imageRect.yMax < pos.y) + ", "+ (viewRect.yMax - imageRect.yMax > pos.y));
        //Debug.Log(imageRect.xMax  + " >  " + lpos.x +" > " + (viewRect.xMax - imageRect.xMax) );
        //Debug.Log(imageRect.yMax / 2  + " >  " + lpos.y +" > " + (viewRect.yMax - imageRect.yMax) );
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    void OnClickPassiveNode(int idx)
    {
        var skill = Get<UI_PassiveSkill>(idx);

        bool result = Managers.Passive.TogglePassiveSkill(skill.PassiveNodeId);
        skill.SetFrame(result);
    }


}
