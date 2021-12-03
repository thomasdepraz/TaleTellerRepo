using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPicker : MonoBehaviour
{
    [Header("References")]
    public List<CardContainer> cardPlaceholders = new List<CardContainer>();
    public Image backgroundPanel;
    public Button confirmButton;
    public CanvasGroup canvasGroup;

    [Header("Data")]
    public float backgroundFadeSpeed;

    List<CardData> selectedCards;
    int numberToSelect;
    bool confirmed;

    public void Start()
    {
        for (int i = 0; i < cardPlaceholders.Count; i++)
        {
            int j = i;
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(data => { SelectCard(cardPlaceholders[j]); });
            cardPlaceholders[i].gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
        }
    }

    public void Pick(EventQueue queue, List<CardData> targetCards,List<CardData> pickedCards, int numberToPick, bool isInstantaneous)
    {
        queue.events.Add(PickRoutine(queue, targetCards, pickedCards, numberToPick, isInstantaneous)) ;
    }
    IEnumerator PickRoutine(EventQueue queue, List<CardData> targetCards, List<CardData> pickedCards, int numberToPick, bool isInstantaneous)
    {
        canvasGroup.blocksRaycasts = true;
        selectedCards = pickedCards;
        numberToSelect = numberToPick;
        confirmButton.interactable = false;

        if (!isInstantaneous) confirmed = false;
        else confirmed = true;

        Color transparent = new Color(0, 0, 0, 0);
        yield return null;

        //Fade in background
        bool fadeEnded = false;
        LeanTween.color(gameObject, Color.black, backgroundFadeSpeed).setOnUpdate((Color col) => { backgroundPanel.color = col; }).setOnComplete(onEnd => { fadeEnded = true; }) ;
        while(!fadeEnded)
        {
            yield return new WaitForEndOfFrame();
        }
        fadeEnded = false;

        InitializePlaceholders(targetCards);

        //TODO fade in cards



        //If necessary show button
        if (!isInstantaneous)
        {
            confirmButton.gameObject.SetActive(true);
        }
        //Wait for ending conditions
        while(selectedCards.Count != numberToPick || !confirmed)
        {
            yield return new WaitForEndOfFrame();
        }

        ResetPlaceHolders();

        LeanTween.color(gameObject, transparent, backgroundFadeSpeed).setOnUpdate((Color col) => { backgroundPanel.color = col; }).setOnComplete( 
            onEnd => { canvasGroup.blocksRaycasts = false;});

        queue.UpdateQueue();
    }


    void InitializePlaceholders(List<CardData> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            for (int j = 0; j < cardPlaceholders.Count; j++)
            {
                if(cardPlaceholders[i].data == null)
                {
                    cardPlaceholders[i].gameObject.SetActive(true);
                    cardPlaceholders[i].InitializeContainer(targets[i], true);
                    break;
                }
            }
        }
    }

    public void ResetPlaceHolders()
    {
        for (int i = 0; i < cardPlaceholders.Count; i++)
        {
            if(cardPlaceholders[i].data != null)
            {
                cardPlaceholders[i].ResetContainer(true);
            }
        }
    }

    public void SelectCard(CardContainer container)
    {
        if(selectedCards.Contains(container.data))
        {
            //Hide selected shader

            //Remove from list
            selectedCards.Remove(container.data);


            confirmButton.interactable = false;
            return;
        }
        
        if (selectedCards.Count == numberToSelect)
        {
            return;
        }
        else if(!selectedCards.Contains(container.data))
        {
            //show selected shader

            //add to list
            selectedCards.Add(container.data);

            if (selectedCards.Count == numberToSelect) confirmButton.interactable = true;
        }
    }

    public void OnButtonClick()
    {
        confirmed = true;
        confirmButton.gameObject.SetActive(false);
    }
}
