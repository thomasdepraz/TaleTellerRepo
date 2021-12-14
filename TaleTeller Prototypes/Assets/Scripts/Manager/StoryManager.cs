using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class StoryManager : Singleton<StoryManager>
{
    [HideInInspector] public int turnCount;
    public List<EventQueue> queueList = new List<EventQueue>();//THIS IS FOR DEBUG
    [HideInInspector] public List<CardData> cardsToDestroy = new List<CardData>();

    [Header("References")]
    public Image fadePanel;

    private void Awake()
    {
        CreateSingleton();
    }

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        StartTurn();
    }

    public void StartTurn()
    {
        StartCoroutine(StartTurnRoutine());
    }
    public IEnumerator StartTurnRoutine()
    {
        yield return null;
        if(turnCount == 0 )
        {
            EventQueue mainQueue = new EventQueue();

            PlotsManager.Instance.ChooseMainPlot(mainQueue, PlotsManager.Instance.schemes);

            mainQueue.StartQueue();
            while(!mainQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }
        }


        //TEMP secondary plot deal --it ll probably be elswhere later
        if (turnCount > 0 && turnCount % 2 == 0 && PlotsManager.Instance.secondaryPlots.Count > 0)
        {
            EventQueue secondaryPlotsQueue = new EventQueue();
            PlotsManager.Instance.ChooseSecondaryPlots(secondaryPlotsQueue);
            secondaryPlotsQueue.StartQueue();

            while (!secondaryPlotsQueue.resolved)
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


        EventQueue drawQueue = new EventQueue();
        CardManager.Instance.cardDeck.DrawCards(numberOfCardsToDeal, drawQueue);

        drawQueue.StartQueue();
        while(!drawQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);

        //Mana Init
        CardManager.Instance.manaSystem.StartTurnManaInit();

        //On Turn Begin Events
        if (turnCount > 0)
        {
            EventQueue onStartTurnQueue = new EventQueue();

            CallGeneralEvent("onTurnStart", onStartTurnQueue);

            onStartTurnQueue.StartQueue();
            while (!onStartTurnQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }
        }


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

        TransitionToNextTurn();
    }

    public void TransitionToNextTurn()
    {
        StartCoroutine(TransitionToNexTurnRoutine());
    }
    public IEnumerator TransitionToNexTurnRoutine()
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
        while(cardsToDestroy.Count > 0)
        {
            Destroy(cardsToDestroy[0]);
            cardsToDestroy.RemoveAt(0);
        }

        GameManager.Instance.currentHero.bonusDamage = 0;

        CardManager.Instance.board.storyLine.ResetPlayerPosition();

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

    #region Feedbacks
    public void HeroLifeFeedback(float value)
    {
        print(value);
        
        if(value<0)
        {
            //Damage
            StartCoroutine(HitFeedback());
        }
        else
        {
            //Heal

        }
    }
    IEnumerator HitFeedback()
    { 
        //heroGraph.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        //heroGraph.color = Color.white;
    }
    #endregion
}
