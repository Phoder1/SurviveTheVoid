﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragNDropVisual : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
	[SerializeField]
	private Canvas mainCanvas;

	private RectTransform itemImageTransform;


	private void Awake()
	{
		itemImageTransform = GetComponent<RectTransform>();
	}


	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("Clicked");
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		Debug.Log("Drag begin");
	}
	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.Log("Drag end");
	}
	public void OnDrag(PointerEventData eventData)
	{
		Debug.Log("On drag");

		// Getting movement delta (the amount of units that the finger moved since the previous frame)
		itemImageTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;
	}
	public void OnDrop(PointerEventData eventData)
	{
		Debug.Log("On drop");
	}
}
