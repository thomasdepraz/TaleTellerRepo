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
        if(effectAvailable && GameManager.Instance.tutorialManager.TurnCount < 3)
        {
            EventQueue messageQueue = new EventQueue();
            HeroMessage heroMessage = new HeroMessage(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, message), messageQueue, true);
            messageQueue.StartQueue();
            while(!messageQueue.resolved) { yield return new WaitForEndOfFrame(); }
        }

        if (doOnce)
            effectAvailable = false;

        currentQueue.UpdateQueue();
    }


}
