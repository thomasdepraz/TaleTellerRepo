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
                slots[i].currentPlacedCard.ResetContainer();

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

        //Pour chaque slot, on appelle l'event OnStartStory et on stocke l'event dans la queue
        EventQueue queue = new EventQueue();
        CallEvents("onStartEvent", queue);

        //Normally have here a bg list of routines to run through
        //StartQueue();
        StartCoroutine(InitRoutine(queue));
    }
    IEnumerator InitRoutine(EventQueue queue)
    {
        queue.StartQueue();

        while(!queue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        ResumeStory();
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
            EventQueue onEnterQueue = new EventQueue();
            //Make a list of effect event and card type related events by trigger the enter card event
            if(slots[slotIndex-1].currentPlacedCard.data.onEnterEvent !=null)
            {

                slots[slotIndex-1].currentPlacedCard.data.onEnterEvent(onEnterQueue);
            }

            //go through them if any
            StartCoroutine(ReadRoutine(onEnterQueue));
        }
        else
        {
            Debug.Log("No card on slot");
            MoveToNextStep();
        }
    }
    IEnumerator ReadRoutine(EventQueue queue)
    {
        queue.StartQueue();

        while (!queue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        MoveToNextStep();
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
        EventQueue closeQueue = new EventQueue();
        CallEvents("onEndEvent", closeQueue);

        StartCoroutine(CloseRoutine(closeQueue));
        //StartQueue();
    }
    IEnumerator CloseRoutine(EventQueue queue)
    {
        queue.StartQueue();

        while (!queue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        ResumeStory();
    }
    void TempStoryEnd()
    {
        Debug.Log("Turn ended");
        CardManager.Instance.manaSystem.RefillMana(); //TEMPORARY (it'll be elsewhere)
    }
    #endregion

    #region CardManagement Method
    public void DiscardCardFromBoard(CardContainer card, EventQueue queue)
    {
        //new discard method
        queue.events.Add(DiscardCardFromBoardRoutine(card, queue));
    }
    IEnumerator DiscardCardFromBoardRoutine(CardContainer card, EventQueue currentQueue)
    {
        card.data.ResetData(card.data);

        CardManager.Instance.cardDeck.discardPile.Add(card.data);

        //remove from board list
        card.currentSlot.currentPlacedCard = null;
        card.currentSlot.canvasGroup.blocksRaycasts = true;

        card.ResetContainer();
        yield return new WaitForSeconds(0.2f);//TEMP

        currentQueue.UpdateQueue();
    }

    public void ReturnCardToHand(CardContainer card, bool canPushOverCard, EventQueue queue)
    {
        queue.events.Add(ReturnCardToHandRoutine(card, canPushOverCard, queue));
    }
    IEnumerator ReturnCardToHandRoutine(CardContainer card, bool canPushOverCard, EventQueue currentQueue)
    {
        yield return null;

        EventQueue returnQueue = new EventQueue();
        
        if (canPushOverCard)
        {
            if (CardManager.Instance.cardHand.currentHand.Count == CardManager.Instance.cardHand.maxHandSize-1)//if max cards in hand make the player select a card
            {
                //MAKE the player pick a card and discard it
                EventQueue pickQueue = new EventQueue();
                List<CardData> pickedCards = new List<CardData>();

                CardManager.Instance.cardPicker.Pick(pickQueue, CardManager.Instance.cardHand.GetHandDataList(), pickedCards, 1, false);

                pickQueue.StartQueue();
                while(!pickQueue.resolved)
                {
                    yield return new WaitForEndOfFrame();
                }

                //discard all of the picked cards
                for (int i = 0; i < pickedCards.Count; i++)
                {
                    CardManager.Instance.cardHand.DiscardCardFromHand(pickedCards[i].currentContainer, returnQueue);
                }

                //return card to hand
                ReturnCard(card, returnQueue);
            }
            else//just return card to hand
            {
                ReturnCard(card, returnQueue);
            }
        }
        else
        {
            if (CardManager.Instance.cardHand.currentHand.Count == CardManager.Instance.cardHand.maxHandSize + 1)//if max cards in hand discard
            {
                DiscardCardFromBoard(card, returnQueue);
            }
            else//just return card to hand
            {
                ReturnCard(card, returnQueue);
            }
        }

        returnQueue.StartQueue(); //Actual discard / return happens here

        while(!returnQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        currentQueue.UpdateQueue();
    }

    private void ReturnCard(CardContainer card, EventQueue queue)
    {
        queue.events.Add(ReturnCardRoutine(card, queue));
    }
    IEnumerator ReturnCardRoutine(CardContainer card, EventQueue queue)
    {
        //remove from board list
        card.currentSlot.currentPlacedCard = null;
        card.currentSlot.canvasGroup.blocksRaycasts = true;
        card.currentSlot = null;


        //use method from deck to move cardBack to hand
        CardManager.Instance.cardHand.MoveCard(card, CardManager.Instance.cardHand.RandomPositionInRect(CardManager.Instance.cardHand.handTransform), false); //TODO add event queue from this method
        CardManager.Instance.cardHand.currentHand.Add(card);
        yield return new WaitForSeconds(0.5f);

        queue.UpdateQueue();
    }
    #endregion

    #region EventManagement
    public void CallEvents(string eventName, EventQueue queue)
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
                        if (data.onStartEvent != null) data.onStartEvent(queue);
                        break;

                    case nameof(data.onEndEvent):
                        if (data.onEndEvent != null) data.onEndEvent(queue);
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
    #endregion
}
