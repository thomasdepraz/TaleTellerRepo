using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreativityManager : MonoBehaviour
{
    [Header("Stats")]
    private int _creativity;
    private int _currentBoardCreativityCost;

    public int creativity
    {
        get => _creativity;
        set
        {
            _creativity = value;
            if (_creativity > maxCreativity)
                _creativity = maxCreativity;
            else if(_creativity <=0)
            {
                _creativity = 0;
                //Trigger game over*
                GameManager.Instance.GameOver();
            }

            creativityBar.fillAmount = (float)_creativity / (float)maxCreativity;
            secondaryCreativityBar.fillAmount = (float)_creativity / (float)maxCreativity;
            maskBar.fillAmount = (1f - creativityBar.fillAmount) + (float)_currentBoardCreativityCost / (float)maxCreativity ;
        }
    }
    public int maxCreativity;
    public int currentBoardCreativityCost
    {
        get=>_currentBoardCreativityCost;
        set
        {
            _currentBoardCreativityCost = value;

            maskBar.fillAmount = (1f - creativityBar.fillAmount) + (float)_currentBoardCreativityCost / (float)maxCreativity;

            if(creativity - _currentBoardCreativityCost <=0 )
            {
                //set button false
                GameManager.Instance.goButton.SetActive(false);
            }
            else
            {
                //set button true
                GameManager.Instance.goButton.SetActive(true);
            }
        }
    }

    [Header("References")]
    public Image creativityBar;
    public Image secondaryCreativityBar;
    public Image maskBar;


    // Start is called before the first frame update
    void Start()
    {
        //set creativity to max
        creativity = maxCreativity;

        
    }

}
