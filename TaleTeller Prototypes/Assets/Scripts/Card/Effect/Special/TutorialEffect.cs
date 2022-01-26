using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEffect : Effect
{
    public string tutorialText;
    public override IEnumerator EffectLogic(EventQueue currentQueue, CardData data = null)
    {
        TutorialScreen screen = new TutorialScreen(tutorialText, "$TUTORIAL$");
        bool wait = true;
        screen.Open(()=> wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }

        while (screen.open) { yield return new WaitForEndOfFrame(); }

        wait = true;
        screen.Close(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }

        currentQueue.UpdateQueue();
    }
}
