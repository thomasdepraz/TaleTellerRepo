using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PickScreenMode
{
    ADD, 
    WITHDRAW,
    CHOICE,
    REPLACE
}
public class CardPickerScreen : GameScreen
{

    CardPickerScreenVisual visuals;
    public List<PlaceholderCard> pickedCards;
    public List<CardData> targetCards;
    public PickScreenMode screenMode;
    public int numberToPick;
    bool skippable;

    public CardPickerScreen(PickScreenMode mode, int numberToPick, List<CardData> targetCards, bool skippable)
    {
        ScreenManager.Instance.currentScreen = this;

        if (targetCards.Count < numberToPick) numberToPick = targetCards.Count;

        visuals = ScreenManager.Instance.pickerScreenVisual;
        pickedCards = new List<PlaceholderCard>();
        this.numberToPick = numberToPick;
        this.skippable = skippable;
        this.targetCards = targetCards;
        screenMode = mode;

        visuals.Initialize(this);
    }

    public override void Open(Action onComplete)
    {
        open = true;
        visuals.Open((onComplete));
        LeanTween.value(1, 1, 0.1f).setOnUpdate((float value) => visuals.scrollBar.value = value);
    }
    public override void Close(Action onComplete)
    {
        visuals.Close(onComplete);
    }

    public bool CheckValid()
    {
        visuals.UpdateCount(pickedCards.Count, numberToPick);

        if (skippable) return true;
        return numberToPick == pickedCards.Count;
    }

    public void Confirm()
    {
        open = false;
    }

    public void SelectCard(PlaceholderCard card)
    {
        if (pickedCards.Contains(card))
        {
            pickedCards.Remove(card);
            card.selected = false;
        }
        else
        {
            pickedCards.Add(card);
            Debug.Log(card.container.data);
        }

        if (pickedCards.Count > numberToPick)
        {
            pickedCards[0].selected = false;
            pickedCards.RemoveAt(0);
        }

        visuals.confirmButton.interactable = CheckValid();
    }
}
