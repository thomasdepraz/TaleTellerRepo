using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisuals : MonoBehaviour
{
    #region Variables
    public CardVisualsData profile;

    
    [Header("Images")]
    public Image cardBackgoundFitter;
    public Image cardBackgound;
    public Image cardHighlight;
    public Image cardFrame;
    public Image cardFrameMask;
    public Image cardIllustration;
    public Image cardFlag;
    public Image cardIcon;
    public Image cardRarity;
    public Image characterHealthFrame;
    public Image characterAttackFrame;
    public Image cardManaFrame;
    public Image cardTimerFrame;
    public Image underManaFlag;
    public Image plotIcon;
    public Image plotUnderFlag;

    [Header("Texts")]
    public TextMeshProUGUI manaCostText;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardDescriptionText;
    public TextMeshProUGUI cardFlagText;
    [Space]
    public TextMeshProUGUI characterAttackText;
    public TextMeshProUGUI characterHealthText;
    public TextMeshProUGUI timerText;
    #endregion

    public void InitializeVisuals(CardData data)
    {
        //Init basics
        #region Basics
        cardIllustration.sprite = data.cardGraph;

        Type dataType = data.GetType();
        if(dataType == typeof(IdeaCard))
        {
            cardBackgoundFitter.sprite = profile.ideaBackground;
            cardBackgound.sprite = profile.ideaBackground;

            cardFlag.sprite = profile.ideaFlag;

            cardManaFrame.sprite = profile.normalManaFrame;

            plotIcon.gameObject.SetActive(false);
            plotUnderFlag.gameObject.SetActive(false);
            cardTimerFrame.gameObject.SetActive(false);

        }
        else if (dataType == typeof(PlotCard))
        {
            cardBackgoundFitter.sprite = profile.plotBackground;
            cardBackgound.sprite = profile.plotBackground;

            cardFlag.sprite = profile.plotFlag;
            cardManaFrame.sprite = profile.normalManaFrame;

            plotIcon.gameObject.SetActive(true);
            plotUnderFlag.gameObject.SetActive(true);
            cardTimerFrame.gameObject.SetActive(true);


            PlotCard card = data as PlotCard;
            if (card.isMainPlot) plotUnderFlag.sprite = profile.mainUnderFlag;
            else plotUnderFlag.sprite = profile.secondaryUnderFlag;

            UpdatePlotElements(card);

        }
        else if(dataType == typeof(DarkIdeaCard))
        {
            cardBackgoundFitter.sprite = profile.curseBackground;
            cardBackgound.sprite = profile.curseBackground;

            cardFlag.sprite = profile.curseFlag;
            cardManaFrame.sprite = profile.curseManaFrame;

            plotIcon.gameObject.SetActive(false);
            plotUnderFlag.gameObject.SetActive(false);
            cardTimerFrame.gameObject.SetActive(false);

        }
        else if(dataType == typeof(JunkCard))
        {
            cardBackgoundFitter.sprite = profile.junkBackground;
            cardBackgound.sprite = profile.junkBackground;

            cardFlag.sprite = profile.plotFlag;
            cardManaFrame.sprite = profile.normalManaFrame;

            plotIcon.gameObject.SetActive(false);
            plotUnderFlag.gameObject.SetActive(false);
            cardTimerFrame.gameObject.SetActive(false);
        }

        if(data.rarity != CardRarity.None)
        {
            cardRarity.gameObject.SetActive(true);
            switch (data.rarity)
            {
                case CardRarity.Common:
                    cardRarity.sprite = profile.commonRarity;
                    break;
                case CardRarity.Uncommon:
                    cardRarity.sprite = profile.uncommonRarity;
                    break;
                case CardRarity.Rare:
                    cardRarity.sprite = profile.rareRarity;
                    break;
                case CardRarity.Epic:
                    cardRarity.sprite = profile.epicRarity;
                    break;
                case CardRarity.Legendary:
                    cardRarity.sprite = profile.legendaryRarity;
                    break;
            }
        }
        else
        {
            cardRarity.gameObject.SetActive(false);
        }

        #endregion

        //Init type based values
        #region TypeBasedValue
        if (data.cardType!=null)
        {
            Type cardType = data.cardType.GetType();
            if(cardType == typeof(CharacterType))
            {
                cardIcon.sprite = profile.characterIcon;
                CharacterType chara = data.cardType as CharacterType;
                if(chara.behaviour == CharacterBehaviour.Agressive)
                {
                    if(data.GetType() == typeof(DarkIdeaCard))
                    {
                        cardFrame.sprite = profile.curseCharacterFrameBad;
                        cardFrameMask.sprite = profile.curseCharacterFrameMask;
                    }
                    else
                    {
                        cardFrame.sprite = profile.characterFrameBad;
                        cardFrameMask.sprite = profile.characterFrameMask;
                    }

                }
                else if(chara.behaviour == CharacterBehaviour.Peaceful)
                {
                    if (data.GetType() == typeof(DarkIdeaCard))
                    {
                        cardFrame.sprite = profile.curseCharacterFrameGood;
                        cardFrameMask.sprite = profile.curseCharacterFrameMask;
                    }
                    else
                    {
                        cardFrame.sprite = profile.characterFrameGood;
                        cardFrameMask.sprite = profile.characterFrameMask;
                    }
                }
                else
                {
                    if (data.GetType() == typeof(DarkIdeaCard))
                    {
                        cardFrame.sprite = profile.curseCharacterFrameNeutral;
                        cardFrameMask.sprite = profile.curseCharacterFrameMask;
                    }
                    else
                    {
                        cardFrame.sprite = profile.characterFrameNeutral;
                        cardFrameMask.sprite = profile.characterFrameMask;
                    }
                }

                characterAttackFrame.gameObject.SetActive(true);
                characterHealthFrame.gameObject.SetActive(true);

                if (data.GetType() != typeof(JunkCard))
                    cardTimerFrame.gameObject.SetActive(true);
                else
                    cardTimerFrame.gameObject.SetActive(false);

                if (dataType == typeof(DarkIdeaCard))
                {
                    characterAttackFrame.sprite = profile.curseCharacterAttack;
                    characterHealthFrame.sprite = profile.curseCharacterHealth;
                }
                else
                {
                    characterAttackFrame.sprite = profile.normalCharacterAttack;
                    characterHealthFrame.sprite = profile.normalCharacterHealth;
                }

                CharacterType character = data.cardType as CharacterType;
                UpdateCharacterElements(character);
            }
            else
            {
                characterAttackFrame.gameObject.SetActive(false);
                characterHealthFrame.gameObject.SetActive(false);
            }

            if(cardType == typeof(ObjectType))
            {
                cardIcon.sprite = profile.itemIcon;

                if(data.GetType() == typeof(DarkIdeaCard))
                {
                    cardFrame.sprite = profile.curseItemFrame;
                    cardFrameMask.sprite = profile.curseItemFrameMask;
                }
                else
                {
                    cardFrame.sprite = profile.itemFrame;
                    cardFrameMask.sprite = profile.itemFrameMask;
                }

            }

            if(cardType == typeof(LocationType))
            {
                cardIcon.sprite = profile.placeIcon;
                if (data.GetType() == typeof(DarkIdeaCard))
                {
                    cardFrame.sprite = profile.cursePlaceFrame;
                    cardFrameMask.sprite = profile.cursePlaceFrameMask;
                }
                else
                {
                    cardFrame.sprite = profile.placeFrame;
                    cardFrameMask.sprite = profile.placeFrameMask;
                }
            }
        }
        else
        {
            //
            characterAttackFrame.gameObject.SetActive(false);
            characterHealthFrame.gameObject.SetActive(false);

            if(data.GetType() !=  typeof(PlotCard) || data.GetType() == typeof(JunkCard))
            {
                cardTimerFrame.gameObject.SetActive(false);
            }
        }
        #endregion

        //Update Texts
        UpdateBaseElements(data);
    }

    public void UpdateBaseElements(CardData data)
    {
        if(data is DarkIdeaCard)
        {
            manaCostText.text = data.manaCost.ToString();
            manaCostText.color = Color.white;
            cardNameText.text = data.cardName;
            cardNameText.color = Color.white;

            //GetDescription // Update Description
            cardDescriptionText.text = BuildDescription(data);
            cardDescriptionText.color = Color.white;
        }
        else
        {
            manaCostText.text = data.manaCost.ToString();
            manaCostText.color = Color.black;
            cardNameText.text = data.cardName;
            cardNameText.color = Color.black;

            //GetDescription // Update Description
            cardDescriptionText.text = BuildDescription(data);
            cardDescriptionText.color = Color.black;
        }
    }

    public void UpdateCharacterElements(CharacterType character)
    {
        if (character.data is DarkIdeaCard)
        {
            characterAttackText.text = character.stats.baseAttackDamage.ToString();
            characterAttackText.color = Color.white;
            characterHealthText.text = character.stats.baseLifePoints.ToString();
            characterHealthText.color = Color.white;

            timerText.text = character.useCount.ToString();
            timerText.color = Color.white;
        }
        else
        {
            characterAttackText.text = character.stats.baseAttackDamage.ToString();
            characterAttackText.color = Color.black;
            characterHealthText.text = character.stats.baseLifePoints.ToString();
            characterHealthText.color = Color.black;

            if (character.data.GetType() != typeof(PlotCard))
            {
                timerText.text = character.useCount.ToString();
                timerText.color = Color.black;
            }
        }
        
        
    }

    public void UpdatePlotElements(PlotCard card)
    {
        timerText.text = card.completionTimer.ToString();
        timerText.color = Color.black;
        //cardFlagText.text
    }

    public string BuildDescription(CardData data)
    {
        string result = String.Empty;

        if(data.cardType?.GetType() == typeof(CharacterType))
        {
            CharacterType chara = data.cardType as CharacterType;

            if(chara.doubleStrike) result += "<b>Double Strike</b>" + "\n";
            if (chara.undying) result += "<b>Undying</b>" + "\n";

        }
        for (int i = 0; i < data.effects.Count; i++)
        {
            result += data.effects[i].GetDescription(data.effects[i]);
            result += "\n";   
        }

        //Add objective description ? NOTE
        if(data.GetType() == typeof(PlotCard))
        {
            PlotCard plot = data as PlotCard;
            result += "Objective : " + plot.objective.objectiveName;
        }
       
        return result;
    }

    #region Tweening
    public void EffectChangeFeedback(CardContainer container, int direction, EventQueue queue)
    {
        Vector3 scale = direction > 0 ? new Vector3(1.2f, 1.2f, 1.2f) : new Vector3(0.8f, 0.8f, 0.8f);
        LeanTween.scale(container.gameObject, scale, 0.1f ).setEaseInOutCubic().setLoopPingPong(1).setOnComplete(value=> { if (queue != null) queue.resolved = true; });
    }

    #endregion

}
