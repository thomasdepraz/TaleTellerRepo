using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public RectTransform handTransform;
    public List<CardContainer> hiddenHand = new List<CardContainer>();
    public List<CardContainer> currentHand = new List<CardContainer>();
    public int maxHandSize;

    public void InitCard(CardData data, bool fromDeck = true)
    {
        for (int i = 0; i < hiddenHand.Count; i++)
        {
            if(!hiddenHand[i].gameObject.activeSelf)
            {
                //Set parent and move
                if (fromDeck)
                {
                    hiddenHand[i].rectTransform.SetParent(handTransform);
                    MoveCard(hiddenHand[i],RandomPositionInRect(handTransform), true);
                }

                currentHand.Add(hiddenHand[i]);
                hiddenHand[i].CardInit(data);

                break;
            }
        }
    }

    public void DiscardAllHand()//TODO Implement queuing system
    {
        int cachedCount = currentHand.Count;
        for (int i = 0; i < cachedCount; i++)
        {
            CardManager.Instance.cardDeck.discardPile.Add(currentHand[0].data);
            currentHand[0].ResetCard();
        }
    }

    public void DiscardCardFromHand(CardContainer card)//TODO Implement queuing system
    {
        card.data.ResetData(card.data);

        CardManager.Instance.cardDeck.discardPile.Add(card.data);
        card.ResetCard();
    }

    #region Visuals
    public void MoveCard(CardContainer card, Vector3 target, bool appear)
    {
        //if appear also make card scale go from small to normal and color from black to white
        if(appear)
        {
            card.rectTransform.localScale = Vector3.zero;
            card.selfImage.color = Color.black;

            LeanTween.value(gameObject, card.selfImage.color, Color.white, 0.3f).setOnUpdate((Color val) => {card.selfImage.color = val;});
            LeanTween.scale(card.rectTransform, Vector3.one, 0.5f).setEaseOutQuint();
            LeanTween.move(card.rectTransform, target, 0.8f).setEaseOutQuint();
        }
        else
        {
            LeanTween.move(card.rectTransform, target, 0.8f).setEaseOutQuint();
        }
    }

    public Vector3 RandomPositionInRect(RectTransform transform)
    {
        Vector3 randomPos;
        float height = transform.rect.height;
        float width = transform.rect.width;

        randomPos = new Vector3(Random.Range(-width/2, width/2), Random.Range(-height/4, height/4), 0);

        return randomPos;
    }

    public void UpdateKeyCardStatus()
    {
        for (int i = 0; i < currentHand.Count; i++)
        {
            if (currentHand[i].data.keyCardActivated)
            {
                if (currentHand[i].data.currentInterestCooldown <= 0)
                {
                    GameManager.Instance.creativityManager.creativity -= currentHand[i].data.creativityBurn;//Affect creativity
                }
                currentHand[i].data.currentInterestCooldown -= 1;//Lower cooldown
                currentHand[i].timerText.text = currentHand[i].data.currentInterestCooldown.ToString();
            }
        }
    }
    #endregion
}
