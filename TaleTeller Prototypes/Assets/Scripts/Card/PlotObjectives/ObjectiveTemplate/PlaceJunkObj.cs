using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

public class PlaceJunkObj : JunkDrivenObj
{
    [Header("Placement Restriction")]
    [Tooltip("Check this if you want all the plot junk to be placed")]
    public bool mustPlaceAll;
    [HideIf("mustPlaceAll")]
    public List<JunkCard> specificJunkToPlace = new List<JunkCard>();

    [Header("Plot Restriction")]
    [Tooltip("Check this if the plot need to be on board aswell")]
    public bool plotOnBoard;

    public override void SubscribeUpdateStatus(PlotCard data)
    {
        data.onCardEnter += UpdateStatus;
        if(!plotOnBoard)
            data.onStoryStart += UpdateStatus;
    }

    public override IEnumerator UpdateStatusRoutine(EventQueue currentQueue, CardData data)
    {
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
        bool complete = true;

        var junkToPlaceData = specificJunkToPlace.Select(j => j.dataReference);

        var junkToTest = mustPlaceAll ?
            linkedJunkedCards
            : linkedJunkedCards.Where(j => junkToPlaceData.Contains(j.dataReference));

        var junkContainer = junkToTest.Select(j => j.currentContainer);

        foreach(CardContainer container in junkContainer)
        {
            if(container == null)
            {
                complete = false;
                break;
            }
            else if (!CardManager.Instance.board.IsCardOnBoard(container.data))
            {
                complete = false;
                break;
            }
        }

        if(plotOnBoard && (complete == true))
        {
            if (linkedPlotData.currentContainer == null)
                complete = false;
            else if (!CardManager.Instance.board.IsCardOnBoard(linkedPlotData))
                complete = false;
        }
            
        return complete;
    }
}
