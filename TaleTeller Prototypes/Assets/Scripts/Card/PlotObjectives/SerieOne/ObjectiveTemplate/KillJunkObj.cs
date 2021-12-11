using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

public class KillJunkObj : JunkDrivenObj
{
    //Enum
    protected enum PlotPos
    {
        Anywhere,
        NextToJunk,
    }

    //Fields
    [Header("Kill Objectives")]
    public int numberToKill;
    int killCount;

    [Header("Kill Restrictions")]
    [Tooltip("Check this if you dont want all junk to be used in the completion logic")]
    public bool mustKillSpecific;
    [ShowIf("mustKillSpecific")]
    public List<JunkCard> specificJunkToKill = new List<JunkCard>();

    [Header("Plot Position Restrictions")]
    [Tooltip("Check this if the plot need to be on board to validate a kill")]
    public bool mustBeOnBoard;
    [ShowIf("mustBeOnBoard"), SerializeField]
    protected PlotPos positionToValidate;

    public override void SubscribeUpdateStatus(PlotCard data)
    {
        var specificJunkToKillData = specificJunkToKill.Select(j => j.dataReference);

        //Subscribe UpdateStatus to junk death
        foreach (var junk in data.objective.linkedJunkedCards)
        {
            if (mustKillSpecific)
                if (!specificJunkToKill.Contains(junk.dataReference))
                    continue;
            junk.onCharDeath += UpdateStatus;
        }
    }

    public override IEnumerator UpdateStatusRoutine(EventQueue currentQueue, CardData data)
    {
        KillCompletionTest(data);

        if (PlotCompletionTest())
        {
            //If so, complet plot
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

    public virtual bool PlotCompletionTest()
    {
        bool complete = false;

        if (killCount >= numberToKill)
            complete = true;
        else
            complete = false;

        return complete;
    }

    public virtual void KillCompletionTest(CardData data)
    {
        bool increaseCount = true;

        if (mustBeOnBoard)
        {
            if (linkedPlotData.currentContainer.currentSlot != null)
            {
                switch (positionToValidate)
                {
                    case PlotPos.NextToJunk:
                        int slotDistance = 0;
                        slotDistance = Mathf.Abs(linkedPlotData.currentContainer.currentSlot.slotIndex - data.currentContainer.currentSlot.slotIndex);
                        if (slotDistance != 1)
                            increaseCount = false;
                        break;
                }
            }
            else
                increaseCount = false;
        }

        if(increaseCount)
            killCount++;
    }
}
