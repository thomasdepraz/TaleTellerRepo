using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class StoryManager : Singleton<StoryManager>
{
    [HideInInspector] public int turnCount;
    [HideInInspector] public int actCount;
    public List<EventQueue> queueList = new List<EventQueue>();//THIS IS FOR DEBUG
    [HideInInspector] public List<CardData> cardsToDestroy = new List<CardData>();

    [Header("References")]
    public Image fadePanel;

    private bool transitionToNextAct;

    private void Awake()
    {
        CreateSingleton();
    }

    public IEnumerator Start()
    {
        //Play tutorial or not 
        if(GameManager.Instance.currentState == GameState.TUTORIAL)
        {
            CardManager.Instance.cardDeck.enableNoShuffle = true;
            GameManager.Instance.tutorialManager.InitializeGameScreen();
        }
        else
        {
            CardManager.Instance.UpdateHandCount();

            yield return new WaitForSeconds(1);
            StartTurn();
        }
    }

    public void StartTurn()
    {
        StartCoroutine(StartTurnRoutine());
    }
    public IEnumerator StartTurnRoutine()
    {
        yield return null;
        if(turnCount == 0 && GameManager.Instance.currentState == GameState.GAME)
        {
            EventQueue mainQueue = new EventQueue();

            PlotsManager.Instance.ChooseMainPlot(mainQueue, PlotsManager.Instance.schemePools[actCount].schemes);

            mainQueue.StartQueue();
            while(!mainQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        //Deal Cards
        int numberOfCardsToDeal = 0;
        if (turnCount == 0)
            numberOfCardsToDeal = CardManager.Instance.cardDeck.drawAmountFirstTurn;
        else
            numberOfCardsToDeal = CardManager.Instance.cardDeck.drawAmount;

        if (GameManager.Instance.currentState == GameState.TUTORIAL) numberOfCardsToDeal = GameManager.Instance.tutorialManager.GetCardCount();

        EventQueue drawQueue = new EventQueue();
        CardManager.Instance.cardDeck.DrawCards(numberOfCardsToDeal, drawQueue);

        drawQueue.StartQueue();
        while(!drawQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);


        #region OnTurnStartEvent
        EventQueue onStartTurnQueue = new EventQueue();
        CallGeneralEvent("onTurnStart", onStartTurnQueue);
        if (GameManager.Instance.currentState == GameState.TUTORIAL) GameManager.Instance.tutorialManager.StartTurnAction(onStartTurnQueue);
        onStartTurnQueue.StartQueue();
        while (!onStartTurnQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }
        #endregion

        //Mana Init
        CardManager.Instance.manaSystem.StartTurnManaInit();

        //Enable interaction with cards and go button
        CardManager.Instance.board.currentBoardState = BoardState.Idle;
    }

    public void TurnEnd()
    {
        StartCoroutine(TurnEndRoutine());
    }
    IEnumerator TurnEndRoutine()
    {
        Debug.Log("End of turn");
        CardManager.Instance.board.currentBoardState = BoardState.None;

        EventQueue onEndQueue = new EventQueue();

        CallGeneralEvent("onTurnEnd", onEndQueue);
        
        onEndQueue.StartQueue();

        while(!onEndQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        if(transitionToNextAct)
        {
            transitionToNextAct = false;

            //start transition to next act routine
            TransitionToNextAct();
        }
        else
        {
            //TransitionToNextAct();
            TransitionToNextTurn();
        }
    }

    public void TransitionToNextTurn()
    {
        StartCoroutine(TransitionToNextTurnRoutine());
    }
    public IEnumerator TransitionToNextTurnRoutine()
    {
        //fade in
        bool transtionEnded = false;
        LeanTween.color(gameObject, Color.black, 1f).setOnUpdate(
            (Color col) => { fadePanel.color = col; }
            ).setOnComplete(onEnd => { transtionEnded = true; });

        while(!transtionEnded)
        {
            yield return new WaitForEndOfFrame();
        }
        transtionEnded = false;


        //reset of the hings needing a reset
        TransitionReset();

       
        yield return new WaitForSeconds(1);

        //fade out
        LeanTween.value(gameObject, fadePanel.color.a, 0, 1f).setOnUpdate(
            (float value) => {fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, value);}
            ).setOnComplete(onEnd => { transtionEnded = true; });

        while (!transtionEnded)
        {
            yield return new WaitForEndOfFrame();
        }

         turnCount++;

        StartTurn();
    }

    public void TransitionToNextAct()
    {
        StartCoroutine(TransitionToNextActRoutine());
    }
    public IEnumerator TransitionToNextActRoutine()
    {
        yield return null;

        //Fade to black 
        #region fade to black
        bool transtionEnded = false;
        LeanTween.color(gameObject, Color.black, 1f).setOnUpdate(
            (Color col) => { fadePanel.color = col; }
            ).setOnComplete(onEnd => { transtionEnded = true; });

        while (!transtionEnded)
        {
            yield return new WaitForEndOfFrame();
        }
        transtionEnded = false;
        #endregion

        //destroy
        TransitionReset(false);

        //reset player values

        //clean hand deck and discard + reset all containers
        CardManager.Instance.ClearCardLists();

        //text ?

        //init cacheddeckList + deck building phase
        //CardManager.Instance.cardDeck.InitCachedDeck();

        //EventQueue discardQueue = new EventQueue();
        
        //List<CardData> pickedCards = new List<CardData>();
        //string instruction = LocalizationManager.Instance.GetString(LocalizationManager.Instance.instructionsDictionary, GameManager.Instance.instructionsData.chooseXCardToDiscardInstruction);
        //string newInstruction = instruction.Replace("$value$", "5");

        //CardManager.Instance.cardPicker.Pick(discardQueue,CardManager.Instance.cardDeck.cachedDeck,pickedCards,5, newInstruction);

        //discardQueue.StartQueue();
        //while (!discardQueue.resolved) { yield return new WaitForEndOfFrame(); }

        //for (int i = 0; i < pickedCards.Count; i++)
        //{
        //    CardManager.Instance.cardDeck.cachedDeck.Remove(pickedCards[i]); //TODO Animate this with a card management method such as SendToOblivion
        //}

        //refill deck + reinit cards
        CardManager.Instance.cardDeck.ResetCachedDeck();

        //text ?

        //fade to transparent
        LeanTween.value(gameObject, fadePanel.color.a, 0, 1f).setOnUpdate(
            (float value) => { fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, value); }
                ).setOnComplete(onEnd => { transtionEnded = true; });

        while (!transtionEnded)
        {
            yield return new WaitForEndOfFrame();
        }


        actCount++;
        StartTurn();
    }

    void TransitionReset(bool turnReset = true)
    {
        while (cardsToDestroy.Count > 0)
        {
            Destroy(cardsToDestroy[0]);
            cardsToDestroy.RemoveAt(0);
        }

        //DEPRECATED méca de Damage bonus entre les tours
        /*if(turnReset)
            GameManager.Instance.currentHero.bonusDamage = (int)Mathf.Ceil(GameManager.Instance.currentHero.bonusDamage / 2f);
        else*/
            GameManager.Instance.currentHero.bonusDamage = 0;

        CardManager.Instance.board.storyLine.ResetPlayerPosition();

    }

    public void CallGeneralEvent(string eventName, EventQueue queue)//Calls the events for all the cards in hand
    {
        for (int i = 0; i < CardManager.Instance.cardHand.currentHand.Count; i++)
        {
            CardData data = CardManager.Instance.cardHand.currentHand[i].data;
            switch (eventName)
            {
                case nameof(data.onTurnStart):
                    if (data.onTurnStart != null) data.onTurnStart(queue);
                    break;

                case nameof(data.onTurnEnd):
                    if (data.onTurnEnd != null) data.onTurnEnd(queue);
                    break;
            }
        }
    }

    public void NextStoryArc()
    {
        transitionToNextAct = true;
        turnCount = 0;
    }


}
