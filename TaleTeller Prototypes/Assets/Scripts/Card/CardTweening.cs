using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTweening : MonoBehaviour
{
    public void ShakeCard(CardContainer container, EventQueue queue)
    {
        LeanTween.rotateZ(container.gameObject, -3, 0.025f).setEaseOutCubic().setOnComplete(
            value => LeanTween.rotateZ(container.gameObject, 3, 0.05f).setEaseInOutCubic().setLoopPingPong(2).setOnComplete(
                val => LeanTween.rotateZ(container.gameObject, 0, 0.025f).setEaseOutCubic().setOnComplete(end => queue.resolved = true)));
    }

    public void CardAttack(CardContainer container, int direction, EventQueue queue = null)
    {
        if (Mathf.Abs(direction) > 0)
        {
            float originX = container.rectTransform.anchoredPosition.x;
            LeanTween.moveLocalX(container.gameObject, originX + direction * -2f, 0.5f).setEaseOutQuint().setOnComplete(
                value => LeanTween.moveLocalX(container.gameObject, originX + direction * 50, 0.2f).setEaseInQuint().setOnComplete(
                    val => LeanTween.moveLocalX(container.gameObject, originX, 1).setEaseOutQuint().setOnComplete(end => { if (queue != null) queue.resolved = true; })));
        }
        else
        {
            float originY = container.rectTransform.anchoredPosition.y;
            LeanTween.moveLocalY(container.gameObject, originY - 2f, 0.5f).setEaseOutQuint().setOnComplete(
               value => LeanTween.moveLocalY(container.gameObject, originY + 50, 0.2f).setEaseInQuint().setOnComplete(
                   val => LeanTween.moveLocalY(container.gameObject, originY, 1).setEaseOutQuint().setOnComplete(end => { if (queue != null) queue.resolved = true; })));
        }
    }

    public void EffectChangeFeedback(CardContainer container, int direction, EventQueue queue)
    {
        Vector3 scale = direction > 0 ? new Vector3(1.2f, 1.2f, 1.2f) : new Vector3(0.8f, 0.8f, 0.8f);
        LeanTween.scale(container.gameObject, scale, 0.1f).setEaseInOutCubic().setLoopPingPong(1).setOnComplete(value => { if (queue != null) queue.resolved = true; });
    }

    public void MoveCard(CardContainer container, Vector3 target, bool useScale = false, bool appear = false, EventQueue queue = null)
    {
        if (useScale)
        {
            if (appear)
            {
                container.rectTransform.localScale = Vector3.zero;
                container.selfImage.color = Color.black;

                LeanTween.value(gameObject, container.selfImage.color, Color.white, 0.3f).setOnUpdate((Color val) => { container.selfImage.color = val; });
                LeanTween.scale(container.rectTransform, Vector3.one, 0.5f).setEaseInOutQuint();
                LeanTween.move(container.rectTransform, target, 0.8f).setEaseInOutQuint().setOnComplete(value => { if (queue != null) queue.resolved = true; });
            }
            else
            {
                LeanTween.value(gameObject, container.selfImage.color, Color.black, 0.8f).setOnUpdate((Color val) => { container.selfImage.color = val; });
                LeanTween.scale(container.rectTransform, Vector3.zero, 0.8f).setEaseInOutQuint();
                LeanTween.move(container.rectTransform, target, 0.8f).setEaseInOutQuint().setOnComplete(value => { if (queue != null) queue.resolved = true; });
            }
        }
        else
        {
            LeanTween.move(container.rectTransform, target, 0.8f).setEaseInOutQuint().setOnComplete(value => { if (queue != null) queue.resolved = true; });
        }
    }




}
