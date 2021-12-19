using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Remi Secher - 12/10/2021 00:46 - Creation
/// </summary>
public class TempleAltarBObj : PlotObjective
{
    public override void SubscribeUpdateStatus(PlotCard data)
    {
        //Subscribe UpdateStatus to junk death
        foreach (var junk in data.objective.linkedJunkedCards)
            junk.onCharDeath += UpdateStatus;
    }

    public override IEnumerator UpdateStatusRoutine(EventQueue currentQueue, CardData data)
    {
        //Test if the plot is on the Board when junk is killed
        bool plotOnBoard = linkedPlotData.currentContainer.currentSlot != null;

        if (plotOnBoard)
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
