using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardVisuals : MonoBehaviour
{
    [Header("CardBackgrounds")]
    public Image cardBackgound;
    [Space]
    public Sprite ideaBackground;
    public Sprite curseBackground;
    public Sprite plotBackground;
    public Sprite junkBackground;

    [Header("Card Frames")]
    public Image cardFrame;
    [Space]
    public Sprite characterFrame;
    public Sprite itemFrame;
    public Sprite placeFrame;

    [Header("Card Flags")]
    public Image cardFlag;
    [Space]
    public Sprite ideaFlag;
    public Sprite plotFlag;
    public Sprite curseFlag;

    [Header("Card Icons")]
    public Image cardIcon;
    [Space]
    public Sprite characterIcon;
    public Sprite itemIcon;
    public Sprite placeIcon;

    [Header("Character Elements")]
    public Image characterAffilitionFrame;
    public Image characterHealthFrame;
    public Image characterAttackFrame;

    [Space]
    public Sprite normalCharacterAffiliation;
    public Sprite curseCharacterAffiliation;
    public Sprite normalCharacterHealth;
    public Sprite curseCharacterHealth;
    public Sprite normalCharacterAttack;
    public Sprite curseCharacterAttack;

    [Header("Misc")]
    public Image cardManaFrame;
    public Image cardTimerFrame;
    public Image underManaFlag;
    [Space]
    public Sprite normalManaFrame;
    public Sprite curseManaFrame;

    [Header("Plot Elements")]
    public Image plotIcon;
    public Image plotUnderFlag;

    public Sprite mainUnderFlag;
    public Sprite secondaryUnderFlag;
}
