using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

    //Different Pools based on each archetype -- ALL OF THESE CARDS MUST BE INSTANTIATED
    public List<CardData> secondaryRewardCards = new List<CardData>();
    public List<CardData> rewardPoolTrading = new List<CardData>();

    //
    bool confirmed;

    #region MainPlotReward Variables
    int batchOneNumberToSelect;
    List<CardData> batchOneSelectedCards = new List<CardData>();
    int batchTwoNumberToSelect;
    List<CardData> batchTwoSelectedCards = new List<CardData>();
    #endregion

    private void Awake()
    {
        CreateSingleton();
    }

    private void Start()
    {
        for (int i = 0; i < batchOneContainers.Count; i++)
        {
            int j = i;
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(data => { SelectCard(batchOneContainers[j]); });
            batchOneContainers[i].gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
        }
        for (int i = 0; i < batchTwoContainers.Count; i++)
        {
            int j = i;
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(data => { SelectCard(batchTwoContainers[j]); });
            batchTwoContainers[i].gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
        }

        InitCards();
    }

    void InitCards()
    {
        for (int i = 0; i < secondaryRewardCards.Count; i++)
        {
            secondaryRewardCards[i].InitializeData(secondaryRewardCards[i]);
        }

        for (int i = 0; i < rewardPoolTrading.Count; i++)
        {
            rewardPoolTrading[i].InitializeData(rewardPoolTrading[i]);
        }
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
        confirmButton.gameObject.SetActive(true);
        batchOneNumberToSelect = 3;
        batchTwoNumberToSelect = 1;//NOTE PROBABLY NEED TO EXPOSE THOSE VARIABLES

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
            onEnd => { canvasGroup.blocksRaycasts = false;});


        yield return new WaitForSeconds(fadeSpeed);
        #endregion

        EventQueue toDeckQueue = new EventQueue();
        //Do something with the picked cards
        for (int i = 0; i < batchOneSelectedCards.Count; i++)
        {
            PlotsManager.Instance.SendPlotToDeck(toDeckQueue, batchOneSelectedCards[i]);//NOTE : MAYBE USE A DIFFERENT METHOD LATER
        }
        for (int i = 0; i < batchTwoSelectedCards.Count; i++)
        {
            PlotsManager.Instance.SendPlotToDeck(toDeckQueue, batchTwoSelectedCards[i]);//NOTE : MAYBE USE A DIFFERENT METHOD LATER
        }

        toDeckQueue.StartQueue();
        while(!toDeckQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        queue.UpdateQueue();
    }


    public void ChooseMainPlotRewardFinal(EventQueue queue, PlotCard card)
    {
        queue.events.Add(ChooseMainPlotRewardFinalRoutine(queue, card));
    }
    IEnumerator ChooseMainPlotRewardFinalRoutine(EventQueue queue, PlotCard card)//Pick Legendary Card
    {
        yield return null;

        //use card picker

        queue.UpdateQueue();
    }

    public void ChooseSecondaryPlotReward(EventQueue queue, PlotCard card)
    {
        queue.events.Add(ChooseSecondaryPlotRewardRoutine(queue, card));
    }
    IEnumerator ChooseSecondaryPlotRewardRoutine(EventQueue queue, PlotCard card)//Pick between one card / one action / one hero stats boost
    {
        yield return null;



        queue.UpdateQueue();
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
            if(filterdList.Count>0)
            {
                int r = Random.Range(0, filterdList.Count-1);
                result.Add(filterdList[r]);
                filterdList.RemoveAt(r);
            }
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
            if(filteredList.Count>0)
            {
                int r = Random.Range(0, filteredList.Count-1);
                results.Add(filteredList[r]);
                filteredList.RemoveAt(r);
            }
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
        List<CardContainer> placeholders = batch == 1 ? batchOneContainers : batchTwoContainers; 
        
        for (int i = 0; i < targets.Count; i++)
        {
            for (int j = 0; j < placeholders.Count; j++)
            {
                if (placeholders[i].data == null)
                {
                    placeholders[i].gameObject.SetActive(true);
                    placeholders[i].InitializeContainer(targets[i], true);
                    break;
                }
            }
        }
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

    public void SelectCard(CardContainer container)
    {
        if(batchOneContainers.Contains(container))
        {
            #region select for batch one
            if (batchOneSelectedCards.Contains(container.data))
            {
                //Hide selected shader

                //Remove from list
                batchOneSelectedCards.Remove(container.data);

                return;
            }

            if (batchOneSelectedCards.Count == batchOneNumberToSelect)
            {
                return;
            }

            else if (!batchOneSelectedCards.Contains(container.data))
            {
                //show selected shader

                //add to list
                batchOneSelectedCards.Add(container.data);
            }
            #endregion
        }
        else if(batchTwoContainers.Contains(container))
        {
            #region select for batch two
            if (batchTwoSelectedCards.Contains(container.data))
            {
                //Hide selected shader

                //Remove from list
                batchTwoSelectedCards.Remove(container.data);

                return;
            }

            if (batchTwoSelectedCards.Count == batchTwoNumberToSelect)
            {
                return;
            }

            else if (!batchTwoSelectedCards.Contains(container.data))
            {
                //show selected shader

                //add to list
                batchTwoSelectedCards.Add(container.data);
            }
            #endregion
        }
    }

    public void OnButtonClick()
    {
        confirmed = true;
        confirmButton.gameObject.SetActive(false);
    }

    #endregion
}
