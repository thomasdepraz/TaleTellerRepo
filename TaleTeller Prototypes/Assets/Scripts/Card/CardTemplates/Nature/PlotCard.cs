using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Plot Card", menuName = "Data/PlotCard")]
public class PlotCard : CardData
{
    [Expandable]
    public PlotObjective objective;
    public int completionTimer;
    public bool isMainPlot;
    public bool isFinal;

    //Eventually CardData malusCardToSpawn 

    //TEMP
    public override CardData InitializeData(CardData data)
    {
        PlotCard plot = Instantiate(dataReference) as PlotCard;//make data an instance of itself

        //Write logic to determine how the card subscribe to the events
        if (plot.cardTypeReference != null)
        {
            plot.cardType = Instantiate(plot.cardTypeReference);
            plot.cardType.InitType(plot);//<--Watch out, subscribing to events can happen in here
        }
        else //All the events that i subscribe in here must be the one that are overidden if I have a certain cardType
        {
            InitializeCardEffects(plot);
        }

        plot.onStoryEnd += plot.OnEndPlot;
        plot.onCardAppear += plot.OnPlotAppear;

        plot.objective = Instantiate(plot.objective);
        plot.objective.InitObjective(plot, plot.objective);

        return plot;
    }


    public void OnPlotAppear(EventQueue queue)
    {
        queue.events.Add(OnPlotAppearRoutine(queue));
    }
    IEnumerator OnPlotAppearRoutine(EventQueue currentQueue)
    {
        yield return null;

        //if deck dont contains this card then animate to hand
        if(!CardManager.Instance.cardDeck.cardDeck.Contains(PlotsManager.Instance.currentPickedCard))
        {
            EventQueue toHandQueue = new EventQueue();

            PlotsManager.Instance.SendPlotToHand(toHandQueue, PlotsManager.Instance.currentPickedCard);

            toHandQueue.StartQueue();
            while(!toHandQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }
        }


        //add all junk cards to deck for now -- TODO later call the appropriate method on plotObjective so it chooses where to send the cards TODO 
        for (int i = 0; i < objective.linkedJunkedCards.Count; i++)
        {
            objective.linkedJunkedCards[i].InitializeData(objective.linkedJunkedCards[i]);
            CardManager.Instance.cardDeck.cardDeck.Add(objective.linkedJunkedCards[i]);
        }


        currentQueue.UpdateQueue();
    }

    public void OnEndPlot(EventQueue queue)
    {
        queue.events.Add(OnEndPlotRoutine(queue));
    }
    IEnumerator OnEndPlotRoutine(EventQueue currentQueue)
    {
        yield return null;
        EventQueue updateCardStatusQueue = new EventQueue();

        UpdateTimer(updateCardStatusQueue);

        updateCardStatusQueue.StartQueue();
        while(!updateCardStatusQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }


        currentQueue.UpdateQueue();
    }

    public void CompletePlot(EventQueue queue)
    {
        queue.events.Add(CompletePlotRoutine(queue));
    }
    IEnumerator CompletePlotRoutine(EventQueue currentQueue)
    {
        yield return null;

        //destroy all linked junk cards
        EventQueue destroyQueue = new EventQueue();

        objective.onPlotComplete(destroyQueue);

        destroyQueue.StartQueue();

        while(!destroyQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }


        //TODO IMPLEMENT QUEUEING IN HERE

        if (isMainPlot)
        {
            if (isFinal)
            {
                //Final Step of main plot reward
                Debug.Log("Complete Final main plot");
            }
            else
            {
                //Main Plot reward
                Debug.Log("Complete main plot");
            }
        }
        else
        {
            //Secondary plot reward
            Debug.Log("Secondary Plot");
        }

        currentQueue.UpdateQueue();
    }

    public void FailPlot(EventQueue queue)
    {
        queue.events.Add(FailPilotRoutine(queue));
    }
    public IEnumerator FailPilotRoutine(EventQueue currentQueue)
    {
        yield return null;
        if (isMainPlot)
        {
            //GameOver

        }
        else
        {
            //Add malus card to player discard pile

            //DestroyCard
        }

        currentQueue.UpdateQueue();
    }

    public void UpdateTimer(EventQueue queue)
    {
        completionTimer--;
        if(completionTimer <= 0)
        {
            FailPlot(queue);
        }
        else
        {
            CardManager.Instance.board.ReturnCardToHand(currentContainer, true,queue);
        }
    }
}
