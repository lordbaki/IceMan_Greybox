using UnityEngine;
using UnityEngine.EventSystems;

public class HoverSelect : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current == null)
            return;

        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}