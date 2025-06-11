using System.Collections;
using Game.Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class Gadget : MonoBehaviour
{
    //Values
    [HideInInspector] public int currentPower;
    private Card cardComponent;
    
    //Editor
    [Tab("Properties")]
    [Header("Character Values")]
    [SerializeField] private int ID;
    public string title;
    [SerializeField] private Sprite image;
    [Space]
    [SerializeField] private int basePower;
    [SerializeField] private int baseEndurance;
    [EndTab]
    
    [Tab("References")]
    public TextMeshProUGUI titleText;
    private TextMeshProUGUI powerText;
    public TextMeshProUGUI effectText;
    public Image cardImage;
    public Image effectTextbox;
    public Image cardBackground;
    public Transform dataHolder;
    public GameObject powerIconPrefab;
    [EndTab]
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPower = basePower;
    }

    //This is called by CardEffectHandler -> EffectTinker()
    public IEnumerator InitializeGadget()
    {
        //Check if everything is properly set up
        if (titleText == null) Debug.LogError("Title text is missing!");
        if (powerText == null) Debug.LogError("Power text is missing!");
        if (effectText == null) Debug.LogError("Effect text is missing!");
        if (cardImage == null) Debug.LogError("Card image is missing!");
        
        yield return StartCoroutine(InitializeBase());
        yield return StartCoroutine(InitializeEffects());
        

    }

    private IEnumerator InitializeBase()
    {
        cardComponent = gameObject.GetComponent<Card>();
        
        titleText.text = title; //Write card title
        cardImage.sprite = image; // Set image sprite
        
        GameObject powerIcon = Instantiate(powerIconPrefab);
        powerIcon.transform.SetParent(dataHolder);
        powerText = powerIcon.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        
        //Write Card Power into new PowerIcon
        powerText.text = basePower.ToString();
        
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
                combinedEffectText += effect.GetCardEffectText() + "\n";
            }

            // Sets EffectText
            effectText.text = combinedEffectText;
            Debug.Log("CardEffectTexts:\n" + combinedEffectText.Trim());
        }

        yield break;
    }
    
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
