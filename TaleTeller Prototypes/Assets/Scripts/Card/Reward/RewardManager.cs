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
    [Header("References")]
    public Image backgroundPanel;
    public CanvasGroup canvasGroup;
    public Button confirmButton;

    public List<CardContainer> batchOneContainers = new List<CardContainer>();
    public List<CardContainer> batchTwoContainers = new List<CardContainer>();

    [Space]
    public CardContainer secondaryRewardCardContainer;
    public Button statsRewardButton;
    public Button actionRewardButton;
    public TextMeshProUGUI statsButtonDescription;
    public TextMeshProUGUI actionButtonDescription;


    [Header("Data")]
    public float fadeSpeed;
    [Space]
    public int batchOne;
    public int batchTwo;

    [Space]

    //Different Pools based on each archetype -- ALL OF THESE CARDS MUST BE INSTANTIATED
    public List<CardData> secondaryRewardCards = new List<CardData>();
    public List<CardData> rewardPoolTrading = new List<CardData>();
    public List<CardData> rewardPoolVision = new List<CardData>();

    //
    bool confirmed;

    #region MainPlotReward Variables
    int batchOneNumberToSelect;
    List<CardData> batchOneSelectedCards = new List<CardData>();
    int batchTwoNumberToSelect;
    List<CardData> batchTwoSelectedCards = new List<CardData>();
    #endregion

    #region SecondaryPlotReward Variables
    CardData secondaryPlotSelectedCard;
    bool selectedStatsReward;
    bool selectedActionReward;
    Action<EventQueue> currentStatRewardAction;
    Action<EventQueue> currentActionRewardAction;
    #endregion

    private void Awake()
    {
        CreateSingleton();
    }

    private void Start()
    {
        InitEvent(batchOneContainers, SelectCard, EventTriggerType.PointerClick);
        InitEvent(batchOneContainers, PointerEnterContainer, EventTriggerType.PointerEnter);
        InitEvent(batchOneContainers, PointerExitContainer, EventTriggerType.PointerExit);

        InitEvent(batchTwoContainers, SelectCard, EventTriggerType.PointerClick);
        InitEvent(batchTwoContainers, PointerEnterContainer, EventTriggerType.PointerEnter);
        InitEvent(batchTwoContainers, PointerExitContainer, EventTriggerType.PointerExit);

        InitEvent(secondaryRewardCardContainer, SelectCardSecondary, EventTriggerType.PointerClick);
        InitEvent(secondaryRewardCardContainer, PointerEnterContainer, EventTriggerType.PointerEnter);
        InitEvent(secondaryRewardCardContainer, PointerExitContainer, EventTriggerType.PointerExit);
    }

    public void InitEvent(List<CardContainer> containers, Action<CardContainer> action, EventTriggerType triggerType)
    {
        for (int i = 0; i < containers.Count; i++)
        {
            int j = i;
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = triggerType;
            entry.callback.AddListener(data => action(containers[j]));
            containers[i].gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
        }
    }
    public void InitEvent(CardContainer container, Action<CardContainer> action, EventTriggerType triggerType)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = triggerType;
        entry.callback.AddListener(data => action(container));
        container.gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
    }

    void InitCards()
    {
        UtilityClass.InitCardList(secondaryRewardCards);
        UtilityClass.InitCardList(rewardPoolTrading);
        UtilityClass.InitCardList(rewardPoolVision);
    }
    void ResetCards()
    {
        UtilityClass.ResetCardList(secondaryRewardCards);
        UtilityClass.ResetCardList(rewardPoolTrading);
        UtilityClass.ResetCardList(rewardPoolVision);
    }

    public void ChooseMainPlotReward(EventQueue queue, PlotCard card)
    {
        queue.events.Add(ChooseMainPlotRewardRoutine(queue, card));
    }
    IEnumerator ChooseMainPlotRewardRoutine(EventQueue queue, PlotCard card)//Pick between nine cards 
    {
        InitCards();
        List<CardData> firstBatch = GetMainPlotRewardsFirstBatch(GetArchetypeList(card), 6);
        List<CardData> secondBatch = GetMainPlotRewardsSecondBatch(GetArchetypeList(card), 3, GetRandomRarity());

        confirmed = false;
        canvasGroup.blocksRaycasts = true;
        batchOneNumberToSelect = batchOne;
        batchTwoNumberToSelect = batchTwo;

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

        confirmButton.gameObject.SetActive(true);
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
            CardManager.Instance.CardAppearToDeck(batchOneSelectedCards[i], toDeckQueue, CardManager.Instance.plotAppearTransform.localPosition);
        }
        for (int i = 0; i < batchTwoSelectedCards.Count; i++)
        {
            CardManager.Instance.CardAppearToDeck(batchTwoSelectedCards[i], toDeckQueue, CardManager.Instance.plotAppearTransform.localPosition);
        }

        toDeckQueue.StartQueue();
        while(!toDeckQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }
        batchOneSelectedCards.Clear();
        batchTwoSelectedCards.Clear();

        ResetCards();
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

        string instruction = LocalizationManager.Instance.GetString(LocalizationManager.Instance.instructionsDictionary, GameManager.Instance.instructionsData.chooseCardInstruction);
        CardManager.Instance.cardPicker.Pick(pickerQueue, card.legendaryCardRewards ,pickedCard, 1, instruction);

        pickerQueue.StartQueue();
        while(!pickerQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        if(pickedCard.Count>0)
        {
            EventQueue toDeckQueue = new EventQueue();

            CardManager.Instance.CardAppearToDeck(pickedCard[0], toDeckQueue, CardManager.Instance.plotAppearTransform.localPosition);

            toDeckQueue.StartQueue();
            while(!toDeckQueue.resolved)
            {
                yield return new WaitForEndOfFrame();
            }
        }


        queue.UpdateQueue();
    }

    public void ChooseSecondaryPlotReward(EventQueue queue, PlotCard card)
    {
        queue.events.Add(ChooseSecondaryPlotRewardRoutine(queue, card));
    }
    IEnumerator ChooseSecondaryPlotRewardRoutine(EventQueue queue, PlotCard card)//Pick between one card / one action / one hero stats boost
    {
        InitCards();
        canvasGroup.blocksRaycasts = true;
        confirmed = false;

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

        confirmButton.gameObject.SetActive(true);

        //Afficher une carte random + un bouton pour les stats + un bouton pour une action random
        InitSecondaryRewardPlaceholder();
        InitStatButton();
        InitActionButton();

        while(!confirmed)
        {
            yield return new WaitForEndOfFrame();
        }

        ResetSecondaryContainers();

        #region FadeOutBackground
        Color transparent = new Color(0, 0, 0, 0);
        LeanTween.color(gameObject, transparent, fadeSpeed).setOnUpdate((Color col) => { backgroundPanel.color = col; }).setOnComplete(
            onEnd => { canvasGroup.blocksRaycasts = false; });


        yield return new WaitForSeconds(fadeSpeed);
        #endregion

        EventQueue rewardQueue = new EventQueue();

        //Do something according to what you pick
        if(secondaryPlotSelectedCard != null)
        {
            //send card to hand
            CardManager.Instance.CardAppearToDeck(secondaryPlotSelectedCard, rewardQueue, CardManager.Instance.plotAppearTransform.localPosition);
        }
        else if(selectedActionReward)//TODO
        {
            //play action 
            currentActionRewardAction(rewardQueue);

        }
        else if(selectedStatsReward)//TODO
        {
            //add stats
            currentStatRewardAction(rewardQueue);

        }


        rewardQueue.StartQueue();
        while(!rewardQueue.resolved)
        {
            yield return new WaitForEndOfFrame();
        }

        secondaryPlotSelectedCard = null;

        ResetCards();
        queue.UpdateQueue();
    }


    #region MainUtility
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

        //NOTE FOR NOW PICK RANDOM CARDS WITHIN THIS FILTERD LIST -- LATER ADD WEIGHT AND DROP RATES
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
        int r = Random.Range(3, 5);
        switch(r)
        {
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

            case Archetype.Vision:
                return rewardPoolVision;
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
                    placeholders[i].selfImage.color = Color.white;
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
            SelectHelper(batchOneSelectedCards, container, batchOneNumberToSelect);
        }
        else if(batchTwoContainers.Contains(container))
        {
            SelectHelper(batchTwoSelectedCards, container, batchTwoNumberToSelect);
        }
    }
    public void SelectHelper(List<CardData> list, CardContainer container, int numberToSelect)
    {
        if (list.Contains(container.data))
        {
            //Hide selected shader
            PickerHelper.DeselectContainerFeedback(container);

            //Remove from list
            list.Remove(container.data);

            return;
        }

        if (list.Count == numberToSelect)
        {
            return;
        }

        else if (!list.Contains(container.data))
        {
            //show selected shader
            PickerHelper.SelectContainerFeedback(container);

            //add to list
            list.Add(container.data);
        }
    }
    public void OnButtonClick()
    {
        confirmed = true;
        confirmButton.gameObject.SetActive(false);
    }
    #endregion

    #region Secondary Rewards Utility
    public void InitSecondaryRewardPlaceholder()
    {
        CardData reward =  GetSecondaryPlotCardReward(secondaryRewardCards);
        secondaryRewardCardContainer.gameObject.SetActive(true);
        secondaryRewardCardContainer.InitializeContainer(reward, true);
    }

    public void InitStatButton()
    {
        currentStatRewardAction = null;
        currentStatRewardAction = StatsUpgrade.GetRandomStatUpgrade(ref statsButtonDescription);

        statsRewardButton.gameObject.SetActive(true);
    }

    public void InitActionButton()
    {
        currentActionRewardAction = null;
        currentActionRewardAction = GetRandomAction();

        actionRewardButton.gameObject.SetActive(true);
    }

    //TODO Make the feedbacks using properties (get & sets)
    public void SelectStats()
    {
        selectedStatsReward = true;

        if(selectedActionReward)
        {
            selectedActionReward = false;


        }
        if(secondaryPlotSelectedCard != null)
        {
            //remove card
            secondaryPlotSelectedCard = null;

        }
    }
    public void SelectAction()
    {
        selectedActionReward = true;

        if(selectedStatsReward)
        {
            selectedStatsReward = false;


        }
        if(secondaryPlotSelectedCard != null)
        {
            //remove card
            secondaryPlotSelectedCard = null;

        }
    }
    public void SelectCardSecondary(CardContainer container)
    {
        if (selectedStatsReward)
            selectedStatsReward = false;

        if (selectedActionReward)
            selectedActionReward = false;

        secondaryPlotSelectedCard = container.data;
    }

    public Action<EventQueue> GetRandomAction()
    {
        Action<EventQueue> result = null;


        return result;
    }//TODO

    void ResetSecondaryContainers()
    {
        secondaryRewardCardContainer.ResetContainer(true);
        actionRewardButton.gameObject.SetActive(false);
        statsRewardButton.gameObject.SetActive(false);

        confirmButton.gameObject.SetActive(false);
    }
    #endregion

    public void PointerEnterContainer(CardContainer container)
    {
        PickerHelper.PointerEnter(container.gameObject);
    }
    public void PointerExitContainer(CardContainer container)
    {
        PickerHelper.PointerExit(container.gameObject);
    }
}
