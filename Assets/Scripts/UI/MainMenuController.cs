using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject controlsPanel;

    public GameObject playButton;
    public GameObject controlsButton;
    public GameObject backButton;

    void Start()
    {
        mainMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);

        SelectButton(playButton);
    }

    void Update()
    {
        bool backPressed = false;
        bool escapePressed = false;

        // Xbox B / PlayStation Circle
        if (Gamepad.current != null &&
            Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            backPressed = true;
        }

        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            escapePressed = true;
        }

        // Controls page -> return to menu
        if (controlsPanel.activeSelf)
        {
            if (backPressed || escapePressed)
            {
                CloseControls();
            }

            return;
        }

        // Main menu -> Escape quits
        if (escapePressed)
        {
            QuitGame();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Prototype_Level");
    }

    public void OpenControls()
    {
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(true);

        SelectButton(backButton);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        SelectButton(controlsButton);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SelectButton(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}