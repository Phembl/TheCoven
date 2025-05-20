using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;
using DG.Tweening;

public class Character : Card
{
    private int currentPower;
    private int effectStrength;
    private int effectTargetID;
    
    [Tab("Properties")]
    [Header("Character Values")]
    [SerializeField] private int ID;
    [SerializeField] private string title;
    [SerializeField] private Sprite image;
    [Space]
    [SerializeField] private int basePower;
    [SerializeField] private int baseEndurance;
    [Space] 
    [SerializeField] private Class characterClass = Class.Brawler;
    [SerializeField] private Tribe characterTribe = Tribe.Cyber;
    [EndTab]
    
    [Tab("Effect")]
    [Header("Effect Settings")]
    [SerializeField] private Passive characterPassive = Passive.None;
    
    [Tooltip("The type of effect this character card has.")]
    [SerializeField] private EffectType characterEffect = EffectType.None;
    
    //If Buff
    [ShowIf("characterEffect", EffectType.Buff)] 
    [SerializeField] private CardTargets buffTarget = CardTargets.Random;
    [SerializeField] private int buffAmount = 1;
    
    //If Carddraw
    [ShowIf("characterEffect", EffectType.CardDraw)] 
    [SerializeField] private int drawAmount = 1;
    [EndIf]
    
    //If Tinker
    [ShowIf("characterEffect", EffectType.Tinker)] 
    [SerializeField] private Gadgets gadget = Gadgets.Bomb;
    [SerializeField] private SummonTargets tinkerTarget = SummonTargets.Right;
    [EndIf]
    
    // Repeat
    [SerializeField] private bool repeat;
    [ShowIf("repeat")] 
    [SerializeField] private int repeatCount = 1;
    [EndIf]
    
    [EndTab]

    [Tab("References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI effectText;
    public Image cardImage;
    public Image effectTextbox;
    [EndTab]
    
    //Enums
    private enum Class
    {
        Brawler,
        Medic,
        Soldier,
        Hacker,
        Sniper,
        Trickster,
        Tinkerer,
        Technician,
        Hexxe
    }

    private enum Tribe
    {
        Cyber,
        Beast
    }
    private enum EffectType
    {
        None,       
        Individual,
        Buff,
        CardDraw,
        Tinker,
        Augment,
        Hex
    }

    private enum Passive
    {
        None,
        Stealth,
        Slick
    }

    private enum CardTargets
    {
        Random,
        Self,
        Right,
        Left,
        All,
        DeckRandom,
        DeckAll,
        HandRandom,
        HandAll
    }

    private enum SummonTargets
    {
        Right,
        Left,
        Random
    }

    private enum Gadgets
    {
        Bomb,
        Placeholder
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start(); // Runs Start in base class
        currentPower = basePower;
        InitializeCard();
    }

    void InitializeCard()
    {
        //Check if everything is properly set up
        if (titleText == null) Debug.LogError("Title text is missing!");
        if (powerText == null) Debug.LogError("Power text is missing!");
        if (effectText == null) Debug.LogError("Effect text is missing!");
        if (cardImage == null) Debug.LogError("Card image is missing!");
        
        titleText.text = title; //Write card title
        cardImage.sprite = image; // Set image sprite
        powerText.text = currentPower.ToString(); //Write Card Power

        string newEffectText = "";
        switch (characterEffect)
        {
            case EffectType.None: 
                effectText.text = "";
                effectTextbox.rectTransform.sizeDelta = new Vector2(effectTextbox.rectTransform.sizeDelta.x, 45f);
                titleText.rectTransform.position = new Vector2(titleText.rectTransform.position.x, -225);
                break;
            case EffectType.Individual:
                break;
            case EffectType.Buff:
                Effect_Buff effectBuffComponent = gameObject.AddComponent<Effect_Buff>();
                effectStrength = buffAmount;
                effectTargetID = (int)buffTarget;
                break;
            case EffectType.CardDraw:
                Effect_CardDraw effectCardDrawComponent = gameObject.AddComponent<Effect_CardDraw>();
                effectStrength = drawAmount;
                effectTargetID = 99; // Irrelevant
                break;
            default:
                break;
                
        }

        if (characterEffect != EffectType.None)
        {
            Effect effectComponent = gameObject.GetComponent<Effect>();
            effectComponent.strength = effectStrength;
            effectComponent.targetID = effectTargetID;
        
            if (!repeat) effectComponent.repeatCount = 0;
            else effectComponent.repeatCount = repeatCount;
        
            newEffectText = effectComponent.GetEffectText();
            effectText.text = newEffectText;
        }
        

    }

    void ConstructEffectText()
    {
        
    }

    public void UpdatePower(int powerToAdd)
    {
        currentPower += powerToAdd;

        if (currentPower == basePower) powerText.color = Color.white;
        if (currentPower > basePower) powerText.color = Color.green;
        if (currentPower < basePower) powerText.color = Color.red;


        transform.DOShakeRotation(0.1f, new Vector3(0, 0, 10));
        powerText.text = currentPower.ToString();
    }
    
}
