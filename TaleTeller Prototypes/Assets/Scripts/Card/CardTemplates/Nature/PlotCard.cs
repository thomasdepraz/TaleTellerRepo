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
        data = Instantiate(dataReference);//make data an instance of itself

        //Write logic to determine how the card subscribe to the events
        if (data.cardTypeReference != null)
        {
            data.cardType = Instantiate(data.cardTypeReference);
            data.cardType.InitType(data);//<--Watch out, subscribing to events can happen in here
        }
        else //All the events that i subscribe in here must be the one that are overidden if I have a certain cardType
        {
            InitializeCardEffects(data);
        }

        data.onEndEvent += OnEndPlot;//TEMP <-- This works only if no type 

        objective = Instantiate(objective);
        objective.InitObjective(data as PlotCard);

        return data;
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
