using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RewardScreenVisuals : GameScreenVisuals
{
    public RectTransform completeTextTransform;
    public Image illustration;
    public RectTransform questTextTransform;
    public RectTransform chooseInstructionTransform;
    public RectTransform upgradeInstructionTransform;
    public List<ScreenButton> heroRewardsButton = new List<ScreenButton>();
    public RectTransform layoutRoot;
    public ScreenButton addButton;
    public ScreenButton removeButton;
    public TextMeshProUGUI completeText;
    public TextMeshProUGUI questText;
    public TextMeshProUGUI chooseInstruction;
    public TextMeshProUGUI upgradeInstruction;
    public TextMeshProUGUI addButtonText;
    public TextMeshProUGUI removeButtonText;
    public TextMeshProUGUI confirmButtonText;

    void InitText(MainPlotScheme currentScheme)
    {
        completeText.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$QUEST_ENDING");
        chooseInstruction.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$CHOOSE_INSTRUCTION");
        upgradeInstruction.text = LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$UPGRADE_INSTRUCTION");
        confirmButton.SetText(LocalizationManager.Instance.GetString(LocalizationManager.Instance.screenDictionary, "$CONFIRM"));
        if (currentScheme.currentStep < currentScheme.schemeSteps.Count) questText.text = currentScheme.schemeSteps[currentScheme.currentStep].chapterDescription;
        else questText.text = currentScheme.plotEndDescription;
    }

    void InitButton(Action<Reward, ScreenButton> selectCard, Action<Reward, ScreenButton> selectHero, AddCardReward addCardReward, RemoveCardReward removeCardReward ,List<Reward>heroRewards)
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

    public override void Initialize(GameScreen gameScreen)
    {
        RewardScreen screen = gameScreen as RewardScreen;

        illustration = null;//TODO

        for (int i = 0; i < heroRewardsButton.Count; i++)
        {
            heroRewardsButton[i].gameObject.SetActive(false);
        }
        InitText(screen.currentScheme);
        InitButton(screen.selectCard, screen.selectHero, screen.addCardReward, screen.removeCardReward, screen.heroRewards);

        if (CardManager.Instance.cardDeck.cachedDeck.Count <= 10 || screen.addRewardData != null) removeButton.interactable = false;
        else removeButton.interactable = true;

        confirmButton.onClick = screen.Confirm;
        confirmButton.interactable = screen.CheckValid();

        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
    }

    public override void Open(Action onComplete)
    {
        contentTransform.localPosition = new Vector3(0, -canvasScaler.referenceResolution.y, 0);
        canvas.gameObject.SetActive(true);
        Color panelColor = backgroundPanel.color;
        backgroundPanel.color = Color.clear;

        LeanTween.moveLocal(contentTransform.gameObject, Vector3.zero + Vector3.up * 10, 0.5f).setEaseInQuint().setOnComplete(() =>
        {
            LeanTween.moveLocal(contentTransform.gameObject, Vector3.zero, 0.3f).setEaseOutQuint();

        });

        LeanTween.color(backgroundPanel.gameObject, panelColor, 0.5f).setDelay(0.5f).setEaseOutQuint().setOnUpdate((Color col) =>
        {
            backgroundPanel.color = col;
        }).setOnComplete(() =>
        {
            onComplete?.Invoke();
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
        });
    }

    public override void Close(Action onComplete)
    {
        LeanTween.moveLocal(contentTransform.gameObject, Vector3.zero + Vector3.down * 10, 0.3f).setEaseInQuint().setOnComplete(() =>
        {
            LeanTween.moveLocal(contentTransform.gameObject, new Vector3(0, canvasScaler.referenceResolution.y, 0), 0.5f).setEaseInQuint();
        });


        Color panelColor = backgroundPanel.color;
        LeanTween.value(backgroundPanel.gameObject, panelColor, Color.clear, 0.5f).setDelay(0.5f).setEaseOutQuint().setOnUpdate((Color col) =>
        {
            backgroundPanel.color = col;
        }).setOnComplete(() =>
        {
            canvas.gameObject.SetActive(false);
            backgroundPanel.color = panelColor;
            contentTransform.localPosition = Vector3.zero;
            onComplete?.Invoke();

        });
    }

    public override void Reset()
    {

    }
}
