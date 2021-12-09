using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Rémi Sécher - 12/08/2021 22:51 - Creation
/// </summary>
public class TheophilusThePriestObj : PlotObjective
{
    public override void SubscribeUpdateStatus(PlotCard data)
    {
        //Subscribe UpdateStatus to junk death
        foreach(var junk in data.objective.linkedJunkedCards)
            junk.onCharDeath += UpdateStatus;
    }

    public override IEnumerator UpdateStatusRoutine(EventQueue currentQueue, CardData data)
    {
        //Test if the junk that trigger the StatusUpdate is next to the plot card
        bool isNextToJunk =
            (data.currentContainer.currentSlot.slotIndex - 1 == linkedPlotData.currentContainer.currentSlot.slotIndex)
            || (data.currentContainer.currentSlot.slotIndex + 1 == linkedPlotData.currentContainer.currentSlot.slotIndex);

        if (isNextToJunk)
        {
            //If so, complet plot
            EventQueue completeQueue = new EventQueue();
            while (!completeQueue.resolved)
                yield return new WaitForEndOfFrame();
        }
        else
            //Else, go on
            yield return new WaitForEndOfFrame();

        currentQueue.UpdateQueue();
    }
}
