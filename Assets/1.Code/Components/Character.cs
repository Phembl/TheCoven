using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;
using DG.Tweening;
using Game.Global;

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
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI effectText;
    public Image cardImage;
    public Image effectTextbox;
    public Image cardBackground;
    [Space] 
    public Transform classHolder;
    public GameObject[] classIcons;
    [EndTab]
    
    //Enums


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
        currentPower = basePower;
        InitializeCharacter();
    }

    void InitializeCharacter()
    {
        //Check if everything is properly set up
        if (titleText == null) Debug.LogError("Title text is missing!");
        if (powerText == null) Debug.LogError("Power text is missing!");
        if (effectText == null) Debug.LogError("Effect text is missing!");
        if (cardImage == null) Debug.LogError("Card image is missing!");
        
        cardComponent = gameObject.GetComponent<Card>();
        
        titleText.text = title; //Write card title
        cardImage.sprite = image; // Set image sprite
        powerText.text = currentPower.ToString(); //Write Card Power

        InitializeClasses();
        InitializeHouses();
        InitializeEffects();




    }
    
#region ------------Character Initialization------------//

    private void InitializeClasses()
    {
        // Add Class Icons
        GameObject nextClassIcon = Instantiate(classIcons[(int)characterClass1]);
        nextClassIcon.transform.SetParent(classHolder);

        if (characterClass2 == CharacterClasses.None) return;
        nextClassIcon = Instantiate(classIcons[(int)characterClass2]);
        nextClassIcon.transform.SetParent(classHolder);
        
        if (characterClass3 == CharacterClasses.None) return;
        nextClassIcon = Instantiate(classIcons[(int)characterClass3]);
        nextClassIcon.transform.SetParent(classHolder);

    }

    private void InitializeHouses()
    {
        switch (characterHouse)
        {
            case Houses.Cyber:
                cardBackground.color = Color.black;
                break;
            case Houses.Beast:
                cardBackground.color = Color.yellow;
                break;
            case Houses.Ying:
                cardBackground.color = Color.red;
                break;
            case Houses.Downer:
                cardBackground.color = Color.blue;
                break;
            case Houses.Sludge:
                cardBackground.color = Color.green;
                break;
        }
    }

    private void InitializeEffects()
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
            Debug.Log("CardEffectTexts:\n" + combinedEffectText.Trim());
        }
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
    }
    
}
