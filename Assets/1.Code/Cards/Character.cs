using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class Character : Card
{
    
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
    [SerializeField] private Effect characterEffect = Effect.None;
    
    //If Buff
    [ShowIf("characterEffect", Effect.Buff)] 
    [SerializeField] private CardTargets buffTarget = CardTargets.Random;
    [SerializeField] private int buffAmount = 1;
    
    //If Carddraw
    [ShowIf("characterEffect", Effect.CardDraw)] 
    [SerializeField] private int drawAmount = 1;
    [EndIf]
    
    //If Tinker
    [ShowIf("characterEffect", Effect.Tinker)] 
    [SerializeField] private Gadgets gadget = Gadgets.Bomb;
    [SerializeField] private SummonTargets tinkerTarget = SummonTargets.Right;
    
    // Repeat
    [SerializeField] private bool repeat;
    [ShowIf("repeat")] 
    [SerializeField] private int repeatAmount = 1;
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
    private enum Effect
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

    void Awake()
    {
        InitializeCard();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start(); // Runs Start in base class
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
        powerText.text = basePower.ToString(); //Write Card Power

        switch (characterEffect)
        {
            case Effect.None: 
                effectText.text = "";
                effectTextbox.rectTransform.sizeDelta = new Vector2(effectTextbox.rectTransform.sizeDelta.x, 45f);
                titleText.rectTransform.position = new Vector2(titleText.rectTransform.position.x, -225);
                break;
            case Effect.Individual:
                break;
            case Effect.Buff:
                Effect_Buff effectBuffComponent = gameObject.AddComponent<Effect_Buff>();
                effectBuffComponent.buffAmount = buffAmount;
                effectBuffComponent.repeatAmount = repeatAmount;
                effectBuffComponent.targetID = (int)buffTarget;
                string newEffectText = effectBuffComponent.GetEffectText();
                effectText.text = newEffectText;
                break;
            case Effect.CardDraw:
                Effect_CardDraw effectCardDrawComponent = gameObject.AddComponent<Effect_CardDraw>();
                effectCardDrawComponent.drawAmount = drawAmount;
                break;
            default:
                break;
                
        }
        
    }

    void ConstructEffectText()
    {
        
    }

    
}
