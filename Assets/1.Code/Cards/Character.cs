using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class Character : Card
{
    
    [Tab("Properties")]
    [Header("Card Properties")]
    [SerializeField] private int ID;
    [SerializeField] private string title;
    [SerializeField] private Sprite image;
    [SerializeField] private int basePower;
    [SerializeField] private int baseEndurance;
    
    private enum Effect
    {
        None,       
        Individual,
        Buff
    }
    
    [Header("Effect Settings")]
    [Tooltip("The type of effect this character card has.")]
    [SerializeField]
    private Effect characterEffect = Effect.None;
    
    [EndTab]

    [Tab("References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI effectText;
    public Image cardImage;
    [EndTab]
    
    

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
