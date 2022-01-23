using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DumpAmountObj : PlotObjective
{
    public EffectValueType typeToDump;
    public int amountToDump;

    int dumpAmount;
    HeroDumpEffect.DumpInfos infos;

    public override void SubscribeUpdateStatus(PlotCard data)
    {
        var cardDumpEffect = linkedPlotData.effects.Where(e => e.GetType() == typeof(HeroDumpEffect));


        foreach(Effect effect in cardDumpEffect)
        {
            var dumpEffect = (HeroDumpEffect)effect;
            dumpEffect.onDump += GetDumpInfos;
        }
    }

    void GetDumpInfos(EventQueue currentQueue, HeroDumpEffect.DumpInfos _infos)
    {
        infos = _infos;
        UpdateStatus(currentQueue, null);
    }

    public override IEnumerator UpdateStatusRoutine(EventQueue currentQueue, CardData data)
    {
        DumpProgression();

        EventQueue feedback = new EventQueue();
        CardManager.Instance.cardTweening.EffectChangeFeedback(linkedPlotData.currentContainer, 1, 0, feedback, false);
        linkedPlotData.currentContainer.UpdateBaseInfo();
        while (!feedback.resolved) { yield return new WaitForEndOfFrame(); }


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

    private void DumpProgression()
    {
        if (infos.typeDumped == typeToDump)
            dumpAmount += infos.value;
    }

    private bool PlotCompletionTest()
    {
        if (dumpAmount >= amountToDump)
            return true;
        else 
            return false;
    }

    public override string GetDescription()
    {
        string description;
        int newValue = amountToDump - dumpAmount;
        if (newValue < 0) newValue = 0;
        if(newValue != amountToDump) description = objectiveName.Replace("$value$", $"*{newValue}*");
        else description = objectiveName.Replace("$value$", newValue.ToString());


        return description;
    }
}
