using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Junk Card", menuName = "Data/JunkCard")]
public class JunkCard : CardData
{
    public PlotObjective objective;
    public int objectiveIndex;
    public override CardData InitializeData(CardData data)
    {
        JunkCard junk = Instantiate(data.dataReference) as JunkCard;
        junk.dataReference = data.dataReference;

        data.currentContainer = null;

        if(data.cardTypeReference != null)
        {
            CardTypes type = Instantiate(data.cardTypeReference);
            junk.cardType = type;
            junk.cardTypeReference = data.cardTypeReference;

            junk.cardType.InitType(junk);
        }
        else
        {
            InitializeCardEffects(junk);
        }

        junk.onStoryEnd += junk.OnEndJunk; //subscribe to basic discard behavior

        JunkCard reference = data as JunkCard;
        if (reference.objective != null)
        {
            junk.objective = reference.objective;
            junk.objective.linkedJunkedCards[objectiveIndex] = junk;
            junk.objectiveIndex = reference.objectiveIndex;
        }

        return junk;
    }

    public void OnEndJunk(EventQueue queue)
    {
        queue.events.Add(OnEndJunkRoutine(queue));
    }
    IEnumerator OnEndJunkRoutine(EventQueue currentQueue)
    {
        yield return null;
        EventQueue discardQueue = new EventQueue();

        CardManager.Instance.board.DiscardCardFromBoard(currentContainer, discardQueue);

        discardQueue.StartQueue();
        while (!discardQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        currentQueue.UpdateQueue();
    }

    public void DestroyJunkCard(EventQueue queue)
    {
        queue.events.Add(DestroyJunkCardRoutine(queue));
    }

    IEnumerator DestroyJunkCardRoutine(EventQueue currentQueue) //LATER probably have this method in the card manager as DestroyCard(CardData data) {}
    {
        EventQueue destroyQueue = new EventQueue();
        //TODO implement add to queue event list in function that take time;
        //Discard the card based on where it is 
        if (currentContainer != null) //means it's in the hand or board
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
