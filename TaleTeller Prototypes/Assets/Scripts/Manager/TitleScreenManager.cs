using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    public void GoToTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen", LoadSceneMode.Single);
    }

    public void Setting()
    {
        SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
