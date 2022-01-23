using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SchemeDescription : MonoBehaviour
{
    public Image illustration;
    public CardContainer cardContainer;
    public TextMeshProUGUI description;
    [HideInInspector] public MainPlotScheme linkedScheme;

    //TODO implement tweening feedback here
    public void LoadIllustration()
    {
        illustration.gameObject.SetActive(true);
        cardContainer.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    public void LoadCardContainer()
    {
        illustration.gameObject.SetActive(false);
        cardContainer.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }
}
