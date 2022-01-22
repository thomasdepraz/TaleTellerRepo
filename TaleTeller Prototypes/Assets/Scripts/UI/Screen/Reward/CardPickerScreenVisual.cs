using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class CardPickerScreenVisual
{
    public Canvas canvas;
    public RectTransform bristolTransform;
    public RectTransform panelTransform;
    public RectTransform scrollviewContentTransform;
    public GameObject scrollBar;
    public GameObject cardRowPrefab;
    public List<PlaceholderCard> cardPlaceholders;
    public ScreenButton confirmButton;

    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI confirmButtonText;



    public void LoadData(List<CardData> datas)
    { 
        for (int i = 0; i < datas.Count; i++)
        {
            if (i > cardPlaceholders.Count) InstantiateMoreCards();
            cardPlaceholders[i].container.InitializeContainer(datas[i], true);
        }

        if (datas.Count > 5) scrollBar.SetActive(false);
        else scrollBar.SetActive(true);

        for (int i = 0; i < cardPlaceholders.Count; i++)
        {
            cardPlaceholders[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < datas.Count; i++)
        {
            cardPlaceholders[i].gameObject.SetActive(true);
        }
        
    }

    public void ResetPlaceholders()
    {
        for (int i = 0; i < cardPlaceholders.Count; i++)
        {
            cardPlaceholders[i].container.ResetContainer(true);
        }
    }

    public void InstantiateMoreCards()
    {
        GameObject newRow = GameObject.Instantiate(cardRowPrefab, scrollviewContentTransform);
        newRow.transform.SetAsLastSibling();
        for (int i = 0; i < newRow.transform.childCount; i++)
        {
           cardPlaceholders.Add(newRow.transform.GetChild(i).GetComponent<PlaceholderCard>());
        }
    }

    public void UpdateCount(int current, int max)
    {
        countText.text = $"{current} / {max}";
    }
}
