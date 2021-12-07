using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Junk Card", menuName = "Data/JunkCard")]
public class JunkCard : CardData
{

    [HideInInspector] PlotObjective linkedObjective;

    public override CardData InitializeData(CardData data)
    {
        return base.InitializeData(data);
    }

    public void LinkObjective(PlotObjective objective)
    {
        //subscribe to an event or go reference yourself to the current objective that defining your lifetime //TODO
        //When the objective is complete it destroys every JunkCard no matter where they are;
        linkedObjective = objective;
        objective.onPlotComplete += DestroyJunkCard;
    }

    void DestroyJunkCard(EventQueue queue)
    {
        queue.events.Add(DestroyJunkCardRoutine(queue));
    }

    IEnumerator DestroyJunkCardRoutine(EventQueue currentQueue) //LATER probably have this method in the card manager as DestroyCard(CardData data) {}
    {
        EventQueue destroyQueue = new EventQueue();

        //TODO implement add to queue event list in function that take time;
        //Discard the card based on where it is 
        if(currentContainer != null) //means it's in the hand or board
        {
            currentContainer.ResetContainer();
            UnsubscribeEvents(this);
            StoryManager.Instance.cardsToDestroy.Add(this);
        }
        else 
        {
            //It's maybe in the deck or discard pile
            if(CardManager.Instance.cardDeck.cardDeck.Contains(this))
            {
                //remove and destroy
                CardManager.Instance.cardDeck.cardDeck.Remove(this);
                UnsubscribeEvents(this);
                StoryManager.Instance.cardsToDestroy.Add(this);
            }
            else if(CardManager.Instance.cardDeck.discardPile.Contains(this))
            {
                //remove and destroy
                CardManager.Instance.cardDeck.discardPile.Remove(this);
                UnsubscribeEvents(this);
                StoryManager.Instance.cardsToDestroy.Add(this);
            }
            else
            {
                Debug.LogError("The junk card that needs to be discarded cant be found");
            }
        }

        destroyQueue.StartQueue();//<-- actual destroy happens here

        while (!destroyQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        //TODO CALL RESUME
        currentQueue.UpdateQueue();
    }
}
