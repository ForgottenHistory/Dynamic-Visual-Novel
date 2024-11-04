using UnityEngine;

public class PlayGame : MonoBehaviour
{
    public void PlayGameButton()
    {
        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
