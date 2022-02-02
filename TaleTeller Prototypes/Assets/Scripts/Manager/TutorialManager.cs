using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public int TurnCount { get; set; } = 0;

    //Object Origins
    [SerializeField] private CanvasScaler scalerReference;
    private Vector3 boardOrigin;
    private Vector3 lineOrigin;
    private Vector3 deckOrigin;
    private Vector3 discardOrigin;
    private Vector3 goButtonOrigin;
    private Vector3 handOrigin;



    IEnumerator IntroductionRoutine()
    {
        //Screen
        #region Screen_01
        TutorialScreen screen_01 = new TutorialScreen("Bienvenue", "TUTO_01");
        bool wait = true;
        screen_01.Open(()=> wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        while (screen_01.open) { yield return new WaitForEndOfFrame(); }
        wait = true;
        screen_01.Close(() => wait = false);
        while(wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Screen
        #region Screen_02
        TutorialScreen screen_02 = new TutorialScreen("Bienvenue bis", "TUTO_02");
        wait = true;
        screen_02.Open(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        while (screen_02.open) { yield return new WaitForEndOfFrame(); }
        wait = true;
        screen_02.Close(() => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }
        #endregion

        //Hero and board
        wait = true;
        AppearObject(CardManager.Instance.board.lineTransform, lineOrigin, () => wait = false);
        while (wait) { yield return new WaitForEndOfFrame(); }

        EventQueue lineMessageQueue = new EventQueue();
        HeroMessage lineMessage = new HeroMessage("Salut le sang", lineMessageQueue, true);
        lineMessageQueue.StartQueue();
        while (!lineMessageQueue.resolved) { yield return new WaitForEndOfFrame(); }


        yield return new WaitForEndOfFrame();
    }


    public void AppearObject(Transform transform, Vector3 target, Action onComplete)
    {
        LeanTween.moveLocal(transform.gameObject, target, 1f).setEaseInOutQuint().setOnComplete(onComplete);
    }

    public void InitializeGameScreen()
    {
        CardManager manager = CardManager.Instance;

        //Hide Board
        boardOrigin = manager.board.boardTransform.localPosition;
        lineOrigin = manager.board.lineTransform.localPosition;
        goButtonOrigin = manager.board.goButtonTransform.localPosition;

        //manager.board.boardTransform.localPosition = new Vector3(boardOrigin.x, -scalerReference.referenceResolution.y ,boardOrigin.z);
        manager.board.lineTransform.localPosition = new Vector3(lineOrigin.x, -scalerReference.referenceResolution.y, lineOrigin.z);




        StartCoroutine(IntroductionRoutine());
    }

    public int GetCardCount()
    {
        switch (TurnCount)
        {
            case 0: return 2; 

            default:
                return 0;
        }
    }

    public void StartTurnAction(EventQueue queue)
    {
        queue.events.Add(StartTurnActionRoutine());
    }

    private IEnumerator StartTurnActionRoutine()
    {
        switch (TurnCount)
        {
            case 0:
                bool valid = false;
                CardManager.Instance.board.boardUpdate += () => valid = ValidConditions();

                EventQueue messageQueue = new EventQueue();
                HeroMessage ideaMessage = new HeroMessage("hello", messageQueue, true);
                HeroMessage cardMessage = new HeroMessage("hello bis", messageQueue, true);
                HeroMessage inkPotMessage = new HeroMessage("hello tris", messageQueue, true);
                HeroMessage manaMessage = new HeroMessage("hello chris :)", messageQueue, true);
                HeroMessage tryCardsMessage = new HeroMessage("hello chris :)", messageQueue, true);
                messageQueue.StartQueue();
                while (messageQueue.resolved) { yield return new WaitForEndOfFrame(); }

                while (valid == false) { yield return new WaitForEndOfFrame(); } //attente que les conditions soient valides
                

                break;

            default:
                break;
        }
        yield return new WaitForEndOfFrame();
    }


    public bool ValidConditions()
    {
        switch (TurnCount)
        {
          

            default:
                return false;
        }
    }



}
