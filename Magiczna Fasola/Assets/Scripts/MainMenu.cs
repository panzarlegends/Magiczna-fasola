using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject controlsScreen;
    public void playGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void controls()
    {
        controlsScreen.SetActive(true);
    }
    public void closeGame()
    {
        Application.Quit();
    }

    public void closeControls()
    {
        controlsScreen.SetActive(false);
    }
}
