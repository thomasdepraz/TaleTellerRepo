using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Card Visual Profile", menuName = "Visuals / Card Visuals Profile")]
public class CardVisualsData : ScriptableObject
{
    
    [Header("CardBackgrounds")]
    [Foldout("Sprites")] public Sprite ideaBackground;
    [Foldout("Sprites")] public Sprite curseBackground;
    [Foldout("Sprites")] public Sprite plotBackground;
    [Foldout("Sprites")] public Sprite junkBackground;

    [Header("Card Frames")]
    [Foldout("Sprites")] public Sprite characterFrameNeutral;
    [Foldout("Sprites")] public Sprite characterFrameGood;
    [Foldout("Sprites")] public Sprite characterFrameBad;
    [Foldout("Sprites")] public Sprite itemFrame;
    [Foldout("Sprites")] public Sprite placeFrame;
    [Foldout("Sprites")] public Sprite characterFrameMask;
    [Foldout("Sprites")] public Sprite characterBadFrameMask;
    [Foldout("Sprites")] public Sprite itemFrameMask;
    [Foldout("Sprites")] public Sprite placeFrameMask;
    [Space]
    [Foldout("Sprites")] public Sprite curseCharacterFrameNeutral;
    [Foldout("Sprites")] public Sprite curseCharacterFrameGood;
    [Foldout("Sprites")] public Sprite curseCharacterFrameBad;
    [Foldout("Sprites")] public Sprite curseItemFrame;
    [Foldout("Sprites")] public Sprite cursePlaceFrame;
    [Foldout("Sprites")] public Sprite curseCharacterFrameMask;
    [Foldout("Sprites")] public Sprite curseCharacterBadFrameMask;
    [Foldout("Sprites")] public Sprite curseItemFrameMask;
    [Foldout("Sprites")] public Sprite cursePlaceFrameMask;

    [Header("Card Flags")]
    [Foldout("Sprites")] public Sprite ideaFlag;
    [Foldout("Sprites")] public Sprite plotFlag;
    [Foldout("Sprites")] public Sprite curseFlag;

    [Header("Card Icons")]
    [Foldout("Sprites")] public Sprite characterIcon;
    [Foldout("Sprites")] public Sprite characterBadIcon;
    [Foldout("Sprites")] public Sprite itemIcon;
    [Foldout("Sprites")] public Sprite placeIcon;

    [Header("Card Rarity")]
    [Foldout("Sprites")] public Sprite commonRarity;
    [Foldout("Sprites")] public Sprite uncommonRarity;
    [Foldout("Sprites")] public Sprite rareRarity;
    [Foldout("Sprites")] public Sprite epicRarity;
    [Foldout("Sprites")] public Sprite legendaryRarity;

    [Header("Character Elements")]
    [Foldout("Sprites")] public Sprite normalCharacterHealth;
    [Foldout("Sprites")] public Sprite curseCharacterHealth;
    [Foldout("Sprites")] public Sprite normalCharacterAttack;
    [Foldout("Sprites")] public Sprite curseCharacterAttack;
    [Foldout("Sprites")] public Sprite normalHourglass;
    [Foldout("Sprites")] public Sprite curseHourglass;

    [Header("Misc")]
    [Foldout("Sprites")] public Sprite normalManaFrame;
    [Foldout("Sprites")] public Sprite curseManaFrame;
    [Foldout("Sprites")] public Sprite timer;
    [Foldout("Sprites")] public Sprite curseTimer;


    [Header("Plot Elements")]
    [Foldout("Sprites")] public Sprite mainUnderFlag;
    [Foldout("Sprites")] public Sprite secondaryUnderFlag;

    [Header("Range Icons")]
    [Foldout("Sprites")] public Sprite rangeIconSelf;
    [Foldout("Sprites")] public Sprite rangeIconRight;
    [Foldout("Sprites")] public Sprite rangeIconLeft;
    [Foldout("Sprites")] public Sprite rangeIconAllRight;
    [Foldout("Sprites")] public Sprite rangeIconAllLeft;
    [Foldout("Sprites")] public Sprite rangeIconHero;
    [Foldout("Sprites")] public Sprite rangeIconRightAndLeft;
    [Foldout("Sprites")] public Sprite rangeIconAll;
    [Foldout("Sprites")] public Sprite rangeIconPlot;

    [Header("Tweening data")]
    public float draggedScale;
    public float hoveredScale;
    public Color highlightColor;
}
