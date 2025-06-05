using System;
using System.Collections;
using UnityEngine;
using VInspector;
using Game.CardEffects;
using Game.Global;

public class Effect : MonoBehaviour
{
    private int cardEffectStrength;
    private string gadgetName;
    private CardEffectTypes cardEffectType;
    private CardEffectTargets cardEffectTarget;
    private CardEffectData cardEffectData;
    
    private GameObject newGadget;
    
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
 
    
    public void InitializeCardEffect()
    {
        cardEffectType = effect;
        
        if (!repeat) repeatCount = 0;
        
        if (cardEffectType != CardEffectTypes.Tinker)
        {
            gadgetName = "";
            gadgetTarget = GadgetTargets.None;
        }
        
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
                gadgetName = gadgetPrefab.GetComponent<Gadget>().title;
                cardEffectTarget = CardEffectTargets.None;
                Vector3 newGadgetPos = new Vector3(-3000,0,0);
                newGadget = Instantiate
                    (
                        gadgetPrefab, 
                        newGadgetPos, 
                        Quaternion.identity
                    );
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
            gadget = newGadget,
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
                (CardEffectHandler.instance.DoEffect(cardEffectData));
        }
    }
    
}
