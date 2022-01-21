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
    public List<ScreenButton> heroRewardsButton;
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
}
