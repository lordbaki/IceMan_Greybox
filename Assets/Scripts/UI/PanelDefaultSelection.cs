using UnityEngine;
using UnityEngine.EventSystems;

public class PanelDefaultSelection : MonoBehaviour
{
    public GameObject defaultButton;

    private void OnEnable()
    {
        if (defaultButton == null || EventSystem.current == null)
            return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }
}