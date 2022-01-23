using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CardPickerScreenVisual
{
    public Canvas canvas;
    public CanvasScaler canvasScaler;
    public RectTransform bristolTransform;
    public Image panel;
    public RectTransform scrollviewContentTransform;
    public Scrollbar scrollBar;
    public GameObject cardRowPrefab;
    public List<PlaceholderCard> cardPlaceholders;
    public List<GameObject> cardRows;
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

        if (datas.Count <= 5) scrollBar.gameObject.SetActive(false);
        else scrollBar.gameObject.SetActive(true);

        for (int i = 0; i < cardPlaceholders.Count; i++)
        {
            cardPlaceholders[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < datas.Count; i++)
        {
            cardPlaceholders[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < cardRows.Count; i++)
        {
            if (!cardRows[i].transform.GetChild(0).gameObject.activeSelf)
                cardRows[i].gameObject.SetActive(false);
            else
                cardRows[i].gameObject.SetActive(true);
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
        cardRows.Add(newRow);
        for (int i = 0; i < newRow.transform.childCount; i++)
        {
           cardPlaceholders.Add(newRow.transform.GetChild(i).GetComponent<PlaceholderCard>());
        }
    }

    public void UpdateCount(int current, int max)
    {
        countText.text = $"{current} / {max}";
    }

    public void OpenTween(Action onComplete)
    {
        Color panelColor = panel.color;
        panel.color = Color.clear;

        bristolTransform.localPosition = new Vector3(-canvasScaler.referenceResolution.x, 0, 0);
        canvas.gameObject.SetActive(true);
        LeanTween.moveLocal(bristolTransform.gameObject, Vector3.zero + Vector3.right * 10, 1f).setEaseInQuint().setOnComplete(() =>
        {
            LeanTween.moveLocal(bristolTransform.gameObject, Vector3.zero, 0.3f).setEaseOutQuint();
        });

        LeanTween.color(panel.gameObject, panelColor, 0.5f).setDelay(1f).setEaseOutQuint().setOnUpdate((Color col) => 
        {
            panel.color = col;
        }).setOnComplete(onComplete);
    }

    public void CloseTween(Action onComplete)
    {

        LeanTween.moveLocal(bristolTransform.gameObject, Vector3.zero - Vector3.right * 10, 0.3f).setEaseInQuint().setOnComplete(() =>
        {
            LeanTween.moveLocal(bristolTransform.gameObject, new Vector3(canvasScaler.referenceResolution.x, 0,0), 1f).setEaseInQuint();
        });

        Color panelColor = panel.color;
        LeanTween.value(panel.gameObject, panelColor, Color.clear, 0.5f).setDelay(1f).setEaseInQuint().setOnUpdate((Color col)=> 
        {
            panel.color = col;
        }).setOnComplete(()=> 
        {
            canvas.gameObject.SetActive(false);
            panel.color = panelColor;
            onComplete?.Invoke();
        
        });

        panel.color = panelColor;
    }
}
