using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class MenuButtonAudio : MonoBehaviour,
    ISelectHandler, IPointerEnterHandler
{
    [Header("Menu Sounds")]
    public AudioClip navigationSound;
    public AudioClip selectionSound;

    [Range(0f, 1f)] public float navigationVolume = 0.3f;
    [Range(0f, 1f)] public float selectionVolume = 0.55f;

    private Button button;
    private static AudioSource menuAudio;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlaySelection);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(PlaySelection);
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Mouse hover already handles mouse navigation.
        if (eventData is PointerEventData)
            return;

        PlayNavigation();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayNavigation();
    }

    private void PlayNavigation()
    {
        if (button != null && button.IsInteractable())
            PlaySound(navigationSound, navigationVolume);
    }

    private void PlaySelection()
    {
        PlaySound(selectionSound, selectionVolume);
    }

    private static void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null)
            return;

        // Keep menu sounds alive during scene changes.
        if (menuAudio == null)
        {
            GameObject audioObject = new GameObject("MenuAudio");
            DontDestroyOnLoad(audioObject);
            menuAudio = audioObject.AddComponent<AudioSource>();
            menuAudio.playOnAwake = false;
            menuAudio.loop = false;
            menuAudio.spatialBlend = 0f;
            menuAudio.volume = 1f;
            menuAudio.ignoreListenerPause = true;
        }

        menuAudio.PlayOneShot(clip, volume);
    }
}
