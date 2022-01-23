using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RewardScreenVisuals
{
    public Canvas canvas;
    public CanvasScaler canvasScaler;
    public Image panel;
    public RectTransform bookTransform;
    public RectTransform completeTextTransform;
    public Image illustration;
    public RectTransform questTextTransform;
    public RectTransform chooseInstructionTransform;
    public RectTransform upgradeInstructionTransform;
    public List<ScreenButton> heroRewardsButton = new List<ScreenButton>();
    public RectTransform layoutRoot;
    public ScreenButton addButton;
    public ScreenButton removeButton;
    public ScreenButton confirmButton;
    public TextMeshProUGUI completeText;
    public TextMeshProUGUI questText;
    public TextMeshProUGUI chooseInstruction;
    public TextMeshProUGUI upgradeInstruction;
    public TextMeshProUGUI addButtonText;
    public TextMeshProUGUI removeButtonText;
    public TextMeshProUGUI confirmButtonText;

    public void InitText(MainPlotScheme currentScheme)
    {
        completeText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$QUEST_ENDING");
        chooseInstruction.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$CHOOSE_INSTRUCTION");
        upgradeInstruction.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$UPGRADE_INSTRUCTION");
        confirmButton.SetText(LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$CONFIRM"));
        if (currentScheme.currentStep < currentScheme.schemeSteps.Count) questText.text = currentScheme.schemeSteps[currentScheme.currentStep].chapterDescription;
        else questText.text = currentScheme.plotEndDescription;
    }

    public void InitButton(Action<Reward, ScreenButton> selectCard, Action<Reward, ScreenButton> selectHero, AddCardReward addCardReward, RemoveCardReward removeCardReward ,List<Reward>heroRewards)
    {
        addButton.onClick = () => selectCard(addCardReward, addButton);
        addButton.SetText(addCardReward.GetString());
        addButton.selected = false;

        removeButton.onClick = () => selectCard(removeCardReward, removeButton);
        removeButton.SetText(removeCardReward.GetString());
        removeButton.selected = false;

        for (int i = 0; i < heroRewards.Count; i++)
        {
            int index = i;

            heroRewardsButton[index].selected = false;
            heroRewardsButton[index].gameObject.SetActive(true);
            heroRewardsButton[index].onClick = ()=> selectHero(heroRewards[index], heroRewardsButton[index]);
            heroRewardsButton[index].SetText(heroRewards[index].GetString());
        }
    }

    public void OpenTween(Action onComplete)
    {
        bookTransform.localPosition = new Vector3(0, -canvasScaler.referenceResolution.y, 0);
        canvas.gameObject.SetActive(true);
        Color panelColor = panel.color;
        panel.color = Color.clear;

        LeanTween.moveLocal(bookTransform.gameObject, Vector3.zero + Vector3.up * 10, 0.5f).setEaseInQuint().setOnComplete(() =>
        {
            LeanTween.moveLocal(bookTransform.gameObject, Vector3.zero, 0.3f).setEaseOutQuint();

        });

        LeanTween.color(panel.gameObject, panelColor, 0.5f).setDelay(0.5f).setEaseOutQuint().setOnUpdate((Color col) =>
        {
            panel.color = col;
        }).setOnComplete(()=> 
        {
            onComplete?.Invoke();
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
        });
    }

    public void CloseTween(Action onComplete)
    {
        LeanTween.moveLocal(bookTransform.gameObject, Vector3.zero + Vector3.down * 10, 0.3f).setEaseInQuint().setOnComplete(() =>
        {
            LeanTween.moveLocal(bookTransform.gameObject, new Vector3(0, canvasScaler.referenceResolution.y, 0), 0.5f).setEaseInQuint();
        });


        Color panelColor = panel.color;
        LeanTween.value(panel.gameObject, panelColor, Color.clear, 0.5f).setDelay(0.5f).setEaseOutQuint().setOnUpdate((Color col) =>
        {
            panel.color = col;
        }).setOnComplete(() =>
        {
            canvas.gameObject.SetActive(false);
            panel.color = panelColor;
            bookTransform.localPosition = Vector3.zero;
            onComplete?.Invoke();

        });
    }
}
