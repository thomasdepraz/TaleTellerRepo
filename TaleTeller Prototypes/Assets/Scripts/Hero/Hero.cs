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

    [HideInInspector]public AudioSource audioSource;

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

    [HideInInspector]public int lifePoints 
    { 
        get => _lifePoints ;
        set
        {
            int diff = value - lifePoints;

            _lifePoints = value;

            if (_lifePoints < 0)
                _lifePoints = 0;
            else if (_lifePoints > maxLifePoints)
                _lifePoints = maxLifePoints;

            heroHpUI.text = (lifePoints).ToString() + "/" + maxLifePoints;

            FrameTweening(hpFrame.gameObject);
        }
    }
    [HideInInspector]public int attackDamage
    {
        get => _attackDamage;
        set
        {
            _attackDamage = value;

            if (_bonusDamage != 0)
                heroAttackUI.text = value.ToString() + " + " + _bonusDamage.ToString();
            else
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
            if(value>_goldPoints)
            {
                if(audioSource == null)audioSource = SoundManager.Instance.GenerateAudioSource(gameObject);
                Sound gold = new Sound(audioSource, "SFX_GAINGOLD", SoundType.SFX, false, false);
                gold.Play();
            }
            
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
