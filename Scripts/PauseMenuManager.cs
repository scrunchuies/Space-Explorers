using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioClip awesome;
    public GameObject pauseMenuCanvas; // Reference to the pause menu canvas
    private bool isPaused = false; // Tracks whether the game is paused

    void Update()
    {
        // Check for the pause key (Escape or P)
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            UnityEngine.Debug.Log("Pause key pressed!");

            if (isPaused)
            {
                
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuCanvas.SetActive(true); // Show the pause menu
        UnityEngine.Debug.Log("Pause menu activated!");
        Time.timeScale = 0f; // Freeze the game
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false); // Hide the pause menu
        UnityEngine.Debug.Log("Pause menu deactivated!"); // Debugging log
        Time.timeScale = 1f; // Unfreeze the game
        isPaused = false;
    }

    public void OpenOptions()
    {
        UnityEngine.Debug.Log("Options menu clicked! Implement options here.");
    }

    public void SaveAndQuit()
    {
        SoundController.instance.PlaySound(awesome);

        // Saving to be implemented in the future
    }
}
