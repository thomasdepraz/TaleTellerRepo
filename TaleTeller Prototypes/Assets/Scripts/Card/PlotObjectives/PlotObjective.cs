using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotObjective : ScriptableObject
{
    [HideInInspector] public PlotCard linkedPlotData;
    public string objectiveName;
    public List<JunkCard> linkedJunkedCards = new List<JunkCard>();
    public Action<EventQueue> onPlotComplete;

    public virtual void InitObjective(PlotCard data, PlotObjective objective)
    {
        linkedPlotData = data;
        //Subscribe objectiveUpdate to the related event 
        data.onCardEnter += UpdateStatus;//<-- THIS IS THE GENERIC RULE

        for (int i = 0; i < linkedJunkedCards.Count; i++)
        {
            linkedJunkedCards[i].InitializeData(linkedJunkedCards[i]);
            JunkCard card = linkedJunkedCards[i] as JunkCard;
            card.LinkObjective(objective);
        }
    }

    public virtual void UpdateStatus(EventQueue queue)//TODO REMOVE VIRTUAL
    {
        //Here is the logic that checks the objective winning condition
        queue.events.Add(UpdateStatusRoutine(queue));
    }

    public virtual IEnumerator UpdateStatusRoutine(EventQueue currentQueue)
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
}
