using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    int numberToPick;
    bool skippable;
    public CardPickerScreen(PickScreenMode mode, int numberToPick, List<CardData> targetCards, bool skippable)
    {
        if (targetCards.Count < numberToPick) numberToPick = targetCards.Count;

        visuals = ScreenManager.Instance.pickerScreenVisual;
        pickedCards = new List<PlaceholderCard>();
        this.numberToPick = numberToPick;
        this.skippable = skippable;

        visuals.LoadData(targetCards);
        for (int i = 0; i < visuals.cardPlaceholders.Count; i++)
        {
            int j = i;
            visuals.cardPlaceholders[j].onClick = () => SelectCard(visuals.cardPlaceholders[j]);
            visuals.cardPlaceholders[j].selected = false;
        }

        visuals.confirmButton.onClick = Confirm;
        visuals.confirmButton.interactable = CheckValid();

        switch (mode)
        {
            case PickScreenMode.ADD:
                visuals.instructionText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$ADD");
                break;
            case PickScreenMode.WITHDRAW:
                visuals.instructionText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$WITHDRAW");
                break;
            case PickScreenMode.CHOICE:
                visuals.instructionText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$CHOICE");
                break;
            case PickScreenMode.REPLACE:
                visuals.instructionText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$REPLACE");
                break;
            default:
                break;
        }
        string text =
        visuals.instructionText.text = visuals.instructionText.text.Replace("$value$", $"{numberToPick}");
    }

    public override void Close(Action onComplete)
    {
        visuals.canvas.gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    public override void InitializeContent(Action onComplete)
    {
        throw new NotImplementedException();
    }

    public override void Open(Action onComplete)
    {
        open = true;
        visuals.canvas.gameObject.SetActive(true);
        onComplete?.Invoke();
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
        }

        if (pickedCards.Count > numberToPick)
        {
            pickedCards[0].selected = false;
            pickedCards.RemoveAt(0);
        }

        visuals.confirmButton.interactable = CheckValid();
    }

    ~CardPickerScreen()
    {
        visuals.ResetPlaceholders();
    }
}
