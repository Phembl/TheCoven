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
    
    [Header("Effect Settings")]
    [Tooltip("The type of effect this character card has.")]
    [SerializeField]
    private Effect characterEffect = Effect.None;

    [ShowIf("characterEffect", Effect.Buff)] 
    [SerializeField] private int buffValue;
    [SerializeField] private CardTargets buffTarget = CardTargets.Random;
    [ShowIf("buffTarget", CardTargets.Random)]
    [SerializeField] private int randomAmount;
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
        Hextech
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
        Summon
    }

    private enum CardTargets
    {
        Random,
        Self,
        Next,
        Last,
        All,
        DeckRandom,
        DeckAll,
        HandRandom,
        HandAll
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
                Effect_Buff effectComponent = gameObject.AddComponent<Effect_Buff>();
                effectComponent.buffAmount = buffValue;
                effectComponent.randomAmount = randomAmount;
                effectComponent.targetID = (int)buffTarget;
                string newEffectText = effectComponent.GetEffectText();
                effectText.text = newEffectText;
                break;
        }
        
    }

    void ConstructEffectText()
    {
        
    }

    
}
