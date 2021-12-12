using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Remi Secher - 12/10/2021 00:46 - Creation
/// </summary>
public class TempleAltarAObj : PlotObjective
{
    public override void SubscribeUpdateStatus(PlotCard data)
    {
        data.onCardEnter += UpdateStatus;
    }

    public override IEnumerator UpdateStatusRoutine(EventQueue currentQueue, CardData data)
    {
        bool allArtifactOnBoard = true;

        //Test if All Artifacts are placed on the board
        foreach(CardData junk in linkedPlotData.objective.linkedJunkedCards)
        {
            if(junk.currentContainer.currentSlot == null)
            {
                allArtifactOnBoard = false;
                break;
            }
        }

        if (PlotCompletionTest())
        {
            EventQueue completeQueue = new EventQueue();

            linkedPlotData.CompletePlot(completeQueue);
            completeQueue.StartQueue();

            while (!completeQueue.resolved)
                yield return new WaitForEndOfFrame();
        }
        else
            yield return new WaitForEndOfFrame();

        currentQueue.UpdateQueue();
    }

    public bool PlotCompletionTest()
    {
        bool complete = false;

        return complete;
    }
}
