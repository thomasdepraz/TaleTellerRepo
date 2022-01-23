using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PickerHelper
{
    public static void PointerEnter(GameObject go)
    {
        LeanTween.rotate(go, Vector3.zero, 0.2f).setEaseInOutCubic();
        LeanTween.scale(go, Vector3.one * 1.3f, 0.1f).setEaseInOutCubic();
    }

    public static void PointerExit(GameObject go)
    {
        LeanTween.scale(go, Vector3.one, 0.1f).setEaseInOutCubic();
    }

    public static void SelectContainerFeedback(CardContainer container)
    {
        container.selfImage.color = Color.green;
    }

    public static void DeselectContainerFeedback(CardContainer container)
    {
        container.selfImage.color = Color.white;
    }
}