using System;
using System.Collections;
using UnityEngine;
using VInspector;
using Game.CardEffects;

public class Effect : MonoBehaviour
{
    private int cardEffectStrength;
    private string gadgetName;
    private CardEffectTypes cardEffectType;
    private CardEffectTargets cardEffectTarget;
    private CardEffectData cardEffectData;
    
    [Header("Effect Settings")]
    [Tooltip("The type of effect this card has.")]
    [SerializeField] private CardEffectTypes effect = CardEffectTypes.None;
    
    //If Buff
    [ShowIf("effect", CardEffectTypes.Buff)] 
    [SerializeField] private CardEffectTargets buffTarget = CardEffectTargets.Random;
    [SerializeField] private int buffAmount = 1;
    
    //If Carddraw
    [ShowIf("effect", CardEffectTypes.CardDraw)] 
    [SerializeField] private int drawAmount = 1;
    [EndIf]
    
    //If Tinker
    [ShowIf("effect", CardEffectTypes.Tinker)] 
    [SerializeField] private GameObject gadgetPrefab;
    [SerializeField] private GadgetTargets gadgetTarget = GadgetTargets.None;
    [EndIf]
    [Space]
    // Repeat
    [SerializeField] private bool repeat;
    [ShowIf("repeat")] 
    [SerializeField] private int repeatCount;

    [EndIf]
    private void Awake()
    {
        if (!repeat) repeatCount = 0;
        if (cardEffectType != CardEffectTypes.Tinker)
        {
            gadgetName = "";
            gadgetTarget = GadgetTargets.None;
        }
    }
    
    public void InitializeCardEffect()
    {
        cardEffectType = effect;
        
        switch (cardEffectType)
        {
            case CardEffectTypes.Buff:
                cardEffectStrength = buffAmount;
                cardEffectTarget = buffTarget;
                break;
            
            case CardEffectTypes.CardDraw:
                cardEffectStrength = drawAmount;
                cardEffectTarget = CardEffectTargets.None;
                break;
            
            case CardEffectTypes.Tinker:
                cardEffectStrength = 0;
                gadgetName = gadgetPrefab.name;
                cardEffectTarget = CardEffectTargets.None;
                break;
            
        }
        
        ConstructEffectData(); 
    }

    private void ConstructEffectData()
    {
        cardEffectData = new CardEffectData
        {
            cardEffectType = this.cardEffectType,
            cardEffectTarget = this.cardEffectTarget,
            gadgetTarget = this.gadgetTarget,
            gadgetName = this.gadgetName,
            cardEffectStrength = this.cardEffectStrength,
            cardEffectUserBoardID = 0,
            cardEffectRepeatCount = repeatCount
        };
    }
    
    //This is called by the Character component
    public string GetCardEffectText()
    {
        string cardEffectText = CardEffectText.
            GetCardEffectText(cardEffectData);
        return cardEffectText;
    }

    public IEnumerator DoEffect(int boardID)
    {
        cardEffectData.cardEffectUserBoardID = boardID;
        
        for (int i = 0; i <= repeatCount; i++)
        {
            
            yield return StartCoroutine
                (CardEffects.DoEffect(cardEffectData));
        }
    }


    
    /*
    protected string ConstructTinkerTargetText()
    {
        string effectTargetText = ""; 
        
        return effectTargetText;
    }
    */
}
