using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/* --- Put this script onto the Child UI and optionally Page Panel --- */

public class ECUIEvent : MonoBehaviour, IBeginDragHandler, ICancelHandler, IDeselectHandler, IDragHandler, IDropHandler, IEndDragHandler,
    IInitializePotentialDragHandler, IMoveHandler, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler,
    IPointerUpHandler, IScrollHandler, ISelectHandler, ISubmitHandler, IUpdateSelectedHandler
{
    public bool[] mouseButton = { true, false, false, false, false, false };

    Vector2 position;
    public bool isOverlapping = false;
    public bool isClicked = false;
    public bool isPressing = false;
    public bool isDragging = false;
    public bool isSelected = false;

    public static GameObject overlappedObject;
    public static GameObject clickedObject;
    public static GameObject pressedObject;
    public static GameObject releasedObject;
    public static GameObject draggedObject;
    public static GameObject selectedObject;

    public static GraphicRaycaster raycaster;
    void Start()
    {
        if (raycaster == null) raycaster = GetComponentInParent<GraphicRaycaster>();
    }

    void Update()
    {
        if (isClicked)
        {
            isPressing = true;
            isClicked = false;
            if (clickedObject == gameObject) clickedObject = null;
            pressedObject = gameObject;
        }
        else if (isPressing && isDragging && !isOverlapping)
        {
            isPressing = false;
            if (pressedObject == gameObject) pressedObject = null;
        }
        else if (!isPressing && isDragging && isOverlapping)
        {
            isPressing = true;
            pressedObject = gameObject;
        }
    }
    //EventTrigger

    public void OnBeginDrag(PointerEventData data)
    {
        //Debug.Log("OnBeginDrag - " + data);
    }

    public void OnCancel(BaseEventData data)
    {
        //Debug.Log("OnCancel - " + data);
    }

    public void OnDeselect(BaseEventData data)
    {
        isSelected = false;
        if (selectedObject == gameObject) selectedObject = null;
        //Debug.Log("OnDeselect - " + data);
    }
    public void OnDrag(PointerEventData data)
    {
        if (MousePressed())
        {
            isDragging = true;
            draggedObject = gameObject;
            //Debug.Log("OnDrag - " + data);
        }
    }

    public void OnDrop(PointerEventData data)
    {
        //Debug.Log("OnDrop - " + data);
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (MousePressed(false))
        {
            isDragging = false;
            if (draggedObject == gameObject) draggedObject = null;
            //Debug.Log("OnEndDrag - " + data);
        }
    }

    public void OnInitializePotentialDrag(PointerEventData data)
    {
        //Debug.Log("OnInitializePotentialDrag - " + data);
    }
    public void OnMove(AxisEventData data)
    {
        //Debug.Log("OnMove - " + data);
    }

    public void OnPointerClick(PointerEventData data)
    {
        //Debug.Log("OnPointerClick - " + data);
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (MousePressed())
        {
            isClicked = true;
            clickedObject = gameObject;
            //Debug.Log("OnPointerDown - " + data);
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {
        isOverlapping = true;
        overlappedObject = gameObject;
        //Debug.Log("OnPointerEnter - " + data);
    }

    public void OnPointerExit(PointerEventData data)
    {
        isOverlapping = false;
        if (overlappedObject == gameObject) overlappedObject = null;
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (MousePressed(false))
        {
            isClicked = false;
            isPressing = false;
            if (clickedObject == gameObject)
            {
                clickedObject = null;
                releasedObject = gameObject;
            }
            if (pressedObject == gameObject)
            {
                pressedObject = null;
                releasedObject = gameObject;
            }
            //Selectable ui = gameObject.GetComponent<Selectable>();
            //if (ui != null)
            //{
            //    ui.enabled = false;
            //    ui.enabled = true;
            //}
            //Debug.Log("OnPointerUp - " + data);
        }
    }

    public void OnScroll(PointerEventData data)
    {
        //Debug.Log("OnScroll - " + data);
    }

    public void OnSelect(BaseEventData data)
    {
        isSelected = true;
        selectedObject = gameObject;
        //Debug.Log("OnSelect - " + data);
    }

    public void OnSubmit(BaseEventData data)
    {
        //Debug.Log("OnSubmit - " + data);
    }

    public void OnUpdateSelected(BaseEventData data)
    {
        //Debug.Log("OnUpdateSelected - " + data);
    }

    bool MousePressed(bool isPressed = true)
    {
        for (int i = 0; i < mouseButton.Length; i++)
        {
            if (mouseButton[i] && isPressed == Input.GetKey((KeyCode)(323 + i)))
            {
                return true;
            }
        }
        return false;
    }
}
