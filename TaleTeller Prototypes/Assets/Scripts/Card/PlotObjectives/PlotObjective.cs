using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotObjective : ScriptableObject
{
    [HideInInspector] public PlotCard linkedPlotData;
    public string objectiveName;

    public virtual void InitObjective(PlotCard data)
    {
        linkedPlotData = data;
        //Subscribe objectiveUpdate to the related event 
        data.onEnterEvent += UpdateStatus;//<-- THIS IS THE GENERIC RULE
    }

    public virtual void UpdateStatus(EventQueue queue)
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
