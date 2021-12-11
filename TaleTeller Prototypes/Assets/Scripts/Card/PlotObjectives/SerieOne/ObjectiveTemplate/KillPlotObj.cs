using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlotObj : PlotDrivenObj
{
    public override void SubscribeUpdateStatus(PlotCard data)
    {
        data.onCharDeath += UpdateStatus;
    }

    public override IEnumerator UpdateStatusRoutine(EventQueue currentQueue, CardData data)
    {
        EventQueue completeQueue = new EventQueue();

        linkedPlotData.CompletePlot(completeQueue);
        completeQueue.StartQueue();

        while (!completeQueue.resolved)
            yield return new WaitForEndOfFrame();

        currentQueue.UpdateQueue();
    }
}
