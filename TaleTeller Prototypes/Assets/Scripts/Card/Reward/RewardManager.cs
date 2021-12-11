using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RewardManager : Singleton<RewardManager>
{
    [Header("References")]
    public Image backgroundPanel;
    public CanvasGroup canvasGroup;
    public Button confirmButton;

    public List<CardContainer> batchOneContainers = new List<CardContainer>();
    public List<CardContainer> batchTwoContainers = new List<CardContainer>();


    [Header("Data")]
    public float fadeSpeed;

    [Space]

    //Different Pools based on each archetype
    public List<CardData> secondaryReward = new List<CardData>();
    public List<CardData> rewardPoolTrading = new List<CardData>();

    //
    bool confirmed;

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
        List<CardData> firstBatch = GetMainPlotRewardsFirstBatch(GetArchetypeList(card), 6);
        List<CardData> secondBatch = GetMainPlotRewardsSecondBatch(GetArchetypeList(card), 3, GetRandomRarity());

        canvasGroup.blocksRaycasts = true;
        confirmButton.interactable = false;

        //Fade in background
        #region FadeInBackground
        bool fadeEnded = false;
        LeanTween.color(gameObject, Color.black, fadeSpeed).setOnUpdate((Color col) => { backgroundPanel.color = col; }).setOnComplete(onEnd => { fadeEnded = true; });
        while (!fadeEnded)
        {
            yield return new WaitForEndOfFrame();
        }
        fadeEnded = false;
        #endregion

        InitializePlaceholder(firstBatch, 1);
        InitializePlaceholder(secondBatch, 2);

        while(!confirmed)
        {
            yield return new WaitForEndOfFrame();
        }

        ResetPlaceholders();

        #region FadeOutBackground
        Color transparent = new Color(0, 0, 0, 0);
        LeanTween.color(gameObject, transparent, fadeSpeed).setOnUpdate((Color col) => { backgroundPanel.color = col; }).setOnComplete(
            onEnd => { canvasGroup.blocksRaycasts = false; queue.UpdateQueue(); });
        #endregion
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


    #region Utility
    CardData GetSecondaryPlotCardReward(List<CardData> list)
    {
        int r = Random.Range(0, list.Count - 1);
        return list[r];
    }

    List<CardData> GetMainPlotRewardsFirstBatch(List<CardData> list, int count)
    {
        List<CardData> filterdList = new List<CardData>();//Only retains non epic cards
        for (int i = 0; i < list.Count; i++)
        {
            if(list[i].rarity !=  CardRarity.Epic && list[i].rarity != CardRarity.Legendary)
            {
                filterdList.Add(list[i]);
            }
        }

        List<CardData> result = new List<CardData>();

        //FOR NOW PICK RANDOM CARDS WITHIN THIS FILTERD LIST -- LATER ADD WEIGHT AND DROP RATES
        for (int i = 0; i < count; i++)
        {
            int r = Random.Range(0, filterdList.Count-1);
            result.Add(filterdList[r]);
            filterdList.RemoveAt(r);
        }

        return result;
    }

    List<CardData> GetMainPlotRewardsSecondBatch(List<CardData> list, int count, CardRarity targetRarity)
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
            int r = Random.Range(0, filteredList.Count);
            results.Add(filteredList[r]);
            filteredList.RemoveAt(r);
        }

        return results;
    }

    CardRarity GetRandomRarity()
    {
        CardRarity result = CardRarity.None;

        //TEMP FOR NOW PICK A RANDOM RARITY -- LATER ADD RATES
        int r = Random.Range(1, 5);
        switch(r)
        {
            case 1:
                result = CardRarity.Common;
                break;
            case 2:
                result = CardRarity.Uncommon;
                break;
            case 3:
                result = CardRarity.Rare;
                break;
            case 4:
                result = CardRarity.Epic;
                break;
        }

        return result;
    }

    List<CardData> GetArchetypeList(CardData data)
    {
        switch (data.archetype)
        {
            case Archetype.None:
                Debug.LogError("The card archetype can't be none");
                return null;

            case Archetype.Trading:
                return rewardPoolTrading;
        }
        Debug.LogError("Archetype List wasn't found");
        return null;
    }


    void InitializePlaceholder(List<CardData> targets , int batch)
    {

    }

    void ResetPlaceholders()
    {
        for (int i = 0; i < batchOneContainers.Count; i++)
        {
            if (batchOneContainers[i].data != null)
            {
                batchOneContainers[i].ResetContainer(true);
            }
        }
        for (int i = 0; i < batchTwoContainers.Count; i++)
        {
            if (batchTwoContainers[i].data != null)
            {
                batchTwoContainers[i].ResetContainer(true);
            }
        }
    }

    #endregion
}
