using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : Singleton<RewardManager>
{
    private void Awake()
    {
        CreateSingleton();
    }


    public void ChooseMainPlotReward(EventQueue queue, PlotCard card)
    {
        queue.events.Add(ChooseMainPlotRewardRoutine(queue, card));
    }
    IEnumerator ChooseMainPlotRewardRoutine(EventQueue queue, PlotCard card)//Pick between nine cards 
    {
        yield return null;





    }

    public void ChooseMainPlotRewardFinal(EventQueue queue, PlotCard card)
    {
        queue.events.Add(ChooseMainPlotRewardFinalRoutine(queue, card));
    }
    IEnumerator ChooseMainPlotRewardFinalRoutine(EventQueue queue, PlotCard card)//Pick Legendary Card
    {
        yield return null;
        
        //use card picker


    }

    public void ChooseSecondaryPlotReward(EventQueue queue, PlotCard card)
    {
        queue.events.Add(ChooseSecondaryPlotRewardRoutine(queue, card));
    }
    IEnumerator ChooseSecondaryPlotRewardRoutine(EventQueue queue, PlotCard card)//Pick between one card / one action / one hero stats boost
    {
        yield return null;




    }
}
