using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;
using System.Collections.Generic;

public class PauseMenuHandler : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    public PostProcessVolume postProcessingVolume;
    public RigidbodyWalker playerController;

    private bool isPaused = false;
    private bool[] initialBodyStates;
    private Rigidbody[] allRigidbodies;

    // Store original audio source properties
    private class AudioSourceState
    {
        public AudioSource Source;
        public float OriginalVolume;
        public bool WasPlaying;
    }
    private List<AudioSourceState> audioSourceStates = new List<AudioSourceState>();

    void Start()
    {
        allRigidbodies = FindObjectsOfType<Rigidbody>();
        initialBodyStates = new bool[allRigidbodies.Length];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        // Find and prepare audio sources for fading
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        audioSourceStates.Clear();

        foreach (AudioSource source in audioSources)
        {
            if (source.isPlaying)
            {
                AudioSourceState state = new AudioSourceState
                {
                    Source = source,
                    OriginalVolume = source.volume,
                    WasPlaying = true
                };
                audioSourceStates.Add(state);
                StartCoroutine(FadeAudioSource(state, 0.5f)); // 0.5 second fade out
            }
        }

        // Pause all rigidbodies
        for (int i = 0; i < allRigidbodies.Length; i++)
        {
            if (allRigidbodies[i] != null)
            {
                initialBodyStates[i] = allRigidbodies[i].isKinematic;
                allRigidbodies[i].isKinematic = true;
            }
        }

        // Disable player controller
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        pauseMenuCanvas.SetActive(true);
        postProcessingVolume.weight = 1f;

        // Fully unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause time
        Time.timeScale = 0f;

        isPaused = true;
    }

    public void ResumeGame()
    {
        // Stop any ongoing fade coroutines
        StopAllCoroutines();

        // Restore audio sources
        foreach (AudioSourceState state in audioSourceStates)
        {
            if (state.Source != null)
            {
                // Restore original volume
                state.Source.volume = state.OriginalVolume;

                // Unpause if it was originally playing
                if (state.WasPlaying)
                {
                    state.Source.UnPause();
                }
            }
        }
        audioSourceStates.Clear();

        // Restore rigidbody states
        for (int i = 0; i < allRigidbodies.Length; i++)
        {
            if (allRigidbodies[i] != null)
            {
                allRigidbodies[i].isKinematic = initialBodyStates[i];
            }
        }

        // Re-enable player controller
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        pauseMenuCanvas.SetActive(false);
        postProcessingVolume.weight = 0f;

        // Lock cursor and hide it immediately
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Resume time
        Time.timeScale = 1f;

        isPaused = false;
    }

    // Coroutine to fade out audio source
    private IEnumerator FadeAudioSource(AudioSourceState audioSourceState, float fadeDuration)
    {
        AudioSource audioSource = audioSourceState.Source;
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Pause();
    }

    // Quit (saving to be implemented in the future)
    public void SaveAndQuit()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("TitleScreen");
    }
}