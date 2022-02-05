using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]

public enum TutorialState { RUNNING, PENDING}
public enum ConditionsType { PLACED, ADJACENT}

[Serializable]
public struct TutorialConditions
{
    public List<int> indexToCompare;
    public ConditionsType conditions;

    public bool isValid(List<CardData> tutorialCards)
    {
        switch (conditions)
        {
            case ConditionsType.PLACED:
                for (int i = 0; i < indexToCompare.Count; i++)
                {
                    if (!CardManager.Instance.board.IsCardOnBoard(tutorialCards[indexToCompare[i]]))
                        return false;
                }
                return true;
            case ConditionsType.ADJACENT:
                List<CardData> cardsToCompare = new List<CardData>();
                for (int i = 0; i < indexToCompare.Count; i++)
                {
                    cardsToCompare.Add(tutorialCards[indexToCompare[i]]);
                }

                for (int i = 0; i < cardsToCompare.Count; i++)
                {
                    if (!CardManager.Instance.board.IsCardOnBoard(cardsToCompare[i]))
                        return false;
                }

                int count = 0;
                for (int slot = 0; slot < CardManager.Instance.board.slots.Count; slot++)
                {
                    CardData data = null;

                    if (CardManager.Instance.board.slots[slot].currentPlacedCard != null)
                        data = CardManager.Instance.board.slots[slot].currentPlacedCard.data;

                    if(data != null && cardsToCompare.Contains(data))
                    {
                        count++;
                    }
                    else
                    {
                        count = 0;
                    }
                    if (count == cardsToCompare.Count) return true;
                }

                return false;

            default:
                return false;
        }
    }
}
public class TutorialManager : MonoBehaviour
{
    public int TurnCount { get; set; } = 0;
    [HideInInspector] public TutorialState currentState = TutorialState.RUNNING;

    //Object Origins
    [SerializeField] private CanvasScaler scalerReference;
    private Vector3 boardOrigin;
    private Vector3 lineOrigin;
    private Vector3 deckOrigin;
    private Vector3 discardOrigin;
    private Vector3 goButtonOrigin;
    private Vector3 handOrigin;

    List<CardData> tutorialCards = new List<CardData>();
    public List<TutorialConditions> tutorialConditions = new List<TutorialConditions>();
    public PlotCard tutorialPlotCard;

