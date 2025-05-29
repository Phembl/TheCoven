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
    [SerializeField] private GadgetTypes gadget = GadgetTypes.Bomb;
    [SerializeField] private GadgetTargets tinkerTarget = GadgetTargets.Right;
    [EndIf]
    [Space]
    // Repeat
    [SerializeField] private bool repeat;
    [ShowIf("repeat")] 
    [SerializeField] private int repeatCount;
    [EndIf]
    
   
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
            
        }
    }

    public IEnumerator DoEffect(int boardID)
    {
        yield return new WaitForSeconds(1f);
    }

    //This is called by the Card component
    public string GetCardEffectText()
    {
        string cardEffectText = CardEffectText.
            GetCardEffectText(cardEffect, cardEffectTarget, effectStrength, repeatCount);
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
