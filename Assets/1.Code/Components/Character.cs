using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;
using DG.Tweening;
using Game.Global;
using Game.CharacterClasses;

public class Character : MonoBehaviour
{
    //Values
    [HideInInspector] public int currentPower;
    private Card cardComponent;

    //Editor
    [Tab("Properties")]
    [Header("Character Values")]
    [SerializeField] private int ID;
    [SerializeField] private string title;
    [SerializeField] private Sprite image;
    [Space]
    [SerializeField] private int basePower;
    [SerializeField] private int baseEndurance;
    [Space] 
    [SerializeField] private Houses characterHouse = Houses.Cyber;
    [Space]
    public CharacterClasses characterClass1 = CharacterClasses.None;
    public CharacterClasses characterClass2 = CharacterClasses.None;
    public CharacterClasses characterClass3 = CharacterClasses.None;
    [EndTab]

    [Tab("References")]
    public TextMeshProUGUI titleText;
    private TextMeshProUGUI powerText;
    public TextMeshProUGUI effectText;
    public Image cardImage;
    public Image effectTextbox;
    public Image cardBackground;
    [Space] 
    public Transform dataHolder;
    public GameObject[] classIcons;
    [EndTab]
    
    private enum Houses
    {
        Cyber,
        Beast,
        Ying,
        Downer,
        Sludge
    }
    
    private enum Passives
    {
        None,
        Stealth,
        Slick
    }
    
    void Start()
    {
        
    }

    //This is called by BattleHandler -> InitializeDeck()
    public IEnumerator InitializeCharacter()
    {
        //Check if everything is properly set up
        if (titleText == null) Debug.LogError("Title text is missing!");
        if (effectText == null) Debug.LogError("Effect text is missing!");
        if (cardImage == null) Debug.LogError("Card image is missing!");
        
        yield return StartCoroutine(InitializeBase());
        yield return StartCoroutine(InitializeClasses());
        yield return StartCoroutine(InitializeHouses());
        yield return StartCoroutine(InitializeEffects());
    }
    
#region ------------Character Initialization------------//

    private IEnumerator InitializeBase()
    {
        currentPower = basePower;
        
        cardComponent = gameObject.GetComponent<Card>();
        
        titleText.text = title; //Write card title
        cardImage.sprite = image; // Set image sprite
        
        GameObject powerIcon = Instantiate(classIcons[0]);
        powerIcon.transform.SetParent(dataHolder);
        powerText = powerIcon.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        
        //Write Card Power into new PowerIcon
        powerText.text = currentPower.ToString();
        cardComponent.currentCardPower = currentPower;
       
        
        yield break;
    }
    private IEnumerator InitializeClasses()
    {
        // Add Class Icons
        GameObject nextClassIcon = Instantiate(classIcons[(int)characterClass1]);
        nextClassIcon.transform.SetParent(dataHolder);

        if (characterClass2 == CharacterClasses.None) yield break;
        nextClassIcon = Instantiate(classIcons[(int)characterClass2]);
        nextClassIcon.transform.SetParent(dataHolder);
        
        if (characterClass3 == CharacterClasses.None) yield break;
        nextClassIcon = Instantiate(classIcons[(int)characterClass3]);
        nextClassIcon.transform.SetParent(dataHolder);
        
    }

    private IEnumerator InitializeHouses()
    {
        Color houseColor = new Color(1, 1, 1, 1);
        
        switch (characterHouse)
        {
            case Houses.Cyber:
                ColorUtility.TryParseHtmlString("#5A5A5A", out houseColor);
                break;
            case Houses.Beast:
                ColorUtility.TryParseHtmlString("#E38C3F", out houseColor);
                cardBackground.color = Color.yellow;
                break;
            case Houses.Ying:
                ColorUtility.TryParseHtmlString("#5A5A5A", out houseColor);
                cardBackground.color = Color.red;
                break;
            case Houses.Downer:
                ColorUtility.TryParseHtmlString("#5A5A5A", out houseColor);
                cardBackground.color = Color.blue;
                break;
            case Houses.Sludge:
                ColorUtility.TryParseHtmlString("#5A5A5A", out houseColor);
                cardBackground.color = Color.green;
                break;
        }
        
        cardBackground.color = houseColor;
        yield break;
    }

    private IEnumerator InitializeEffects()
    {
        //Checks for effects
        Effect[] cardEffects = GetComponents<Effect>();
        
        if (cardEffects.Length == 0)
        {
            //Removes Effect textbox if there is no effect
            effectText.text = "";
            effectTextbox.rectTransform.sizeDelta = new Vector2(effectTextbox.rectTransform.sizeDelta.x, 45f);
            titleText.rectTransform.position = new Vector2(titleText.rectTransform.position.x, -225);
        }
        else
        {
            
            string combinedEffectText = "";
            foreach (Effect effect in cardEffects)
            {
                effect.InitializeCardEffect();
                combinedEffectText += effect.GetCardEffectText() + "\n";
            }

            // Sets EffectText
            effectText.text = combinedEffectText;
        }
        
        yield break;
    }

#endregion ------------Character Initialization------------//
    
    public void UpdatePower(int powerToAdd)
    {
        StartCoroutine(cardComponent.AnimateCard(CardAnimations.UpdatePower));
        
        currentPower += powerToAdd;

        if (currentPower == basePower) powerText.color = Color.white;
        if (currentPower > basePower) powerText.color = Color.green;
        if (currentPower < basePower) powerText.color = Color.red;
        
        powerText.text = currentPower.ToString();
        cardComponent.currentCardPower = currentPower;
    }

    public void CheckClassEffectOnAttack()
    {
        
    }
    
}
