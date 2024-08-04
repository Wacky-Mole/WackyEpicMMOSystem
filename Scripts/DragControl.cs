using EpicMMOSystem.MonoScripts;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EpicMMOSystem
{
public class DragControl : MonoBehaviour //IBeginDragHandler//, IDragHandler, IEndDragHandler
    {
    private RectTransform window;
    //delta drag
    private Vector2 delta;
    public static Coroutine coroutine = null!;
       

    public void Awake()
    {
        var window2 = (RectTransform)transform;
            //window = EpicMMOSystem.RestorePosition(window2); 
            //EpicMMOSystem.MLLogger.LogInfo("Vector3 Awake");
            window = window2;
            coroutine = StartCoroutine("Positionpanels");
        }

    public void StartDelayPosition()
     {
        coroutine = StartCoroutine("Positionpanels");
     }

    // Coroutine
    IEnumerator Positionpanels()
    {
        yield return new WaitForSeconds(0.001f); // For some reason it doesn't work when directly called in Start(), have to add a small delay
        SaveWindowPositions(gameObject, true);
        StopCoroutine(coroutine);
    }
    public static void ApplyDragWindowCntrl(GameObject go)
    {
        go.AddComponent<DragControl>();
    }

    internal static void RestoreWindow(GameObject go, bool checkfor0 = true)
    {
        
        var rectTransform = go.GetComponent<RectTransform>();
            // EpicMMOSystem.MLLogger.LogInfo("Restore Window " + go.name + " " + rectTransform.anchoredPosition);

            if (checkfor0)
            {
                switch (go.name)
                {
                    case "EpicHudPanel":
                        rectTransform.anchoredPosition = EpicMMOSystem.HudPanelPosition.Value;
                        break;
                    case "Exp":
                        if (EpicMMOSystem.ExpPanelPosition.Value != new Vector2(0, 0))
                            rectTransform.anchoredPosition = EpicMMOSystem.ExpPanelPosition.Value;
                        break;
                    case "Hp":
                        if (EpicMMOSystem.HpPanelPosition.Value != new Vector2(0, 0))
                            rectTransform.anchoredPosition = EpicMMOSystem.HpPanelPosition.Value;
                        break;
                    case "Stamina":
                        if (EpicMMOSystem.StaminaPanelPosition.Value != new Vector2(0, 0))
                            rectTransform.anchoredPosition = EpicMMOSystem.StaminaPanelPosition.Value;
                        break;
                    case "Eitr":
                        if (EpicMMOSystem.EitrPanelPosition.Value != new Vector2(0, 0))
                            rectTransform.anchoredPosition = EpicMMOSystem.EitrPanelPosition.Value;
                        break;
                }
            }else
            {
        
                rectTransform.anchoredPosition = go.name switch
                {
                    "EpicHudPanel" => EpicMMOSystem.HudPanelPosition.Value,
                    "Exp" => EpicMMOSystem.ExpPanelPosition.Value,
                    "Hp" => EpicMMOSystem.HpPanelPosition.Value,
                    "Stamina" => EpicMMOSystem.StaminaPanelPosition.Value,
                    "Eitr" => EpicMMOSystem.EitrPanelPosition.Value,
                    _ => rectTransform.anchoredPosition,
                };
            }
        }
        internal static void MoveEitr(GameObject go, int Extra)
        {
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(Extra, Extra);
        }
        internal static void RestoreEitr(GameObject go)
        {
            go.GetComponent<RectTransform>().anchoredPosition = EpicMMOSystem.EitrPanelPosition.Value;
        }
    internal static void SaveWindowPositions(GameObject go, bool initialLoad)
        {
        var rectTransform = go.GetComponent<RectTransform>();
            //EpicMMOSystem.MLLogger.LogInfo("Vector2 " + go.name + " " + rectTransform.anchoredPosition);
           // EpicMMOSystem.MLLogger.LogInfo("Vector3 " + go.name + " Changed to: " + rectTransform.anchoredPosition);
            if (initialLoad)
        {
            /*
            rectTransform.anchoredPosition = go.name switch
            {
                "EpicHudPanel" => EpicMMOSystem.HudPanelPosition.Value,
                "Exp" => EpicMMOSystem.ExpPanelPosition.Value,
                "Hp" => EpicMMOSystem.HpPanelPosition.Value,
                "Stamina" => EpicMMOSystem.StaminaPanelPosition.Value,
                "Eitr" => EpicMMOSystem.EitrPanelPosition.Value,
                _ => rectTransform.anchoredPosition
            };
            */

            switch (go.name)
            {
                case "EpicHudPanel":
                    rectTransform.anchoredPosition = EpicMMOSystem.HudPanelPosition.Value;
                    break;
                case "Exp":
                    if(EpicMMOSystem.ExpPanelPosition.Value != new Vector2(0, 0))
                        rectTransform.anchoredPosition = EpicMMOSystem.ExpPanelPosition.Value;
                    break;
                case "Hp":
                    if (EpicMMOSystem.HpPanelPosition.Value != new Vector2(0, 0))
                        rectTransform.anchoredPosition = EpicMMOSystem.HpPanelPosition.Value;
                    break;
                case "Stamina":
                    if (EpicMMOSystem.StaminaPanelPosition.Value != new Vector2(0, 0))
                        rectTransform.anchoredPosition = EpicMMOSystem.StaminaPanelPosition.Value;
                    break;
                case "Eitr":
                    if (EpicMMOSystem.EitrPanelPosition.Value != new Vector2(0, 0))
                        rectTransform.anchoredPosition = EpicMMOSystem.EitrPanelPosition.Value;
                    break;

                    default:break;
            }
          // EpicMMOSystem.MLLogger.LogInfo("Vector3 " + go.name + " Changed to: " + rectTransform.anchoredPosition);
        }
        else
        {
            switch (go.name)
            {
                case "EpicHudPanel": EpicMMOSystem.HudPanelPosition.Value = rectTransform.anchoredPosition;
                    break;
                case "Exp":
                    EpicMMOSystem.ExpPanelPosition.Value = rectTransform.anchoredPosition;
                    break;
                case "Hp":
                    EpicMMOSystem.HpPanelPosition.Value = rectTransform.anchoredPosition;
                    break;
                case "Stamina":
                    EpicMMOSystem.StaminaPanelPosition.Value = rectTransform.anchoredPosition;
                    break;
                case "Eitr":
                    EpicMMOSystem.EitrPanelPosition.Value = rectTransform.anchoredPosition;
                    break;
                        default :break;
            }
        }
        EpicMMOSystem.Instance.Config.Save();
    }

    public void BeginDrag()
    {
        delta = Input.mousePosition - window.position;
    }
    public void Drag()
    {
        Vector2 newPos = (Vector2)Input.mousePosition - delta;
        Vector2 Transform = new Vector2(window.rect.width * transform.root.lossyScale.x, window.rect.height * transform.root.lossyScale.y);
        Vector2 OffsetMin, OffsetMax;
        OffsetMin.x = newPos.x - window.pivot.x * Transform.x;
        OffsetMin.y = newPos.y - window.pivot.y * Transform.y;
        OffsetMax.x = newPos.x + (1 - window.pivot.x) * Transform.x;
        OffsetMax.y = newPos.y + (1 - window.pivot.y) * Transform.y;
        if (OffsetMin.x < 0)
        {
            newPos.x = window.pivot.x * Transform.x;
        }
        else if (OffsetMax.x > Screen.width)
        {
            newPos.x = Screen.width - (1 - window.pivot.x) * Transform.x;
        }
        if (OffsetMin.y < 0)
        {
            newPos.y = window.pivot.y * Transform.y;
        }
        else if (OffsetMax.y > Screen.height)
        {
            newPos.y = Screen.height - (1 - window.pivot.y) * Transform.y;
        }
        window.position = newPos;
    }

      public void OnEndDrag()
        {
            
           SaveWindowPositions(window.gameObject, false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //throw new System.NotImplementedException();
        }
    }
}
