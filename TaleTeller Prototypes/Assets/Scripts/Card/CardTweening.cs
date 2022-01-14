using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            float originX = container.rectTransform.localPosition.x;
            LeanTween.moveLocalX(container.gameObject, originX + direction * -2f, 0.5f).setEaseOutQuint().setOnComplete(
                value => LeanTween.moveLocalX(container.gameObject, originX + direction * 50, 0.2f).setEaseInQuint().setOnComplete(
                    val => LeanTween.moveLocalX(container.gameObject, originX, 1).setEaseOutQuint().setOnComplete(end => { if (queue != null) queue.resolved = true; })));
        }
        else
        {
            float originY = container.rectTransform.localPosition.y;
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

    public void MoveCard(CardContainer container, Vector3 target, bool useScale = false, bool appear = false, EventQueue queue = null, float scaleMultiplier = 1)
    {
        container.transform.SetAsLastSibling();
        if (useScale)
        {
            if (appear)
            {
                container.rectTransform.localScale = Vector3.zero;
                container.selfImage.color = Color.black;

                LeanTween.value(gameObject, container.selfImage.color, Color.white, 0.3f).setOnUpdate((Color val) => { container.selfImage.color = val; });
                LeanTween.scale(container.rectTransform, Vector3.one * scaleMultiplier, 0.5f).setEaseInOutQuint();
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
            if(container.rectTransform.localScale != Vector3.one)
                LeanTween.scale(container.rectTransform, Vector3.one, 0.5f).setEaseInOutQuint();
            LeanTween.move(container.rectTransform, target, 0.8f).setEaseInOutQuint().setOnComplete(value => { if (queue != null) queue.resolved = true; });
        }
    }

    public void ShowHighlight(Image image, Color highlightColor)
    {
        LeanTween.cancel(image.gameObject);
        LeanTween.value(image.gameObject, 0, 1, 0.3f).setOnUpdate((float value) =>
        {
            image.color = new Color(highlightColor.r, highlightColor.g, highlightColor.b, value);
        }).setEaseInOutQuint();
    }

    public void HideHighlight(Image image)
    {
        LeanTween.cancel(image.gameObject);
        image.color = Color.clear;
    }

    public void ScaleBounce(GameObject gameObject, float scaleFactor)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, scaleFactor * Vector3.one, 0.2f).setLoopPingPong(1).setEaseInOutQuint();
    }

    public void ScaleBounce(GameObject gameObject, float scaleFactor, EventQueue queue)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, scaleFactor * Vector3.one, 0.2f).setLoopPingPong(1).setEaseInOutQuint().setOnComplete(()=> 
        {
            queue.resolved = true;
        });
    }

    public void ScaleBounceLoop(GameObject gameObject, float scaleFactor)
    {
        LeanTween.scale(gameObject, scaleFactor * Vector3.one, 0.2f).setLoopPingPong(1).setEaseInOutQuint().setOnComplete(()=> 
        {
            gameObject.transform.localScale = Vector3.one;
            ScaleBounceLoop(gameObject, scaleFactor);
        });
    }

    public void Cancel(GameObject gameObject)
    {
        LeanTween.cancel(gameObject);
    }

}
