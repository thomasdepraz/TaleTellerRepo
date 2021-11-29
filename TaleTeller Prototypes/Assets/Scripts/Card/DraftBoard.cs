using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardState
{
    Idle,
    Starting, 
    Processing, 
    Closing
}
public class DraftBoard : MonoBehaviour
{
    public int numberOfSlots;
    public List<DraftSlot> slots = new List<DraftSlot>();

    //Event Queues
    [HideInInspector] public List<IEnumerator> currentQueue = new List<IEnumerator>();
    [HideInInspector] public List<IEnumerator> cardEffectQueue = new List<IEnumerator>();
    [HideInInspector] public List<IEnumerator> cardEventQueue = new List<IEnumerator>();

    private int currentSlot = 0;
    [HideInInspector] BoardState currentBoardState;
    System.Action currentOnEndQueueAction;

    #region OldLogic
    public void CreateStory()
    {
        
        for (int i = 0; i < slots.Count; i++)
        {
            //Create event based on card data
            if(slots[i].currentPlacedCard != null)
            {
                Debug.Log($"Created {slots[i].currentPlacedCard.data.cardName} at step {i+1}.");

                CardData cardData = slots[i].currentPlacedCard.data;
                CardToInit card = new CardToInit(cardData, i);
                GameManager.Instance.storyManager.cardsToInit.Add(card);
                GameManager.Instance.creativityManager.creativity -= GameManager.Instance.creativityManager.currentBoardCreativityCost;
                GameManager.Instance.creativityManager.currentBoardCreativityCost = 0;//reset board cost
            }
        }
    }

