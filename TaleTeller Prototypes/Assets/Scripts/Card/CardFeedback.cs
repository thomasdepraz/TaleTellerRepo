using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardFeedback : MonoBehaviour
{
    [Header("References")]
    public Image feedbackImage;
    public Image face;
    public GameObject statsContainer;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;

    [Header("Sprites")]
    public Sprite friendlyFace;
    public Sprite ennemyFace;

    public void InitCardFeedback(CardData card)
    {
        gameObject.SetActive(true);
        card.feedback = this;
        feedbackImage.sprite = card.cardGraph;

        //Init the rest of the stats
        switch (card.type)
        {
            case CardType.Character:
                statsContainer.SetActive(true);

                if (card.characterBehaviour == CharacterBehaviour.Agressive)
                    face.sprite = ennemyFace;
                else
                    face.sprite = friendlyFace; 

                hpText.text = card.characterStats.baseLifePoints.ToString();
                attackText.text = card.characterStats.baseAttackDamage.ToString();
                break;
            case CardType.Object:
                break;
            case CardType.Location:
                break;
            default:
                break;
        }
    }

    public void UnloadCardFeedback(CardData card)
    {
        gameObject.SetActive(false);
        statsContainer.SetActive(false);
        card.feedback = null;
        feedbackImage.sprite = null;
    }

    public void UpdateText(CardData card)
    {
        if(card.characterStats.baseLifePoints < int.Parse(hpText.text))
        {
            StartCoroutine(DamageFeedback());
        }
        hpText.text = card.characterStats.baseLifePoints.ToString();
        attackText.text = card.characterStats.baseAttackDamage.ToString();
    }

    public IEnumerator DamageFeedback()
    {
        feedbackImage.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        feedbackImage.color = Color.white;
    }
}
