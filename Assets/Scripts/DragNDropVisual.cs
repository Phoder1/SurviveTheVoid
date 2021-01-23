using UnityEngine;
using UnityEngine.EventSystems;

public class DragNDropVisual : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	[SerializeField]
	private Canvas mainCanvas;
	private RectTransform itemImageTransform;
	private CanvasGroup itemCanvasGroup;
	private Vector2 startPosition;
	[SerializeField]
	private Transform parent;
	[SerializeField]
	private Transform dragParent;

	public bool IsDraggable;

	private void Awake()
	{
		itemImageTransform = GetComponent<RectTransform>();
		itemCanvasGroup = GetComponent<CanvasGroup>();

		startPosition = itemImageTransform.position;
	}


	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("Clicked");
	}
	public void OnBeginDrag(PointerEventData eventData)
	{
		if (IsDraggable == true)
		{
			Debug.Log("Drag begin");
			itemCanvasGroup.alpha = 0.6f;
			itemCanvasGroup.blocksRaycasts = false;
			transform.SetParent(dragParent);
		}
	}
	public void OnDrag(PointerEventData eventData)
	{
		Debug.Log("On drag");

		// Getting movement delta (the amount of units that the finger moved since the previous frame)
		itemImageTransform.anchoredPosition += eventData.delta / mainCanvas.scaleFactor;
	}
	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.Log("Drag end");

		itemCanvasGroup.alpha = 1f;
		itemCanvasGroup.blocksRaycasts = true;
		itemImageTransform.position = startPosition;
		transform.SetParent(parent);
		transform.SetSiblingIndex(0);
	}
}
