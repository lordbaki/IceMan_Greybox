using UnityEngine;
using TMPro;
using System.Collections;

public class RaceCountdown : MonoBehaviour
{
    public TMP_Text countdownText;
    public PlayerCarController playerController;

    [Header("Animation")]
    public float popScale = 1.5f;
    public float popDuration = 0.2f;

    [Header("Audio (Optional)")]
    public AudioSource audioSource;
    public AudioClip countdownBeep;
    public AudioClip goSound;
    public AudioSource backgroundMusic;
    [Min(0f)] public float musicStartDelay = 1f;

    private bool countingDown;
    private bool controllerWasEnabled;
    private bool musicCancelled;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (countdownText == null || playerController == null)
        {
            Debug.LogWarning("Countdown references missing.");
            enabled = false;
            return;
        }

        if (backgroundMusic != null)
            backgroundMusic.Stop();

        countingDown = true;
        Time.timeScale = 0f;
        controllerWasEnabled = playerController.enabled;
        playerController.enabled = false;
    }

    IEnumerator Start()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            countdownText.color = i == 1
                ? new Color(1f, 0.8f, 0.2f)
                : Color.white;

            PlaySound(countdownBeep);
            yield return StartCoroutine(Pop(1f));
        }

        countdownText.text = "GO!";
        countdownText.color = new Color(0.3f, 1f, 0.5f);

        ResumeRace();
        PlaySound(goSound);

        if (backgroundMusic != null)
            StartCoroutine(StartMusicAfterDelay());

        yield return StartCoroutine(Pop(0.7f));
        countdownText.gameObject.SetActive(false);
    }

    IEnumerator StartMusicAfterDelay()
    {
        float delay = musicStartDelay;

        // Let the GO sound finish before starting the music.
        if (goSound != null && audioSource != null)
        {
            float pitch = Mathf.Max(0.01f, Mathf.Abs(audioSource.pitch));
            delay = Mathf.Max(delay, goSound.length / pitch);
        }

        yield return new WaitForSecondsRealtime(delay);

        // Do not restart music if the player failed during the GO delay.
        IceCondition ice = playerController != null
            ? playerController.GetComponent<IceCondition>()
            : null;

        if (musicCancelled || (ice != null && ice.HasFailed()))
            yield break;

        if (backgroundMusic != null)
            backgroundMusic.Play();
    }

    public void StopBackgroundMusic()
    {
        musicCancelled = true;

        if (backgroundMusic != null)
            backgroundMusic.Stop();
    }
    IEnumerator Pop(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(
                elapsed / Mathf.Max(popDuration, 0.01f)
            );
            float scale = Mathf.Lerp(popScale, 1f, t);
            countdownText.transform.localScale = Vector3.one * scale;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        countdownText.transform.localScale = Vector3.one;
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    void ResumeRace()
    {
        if (!countingDown)
            return;

        countingDown = false;
        Time.timeScale = 1f;

        if (playerController != null)
            playerController.enabled = controllerWasEnabled;
    }

    void OnDisable()
    {
        ResumeRace();
    }
}