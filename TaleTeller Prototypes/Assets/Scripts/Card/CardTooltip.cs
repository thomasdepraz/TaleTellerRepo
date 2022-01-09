using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardTooltip : MonoBehaviour
{
    public RectTransform self;
    public TextMeshProUGUI tooltipTextHidden;
    public TextMeshProUGUI tooltipTextVisible;
    private string text;
    private float tweeningValue;

    public void AppearTooltip(string key, int direction, float delay)
    {
        //Load text
        text = $"{UtilityClass.ToBold(key)}:\n {LocalizationManager.Instance.GetString(LocalizationManager.Instance.tooltipDictionary, key)} ";
        tooltipTextHidden.text = text;
        tooltipTextVisible.text = text;

        LeanTween.cancel(gameObject);

        //Tween text
        LeanTween.value(gameObject,0, 1, 0.5f).setDelay(delay).setOnUpdate((float value) =>
        {
            tweeningValue = value;

            if (direction > 0)
            {
                self.anchorMin = new Vector2(value, self.anchorMin.y);
                self.anchorMax = new Vector2(value + 1, self.anchorMax.y);
            }
            else
            {
                self.anchorMin = new Vector2(-value, self.anchorMin.y);
                self.anchorMax = new Vector2(1 - value, self.anchorMax.y);
            }
        }).setEaseInOutQuint().setOnStart(() => 
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        });
    }

    public void HideTooltip(float delay)
    {
        int direction = 0;
        if (self.anchorMin.x < 0) direction = -1;
        else direction = 1;

        LeanTween.cancel(gameObject);

        //Tween text
        LeanTween.value(gameObject,tweeningValue, 0, 0.5f).setDelay(delay).setOnUpdate((float value) =>
        {
            if (direction > 0)
            {
                self.anchorMin = new Vector2(value, self.anchorMin.y);
                self.anchorMax = new Vector2(value + 1, self.anchorMax.y);
            }
            else
            {
                self.anchorMin = new Vector2(-value, self.anchorMin.y);
                self.anchorMax = new Vector2(1 - value, self.anchorMax.y);
            }
        }).setEaseInOutQuint().setOnComplete(()=> 
        {
            gameObject.SetActive(false);
        });

        tweeningValue = 0;
    }
}
