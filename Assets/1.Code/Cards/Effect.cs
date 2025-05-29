using System;
using System.Collections;
using UnityEngine;
using VInspector;
using Game.CardEffects;

public class Effect : MonoBehaviour
{
    private int effectStrength;
    private int gadgetID;
    private CardEffectTargets cardEffectTarget;
    
    [Header("Effect Settings")]
    [Tooltip("The type of effect this card has.")]
    [SerializeField] private CardEffectTypes cardEffect = CardEffectTypes.None;
    
    //If Buff
    [ShowIf("cardEffect", CardEffectTypes.Buff)] 
    [SerializeField] private CardEffectTargets buffTarget = CardEffectTargets.Random;
    [SerializeField] private int buffAmount = 1;
    
    //If Carddraw
    [ShowIf("cardEffect", CardEffectTypes.CardDraw)] 
    [SerializeField] private int drawAmount = 1;
    [EndIf]
    
    //If Tinker
    [ShowIf("cardEffect", CardEffectTypes.Tinker)] 
    [SerializeField] private GadgetTypes gadget = GadgetTypes.None;
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
        if (cardEffect != CardEffectTypes.Tinker)
        {
            gadget = GadgetTypes.None;
            gadgetTarget = GadgetTargets.None;
        }
    }

    void Start()
    {
        switch (cardEffect)
        {
            case CardEffectTypes.Buff:
                effectStrength = buffAmount;
                cardEffectTarget = buffTarget;
                break;
            
            case CardEffectTypes.CardDraw:
                effectStrength = drawAmount;
                cardEffectTarget = CardEffectTargets.None;
                break;
            
            case CardEffectTypes.Tinker:
                effectStrength = 0;
                cardEffectTarget = CardEffectTargets.None;
                break;
            
        }
    }

    public IEnumerator DoEffect(int boardID)
    {
        GameObject targetCard = null;
        
        for (int i = 0; i <= repeatCount; i++)
        {
            if (cardEffectTarget != CardEffectTargets.None)
            {
                targetCard = BattleManager.instance.GetCardTarget(cardEffectTarget, boardID);
            }
            
            yield return StartCoroutine
                (CardEffects.DoEffect(cardEffect, targetCard, effectStrength, gadget, gadgetTarget));
        }
    }

    //This is called by the Card component
    public string GetCardEffectText()
    {
        string cardEffectText = CardEffectText.
            GetCardEffectText(cardEffect, cardEffectTarget, effectStrength, repeatCount, gadget, gadgetTarget);
        return cardEffectText;
    }
    
    /*
    protected string ConstructTinkerTargetText()
    {
        string effectTargetText = ""; 
        
        return effectTargetText;
    }
    */
}
