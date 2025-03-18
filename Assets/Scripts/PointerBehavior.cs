using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject hover;
    [SerializeField] public string hoverText;
    [SerializeField] Vector2 offSet;
    GameObject instantiated;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverText == "") return;
        hover.GetComponentInChildren<TextMeshProUGUI>().text = hoverText;
        instantiated = Instantiate(hover, GameObject.Find("CONSOLE_CANVAS_PARENT").transform);
        instantiated.transform.position = eventData.position + offSet;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (instantiated != null)
            Destroy(instantiated);
    }
}
