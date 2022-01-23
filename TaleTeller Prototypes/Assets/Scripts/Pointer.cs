using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public Vector2 screenSize;
    public Transform self;

    public RectTransform targetTransform;
    public RectTransform tooltipTransform;
    public TextMeshProUGUI tooltipText;
    private bool tooltipOpen;
    private bool hovering;

    public List<RaycastResult> results = new List<RaycastResult>();
    public Vector3 pointerDirection;
    public void Start()
    {
        screenSize.x = Screen.width;
        screenSize.y = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition) - new Vector3(0, 0, -10);
        pointerDirection = new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0).normalized;

        if(tooltipOpen)
        {
            targetTransform.position = transform.position;
        }
    }

    public void ShowTooltip(string description, float delay = 0.3f)
    {
        tooltipOpen = true;
        hovering = true;
        tooltipText.SetText(description);

        //set pivot 
        SetPivotFromScreenPos(tooltipTransform);


        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, targetTransform.localScale.x, 1, 0.1f).setDelay(delay).setOnStart(()=> 
        {
            if (!hovering)
                LeanTween.cancel(gameObject);
        
        }).setOnUpdate((float value) =>
        {
            targetTransform.localScale = new Vector3(value, value, 1);
        }).setEaseInOutQuint();
    }

    public void HideTooltip()
    {
        hovering = false;
        LeanTween.cancel(gameObject);
        targetTransform.localScale = Vector3.zero;
        tooltipOpen = false;
    }

    void SetPivotFromScreenPos(RectTransform t)
    {
        if(transform.position.x < 0)
        {
            if(transform.position.y<0)
            {
                t.pivot = new Vector2(0,0);
            }
            else
            {
                t.pivot = new Vector2(0, 1);
            }
        }
        else
        {
            if (transform.position.y < 0)
            {
                t.pivot = new Vector2(1, 0);
            }
            else
            {
                t.pivot = new Vector2(1, 1);
            }
        }
    }
}
