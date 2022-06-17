using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler,IPointerUpHandler, IDragHandler,IBeginDragHandler,IEndDragHandler,IDropHandler
{
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnDragHandler = null;
	public Action<PointerEventData> OnBiOnBeginDragHandler = null;
	public Action<PointerEventData> OnEndDragHandler = null;
	public Action<PointerEventData> OnDropHandler = null;
	public Action<PointerEventData> OnPointerUpHandler = null;


    public void OnPointerClick(PointerEventData eventData)
	{
		if (OnClickHandler != null)
			OnClickHandler.Invoke(eventData);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (OnPointerUpHandler != null)
			OnPointerUpHandler.Invoke(eventData);
	}


	public void OnDrag(PointerEventData eventData)
    {
		if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
    {
		if (OnEndDragHandler != null)
			OnEndDragHandler.Invoke(eventData);
	}

    public void OnBeginDrag(PointerEventData eventData)
	{
		if (OnBiOnBeginDragHandler != null)
			OnBiOnBeginDragHandler.Invoke(eventData);
	}

    public void OnDrop(PointerEventData eventData)
	{
		if (OnDropHandler != null)
			OnDropHandler.Invoke(eventData);
	}

}