    IEnumerator IntroductionRoutine()
    {
        //Screen
        #region Screen_01
        TutorialScreen screen_01 = new TutorialScreen("Bienvenue", "TUTO_01", null);
        bool wait = true;
        screen_01.Open(()=> wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        while (screen_01.open) { yield return new WaitForEndOfFrame(); }
        wait = true;
        screen_01.Close(() => wait = false);
        while(wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Screen
        #region Screen_02
        TutorialScreen screen_02 = new TutorialScreen("Bienvenue bis", "TUTO_02", null);
        wait = true;
        screen_02.Open(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        while (screen_02.open) { yield return new WaitForEndOfFrame(); }
        wait = true;
        screen_02.Close(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Hero and board
        wait = true;
        AppearObject(CardManager.Instance.board.lineTransform, lineOrigin, () => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }

        EventQueue lineMessageQueue = new EventQueue();
        HeroMessage lineMessage = new HeroMessage("Salut le sang", lineMessageQueue, true);
        lineMessageQueue.StartQueue();
        while (!lineMessageQueue.resolved) { yield return new WaitForEndOfFrame(); }


        //GetTutorialCards
        for (int i = 0; i < CardManager.Instance.cardDeck.cardDeck.Count; i++)
        {
            tutorialCards.Add(CardManager.Instance.cardDeck.cardDeck[i]);
        }

        //Add plot card 
        CardData plot = tutorialPlotCard.InitializeData(tutorialPlotCard);
        tutorialCards.Add(plot);

        //return to menu test
        GameOverScreen gameOverScreen = new GameOverScreen("Test", "this is the end of the tutorial", plot);
        wait = true;
        gameOverScreen.Open(() => wait = false);
        while(wait) { yield return new WaitForEndOfFrame(); }

        while(gameOverScreen.open) { yield return new WaitForEndOfFrame(); }


        //StartTurn 
        //StoryManager.Instance.StartTurn();
    }

    public void AppearObject(Transform transform, Vector3 target, Action onComplete)
    {
        LeanTween.moveLocal(transform.gameObject, target, 1f).setEaseInOutQuint().setOnComplete(onComplete);
    }

    public void InitializeGameScreen()
    {
        CardManager manager = CardManager.Instance;

        //Hide Board
        boardOrigin = manager.board.boardTransform.localPosition;
        lineOrigin = manager.board.lineTransform.localPosition;
        goButtonOrigin = manager.board.goButtonTransform.localPosition;

        //manager.board.boardTransform.localPosition = new Vector3(boardOrigin.x, -scalerReference.referenceResolution.y ,boardOrigin.z);
        manager.board.lineTransform.localPosition = new Vector3(lineOrigin.x, -scalerReference.referenceResolution.y, lineOrigin.z);




        StartCoroutine(IntroductionRoutine());
    }

    public int GetCardCount()
    {
        switch (TurnCount)
        {
            case 0: return 2; 

            default:
                return 1;
        }
    }

    public void StartTurnAction(EventQueue queue)
    {
        queue.events.Add(StartTurnActionRoutine(queue));
    }

    private IEnumerator StartTurnActionRoutine(EventQueue queue)
    {
        switch (TurnCount)
        {
            case 0:
                EventQueue messageQueue = new EventQueue();
                HeroMessage ideaMessage = new HeroMessage("hello", messageQueue, true);
                HeroMessage cardMessage = new HeroMessage("hello bis", messageQueue, true);
                HeroMessage inkPotMessage = new HeroMessage("hello tris", messageQueue, true);
                HeroMessage manaMessage = new HeroMessage("hello chris :)", messageQueue, true);
                HeroMessage tryCardsMessage = new HeroMessage("hello chris :)", messageQueue, true);
                messageQueue.StartQueue();
                while (!messageQueue.resolved) { yield return new WaitForEndOfFrame(); }
                break;

            default:
                break;
        }
        yield return new WaitForEndOfFrame();
        queue.UpdateQueue();
    }

    public bool ValidConditions()
    {
        //NOTE : dont forget to call initboard somewhere if theres no message
        bool valid = false;
        switch (TurnCount)
        {
            case 0:
                valid = tutorialConditions[0].isValid(tutorialCards);
                //Make different routines
                if (valid) StartCoroutine(ValidationMessageRoutine("Well Done", valid));
                else StartCoroutine(ValidationMessageRoutine("Wesh tu fais quoi frr", valid));
                return valid;

            default:
                return false;
        }
    }

    IEnumerator ValidationMessageRoutine(string message, bool init)
    {
        currentState = TutorialState.PENDING;
        EventQueue messageQueue = new EventQueue();

        HeroMessage heroMessage = new HeroMessage(message, messageQueue, true);

        messageQueue.StartQueue();
        while (!messageQueue.resolved) { yield return new WaitForEndOfFrame(); }


        yield return new WaitForEndOfFrame();

        currentState = TutorialState.RUNNING;

        if(init) CardManager.Instance.board.InitBoard();
    }

    public void EndTutorial()
    {
        StartCoroutine(EndTutorialRoutine());
    }
    IEnumerator EndTutorialRoutine()
    {

        List<EventQueue> events = new List<EventQueue>();
        events = StoryManager.Instance.queueList;

        //Destroy all event queue and stop all of there coroutines
        while (events.Count > 0)
        {
            for (int i = 0; i < events[0].events.Count; i++)
            {
                StopCoroutine(events[0].events[i]);
            }

            events.RemoveAt(0);
        }


        //Screen
        #region Screen_01
        TutorialScreen screen_01 = new TutorialScreen("Bienvenue", "TUTO_01", null);
        bool wait = true;
        screen_01.Open(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        while (screen_01.open) { yield return new WaitForEndOfFrame(); }
        wait = true;
        screen_01.Close(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //fade to black : todo return to menu screen

        //end and return to menu

        //clear queues
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);//TEMP
    }

    public void AppearPlot(EventQueue queue)
    {
        queue.events.Add(AppearPlotRoutine(queue));
    }
    IEnumerator AppearPlotRoutine(EventQueue queue)
    {
        EventQueue appearQueue = new EventQueue();
        CardManager.Instance.CardAppearToHand(tutorialPlotCard,appearQueue, CardManager.Instance.plotAppearTransform.position);
        appearQueue.StartQueue();
        while(!appearQueue.resolved) { yield return new WaitForEndOfFrame(); }
        queue.UpdateQueue();
    }
}
