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
    , IPointerClickHandler
{
    [Header("Sprites")]
    public Image image;
    public Sprite defaultSprite;
    public Sprite hoveredSprite;
    public Color onPressColor;

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
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        image.color = Color.white;
    }

    public bool CheckValid()
    {

        if (CardManager.Instance.board.currentBoardState != BoardState.Idle)
        {
            HeroMessage errorMessage = new HeroMessage("Can't do that now");
            return false;
        }
        else if (board.IsEmpty())
        {
            HeroMessage errorMessage = new HeroMessage("The board is empty !");
            return false;
        }
        return true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.currentState == GameState.TUTORIAL && GameManager.Instance.tutorialManager.currentState != TutorialState.PENDING && CardManager.Instance.board.currentBoardState == BoardState.Idle && GameManager.Instance.tutorialManager.canUseGoButton)
            GameManager.Instance.tutorialManager.ValidConditions();
        else if(GameManager.Instance.currentState == GameState.GAME)
        {
            if (CheckValid()) board.InitBoard();
        }

    }
}
