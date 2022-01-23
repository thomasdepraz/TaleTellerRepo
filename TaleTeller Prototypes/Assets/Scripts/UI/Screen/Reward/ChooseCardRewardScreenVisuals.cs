using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class ChooseCardRewardScreenVisuals
{
    public Canvas canvas;
    public RectTransform bristolTransform;
    public RectTransform layoutRoot;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI confirmButtonText;

    public ScreenButton confirmButton;
    public List<CardContainer> cardContainers = new List<CardContainer>();
    public List<PlaceholderCard> cardPlacholders = new List<PlaceholderCard>();

    public void UpdateCount(int currentValue, int maxValue)
    {
        countText.text = $"{currentValue} / {maxValue}";
    }
}
