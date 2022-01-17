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
    public bool plotOnBoard;
    [ShowIf("plotOnBoard"), SerializeField]
    protected PlotPos positionToValidate;

    [Header("Junk Position Restriction")]
    [Tooltip("Check this if other junk need to be on the board when the junk is killed")]
    public bool otherJunkOnBoard;
    [ShowIf("otherJunkOnBoard")]
    public List<JunkCard> otherJunkToPlace = new List<JunkCard>();


    public override void SubscribeUpdateStatus(PlotCard data)
    {
        var specificJunkToKillData = specificJunkToKill.Select(j => j.dataReference);

        //Subscribe UpdateStatus to junk death
        foreach (var junk in data.objective.linkedJunkedCards)
        {
            SubscribeToJunkDeath(junk);
        }
    }

    public void SubscribeToJunkDeath(JunkCard junk)
    {
        if (mustKillSpecific)
            if (!specificJunkToKill.Contains(junk.dataReference))
                return;
        junk.onCharDeath += UpdateStatus;
    }

    public override IEnumerator UpdateStatusRoutine(EventQueue currentQueue, CardData data)
    {
        KillCompletionTest(data);

        EventQueue feedback = new EventQueue();
        CardManager.Instance.cardTweening.EffectChangeFeedback(linkedPlotData.currentContainer, 1, 0, feedback, false);
        linkedPlotData.currentContainer.UpdateBaseInfo();
        while(!feedback.resolved) { yield return new WaitForEndOfFrame(); }

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

        if (plotOnBoard)
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

        if (otherJunkOnBoard)
        {
            var cardDataBoard = CardManager.Instance.board.slots
                .Where(s => s.currentPlacedCard.data != data)
                .Select(s => s.currentPlacedCard.data.dataReference);
            var searchedJunkData = otherJunkToPlace.Select(j => j.dataReference).ToList();

            foreach (CardData boardData in cardDataBoard)
            {
                if (searchedJunkData.Contains(boardData))
                    searchedJunkData.Remove(boardData);
            }

            if (searchedJunkData.Count > 0)
                increaseCount = false;
        }

        if (increaseCount)
            killCount++;
    }

    public override string GetDescription()
    {
        if (objectiveName.Contains("$value$"))
        {
            string description;
            int newValue = numberToKill - killCount;
            if (newValue < 0) newValue = 0;
            
            if(newValue != numberToKill) description = objectiveName.Replace("$value$", $"*{newValue}*");
            else description = objectiveName.Replace("$value$", newValue.ToString());
            return description;
        }
        else return objectiveName;
    }
}
