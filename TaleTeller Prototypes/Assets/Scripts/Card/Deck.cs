using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<CardData> cardDeck;
    public List<CardData> discardPile;


    public IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        for (int i = 0; i < cardDeck.Count; i++)
        {
            cardDeck[i] = cardDeck[i].InitializeData(cardDeck[i]);
            
        }

        ShuffleCards(cardDeck);
        DealCards(CardManager.Instance.cardHand.maxHandSize);//Deal First hand
    }

    public List<CardData> ShuffleCards(List<CardData> deckToShuffle) //FisherYates Shuffle
    {
        int n = deckToShuffle.Count;
        for (int i = 0; i < (n - 1); i++)
        {
            int r = i + Random.Range(0, n-i);
            CardData t = deckToShuffle[r];
            deckToShuffle[r] = deckToShuffle[i];
            deckToShuffle[i] = t;
        }
        return deckToShuffle;
    }

    public void DealCards(int count)
    {
        StartCoroutine(Deal(count));
    }

    IEnumerator Deal(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(0.2f);
            if (cardDeck.Count > 0)//Deal card while deck is not empty
            {
                CardManager.Instance.cardHand.InitCard(cardDeck[0]);
                cardDeck.RemoveAt(0);
            }
            else //Get card from discrad back to deck
            {
                if (discardPile.Count == 0)//if discard is empty break the loop
                    break;

                for (int j = 0; j < discardPile.Count; j++)
                {

                    cardDeck.Add(discardPile[j]);

                    //Remove 1 creativity per recycled card
                    GameManager.Instance.creativityManager.creativity--;
                }
                discardPile.Clear();
                ShuffleCards(cardDeck);
                Debug.LogError("Shuffling discard pile in deck");

                i--;
            }
        }
    }
}
