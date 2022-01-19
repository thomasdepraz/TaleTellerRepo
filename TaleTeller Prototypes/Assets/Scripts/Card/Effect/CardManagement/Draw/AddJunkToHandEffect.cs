using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class AddJunkToHandEffect : Effect
{
    public bool junkInLinkedList;
    [HideIf("junkInLinkedList")]
    public List<JunkCard> junkCardsToSpawn = new List<JunkCard>();
    public enum JunkSpawnLocation { Hand, EndDeck, DeckRandom, }
    public List<JunkSpawnLocation> junkSpawnLocations = new List<JunkSpawnLocation>();

    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        EventQueue drawQueue = new EventQueue();

        List<JunkCard> cards = new List<JunkCard>();
        if (!junkInLinkedList)
        {
            for (int i = 0; i < junkCardsToSpawn.Count; i++)
            { 
                JunkCard card = junkCardsToSpawn[i].InitializeData(junkCardsToSpawn[i]) as JunkCard;
                cards.Add(card);
                if(linkedData.GetType() == typeof(JunkCard))
                {
                    (linkedData as JunkCard).objective.linkedJunkedCards.Add(card); 
                }
                else
                {
                    (linkedData as PlotCard).objective.linkedJunkedCards.Add(card);
                }
            }
        }

        for (int i = 0; i < junkCardsToSpawn.Count; i++)
        {
            JunkCard cardToInit;
            if (!junkInLinkedList) cardToInit = cards[i];
            else cardToInit = junkCardsToSpawn[i];

            switch (junkSpawnLocations[i])
            {
                case JunkSpawnLocation.EndDeck:
                    CardManager.Instance.cardDeck.cardDeck.Add(cardToInit);

                    EventQueue endDeckfeedback = new EventQueue();
                    CardManager.Instance.CardAppearToDeck(cardToInit, endDeckfeedback, CardManager.Instance.plotAppearTransform.position, false);
                    endDeckfeedback.StartQueue();
                    while (!endDeckfeedback.resolved) { yield return new WaitForEndOfFrame(); }


                    break;
                case JunkSpawnLocation.DeckRandom:
                    CardManager.Instance.cardDeck.cardDeck.Insert(Random.Range(0, CardManager.Instance.cardDeck.cardDeck.Count), cardToInit);

                    EventQueue deckRandomFeedback = new EventQueue();
                    CardManager.Instance.CardAppearToDeck(cardToInit, deckRandomFeedback, CardManager.Instance.plotAppearTransform.position, false);

                    deckRandomFeedback.StartQueue();
                    while (!deckRandomFeedback.resolved) { yield return new WaitForEndOfFrame(); }


                    break;

                case JunkSpawnLocation.Hand:

                    EventQueue drawToHandQueue = new EventQueue();

                    CardManager.Instance.CardAppearToHand(cardToInit, drawToHandQueue, CardManager.Instance.plotAppearTransform.position);

                    drawToHandQueue.StartQueue();//Actual draw
                    while (!drawToHandQueue.resolved)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    break;
            }

        }

        currentQueue.UpdateQueue();
    }
}
