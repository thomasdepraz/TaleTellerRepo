using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState {GAME, TUTORIAL}

public enum GameOverType { PLAYER_DEATH, PLOT_DEATH, PLOT_TIMER};
public class GameManager : Singleton<GameManager>
{
    [Header("References")]
    public Hero currentHero;
    public InstructionsData instructionsData;
    public TutorialManager tutorialManager;
    public GameObject goButton;
    public Image fadePanel;
    public Pointer pointer;
    public Button returnToMenuButton;

    [HideInInspector]public bool pause;
    public GameState currentState = GameState.TUTORIAL;


    public void Awake()
    {
        CreateSingleton(false);
        instructionsData = Instantiate(instructionsData);
    }

    public void Start()
    {
        #region Load Save
        if (CoreManager.Instance.playTutorial || !CoreManager.Instance.completeTutorial)
        {
            CoreManager.Instance.playTutorial = false;
            currentState = GameState.TUTORIAL;
        }
        else
        {
            currentState = GameState.GAME;
        }
        #endregion
    }

    public void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F))
        {
            if (Time.timeScale > 1)
                Time.timeScale = 1;
            else
                Time.timeScale = 3;
        }
    }

    public void Fade(bool toBlack)
    {
        Color transparent = new Color(0,0,0,0);
        if (toBlack)
        {
            LeanTween.color(gameObject, Color.black, 0.5f).setOnUpdate((Color col) => { fadePanel.color = col; });
        }
        else
        {
            LeanTween.value(gameObject, Color.black, transparent, 1f).setOnUpdate((Color col) => {fadePanel.color = col;});
        }
    }

    #region GameOver
    public void GameOver(EventQueue queue, GameOverType gameOverType)
    {
        queue.events.Add(GameOverCoroutine(queue, gameOverType));
    }

    IEnumerator GameOverCoroutine(EventQueue queue, GameOverType gameOverType)
    {
        //Pause so other routines stop;
        pause = true;

        List<EventQueue> events  = new List<EventQueue>();
        events = StoryManager.Instance.queueList;

        //Destroy all event queue and stop all of there coroutines
        while(events.Count > 1)
        {
            for (int i = 0; i < events[0].events.Count; i++)
            {
                StopCoroutine(events[0].events[i]);
            }

            events.RemoveAt(0);
        }
        string instruction = string.Empty;
        string content = string.Empty;

        switch (gameOverType)
        {
            case GameOverType.PLAYER_DEATH:
                instruction = "The End (not the good one)";
                content = "The Hero died... and no, he is not getting revived.";
                break;
            case GameOverType.PLOT_DEATH:
                instruction = "The End (not the good one)";
                content = "The key character of your plot died, how are you going to end it now...";
                break;
            case GameOverType.PLOT_TIMER:
                instruction = "The End (not the good one)";
                content = "You didn't achieve the objective in time, and nobody likes reading a boring book...";
                break;
            default:
                break;
        }

        //return to menu test
        GameOverScreen gameOverScreen = new GameOverScreen(instruction, content, CardManager.Instance.activatedCard);
        bool wait = true;
        gameOverScreen.Open(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }

        while (gameOverScreen.open) { yield return new WaitForEndOfFrame(); }
    }
    #endregion

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }   
}
