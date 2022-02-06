using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class TitleScreenManager : MonoBehaviour
{
    public Image panel;
    public GameObject tutorialButton;

    [Scene]public int gameScene;

    public void Start()
    {
        InitButtons();
        FadePanel();
    }
    public void InitButtons()
    {
        if(CoreManager.Instance.completeTutorial)
        {
            //appear tutorial button
            tutorialButton.SetActive(true);
        }
        else
        {
            //Hide tutorial button
            tutorialButton.SetActive(false);
        }
    }

    public void FadePanel()
    {
        LeanTween.value(panel.gameObject, panel.color, Color.clear, 1).setOnUpdate((Color col)=> 
        {
            panel.color = col;
        }).setOnComplete(()=> panel.gameObject.SetActive(false)).setDelay(0.5f);
    }

    public void StartGame()
    {
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(gameScene);
        sceneLoad.allowSceneActivation = false;
        FadeAndLoad(sceneLoad);
    }

    public void StartTutorial()
    {
        CoreManager.Instance.playTutorial = true;
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(gameScene);
        sceneLoad.allowSceneActivation = false;
        FadeAndLoad(sceneLoad);
    }

    void FadeAndLoad(AsyncOperation sceneToActivate)
    {
        panel.gameObject.SetActive(true);
        LeanTween.value(panel.gameObject, panel.color, Color.black, 1).setOnUpdate((Color col) =>
        {
            panel.color = col;
        }).setOnComplete(() => sceneToActivate.allowSceneActivation = true);
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
