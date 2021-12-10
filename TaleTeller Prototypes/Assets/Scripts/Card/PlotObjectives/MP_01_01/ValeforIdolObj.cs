using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValeforIdolObj : PlotObjective
{
    public int killObjective;
    int killCount;
    public override void SubscribeUpdateStatus(PlotCard data)
    {
        //Subscribe UpdateStatus to junk death
        foreach (var junk in data.objective.linkedJunkedCards)
            junk.onCharDeath += UpdateStatus;
    }

    public override IEnumerator UpdateStatusRoutine(EventQueue currentQueue, CardData data)
    {
        killCount++;

        if (killCount >= killObjective)
        {
            //If so, complet plot
            EventQueue completeQueue = new EventQueue();

            linkedPlotData.CompletePlot(completeQueue);
            completeQueue.StartQueue();

            while (!completeQueue.resolved)
                yield return new WaitForEndOfFrame();
        }
        else
            //Else, go on
            yield return new WaitForEndOfFrame();

        currentQueue.UpdateQueue();
    }
}
