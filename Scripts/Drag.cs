using UnityEngine;
using UnityEngine.EventSystems;

namespace EpicMMOSystem.MonoScripts;

public class DragWindowCntrl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler //IScrollHandler
{
    private float size;

    /// <summary>
    /// Current scale which is used to keep track whether it is within boundaries
    /// </summary>
    private float _scale = 1f;

    /// <summary>
    /// The minimum scale of the RectTransform Target
    /// </summary>
    [SerializeField] private float minimumScale = 0.5f;

    /// <summary>
    /// The maximum scale of the RectTransform Target
    /// </summary>
    [SerializeField] private float maximumScale = 3f;

    /// <summary>
    /// The scale value it should increase / decrease based on mouse wheel event
    /// </summary>
    [SerializeField] private float scaleStep = 0.10f;

    public static void ApplyDragWindowCntrl(GameObject go)
    {
        go.AddComponent<DragWindowCntrl>();
    }

    private RectTransform _window;

    //delta drag
    private Vector2 _delta;

    private void Awake()
    {
        _window = (RectTransform)transform;
    }

    private void Start()
    {
        size = _window.sizeDelta.x;
        _scale = _window.localScale.x;
        //EpicMMOSystem.MLLogger.LogInfo("start");
        RestoreWindow(_window.gameObject);
    }
    internal static void RestoreWindow(GameObject go, bool checkfor0 = true)
    {

        var rectTransform = go.GetComponent<RectTransform>();
        // EpicMMOSystem.MLLogger.LogInfo("Restore Window " + go.name + " " + rectTransform.anchoredPosition);

        if (checkfor0) { 
            switch (go.name)
            {
                case "NavigatePanel":
                    if (EpicMMOSystem.LevelNavPosition.Value != new Vector2(0, 0))
                        rectTransform.anchoredPosition = EpicMMOSystem.LevelNavPosition.Value;
                    break;
                case "PointPanel":
                    if (EpicMMOSystem.LevelPointPosition.Value != new Vector2(0, 0))
                        rectTransform.anchoredPosition = EpicMMOSystem.LevelPointPosition.Value;
                    break;
            }
        }else
        {
            rectTransform.anchoredPosition = go.name switch
            {
                "NavigatePanel" => EpicMMOSystem.LevelNavPosition.Value,
                "PointPanel" => EpicMMOSystem.LevelPointPosition.Value,
                _ => rectTransform.anchoredPosition,
            };
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _delta = Input.mousePosition - _window.position;
    }


    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPos = (Vector2)Input.mousePosition - _delta;
        Vector3 lossyScale = transform.root.lossyScale;
        Rect rect = _window.rect;
        Vector2 currentTransform = new(rect.width * lossyScale.x,
            rect.height * lossyScale.y);
        Vector2 currOffsetMin, currOffsetMax;
        Vector2 pivot = _window.pivot;
        currOffsetMin.x = newPos.x - pivot.x * currentTransform.x;
        currOffsetMin.y = newPos.y - pivot.y * currentTransform.y;
        currOffsetMax.x = newPos.x + (1 - pivot.x) * currentTransform.x;
        currOffsetMax.y = newPos.y + (1 - pivot.y) * currentTransform.y;
        if (currOffsetMin.x < 0)
            newPos.x = _window.pivot.x * currentTransform.x;
        else if (currOffsetMax.x > Screen.width) newPos.x = Screen.width - (1 - _window.pivot.x) * currentTransform.x;
        if (currOffsetMin.y < 0)
            newPos.y = _window.pivot.y * currentTransform.y;
        else if (currOffsetMax.y > Screen.height) newPos.y = Screen.height - (1 - _window.pivot.y) * currentTransform.y;
        _window.position = newPos;
    }

    
    public void OnEndDrag(PointerEventData eventData)
    {
   
        var go = _window.gameObject;
        var rectTransform = go.GetComponent<RectTransform>();
        //EpicMMOSystem.MLLogger.LogInfo("Vector3 " + go.name + " Changed to: " + rectTransform.anchoredPosition);
        switch (go.name)
        {
            case "NavigatePanel":
                EpicMMOSystem.LevelNavPosition.Value = rectTransform.anchoredPosition;
                break;
            case "PointPanel":
                EpicMMOSystem.LevelPointPosition.Value = rectTransform.anchoredPosition;
                break;


        }   

    }
       
        //EpicMMOSystem.SaveWindowPositions(_window.gameObject, false);
/*
    public void OnScroll(PointerEventData eventData)
    {
        if (MinimalUIPlugin.KeyboardShortcut.Value.IsPressed())
        {
            // Set the new scale
            float scrollDeltaY = (eventData.scrollDelta.y * scaleStep);
            float newScaleValue = _scale + scrollDeltaY;
            ApplyScale(newScaleValue, eventData.position);
            Player.m_localPlayer.Message(MessageHud.MessageType.Center,
                $"{_window.gameObject.name} Scroll apply Scale: " + _scale);
        }
    }
    */
/*
    private void ApplyScale(float newScaleValue, Vector2 position)
    {
        float newScale = Mathf.Clamp(newScaleValue, minimumScale, maximumScale);
        // If the scale did not change, don't do anything
        if (newScale.Equals(_scale)) return;

        // Calculate the local point in the rectangle using the event position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_window, position, GameCamera.instance.m_camera,
            out Vector2 localPointInRect);
        // Set the pivot based on the local point in the rectangle
        //SetPivot(_window, localPointInRect);

        // Set the new scale
        _scale = newScale;
        // Apply the new scale
       // [8:22 PM]
        _window.localScale = new Vector3(_scale, _scale, _scale);
    }


    /// <summary>
    /// Since this is something I need to remember...
    /// Sets the pivot based on the local point of the rectangle <see cref="RectTransformUtility.ScreenPointToLocalPointInRectangle"/>.
    /// Keeps the RectTransform in place when changing the pivot by countering the position change when the pivot is set.
    /// </summary>
    /// <param name="rectTransform">The target RectTransform</param>
    /// <param name="localPoint">The local point of the target RectTransform</param>
    private void SetPivot(RectTransform rectTransform, Vector2 localPoint)
    {
        // Calculate the pivot by normalizing the values
        Rect targetRect = rectTransform.rect;
        float pivotX = (float)((localPoint.x - (double)targetRect.x) / (targetRect.xMax - (double)targetRect.x));
        float pivotY = (float)((localPoint.y - (double)targetRect.y) / (targetRect.yMax - (double)targetRect.y));
        Vector2 newPivot = new Vector2(pivotX, pivotY);

        // Delta pivot = (current - new) * scale
        Vector2 deltaPivot = (rectTransform.pivot - newPivot) * _scale;
        // The delta position to add after pivot change is the inversion of the delta pivot change * size of the rect * current scale of the rect
        Vector2 rectSize = targetRect.size;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * rectSize.x, deltaPivot.y * rectSize.y) * -1f;

        // Set the pivot
        rectTransform.pivot = newPivot;
        rectTransform.localPosition += deltaPosition;
    }*/

}