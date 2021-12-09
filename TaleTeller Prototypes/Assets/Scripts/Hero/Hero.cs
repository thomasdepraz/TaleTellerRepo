using TMPro;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("Stats")]
    public int heroReviveCost;

    [Header("References")]
    [SerializeField] public HeroBaseData heroData;

    [Header("UI")]
    public TextMeshProUGUI heroHpUI;
    public TextMeshProUGUI heroAttackUI;
    public TextMeshProUGUI heroGoldUI;

    //Private hero variables
    private int _maxLifePoints;
    private int _lifePoints;
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
        }
    }
    [HideInInspector]public int lifePoints 
    { 
        get => _lifePoints ;
        set
        {
            int diff = value - lifePoints;

            _lifePoints = value;

            if (_lifePoints > maxLifePoints)
                _lifePoints = maxLifePoints;
            else if (_lifePoints < 0)
                _lifePoints = 0;


            heroHpUI.text = lifePoints.ToString() + "/" + maxLifePoints;

            //Different feedack if damage taken or if healed
            if(diff != 0 && diff!= maxLifePoints)
                GameManager.Instance.storyManager.HeroLifeFeedback(diff);
        }
    }
    [HideInInspector]public int attackDamage
    {
        get => _attackDamage;
        set
        {
            _attackDamage = value;
            heroAttackUI.text = value.ToString();
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

        //Initialize graphics on story line
    }
    
    public void ReviveHero()
    {
        //Apply creativityCost of reviving
        GameManager.Instance.creativityManager.creativity -= heroReviveCost;

        //Reset hero stats
        lifePoints = maxLifePoints;
        bonusDamage = 0;
    }


    // Start is called before the first frame update
    void Start()
    {
        InitializeHero();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
