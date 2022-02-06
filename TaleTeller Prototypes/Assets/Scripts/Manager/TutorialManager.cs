using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility;

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
    private Vector3 statsOrigin;
    private Vector3 inspireOrigin;
    private Vector3 inkOrigin;

    public List<Sprite> tutorialSprites = new List<Sprite>();

    public List<CardData> tutorialCards = new List<CardData>();
    public List<TutorialConditions> tutorialConditions = new List<TutorialConditions>();
    public CardData tutorialPlotCard;

    IEnumerator IntroductionRoutine()
    {
        //Screen
        #region Screen_01
        TutorialScreen screen_01 = new TutorialScreen(
            LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Slide_T0_00"),
            "WELCOME writer ! ", tutorialSprites[0]);
        bool wait = true;
        screen_01.Open(()=> wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        while (screen_01.open) { yield return new WaitForEndOfFrame(); }
        wait = true;
        screen_01.Close(() => wait = false);
        while(wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        GameManager.Instance.currentHero.lifePoints--;

        //Screen
        #region Screen_02
        TutorialScreen screen_02 = new TutorialScreen(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Slide_T0_01"), 
            "", tutorialSprites[1]);
        wait = true;
        screen_02.Open(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        while (screen_02.open) { yield return new WaitForEndOfFrame(); }
        wait = true;
        screen_02.Close(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Appear Line
        #region Appear_01
        wait = true;
        AppearObject(CardManager.Instance.board.lineTransform, lineOrigin, () => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Hero Message
        #region Hero_01
        EventQueue lineMessageQueue = new EventQueue();
        HeroMessage lineMessage = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T0_00"), lineMessageQueue, true);
        lineMessageQueue.StartQueue();
        while (!lineMessageQueue.resolved) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Appear Deck
        #region Appear_02
        wait = true;
        AppearObject(CardManager.Instance.deckTransform, deckOrigin, () => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Hero Message
        #region Hero_02
        EventQueue deckMessageQueue = new EventQueue();
        HeroMessage deckMessage = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T0_01"), deckMessageQueue, true);
        deckMessageQueue.StartQueue();
        while (!deckMessageQueue.resolved) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Appear Discard
        #region Appear_03
        wait = true;
        AppearObject(CardManager.Instance.discardPileTransform, discardOrigin, () => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Hero Message
        #region Hero_03
        EventQueue discardMessageQueue = new EventQueue();
        HeroMessage discardMessage = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T0_02"), discardMessageQueue, true);
        discardMessageQueue.StartQueue();
        while (!discardMessageQueue.resolved) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Appear Hand
        #region Appear_04
        wait = true;
        AppearObject(CardManager.Instance.cardHand.handTransform, handOrigin, () => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Hero Message
        #region Hero_04
        EventQueue handMessageQueue = new EventQueue();
        HeroMessage handMessage = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T0_03"), handMessageQueue, true);
        handMessageQueue.StartQueue();
        while (!handMessageQueue.resolved) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Appear Board
        #region Appear_05
        wait = true;
        AppearObject(CardManager.Instance.board.boardTransform, boardOrigin, () => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Hero Message
        #region Hero_05
        EventQueue boardMessageQueue = new EventQueue();
        HeroMessage boardMessage = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T0_04"), boardMessageQueue, true);
        boardMessageQueue.StartQueue();
        while (!boardMessageQueue.resolved) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Appear Go Button
        #region Appear_06
        wait = true;
        AppearObject(CardManager.Instance.board.goButtonTransform, goButtonOrigin, () => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Hero Message
        #region Hero_06
        EventQueue goButtonMessageQueue = new EventQueue();
        HeroMessage goButtonMessage = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T0_05"), goButtonMessageQueue, true);
        goButtonMessageQueue.StartQueue();
        while (!goButtonMessageQueue.resolved) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Hero Message
        #region Hero_07
        EventQueue tooltipMessageQueue = new EventQueue();
        HeroMessage tooltipMessage = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T0_06"), tooltipMessageQueue, true);
        tooltipMessageQueue.StartQueue();
        while (!tooltipMessageQueue.resolved) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Appear Stats
        #region Appear_07
        wait = true;
        AppearObject(GameManager.Instance.currentHero.statsFrame, statsOrigin, () => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Hero Message
        #region Hero_08
        EventQueue statsMessageQueue = new EventQueue();
        HeroMessage statsMessage = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T0_07"), statsMessageQueue, true);
        statsMessageQueue.StartQueue();
        while (!statsMessageQueue.resolved) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Screen
        #region Screen_03
        TutorialScreen screen_03 = new TutorialScreen(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Slide_T0_02"), "", tutorialSprites[2]);
        wait = true;
        screen_03.Open(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        while (screen_03.open) { yield return new WaitForEndOfFrame(); }
        wait = true;
        screen_03.Close(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Init cards deck
        CardManager.Instance.cardDeck.cardDeck.Clear();
        for (int i = 0; i < tutorialCards.Count; i++)
        {
            tutorialCards[i] = tutorialCards[i].InitializeData(tutorialCards[i]);

            CardManager.Instance.cardDeck.cardDeck.Add(tutorialCards[i]);
        }

        //Add plot card 
        tutorialPlotCard = tutorialPlotCard.InitializeData(tutorialPlotCard);
        tutorialCards.Add(tutorialPlotCard);

        //StartTurn 
        StoryManager.Instance.StartTurn();
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
        deckOrigin = manager.deckTransform.localPosition;
        discardOrigin = manager.discardPileTransform.localPosition;
        handOrigin = manager.cardHand.handTransform.localPosition;
        goButtonOrigin = manager.board.goButtonTransform.localPosition;
        statsOrigin = GameManager.Instance.currentHero.statsFrame.localPosition;
        inspireOrigin = manager.inspire.frame.rectTransform.localPosition;
        inkOrigin = manager.manaSystem.manaFrame.rectTransform.localPosition;

        manager.board.boardTransform.localPosition = new Vector3(boardOrigin.x, -scalerReference.referenceResolution.y ,boardOrigin.z);
        manager.board.lineTransform.localPosition = new Vector3(lineOrigin.x, -scalerReference.referenceResolution.y, lineOrigin.z);
        manager.deckTransform.localPosition = new Vector3(deckOrigin.x, -scalerReference.referenceResolution.y, deckOrigin.z);
        manager.discardPileTransform.localPosition = new Vector3(discardOrigin.x, -scalerReference.referenceResolution.y, discardOrigin.z);
        manager.cardHand.handTransform.localPosition = new Vector3(handOrigin.x, -scalerReference.referenceResolution.y, handOrigin.z);
        manager.board.goButtonTransform.localPosition = new Vector3(goButtonOrigin.x, -scalerReference.referenceResolution.y, goButtonOrigin.z);
        GameManager.Instance.currentHero.statsFrame.localPosition = new Vector3(statsOrigin.x, +scalerReference.referenceResolution.y, statsOrigin.z);
        manager.inspire.frame.rectTransform.localPosition = new Vector3(inspireOrigin.x, -scalerReference.referenceResolution.y, inspireOrigin.z);
        manager.manaSystem.manaFrame.rectTransform.localPosition = new Vector3(inkOrigin.x, -scalerReference.referenceResolution.y, inkOrigin.z);

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
                #region Turn_01
                EventQueue messageQueue = new EventQueue();
                HeroMessage ideaMessage = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T1_00"), messageQueue, true);
                HeroMessage cardMessage = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T1_01"), messageQueue, true);
                messageQueue.StartQueue();
                while (!messageQueue.resolved) { yield return new WaitForEndOfFrame(); }

                TutorialScreen screen_01 = new TutorialScreen(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Slide_T1_00"), "", tutorialSprites[3]);
                bool wait = true;
                screen_01.Open(() => wait = false);
                while (wait) { yield return new WaitForEndOfFrame(); }
                while (screen_01.open) { yield return new WaitForEndOfFrame(); }
                wait = true;
                screen_01.Close(() => wait = false);
                while (wait) { yield return new WaitForEndOfFrame(); }

                messageQueue = new EventQueue();
                HeroMessage inkMessage1 = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T1_02"), messageQueue, true);
                messageQueue.StartQueue();
                wait = true;
                AppearObject(CardManager.Instance.manaSystem.manaFrame.rectTransform, inkOrigin, () => wait = false);
                while (!messageQueue.resolved) { yield return new WaitForEndOfFrame(); }

                messageQueue = new EventQueue();
                HeroMessage inkMessage2 = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T1_03"), messageQueue, true);
                messageQueue.StartQueue();
                while (!messageQueue.resolved) { yield return new WaitForEndOfFrame(); }

                messageQueue = new EventQueue();
                HeroMessage placeMessage = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T1_04"), messageQueue, true);
                messageQueue.StartQueue();
                while (!messageQueue.resolved) { yield return new WaitForEndOfFrame(); }

                #endregion
                break;

            case 1:
                #region Turn_02
                EventQueue messageQueue2 = new EventQueue();
                HeroMessage character1Message2 = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T2_00"), messageQueue2, true);
                messageQueue2.StartQueue();
                while (!messageQueue2.resolved) { yield return new WaitForEndOfFrame(); }

                TutorialScreen screen_02 = new TutorialScreen(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Slide_T2_00"), "", tutorialSprites[4]);
                bool wait2 = true;
                screen_02.Open(() => wait2 = false);
                while (wait2) { yield return new WaitForEndOfFrame(); }
                while (screen_02.open) { yield return new WaitForEndOfFrame(); }
                wait2 = true;
                screen_02.Close(() => wait2 = false);
                while (wait2) { yield return new WaitForEndOfFrame(); }

                messageQueue2 = new EventQueue();
                HeroMessage character2Message2 = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T2_01"), messageQueue2, true);
                messageQueue2.StartQueue();
                while (!messageQueue2.resolved) { yield return new WaitForEndOfFrame(); }
                #endregion
                break;

            case 2:
                #region Turn_03
                TutorialScreen screen_03 = new TutorialScreen(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Slide_T3_00"), "", tutorialSprites[7]);
                bool wait3 = true;
                screen_03.Open(() => wait2 = false);
                while (wait3) { yield return new WaitForEndOfFrame(); }
                while (screen_03.open) { yield return new WaitForEndOfFrame(); }
                wait3 = true;
                screen_03.Close(() => wait2 = false);
                while (wait3) { yield return new WaitForEndOfFrame(); }

                EventQueue messageQueue3 = new EventQueue();
                HeroMessage character2Message3 = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T3_00"), messageQueue3, true);
                messageQueue3.StartQueue();
                while (!messageQueue3.resolved) { yield return new WaitForEndOfFrame(); }
                #endregion
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
                valid = tutorialConditions[TurnCount].isValid(tutorialCards);
                //Make different routines
                if (valid) CardManager.Instance.board.InitBoard();
                else StartCoroutine(ValidationMessageRoutine(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T1_05"), valid));
                return valid;

            case 1:
                valid = tutorialConditions[TurnCount].isValid(tutorialCards);
                //Make different routines
                if (valid) CardManager.Instance.board.InitBoard();
                else StartCoroutine(ValidationMessageRoutine(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, "$Dialogue_T2_01"), valid));
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

        //Save
        SaveManager.Save(new SaveFile(CoreManager.Instance));
        yield return new WaitForSeconds(0.5f);//TEMP : Wait for save writing



        //return to menu test
        GameOverScreen gameOverScreen = new GameOverScreen("Test", "this is the end of the tutorial", tutorialPlotCard);
        bool wait = true;
        gameOverScreen.Open(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }

        while (gameOverScreen.open) { yield return new WaitForEndOfFrame(); }
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
