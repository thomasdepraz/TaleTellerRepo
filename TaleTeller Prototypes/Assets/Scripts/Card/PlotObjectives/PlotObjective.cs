using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotObjective : ScriptableObject
{
    [HideInInspector] public PlotCard linkedPlotData;
    public string objectiveName;
    public List<JunkCard> linkedJunkedCards = new List<JunkCard>();
    public enum JunkSpawnLocation { Hand, EndDeck, DeckRandom, XInDeck }
    public List<JunkSpawnLocation> junkSpawnLocations = new List<JunkSpawnLocation>();
    public List<int> junksPositionsInDeck;

    //NOTE: We should surely pass this from void to nothing
    public virtual void InitObjective(PlotCard data, PlotObjective objective)
    {
        linkedPlotData = data;

        for (int i = 0; i < objective.linkedJunkedCards.Count; i++)
        {
            objective.linkedJunkedCards[i] = objective.linkedJunkedCards[i].InitializeData(objective.linkedJunkedCards[i]) as JunkCard;
            objective.linkedJunkedCards[i].objective = objective;
            objective.linkedJunkedCards[i].objectiveIndex = i;
        }

        //Subscribe objectiveUpdate to the related event 
        SubscribeUpdateStatus(data);
    }

    //THIS IS THE GENERIC RULE
    //Overide and modify in child if needed
    public virtual void SubscribeUpdateStatus(PlotCard data) 
        => data.onCardEnter += UpdateStatus;

    public void UpdateStatus(EventQueue queue)
    {
        //Here is the logic that checks the objective winning condition
        queue.events.Add(UpdateStatusRoutine(queue, null));
    }

    public void UpdateStatus(EventQueue queue, CardData data)
    {
        //Here is the logic that checks the objective winning condition
        queue.events.Add(UpdateStatusRoutine(queue, data));
    }

    public virtual IEnumerator UpdateStatusRoutine(EventQueue currentQueue, CardData data)
    {
        Debug.Log("Update objective routine");

        EventQueue completeQueue = new EventQueue();

        linkedPlotData.CompletePlot(completeQueue);

        completeQueue.StartQueue();

        while(!completeQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        currentQueue.UpdateQueue();
    }

    public void DestroyJunk(EventQueue queue)
    {
        queue.events.Add(DestroyJunksRoutine(queue));
    }
    IEnumerator DestroyJunksRoutine(EventQueue currentQueue)
    {
        yield return null;
        EventQueue destroyQueue = new EventQueue();

        for (int i = 0; i < linkedJunkedCards.Count; i++)
        {
            linkedJunkedCards[i].DestroyJunkCard(destroyQueue);
        }

        destroyQueue.StartQueue();
        while(!destroyQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        currentQueue.UpdateQueue();
    }
}
