using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroMessage
{
    CardManager manager;
    public bool validated;
    public bool overridable;

    public HeroMessage(string message)
    {
        manager = CardManager.Instance;
        manager.playerMessageButton.interactable = false;

        //Protect the non-overridable messages
        if (manager.currentPlayerMessage != null && !manager.currentPlayerMessage.overridable)
            return;

        //Close current if still open
        if (manager.currentPlayerMessage != null)
        {
            manager.currentPlayerMessage.CloseMessageBox();
        }

        manager.currentPlayerMessage = this;

        manager.exclamationText.text = message;
        OpenMessageBox();
    }


    public HeroMessage(string message, EventQueue queue, bool clickable)
    {
        queue.events.Add(MessageRoutine(message, queue, clickable));
    }
    IEnumerator MessageRoutine(string message, EventQueue queue, bool clickable)
    {
        manager = CardManager.Instance;
        manager.playerMessageButton.onClick = () => { validated = true; };
        manager.playerMessageButton.interactable = true;

        BoardState originState = CardManager.Instance.board.currentBoardState;

        CardManager.Instance.board.currentBoardState = BoardState.None;

        if(manager.currentPlayerMessage != null)
        {
            manager.currentPlayerMessage.CloseMessageBox();
        }

        manager.currentPlayerMessage = this;

        manager.exclamationText.text = message;

        bool wait = true;
        OpenMessageBox(()=> { wait = false; });

        if(clickable)
        {
            while (!validated) { yield return new WaitForEndOfFrame(); }
        }
        else
        {
            while (wait) { yield return new WaitForEndOfFrame(); }
        }

        CloseMessageBox();
        yield return new WaitForSeconds(0.2f);

        CardManager.Instance.board.currentBoardState = originState;

        queue.UpdateQueue();
    }


    void OpenMessageBox()
    {
        if(manager.exclamationPanelTransform.position.x > 0)
        {
            manager.exclamationPanelTransform.pivot = new Vector2(1f, 0.5f);
        }
        else
        {
            manager.exclamationPanelTransform.pivot = new Vector2(0f, 0.5f);
        }


        LeanTween.cancel(manager.exclamationPanelTransform.gameObject);
        manager.exclamationPanelTransform.gameObject.SetActive(true);
        LeanTween.value(manager.exclamationPanelTransform.gameObject, 0,0,2f).setOnComplete(CloseMessageBox);
        LayoutRebuilder.ForceRebuildLayoutImmediate(manager.exclamationPanelTransform);
    }

    void OpenMessageBox(Action onComplete)
    {
        if (manager.exclamationPanelTransform.position.x > 0)
        {
            manager.exclamationPanelTransform.pivot = new Vector2(1f, 0.5f);
        }
        else
        {
            manager.exclamationPanelTransform.pivot = new Vector2(0f, 0.5f);
        }

        LeanTween.cancel(manager.exclamationPanelTransform.gameObject);
        manager.exclamationPanelTransform.gameObject.SetActive(true);
        LeanTween.value(manager.exclamationPanelTransform.gameObject, 0, 0, 2f).setOnComplete(onComplete);
        LayoutRebuilder.ForceRebuildLayoutImmediate(manager.exclamationPanelTransform);
    }

    void CloseMessageBox()
    {
        manager.exclamationPanelTransform.localScale = Vector3.one;
        manager.exclamationPanelTransform.gameObject.SetActive(false);
        manager.currentPlayerMessage = null;
    }

}
