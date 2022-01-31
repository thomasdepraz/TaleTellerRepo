using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Message
{
    CardManager manager;
    public bool validated;
    public bool overridable;

    public Message(string message, EventQueue queue, Vector3 appearPosition)
    {
        queue.events.Add(MessageRoutine(message, queue, appearPosition));
    }
    IEnumerator MessageRoutine(string message, EventQueue queue, Vector3 appearPosition)
    {
        manager = CardManager.Instance;
        manager.messageButton.onClick = () => { validated = true; };


        if(manager.currentPlayerMessage != null)
        {
            manager.currentMessage.CloseMessageBox();
        }

        manager.messagePanelTransform.position = appearPosition;
        manager.currentMessage = this;

        manager.messageText.text = message;

       
        OpenMessageBox();


        while (!validated) { yield return new WaitForEndOfFrame(); }
    

        CloseMessageBox();
        yield return new WaitForSeconds(0.2f);

        queue.UpdateQueue();
    }


    void OpenMessageBox()
    {
        LeanTween.cancel(manager.messagePanelTransform.gameObject);
        manager.messagePanelTransform.gameObject.SetActive(true);
        LeanTween.value(manager.messagePanelTransform.gameObject, 0,0,2f).setOnComplete(CloseMessageBox);
        LayoutRebuilder.ForceRebuildLayoutImmediate(manager.messagePanelTransform);
    }

    void CloseMessageBox()
    {
        manager.messagePanelTransform.gameObject.SetActive(false);
        manager.currentPlayerMessage = null;
    }

}
