using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardScreenVisuals : MonoBehaviour
{
    public Image backgroundImage;
    public Image bookImage;
    public RectTransform completeTextTransform;
    public Image illustration;
    public RectTransform questTextTransform;
    public RectTransform chooseInstructionTransform;
    public RectTransform upgradeInstructionTransform;
    public List<GameObject> heroRewardsButton;
    public RectTransform layoutRoot;
    public RectTransform addButtonTransform;
    public RectTransform confirmButtonTransform;
    public TextMeshProUGUI completeText;
    public TextMeshProUGUI questText;
    public TextMeshProUGUI chooseInstruction;
    public TextMeshProUGUI upgradeInstruction;
    public TextMeshProUGUI addButtonText;
    public TextMeshProUGUI removeButtonText;
    public TextMeshProUGUI confirmButtonText;
}
