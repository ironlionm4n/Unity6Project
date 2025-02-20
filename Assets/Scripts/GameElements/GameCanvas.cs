using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    public void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
