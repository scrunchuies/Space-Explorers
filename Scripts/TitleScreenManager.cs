using DigitalRuby.SoundManagerNamespace;
using UnityEngine; // Use Unity's namespace for GameObject, Debug, and Application
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private AudioClip awesome;

    // Starts the game by loading a new scene
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Quits the application
    public void QuitGame()
    {
        UnityEngine.Application.Quit(); // Quit game
        UnityEngine.Debug.Log("Game is quitting!");
    }
}
