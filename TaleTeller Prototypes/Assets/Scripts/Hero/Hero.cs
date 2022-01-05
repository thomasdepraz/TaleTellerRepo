using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public HeroBaseData heroData;

    [Header("UI")]
    public Image hpFrame;
    public Image attackFrame;
    public Image goldFrame;
    [Space]
    public TextMeshProUGUI heroHpUI;
    public TextMeshProUGUI heroOverHealPointsUI;
    public TextMeshProUGUI heroAttackUI;
    public TextMeshProUGUI heroGoldUI;


    //Private hero variables
    private int _maxLifePoints;
    private int _lifePoints;
    private int _overHealPoints;
    private int _attackDamage;
    private int _bonusDamage;
    private int _maxGoldPoints;
    private int _goldPoints;

    [HideInInspector]public int maxLifePoints
    { 
        get => _maxLifePoints ; 
        set 
        {
            _maxLifePoints = value;
            heroHpUI.text = lifePoints + "/" + value.ToString();
            FrameTweening(hpFrame.gameObject);
        }
    }

    /*[HideInInspector] public int overHealPoints
    {
        get => _overHealPoints;
        set
        {
            _overHealPoints = value;
            heroHpUI.text = (lifePoints + _overHealPoints) + "/" + value.ToString();
            
            //Faire feedback spécial pour Overheal
            //FrameTweening(hpFrame.gameObject);
        }
    }*/

    [HideInInspector]public int lifePoints 
    { 
        get => _lifePoints ;
        set
        {
            int diff = value - lifePoints;
            int hPLeftToHeal = diff;

            if(hPLeftToHeal > 0)
            {
                #region Overheal
                if (_lifePoints < _maxLifePoints)
                {
                    if(hPLeftToHeal < _maxLifePoints - _lifePoints)
                    {
                        _lifePoints += hPLeftToHeal;
                        hPLeftToHeal = 0;
                    }
                    else
                    {
                        hPLeftToHeal -= _maxLifePoints - _lifePoints;
                        _lifePoints = _maxLifePoints;

                    }
                }

                if(_lifePoints < (int)Mathf.Ceil(_maxLifePoints * 1.5f))
                {
                    if((int)Mathf.Ceil(hPLeftToHeal / 2f) < (int)Mathf.Ceil(_maxLifePoints * 1.5f) - _lifePoints)
                    {
                        _lifePoints += (int)Mathf.Ceil(hPLeftToHeal / 2f);
                        hPLeftToHeal = 0;
                    }
                    else
                    {
                        hPLeftToHeal -= ((int)Mathf.Ceil(_maxLifePoints * 1.5f) - _lifePoints) * 2;
                        _lifePoints = (int)Mathf.Ceil(_maxLifePoints * 1.5f);
                    }
                }

                if (_lifePoints < (int)Mathf.Ceil(_maxLifePoints * 2f))
                {
                    if ((int)Mathf.Ceil(hPLeftToHeal / 4f) < (int)Mathf.Ceil(_maxLifePoints * 2f) - _lifePoints)
                    {
                        _lifePoints += (int)Mathf.Ceil(hPLeftToHeal / 4f);
                        hPLeftToHeal = 0;
                    }
                    else
                    {
                        hPLeftToHeal -= ((int)Mathf.Ceil(_maxLifePoints * 2f) - _lifePoints) * 4;
                        _lifePoints = (int)Mathf.Ceil(_maxLifePoints * 2f);
                    }
                }

                hPLeftToHeal = 0;
                #endregion
            }
            else
            {
                _lifePoints = value;
            }

            if (_lifePoints < 0)
                _lifePoints = 0;

            heroHpUI.text = (lifePoints).ToString() + "/" + maxLifePoints;

            //Different feedack if damage taken or if healed
            if(diff != 0 && diff!= maxLifePoints)
                GameManager.Instance.storyManager.HeroLifeFeedback(hPLeftToHeal);

            FrameTweening(hpFrame.gameObject);
        }
    }
    [HideInInspector]public int attackDamage
    {
        get => _attackDamage;
        set
        {
            _attackDamage = value;
            heroAttackUI.text = value.ToString();
            FrameTweening(attackFrame.gameObject);
        }
    }
    [HideInInspector]public int bonusDamage
    {
        get => _bonusDamage;
        set
        {
            _bonusDamage = value;

            if(value != 0)
                heroAttackUI.text = _attackDamage.ToString() + " + " + value.ToString();
            else
                heroAttackUI.text = _attackDamage.ToString();

            FrameTweening(attackFrame.gameObject);
        }
    }

    [HideInInspector]public int armor;

    [HideInInspector]public int maxGoldPoints
    {
        get => _maxGoldPoints;
        set
        {
            _maxGoldPoints = value;
            heroGoldUI.text = goldPoints + "/" + value.ToString();
            FrameTweening(goldFrame.gameObject);
        }
    }

    [HideInInspector]public int goldPoints
    {
        get => _goldPoints;
        set
        {
            _goldPoints = value;
            

            if (_goldPoints > maxGoldPoints)
                _goldPoints = maxGoldPoints;
            else if (_goldPoints < 0)
                _goldPoints = 0;

            heroGoldUI.text = goldPoints.ToString() + "/" + maxGoldPoints;
            FrameTweening(goldFrame.gameObject);
        }
    }

    public void InitializeHero()
    {
        maxLifePoints = heroData.baseLifePoints;
        lifePoints = heroData.baseLifePoints;
        attackDamage = heroData.baseAttackDamage;
        armor = 0;
        bonusDamage = 0;
        goldPoints = heroData.baseGold;
        maxGoldPoints = heroData.baseMaxGold;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeHero();
    }

    public void FrameTweening(GameObject frame)
    {
        LeanTween.scale(frame, new Vector3(1.1f, 1.1f, 1.1f), 0.2f).setEaseInOutCubic().setLoopPingPong(1);
    }
}
