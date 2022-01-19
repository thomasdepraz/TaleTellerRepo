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
    [HideIf("mustPlaceAll")]
    public bool specifyNumbers;
    [ShowIf("specifyNumbers")]
    public List<int> numberToPlace = new List<int>();

    public override void SubscribeUpdateStatus(PlotCard data)
    {
        data.onCardEnter += UpdateStatus;
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
            linkedJunkedCards :
            linkedJunkedCards.Where(j => junkToPlaceData.Contains(j.dataReference));

        var junkContainer = junkToTest.Select(j => j.currentContainer);


        if(specifyNumbers)
        {
            List<int> numsToPlace = numberToPlace;

            foreach (CardContainer container in junkContainer)
            {
                if (container != null)
                    for (int i = 0; i < specificJunkToPlace.Count; i++)
                    {
                        if (specificJunkToPlace[i].cardName == container.data.cardName)
                        {
                            numsToPlace[i]--;
                        }

                        if (!CardManager.Instance.board.IsCardOnBoard(container.data))
                        {
                            numsToPlace[i]++;
                            break;
                        }
                    }
                
            }

            for (int i = 0; i < numsToPlace.Count; i++)
            {
                if (numsToPlace[i] > 0)
                {
                    complete = false;
                }
            }
        }
        else
        {
            foreach (CardContainer container in junkContainer)
            {
                if (container == null)
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
        }

        //if(plotOnBoard && (complete == true))
        //{
        //    if (linkedPlotData.currentContainer == null)
        //        complete = false;
        //    else if (!CardManager.Instance.board.IsCardOnBoard(linkedPlotData))
        //        complete = false;
        //}
            
        return complete;
    }
}
