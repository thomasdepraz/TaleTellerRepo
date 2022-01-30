using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RewardManager : Singleton<RewardManager>
{
    public RewardInfo currentRewardInfo;
    public RewardProfile rewardProfile;
   

    //Different Pools based on each archetype -- ALL OF THESE CARDS MUST BE INSTANTIATED
    public List<CardData> rewardPoolAllIdeas = new List<CardData>();

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
        RewardScreen rewardScreen = new RewardScreen(currentRewardInfo, PlotsManager.Instance.currentMainPlotScheme);
        bool wait = true;
        rewardScreen.Open((() => { wait = false; }));
        while (wait) { yield return new WaitForEndOfFrame(); }

        while (rewardScreen.open) { yield return new WaitForEndOfFrame(); }

        wait = true;
        rewardScreen.Close(() => { wait = false; });
        while(wait) { yield return new WaitForEndOfFrame(); }

        EventQueue rewardQueue = new EventQueue();

        rewardScreen.chosenHeroReward.ApplyReward(rewardQueue);
        rewardScreen.chosenCardReward.ApplyReward(rewardQueue);

        rewardQueue.StartQueue();
        while(!rewardQueue.resolved) { yield return new WaitForEndOfFrame(); }

        queue.UpdateQueue();
    }

    public void ChooseMainPlotRewardFinal(EventQueue queue, PlotCard card)
    {
        queue.events.Add(ChooseMainPlotRewardFinalRoutine(queue, card));
    }
    IEnumerator ChooseMainPlotRewardFinalRoutine(EventQueue queue, PlotCard card)//Pick Legendary Card
    {
        List<CardData> pickedCard = new List<CardData>();

        //use card picker
        EventQueue pickerQueue = new EventQueue();

        RewardScreen rewardScreen = new RewardScreen(currentRewardInfo, PlotsManager.Instance.currentMainPlotScheme, card.legendaryCardRewards);
        bool wait = true;
        rewardScreen.Open((() => { wait = false; }));
        while (wait) { yield return new WaitForEndOfFrame(); }

        while (rewardScreen.open) { yield return new WaitForEndOfFrame(); }

        wait = true;
        rewardScreen.Close(() => { wait = false; });
        while (wait) { yield return new WaitForEndOfFrame(); }

        EventQueue rewardQueue = new EventQueue();

        rewardScreen.chosenHeroReward.ApplyReward(rewardQueue);
        rewardScreen.chosenCardReward.ApplyReward(rewardQueue);

        rewardQueue.StartQueue();
        while (!rewardQueue.resolved) { yield return new WaitForEndOfFrame(); }

        queue.UpdateQueue();
    }

    public List<CardData> GetDebugDeckList(List<CardData> list, int count, CardRarity targetRarity)
    {
        List<CardData> filteredList = new List<CardData>();
        for (int i = 0; i < list.Count; i++)
        {
            if(list[i].rarity == targetRarity)
            {
                filteredList.Add(list[i]);    
            }
        }

        List<CardData> results = new List<CardData>();

        for (int i = 0; i < count; i++)
        {
            if(filteredList.Count>0)
            {
                int r = Random.Range(0, filteredList.Count-1);
                results.Add(filteredList[r]);
                filteredList.RemoveAt(r);
            }
        }

        for (int i = 0; i < results.Count; i++)
        {
            results[i] = results[i].InitializeData(results[i]);
        }

        return results;
    }
}
