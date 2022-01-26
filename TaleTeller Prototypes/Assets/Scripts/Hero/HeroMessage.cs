using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMessage
{
    CardManager manager;
    public HeroMessage(string message)
    {
        manager = CardManager.Instance;
        if (manager.currentMessage != null)
        {
            manager.currentMessage.CloseMessageBox();
            manager.currentMessage = this;
        }

        manager.exclamationText.text = message;
        OpenMessageBox();
    }

    void OpenMessageBox()
    {
        LeanTween.cancel(manager.exclamationPanelTransform.gameObject);
        manager.exclamationPanelTransform.gameObject.SetActive(true);
        LeanTween.value(manager.exclamationPanelTransform.gameObject, 0,0,2f).setOnComplete(CloseMessageBox);
    }

    void CloseMessageBox()
    {
        manager.exclamationPanelTransform.gameObject.SetActive(false);
    }

}
