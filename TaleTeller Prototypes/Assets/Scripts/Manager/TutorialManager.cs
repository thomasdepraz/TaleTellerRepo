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
    public TutorialState currentState = TutorialState.RUNNING;

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

    List<CardData> tutorialCards = new List<CardData>();
    public List<TutorialConditions> tutorialConditions = new List<TutorialConditions>();
    public PlotCard tutorialPlotCard;

    IEnumerator IntroductionRoutine()
    {
        //Screen
        #region Screen_01
        TutorialScreen screen_01 = new TutorialScreen(
            "Taleteller is a narrative card game in which you need to achieve objectives in order to develop a story. " +
            "\n" +
            "\nIn this game, you embody a writer that wants to achieve is first own book. For it, he needs to make his protagonist lives several events and involve him in different intrigues." +
            "\n" +
            "\nTo do so, you need to put elements in your games. You can put cards on the board to develop your story.",
            "WELCOME writer ! ", tutorialSprites[0]);
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
        TutorialScreen screen_02 = new TutorialScreen("A turn in Taleteller is divided in two part : " +
            "\n1 - Writing :" +
            "\nIn this part, you are able to prepare your turn.Each card can be placed on a location. You can think about the actions of each card." +
            "\n" +
            "\nOnce you think you’re done, click on the GO button, the second part will start." +
            "\n" +
            "\n2 - Lecture :" +
            "\nIn this part, the player hasn’t got any input. Your hero takes the lead ! In fact, he will cross board and trigger the effect of each card one by one.Once he arrives at the end, it’s the end of the turn.", 
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
        HeroMessage lineMessage = new HeroMessage("Hello adventurer, I’m the main character of the story, you and me are now linked. That’s why I’ll support your learning.", lineMessageQueue, true);
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
        HeroMessage deckMessage = new HeroMessage("It contains all your cards. If it’s empty, no worries, it will refill on its own using your discard pile.", deckMessageQueue, true);
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
        HeroMessage discardMessage = new HeroMessage("Here it is. Your Discard Pile will get all your cards used.", discardMessageQueue, true);
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
        HeroMessage handMessage = new HeroMessage("In this space, you can store your cards. You can store up to 7 cards and 1 special card called “Plot.” " +
            "\nIf you draw a card when your hand is full, you will have to discard it. " +
            "\nYou can play your card on the different locations.", handMessageQueue, true);
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
        HeroMessage boardMessage = new HeroMessage("As you can see, each step is linked to a card location. This line is where I’ll cross the board and trigger the cards placed during the preparation period.", boardMessageQueue, true);
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
        HeroMessage goButtonMessage = new HeroMessage("To make me cross the board, Here is the “Go Button”. Once you click on it, I will take the lead and live the story you prepared to me.", goButtonMessageQueue, true);
        goButtonMessageQueue.StartQueue();
        while (!goButtonMessageQueue.resolved) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Hero Message
        #region Hero_07
        EventQueue tooltipMessageQueue = new EventQueue();
        HeroMessage tooltipMessage = new HeroMessage("If you don’t understand a thing, just keep the cursor in it. It will gives you advices.", tooltipMessageQueue, true);
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
        HeroMessage statsMessage = new HeroMessage("Oh ! I forgot to mention something. Here are my statistics.", statsMessageQueue, true);
        statsMessageQueue.StartQueue();
        while (!statsMessageQueue.resolved) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Screen
        #region Screen_03
        TutorialScreen screen_03 = new TutorialScreen("There is the attack one (icone attack), the health one (icone health) and the gold one (icone gold). " +
            "\nThis(icone attack) represents the amount of damage that I will give when I hit an enemy." +
            "\n" +
            "\nThe(icone gold) represents my currently amount of gold.With those coins, I can trigger some cards effects." +
            "\n" +
            "\nThe(icone hearth) represent my health points.If I run out of it, I die and the game is over." +
            "\n" +
            "\nDuring the game I will get upgrade and all of these statistics has a maximum value that can change during the game." +
            "\n" +
            "\nFor example, at the beginning of the game, my wallet(icone maxGold) can store up to 10 gold.If my wallet is full, and I earn gold, I’ll not able to store those coins.",
            "", tutorialSprites[2]);
        wait = true;
        screen_03.Open(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        while (screen_03.open) { yield return new WaitForEndOfFrame(); }
        wait = true;
        screen_03.Close(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //GetTutorialCards
        for (int i = 0; i < CardManager.Instance.cardDeck.cardDeck.Count; i++)
        {
            tutorialCards.Add(CardManager.Instance.cardDeck.cardDeck[i]);
        }

        //Add plot card 
        CardData plot = tutorialPlotCard.InitializeData(tutorialPlotCard);
        tutorialCards.Add(plot);
        

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
                return 0;
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
                HeroMessage ideaMessage = new HeroMessage("At the beginning of each turn, you will receive cards. Those cards represent the ideas, as a writer, you have. We’ll call them “Idea Cards”.", messageQueue, true);
                HeroMessage cardMessage = new HeroMessage("The card you received are item one, Let me explain this type of card.", messageQueue, true);
                messageQueue.StartQueue();
                while (!messageQueue.resolved) { yield return new WaitForEndOfFrame(); }

                TutorialScreen screen_01 = new TutorialScreen(
            "A card is composed of several elements :" +
            "\nAt the left top corner, you can see a number. This number represent the amount of ink you need to use to play this card." +
            "\n" +
            "\nAt the bottom of the card, you can read the card effect.Sometimes, two effects are present.Each effect as a target represented by an icone.The target is the game element that will experience the effect." +
            "\n" +
            "\nAt the end of a turn, an item card played will go to your discard pile." +
            "\n" +
            "\n(Sometimes a card may have a gem at the bottom of it, it represents the rarety of the card)",
            "", tutorialSprites[2]);
                bool wait = true;
                screen_01.Open(() => wait = false);
                while (wait) { yield return new WaitForEndOfFrame(); }
                while (screen_01.open) { yield return new WaitForEndOfFrame(); }
                wait = true;
                screen_01.Close(() => wait = false);
                while (wait) { yield return new WaitForEndOfFrame(); }
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
}
