using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject controlsPanel;
    public GameObject creditsPanel;

    public GameObject playButton;
    public GameObject controlsButton;
    public GameObject creditsButton;

    public GameObject controlsBackButton;
    public GameObject creditsBackButton;

    void Start()
    {
        mainMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);

        SelectButton(playButton);
    }

    void Update()
    {
        bool backPressed = false;
        bool escapePressed = false;

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

        if (controlsPanel.activeSelf)
        {
            if (backPressed || escapePressed)
                CloseControls();

            return;
        }

        if (creditsPanel.activeSelf)
        {
            if (backPressed || escapePressed)
                CloseCredits();

            return;
        }

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
        creditsPanel.SetActive(false);
        controlsPanel.SetActive(true);

        SelectButton(controlsBackButton);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        SelectButton(controlsButton);
    }

    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(true);

        SelectButton(creditsBackButton);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        SelectButton(creditsButton);
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
        if (EventSystem.current == null || button == null)
            return;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }
}