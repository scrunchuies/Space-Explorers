using DigitalRuby.SoundManagerNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    private void Start()
    {
        // Unlock the cursor when the end screen is loaded
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Quits the application
    public void QuitGame()
    {
        UnityEngine.Application.Quit();
        UnityEngine.Debug.Log("Game is quitting!");
    }
}
