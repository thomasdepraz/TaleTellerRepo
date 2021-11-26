using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftBoard : MonoBehaviour
{
    public int numberOfSlots;
    public List<DraftSlot> slots = new List<DraftSlot>();

    //Event Queues
    [HideInInspector] public List<IEnumerator> onStartQueue = new List<IEnumerator>();
    [HideInInspector] public List<IEnumerator> onEndQueue = new List<IEnumerator>();
    [HideInInspector] public List<IEnumerator> cardEffectQueue = new List<IEnumerator>();
    [HideInInspector] public List<IEnumerator> cardEventQueue = new List<IEnumerator>();

    private int currentSlot = 0;

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
        //Pour chaque slot, on appelle l'event OnStartStory
        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].currentPlacedCard != null)
            {
                if (slots[i].currentPlacedCard.data.onStartEvent != null)
                {
                    slots[i].currentPlacedCard.data.onStartEvent();
                }
            }
        }
        //Normally have here a bg list of routines to run through
        if(onStartQueue.Count > 0)
        {
            StartCoroutine(onStartQueue[0]);
        }
        else
        {
            ProcessStory();
        }
    }
    public void UpdateOnStartQueue() //TODO : Add pause management
    {
        //Unqueue
        onStartQueue.RemoveAt(0);

        //if still event continue
        if(onStartQueue.Count > 0)
        {
            StartCoroutine(onStartQueue[0]);
        }
        //else stop and start processing story
        else
        {
            ProcessStory();
        }
    }
    #endregion

    #region ProcessStory
    public void ProcessStory()
    {
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
            if(cardEffectQueue.Count > 0)
            {
                StartCoroutine(cardEffectQueue[0]);
            }
            else
            {
                if(cardEventQueue.Count > 0)
                {
                    StartCoroutine(cardEventQueue[0]);
                }
                else
                {
                    //At the end keep going
                    Debug.Log("NO Events");
                    MoveToNextStep();
                }
            }
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
    public void UpdateStoryQueue()
    {
        if(cardEffectQueue.Count > 0)//If you havent processed all effect events, continue
        {
            //Unqueue
            cardEffectQueue.RemoveAt(0);
            
            if(cardEffectQueue.Count > 0)
            {
                StartCoroutine(cardEffectQueue[0]);
            }
            else //When all effect are done, you can process with the cardTypes specific events such as fights
            {
                if (cardEventQueue.Count > 0) StartCoroutine(cardEventQueue[0]);
                else MoveToNextStep();
            }
        }
        else if(cardEventQueue.Count > 0)
        {
            //Unqueue
            cardEventQueue.RemoveAt(0);
            if(cardEventQueue.Count > 0)
            {
                StartCoroutine(cardEventQueue[0]);
            }
            else //You have gone through every effect and card related events congrats you can proceed
            {
                MoveToNextStep();
            }
        }
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
        currentSlot = 0;//reset slot pointer

        //Pour chaque slot on appelle l'event OnEndStory
        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].currentPlacedCard != null)
            {
                if(slots[i].currentPlacedCard.data.onEndEvent != null)
                {
                    slots[i].currentPlacedCard.data.onEndEvent();
                }
            }
        }
        if(onEndQueue.Count >0)
        {
            //Run through the resulting list
            StartCoroutine(onEndQueue[0]);
        }
        else
        {
            Debug.Log("Turn ended");
            CardManager.Instance.manaSystem.RefillMana(); //TEMPORARY (it'll be elsewhere)
        }
    }
    public void UpdateOnEndQueue() //TODO : Add pause management
    {
        //Unqueue
        onEndQueue.RemoveAt(0);

        //if still event continue
        if (onEndQueue.Count > 0)
        {
            StartCoroutine(onEndQueue[0]);
        }
        //else stop and end turn 
        else
        {
            Debug.Log("Turn ended");
            CardManager.Instance.manaSystem.RefillMana(); //TEMPORARY (it'll be elsewhere)
        }
    }
    #endregion

    #region CardManagement Method
    public void DiscardCardFromBoard(CardContainer card, ref bool actionEnded)
    {
        CardManager.Instance.cardDeck.discardPile.Add(card.data);
        
        //remove from board list
        card.currentSlot.currentPlacedCard = null;

        card.ResetCard();

        actionEnded = true;
    }
    public void DiscardCardFromBoard(CardContainer card, ref bool actionEnded, System.Action onActionEndedMethod)
    {
        CardManager.Instance.cardDeck.discardPile.Add(card.data);

        //remove from board list
        card.currentSlot.currentPlacedCard = null;

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

    public void ClearEvents()
    {
        onStartQueue.Clear();
        onEndQueue.Clear();
        cardEffectQueue.Clear();
        cardEventQueue.Clear();
    }
}
