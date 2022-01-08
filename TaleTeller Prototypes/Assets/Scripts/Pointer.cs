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

    public RectTransform tooltipPanel;
    public TextMeshProUGUI tooltipText;
    private bool tooltipOpen;

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
            tooltipPanel.position = transform.position;
        }
    }

    public void ShowTooltip(string description)
    {
        tooltipOpen = true;
        tooltipText.SetText(description);

        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, tooltipPanel.localScale.x, 1, 0.1f).setOnUpdate((float value) =>
        {
            tooltipPanel.localScale = new Vector3(value, value, 1);
        }).setEaseInOutQuint();
    }

    public void HideTooltip()
    {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, tooltipPanel.localScale.x, 0, 0.1f).setOnUpdate((float value) =>
        {
            tooltipPanel.localScale = new Vector3(value, value, 1);
        }).setEaseInOutQuint().setOnComplete(()=> 
        {
            tooltipOpen = false;
        });
    }
}
