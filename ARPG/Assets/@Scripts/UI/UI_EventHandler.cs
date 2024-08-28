using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	public event Action<PointerEventData> OnClickHandler = null;
	public event Action<PointerEventData> OnPointerDownHandler = null;
	public event Action<PointerEventData> OnPointerUpHandler = null;
	public event Action<PointerEventData> OnDragHandler = null;
	public event Action<PointerEventData> OnPointerEnterHandler = null;
	public event Action<PointerEventData> OnPointerExitHandler = null;

	public void OnPointerClick(PointerEventData eventData)
	{
		OnClickHandler?.Invoke(eventData);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		OnPointerDownHandler?.Invoke(eventData);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		OnPointerUpHandler?.Invoke(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		OnDragHandler?.Invoke(eventData);
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
		OnPointerEnterHandler?.Invoke(eventData);
	}

	public void OnPointerExit(PointerEventData eventData)
    {
		OnPointerExitHandler?.Invoke(eventData);
	}
}
