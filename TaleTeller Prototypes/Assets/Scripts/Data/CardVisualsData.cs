using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Visual Profile", menuName = "Visuals / Card Visuals Profile")]
public class CardVisualsData : ScriptableObject
{
    [Header("CardBackgrounds")]
    public Sprite ideaBackground;
    public Sprite curseBackground;
    public Sprite plotBackground;
    public Sprite junkBackground;

    [Header("Card Frames")]
    public Sprite characterFrameNeutral;
    public Sprite characterFrameGood;
    public Sprite characterFrameBad;
    public Sprite itemFrame;
    public Sprite placeFrame;
    public Sprite characterFrameMask;
    public Sprite itemFrameMask;
    public Sprite placeFrameMask;
    [Space]
    public Sprite curseCharacterFrameNeutral;
    public Sprite curseCharacterFrameGood;
    public Sprite curseCharacterFrameBad;
    public Sprite curseItemFrame;
    public Sprite cursePlaceFrame;
    public Sprite curseCharacterFrameMask;
    public Sprite curseItemFrameMask;
    public Sprite cursePlaceFrameMask;

    [Header("Card Flags")]
    public Sprite ideaFlag;
    public Sprite plotFlag;
    public Sprite curseFlag;

    [Header("Card Icons")]
    public Sprite characterIcon;
    public Sprite itemIcon;
    public Sprite placeIcon;

    [Header("Card Rarity")]
    public Sprite commonRarity;
    public Sprite uncommonRarity;
    public Sprite rareRarity;
    public Sprite epicRarity;
    public Sprite legendaryRarity;

    [Header("Character Elements")]
    public Sprite normalCharacterHealth;
    public Sprite curseCharacterHealth;
    public Sprite normalCharacterAttack;
    public Sprite curseCharacterAttack;

    [Header("Misc")]
    public Sprite normalManaFrame;
    public Sprite curseManaFrame;

    [Header("Plot Elements")]
    public Sprite mainUnderFlag;
    public Sprite secondaryUnderFlag;
}
