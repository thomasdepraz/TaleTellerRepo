using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GoButton : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerDownHandler
    , IPointerUpHandler
{
    [Header("Sprites")]
    public Image image;
    public Sprite defaultSprite;
    public Sprite hoveredSprite;
    public Color onPressColor;

    bool canClick;
    public RectTransform rectTransform;
    public Board board;
    Vector3 originPos;
    Quaternion originRot;
    Vector3 originScale;

    LTDescr tween;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //do some tweening
        originPos = rectTransform.anchoredPosition;
        originRot = rectTransform.rotation;
        originScale = rectTransform.localScale;

        tween = LeanTween.scale(this.gameObject, new Vector3(1.2f,1.2f,1.2f), 0.1f).setLoopPingPong(1).setOnComplete(HoveringTween);
        image.sprite = hoveredSprite;
        if(Input.GetMouseButton(0))
        {
            image.color = onPressColor;
            canClick = true;
        }
    }
    public void HoveringTween()
    {
        LeanTween.move(this.gameObject, transform.position + new Vector3(0, 0.1f, 0), 0.8f).setEaseInOutSine().setLoopPingPong(1).setRepeat(100);
        LeanTween.rotate(this.gameObject, new Vector3(0, 0, transform.rotation.z - 5), 1f).setEaseInOutSine().setLoopPingPong(1).setRepeat(100);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = defaultSprite;
        image.color = Color.white;
        canClick = false;

        if(LeanTween.isTweening(gameObject))
        {
            LeanTween.cancel(gameObject);
            rectTransform.anchoredPosition = originPos;
            rectTransform.rotation = originRot;
            rectTransform.localScale = originScale;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        image.color = onPressColor;
        if(!board.IsEmpty() && CardManager.Instance.board.currentBoardState == BoardState.Idle)
            canClick = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Launch draft board methods
        if(canClick)
        {
            //board.CreateStory();
            board.InitBoard();
        }

        image.color = Color.white;
        canClick = false;
    }
}
