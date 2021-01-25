using UnityEngine;
using UnityEngine.EventSystems;

public class DragNDropVisual : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	[SerializeField]
	private Canvas mainCanvas;
	[SerializeField]
	private RectTransform itemImageTransform;
	private CanvasGroup itemCanvasGroup;
	private Vector2 startPosition;
	private Transform parent;


	[SerializeField]
	private Transform dragParent;


	public bool IsDraggable;

	private void Awake()
	{
		//itemImageTransform = GetComponent<RectTransform>();
		itemCanvasGroup = itemImageTransform.GetComponent<CanvasGroup>();
		startPosition = itemImageTransform.localPosition;
		parent = this.transform;
	}



	public void OnBeginDrag(PointerEventData eventData)
	{
		if (IsDraggable == true)
		{
			Debug.Log("Drag begin");
			itemCanvasGroup.alpha = 0.6f;
			itemCanvasGroup.blocksRaycasts = false;
			itemImageTransform.SetParent(dragParent);
		}
	}
	public void OnDrag(PointerEventData eventData)
	{

			// Getting movement delta (the amount of units that the finger moved since the previous frame)
			itemImageTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;
		
	}
	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.Log("Drag end");


		ReturnToPos();
	}

	public void ReturnToPos()
	{
		itemCanvasGroup.alpha = 1f;
		itemCanvasGroup.blocksRaycasts = true;

		itemImageTransform.SetParent(parent);
		itemImageTransform.SetSiblingIndex(0);
		itemImageTransform.localPosition = startPosition;
	}


}