    public void ClearDraft()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            //place all data to discard Pile and reset card to hiddenhand
            if(slots[i].currentPlacedCard != null)
            {
                //Call reset method on card
                slots[i].currentPlacedCard.ResetCard();

                //Reset slot
                slots[i].currentPlacedCard = null;
                slots[i].canvasGroup.blocksRaycasts = true;
            }
        }
        //Clear Hand
        //CardManager.Instance.cardHand.DiscardHand();

        //launch story
        GameManager.Instance.storyManager.StartStory();
    }

    public bool IsEmpty()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].currentPlacedCard != null)
                return false;
        }
        return true;
    }
    #endregion

    #region StoryBegin
    public void InitBoard()
    {
        currentBoardState = BoardState.Starting;

        //Pour chaque slot, on appelle l'event OnStartStory
        CallEvents("onStartEvent");

        //Normally have here a bg list of routines to run through
        StartQueue();
    }
    #endregion

    #region ProcessStory
    public void ProcessStory()
    {
        currentBoardState = BoardState.Processing;

        //Move to first step
        MoveToNextStep();
    }

    void ReadStoryStep(int slotIndex)//Actual resolving of events
    {
        currentSlot = slotIndex;
        Debug.Log($"Reading slot number {slotIndex}");

        if(slots[slotIndex - 1].currentPlacedCard != null)
        {
            //Make a list of effect event and card type related events by trigger the enter card event
            if(slots[slotIndex-1].currentPlacedCard.data.onEnterEvent !=null)
                slots[slotIndex-1].currentPlacedCard.data.onEnterEvent();

            //go through them if any
            StartQueue();
        }
        else
        {
            Debug.Log("No card on slot");
            MoveToNextStep();
        }
    }

    void MoveToNextStep()//Move the focus from one step to another thus resolving it's events (Move the player too)
    {
        //Start coroutine for moving that calls for TriggerRead
        StartCoroutine(tempMove(currentSlot));
    }

    void TriggerRead(int currentSlotIndex)
    {
        //if all done read story step
        if (currentSlotIndex < numberOfSlots)
        {
            currentSlotIndex++;
            ReadStoryStep(currentSlotIndex);
        }
        else //end of story
        {
            CloseBoard();
        }
    }

    IEnumerator tempMove(int index)//Temp coroutine to test movement
    {
        Debug.Log("Start Moving");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Stopped Moving");
        yield return new WaitForSeconds(0.2f);

        TriggerRead(index);
    }
    #endregion

    #region StoryEnd
    public void CloseBoard()
    {
        currentBoardState = BoardState.Closing;
        currentSlot = 0;//reset slot pointer

        //Pour chaque slot on appelle l'event OnEndStory
        CallEvents("onEndEvent");

        StartQueue();
    }
    void TempStoryEnd()
    {
        Debug.Log("Turn ended");
        CardManager.Instance.manaSystem.RefillMana(); //TEMPORARY (it'll be elsewhere)
    }
    #endregion

    #region CardManagement Method
    public void DiscardCardFromBoard(CardContainer card, ref bool actionEnded)
    {
        card.data.ResetData(card.data);

        CardManager.Instance.cardDeck.discardPile.Add(card.data);
        
        //remove from board list
        card.currentSlot.currentPlacedCard = null;
        card.currentSlot.canvasGroup.blocksRaycasts = true;

        card.ResetCard();

        actionEnded = true;
    }
    public void DiscardCardFromBoard(CardContainer card, ref bool actionEnded, System.Action onActionEndedMethod)
    {
        card.data.ResetData(card.data);

        CardManager.Instance.cardDeck.discardPile.Add(card.data);

        //remove from board list
        card.currentSlot.currentPlacedCard = null;
        card.currentSlot.canvasGroup.blocksRaycasts = true;

        card.ResetCard();

        actionEnded = true;

        onActionEndedMethod.Invoke();
    }

    public void ReturnCardToHand(CardContainer card, bool canPushOverCard, ref bool actionEnded)
    {
        if(canPushOverCard)
        {
            if(CardManager.Instance.cardHand.currentHand.Count == CardManager.Instance.cardHand.maxHandSize)//if max cards in hand make the player select a card
            {
                //MAKE the player pick a card and discard it

                //FOR NOW
                DiscardCardFromBoard(card, ref actionEnded);
            }
            else//just return card to hand
            {
                //remove from board list
                card.currentSlot.currentPlacedCard = null;
                card.currentSlot.canvasGroup.blocksRaycasts = true;
                card.currentSlot = null;

                //use method from deck to move cardBack to hand
                CardManager.Instance.cardHand.MoveCard(card, CardManager.Instance.cardHand.RandomPositionInRect(CardManager.Instance.cardHand.handTransform), false);
                actionEnded = true; 
            }
        }
        else
        {
            if (CardManager.Instance.cardHand.currentHand.Count == CardManager.Instance.cardHand.maxHandSize + 1)//if max cards in hand discard
            {
                DiscardCardFromBoard(card, ref actionEnded);
            }
            else//just return card to hand
            {
                //remove from board list
                card.currentSlot.currentPlacedCard = null;
                card.currentSlot.canvasGroup.blocksRaycasts = true;
                card.currentSlot = null;


                //use method from deck to move cardBack to hand
                CardManager.Instance.cardHand.MoveCard(card, CardManager.Instance.cardHand.RandomPositionInRect(CardManager.Instance.cardHand.handTransform), false);
                actionEnded = true;
            }
        }
    }

    #endregion

    #region EventManagement
    public void CallEvents(string eventName)
    {
        //Run through each slots and call event for each card
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].currentPlacedCard != null)
            {
                CardData data = slots[i].currentPlacedCard.data;
                switch (eventName)
                {
                    case nameof(data.onStartEvent):
                        if (data.onStartEvent != null) data.onStartEvent();
                        break;

                    case nameof(data.onEndEvent):
                        if (data.onEndEvent != null) data.onEndEvent();
                        break;
                }
            }
        }

    }

    public void ClearEvents()
    {
        currentQueue.Clear();
    }

    public void ResumeStory()
    {
        //based on the current board case it calls the right method
        switch (currentBoardState)
        {
            case BoardState.Idle:
                break;

            case BoardState.Starting:
                ProcessStory();
                break;

            case BoardState.Processing:
                MoveToNextStep();
                break;

            case BoardState.Closing:
                TempStoryEnd();//TEMPORARY
                break;

            default:
                break;
        }
    }

    public void StartQueue()
    {
        if(currentQueue.Count > 0)
        {
            StartCoroutine(currentQueue[0]);
        }
        else
        {
            ResumeStory();
        }
    }

    public void StartQueue(System.Action onEndQueue)
    {
        if (currentQueue.Count > 0)
        {
            StartCoroutine(currentQueue[0]);
        }
        else
        {
            onEndQueue();
        }
    }

    public void UpdateQueue()//generic update queue method // TODO : Add Pause management
    {
        //Unqueue
        if (currentQueue.Count > 0) currentQueue.RemoveAt(0);

        if(currentQueue.Count > 0)//Play next event 
        {
            StartCoroutine(currentQueue[0]);
        }
        else//resume the story where you left it
        {
            if(currentOnEndQueueAction == null)
            {
                ResumeStory();
            }
            else
            {
                currentOnEndQueueAction();
                currentOnEndQueueAction = null;
            }
        }
    }

    public void UpdateQueue(System.Action onEndQueue)
    {
        //Unqueue
        if (currentQueue.Count > 0) currentQueue.RemoveAt(0);

        if (currentQueue.Count > 0)//Play next event 
        {
            StartCoroutine(currentQueue[0]);
        }
        else//resume the story where you left it
        {
            onEndQueue();
        }
    }
    #endregion
}
