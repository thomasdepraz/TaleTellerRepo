using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawFullHandEffect : DrawTypeEffects
{
    protected override int GetAmountToDraw()
    {
        var cardHand = CardManager.Instance.cardHand;
        int roomInHand = cardHand.maxHandSize - cardHand.GetHandCount();

        return roomInHand;
    }
}
