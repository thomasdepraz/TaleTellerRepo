using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCardRewardScreen : GameScreen
{

    ChooseCardRewardScreenVisuals visuals;
    public List<PlaceholderCard> pickedCards;
    int numberToPick;
    public ChooseCardRewardScreen(int numberToPick, List<CardData> cards)
    {
        visuals = ScreenManager.Instance.chooseCardRewardScreenVisuals;
        pickedCards = new List<PlaceholderCard>();
        this.numberToPick = numberToPick;

        visuals.confirmButton.interactable = CheckValid();

        for (int i = 0; i < visuals.cardPlacholders.Count; i++)
        {
            visuals.cardPlacholders[i].gameObject.SetActive(false);
        }


        for (int i = 0; i < cards.Count; i++)
        {
            int j = i;
            visuals.cardPlacholders[i].container.InitializeContainer(cards[i], true);
            visuals.cardPlacholders[j].onClick = ()=> SelectCard(visuals.cardPlacholders[j]);
            visuals.cardPlacholders[i].gameObject.SetActive(true);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(visuals.layoutRoot);
        visuals.confirmButton.onClick = Confirm;
    }

    public override void Close(Action onComplete)
    {
        visuals.canvas.gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    public override void InitializeContent(Action onComplete)
    {

    }

    public override void Open(Action onComplete)
    {
        open = true;
        visuals.canvas.gameObject.SetActive(true);
        onComplete?.Invoke();
    }

    public void SelectCard(PlaceholderCard card)
    {
        if(pickedCards.Contains(card))
        {
            pickedCards.Remove(card);
            card.selected = false;
        }
        else
        {
            pickedCards.Add(card);
        }

        if(pickedCards.Count > numberToPick)
        {
            pickedCards[0].selected = false;
            pickedCards.RemoveAt(0);
        }

        visuals.confirmButton.interactable = CheckValid();
    }

    public bool CheckValid()
    {
        visuals.UpdateCount(pickedCards.Count, numberToPick);
        return pickedCards.Count == numberToPick;
    }

    public void Confirm()
    {
        open = false;
    }
}
