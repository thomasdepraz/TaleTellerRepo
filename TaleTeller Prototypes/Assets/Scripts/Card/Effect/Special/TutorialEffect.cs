using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEffect : Effect
{
    public bool doOnce;
    private bool effectAvailable = true;

    public string tutorialText;
    public Sprite tutorialIllustration;
    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        if(effectAvailable && GameManager.Instance.tutorialManager.TurnCount < 3)
        {
            TutorialScreen screen = new TutorialScreen(LocalizationManager.Instance.GetString(LocalizationManager.Instance.tutorielDictionary, tutorialText), "", tutorialIllustration);
            bool wait = true;
            screen.Open(()=> wait = false);
            while (wait) { yield return new WaitForEndOfFrame(); }

            while (screen.open) { yield return new WaitForEndOfFrame(); }

            wait = true;
            screen.Close(() => wait = false);
            while (wait) { yield return new WaitForEndOfFrame(); }
        }

        if (doOnce)
            effectAvailable = false;


        currentQueue.UpdateQueue();
    }
}
