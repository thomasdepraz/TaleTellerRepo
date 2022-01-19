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

        if (!junkInLinkedList)
            for (int i = 0; i < junkCardsToSpawn.Count; i++)
            { 
                if(linkedData.GetType() == typeof(JunkCard))
                    (linkedData as JunkCard).objective.linkedJunkedCards.Add(junkCardsToSpawn[i].InitializeData(junkCardsToSpawn[i]) as JunkCard); 
                else
                    (linkedData as PlotCard).objective.linkedJunkedCards.Add(junkCardsToSpawn[i].InitializeData(junkCardsToSpawn[i]) as JunkCard);
            }

        for (int i = 0; i < junkCardsToSpawn.Count; i++)
        {
            switch (junkSpawnLocations[i])
            {
                case JunkSpawnLocation.EndDeck:
                    CardManager.Instance.cardDeck.cardDeck.Add(junkCardsToSpawn[i]);

                    EventQueue endDeckfeedback = new EventQueue();
                    CardManager.Instance.CardAppearToDeck(junkCardsToSpawn[i], endDeckfeedback, CardManager.Instance.plotAppearTransform.position, false);
                    endDeckfeedback.StartQueue();
                    while (!endDeckfeedback.resolved) { yield return new WaitForEndOfFrame(); }


                    break;
                case JunkSpawnLocation.DeckRandom:
                    CardManager.Instance.cardDeck.cardDeck.Insert(Random.Range(0, CardManager.Instance.cardDeck.cardDeck.Count), junkCardsToSpawn[i]);

                    EventQueue deckRandomFeedback = new EventQueue();
                    CardManager.Instance.CardAppearToDeck(junkCardsToSpawn[i], deckRandomFeedback, CardManager.Instance.plotAppearTransform.position, false);

                    deckRandomFeedback.StartQueue();
                    while (!deckRandomFeedback.resolved) { yield return new WaitForEndOfFrame(); }


                    break;

                case JunkSpawnLocation.Hand:

                    EventQueue drawToHandQueue = new EventQueue();

                    CardManager.Instance.CardAppearToHand(junkCardsToSpawn[i], drawToHandQueue, CardManager.Instance.plotAppearTransform.position);

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
