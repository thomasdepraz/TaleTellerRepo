using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum BoardState
{
    None, 
    Idle,
    Starting, 
    Processing, 
    Closing
}
public class Board : MonoBehaviour
{
    public int numberOfSlots;
    public List<BoardSlot> slots = new List<BoardSlot>();
    public StoryLine storyLine;

    //Event Queues
    [HideInInspector] public List<IEnumerator> currentQueue = new List<IEnumerator>();

    private int currentSlot = 0;
    [HideInInspector] public BoardState currentBoardState;


    #region StoryBegin
    public void InitBoard()
    {
        currentBoardState = BoardState.Starting;

        //Pour chaque slot, on appelle l'event OnStartStory et on stocke l'event dans la queue
        EventQueue queue = new EventQueue();
        CallBoardEvents("onStoryStart", queue);

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
        var currentPlacedCard = slots[slotIndex - 1].currentPlacedCard;

        if (currentPlacedCard != null)
        {
            EventQueue onEnterQueue = new EventQueue();

            //Make a list of effect event and card type related events by trigger the enter card event
            if(currentPlacedCard.data.onCardEnter !=null)
            {
                currentPlacedCard.data.onCardEnter(onEnterQueue, currentPlacedCard.data);
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
        StartCoroutine(Move(currentSlot));
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

    IEnumerator Move(int index)
    {
        EventQueue moveQueue = new EventQueue();
        storyLine.MovePlayer(moveQueue, index);
        moveQueue.StartQueue();
        while(!moveQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

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
        CallBoardEvents("onStoryEnd", closeQueue);

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
    #endregion

    #region EventManagement
    public void CallBoardEvents(string eventName, EventQueue queue)
    {
        //Run through each slots and call event for each card
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].currentPlacedCard != null)
            {
                CardData data = slots[i].currentPlacedCard.data;
                switch (eventName)
                {
                    case nameof(data.onStoryStart):
                        if (data.onStoryStart != null) data.onStoryStart(queue); //This line support the call of the start of story event
                        break;

                    case nameof(data.onStoryEnd):
                        if (data.onStoryEnd != null) data.onStoryEnd(queue); //This line support the call of the end of story event
                        break;

                    case nameof(data.illumination):
                        if (data.illumination != null) data.illumination(queue);
                        break;

                    case nameof(data.overload):
                        if (data.overload != null) data.overload(queue);
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
                StoryManager.Instance.TurnEnd();
                break;

            default:
                break;
        }
    }
    #endregion

    #region Utility
    public bool IsEmpty()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].currentPlacedCard != null)
                return false;
        }
        return true;
    }

    public bool IsCardOnBoard(CardData data)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].currentPlacedCard?.data == data)
                return true;
        }
        return false;
    }
    #endregion
}
