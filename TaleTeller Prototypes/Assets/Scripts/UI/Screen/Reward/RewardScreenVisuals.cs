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
    public Image backgroundImage;
    public Image bookImage;
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
        questText.text = currentScheme.schemeSteps[currentScheme.currentStep].chapterDescription;
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
}
