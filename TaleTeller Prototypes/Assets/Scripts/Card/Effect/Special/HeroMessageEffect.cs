using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMessageEffect : Effect
{
    public bool doOnce;
    private bool effectAvailable = true;
    public string message;
    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        if(effectAvailable)
        {
            EventQueue messageQueue = new EventQueue();
            HeroMessage heroMessage = new HeroMessage(message, messageQueue, true);
            messageQueue.StartQueue();
            while(!messageQueue.resolved) { yield return new WaitForEndOfFrame(); }
        }

        if (doOnce)
            effectAvailable = false;

        currentQueue.UpdateQueue();
    }


}
