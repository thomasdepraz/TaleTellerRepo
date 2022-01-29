using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CardPickerScreenVisual : GameScreenVisuals
{
    public RectTransform scrollviewContentTransform;
    public Scrollbar scrollBar;
    public GameObject cardRowPrefab;
    public List<PlaceholderCard> cardPlaceholders;
    public List<GameObject> cardRows;
    public List<HorizontalLayoutGroup> cardRowLayouts;

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
            {
                cardRows[i].gameObject.SetActive(false);
            }
            else
            {
                cardRows[i].gameObject.SetActive(true);
            }
        }
    }

    void DisableLayouts()
    {
        for (int i = 0; i < cardRows.Count; i++)
        {
            if (!cardRows[i].transform.GetChild(0).gameObject.activeSelf)
            {
                cardRowLayouts[i].enabled = true;
            }
            else
            {
                cardRowLayouts[i].enabled = false;
            }
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
        cardRowLayouts.Add(newRow.GetComponent<HorizontalLayoutGroup>());
        for (int i = 0; i < newRow.transform.childCount; i++)
        {
           cardPlaceholders.Add(newRow.transform.GetChild(i).GetComponent<PlaceholderCard>());
        }
    }

    public void UpdateCount(int current, int max)
    {
        countText.text = $"{current} / {max}";
    }

    public override void Initialize(GameScreen gameScreen)
    {
        CardPickerScreen screen = gameScreen as CardPickerScreen;
        LoadData(screen.targetCards);

        for (int i = 0; i < cardPlaceholders.Count; i++)
        {
            int j = i;
            cardPlaceholders[j].onClick = () => screen.SelectCard(cardPlaceholders[j]);
            cardPlaceholders[j].selected = false;
        }

        switch (screen.screenMode)
        {
            case PickScreenMode.ADD:
                instructionText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$ADD");
                break;
            case PickScreenMode.WITHDRAW:
                instructionText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$WITHDRAW");
                break;
            case PickScreenMode.CHOICE:
                instructionText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$CHOICE");
                break;
            case PickScreenMode.REPLACE:
                instructionText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$REPLACE");
                break;
            default:
                break;
        }
        string text = instructionText.text.Replace("$value$", $"{screen.numberToPick}");
        instructionText.text = text;

        confirmButton.onClick = screen.Confirm;
        confirmButton.interactable = screen.CheckValid();
    }

    public override void Open(Action onComplete)
    {
        Color panelColor = backgroundPanel.color;
        backgroundPanel.color = Color.clear;

        contentTransform.localPosition = new Vector3(-canvasScaler.referenceResolution.x, 0, 0);
        canvas.gameObject.SetActive(true);
        LeanTween.moveLocal(contentTransform.gameObject, Vector3.zero + Vector3.right * 10, 1f).setEaseInQuint().setOnComplete(() =>
        {
            DisableLayouts();
            LeanTween.moveLocal(contentTransform.gameObject, Vector3.zero, 0.3f).setEaseOutQuint();
        });

        LeanTween.color(backgroundPanel.gameObject, panelColor, 0.5f).setDelay(1f).setEaseOutQuint().setOnUpdate((Color col) =>
        {
            backgroundPanel.color = col;
        }).setOnComplete(onComplete);
    }

    public override void Close(Action onComplete)
    {
        LeanTween.moveLocal(contentTransform.gameObject, Vector3.zero - Vector3.right * 10, 0.3f).setEaseInQuint().setOnComplete(() =>
        {
            LeanTween.moveLocal(contentTransform.gameObject, new Vector3(canvasScaler.referenceResolution.x, 0, 0), 1f).setEaseInQuint();
        });

        Color panelColor = backgroundPanel.color;
        LeanTween.value(backgroundPanel.gameObject, panelColor, Color.clear, 0.5f).setDelay(1f).setEaseInQuint().setOnUpdate((Color col) =>
        {
            backgroundPanel.color = col;
        }).setOnComplete(() =>
        {
            canvas.gameObject.SetActive(false);
            backgroundPanel.color = panelColor;
            onComplete?.Invoke();

        });

        backgroundPanel.color = panelColor;
    }

    public override void Reset()
    {
        ResetPlaceholders();
    }
}
