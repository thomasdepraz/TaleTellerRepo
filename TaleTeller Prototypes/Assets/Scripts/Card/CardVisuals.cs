using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisuals : MonoBehaviour
{
    #region Variables
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

    [Header("Texts")]
    public TextMeshProUGUI manaCostText;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardDescriptionText;
    public TextMeshProUGUI cardFlagText;
    [Space]
    public TextMeshProUGUI characterAttackText;
    public TextMeshProUGUI characterHealthText;
    public TextMeshProUGUI characterAffiliationText;
    public TextMeshProUGUI timerText;
    #endregion

    public void InitializeVisuals(CardData data)
    {
        //Init basics
        #region Basics
        Type dataType = data.GetType();
        if(dataType == typeof(IdeaCard))
        {
            cardBackgound.sprite = ideaBackground;
            cardFlag.sprite = ideaFlag;
            cardManaFrame.sprite = normalManaFrame;

            plotIcon.gameObject.SetActive(false);
            plotUnderFlag.gameObject.SetActive(false);

        }
        else if (dataType == typeof(PlotCard))
        {
            cardBackgound.sprite = plotBackground;
            cardFlag.sprite = plotFlag;
            cardManaFrame.sprite = normalManaFrame;

            plotIcon.gameObject.SetActive(true);
            plotUnderFlag.gameObject.SetActive(true);
            cardTimerFrame.gameObject.SetActive(false);


            PlotCard card = data as PlotCard;
            if (card.isMainPlot) plotUnderFlag.sprite = mainUnderFlag;
            else plotUnderFlag.sprite = secondaryUnderFlag;

            UpdatePlotElements(card);

        }
        else if(dataType == typeof(DarkIdeaCard))
        {
            cardBackgound.sprite = curseBackground;
            cardFlag.sprite = curseFlag;
            cardManaFrame.sprite = curseManaFrame;

            plotIcon.gameObject.SetActive(false);
            plotUnderFlag.gameObject.SetActive(false);

        }
        else if(dataType == typeof(JunkCard))
        {
            cardBackgound.sprite = junkBackground;
            cardFlag.sprite = plotFlag;
            cardManaFrame.sprite = normalManaFrame;

            plotIcon.gameObject.SetActive(false);
            plotUnderFlag.gameObject.SetActive(false);
        }
        #endregion

        //Init type based values
        #region TypeBasedValue
        if (data.cardType!=null)
        {
            Type cardType = data.cardType.GetType();
            if(cardType == typeof(CharacterType))
            {
                cardIcon.sprite = characterIcon;
                cardFrame.sprite = characterFrame;

                characterAttackFrame.gameObject.SetActive(true);
                characterHealthFrame.gameObject.SetActive(true);
                cardTimerFrame.gameObject.SetActive(true);
                characterAffilitionFrame.gameObject.SetActive(true);

                if(dataType == typeof(DarkIdeaCard))
                {
                    characterAttackFrame.sprite = curseCharacterAttack;
                    characterHealthFrame.sprite = curseCharacterHealth;
                    characterAffilitionFrame.sprite = curseCharacterAffiliation;
                }
                else
                {
                    characterAttackFrame.sprite = normalCharacterAttack;
                    characterHealthFrame.sprite = normalCharacterHealth;
                    characterAffilitionFrame.sprite = normalCharacterAffiliation;
                }

                CharacterType character = data.cardType as CharacterType;
                UpdateCharacterElements(character);
            }
            else
            {
                characterAttackFrame.gameObject.SetActive(false);
                characterHealthFrame.gameObject.SetActive(false);
                cardTimerFrame.gameObject.SetActive(false);
                characterAffilitionFrame.gameObject.SetActive(false);
            }

            if(cardType == typeof(ObjectType))
            {
                cardIcon.sprite = itemIcon;
                cardFrame.sprite = itemFrame;
            }

            if(cardType == typeof(LocationType))
            {
                cardIcon.sprite = placeIcon;
                cardFrame.sprite = placeFrame;
            }
        }
        else
        {
            //
            characterAttackFrame.gameObject.SetActive(false);
            characterHealthFrame.gameObject.SetActive(false);
            characterAffilitionFrame.gameObject.SetActive(false);

            if(data.GetType() !=  typeof(PlotCard))
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
        manaCostText.text = data.manaCost.ToString();
        cardNameText.text = data.cardName;

        //GetDescription // Update Description
        //cardDescriptionText.text = BuildDescription(data);

        //TEMP
        cardDescriptionText.text = data.description;
    }


    public void UpdateCharacterElements(CharacterType character)
    {
        switch(character.behaviour)//NOTE move the hardcoded string into a scriptable object or smthing
        {
            case CharacterBehaviour.Agressive:
                characterAffiliationText.text = "Bad";
                break;
            case CharacterBehaviour.Peaceful:
                characterAffiliationText.text = "Good";
                break;
            case CharacterBehaviour.None:
                characterAffiliationText.text = "Neutral";
                break;
        }

        characterAttackText.text = character.stats.baseAttackDamage.ToString();
        characterHealthText.text = character.stats.baseLifePoints.ToString();

        if(character.data.GetType() !=  typeof(PlotCard))
        {
            timerText.text = character.useCount.ToString();
        }
    }

    public void UpdatePlotElements(PlotCard card)
    {
        timerText.text = card.completionTimer.ToString();
        
        //cardFlagText.text
    }

    public string BuildDescription(CardData data)
    {
        string result = String.Empty;
       
        return result;
    }


}
