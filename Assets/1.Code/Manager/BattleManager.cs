using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;
using Game.CardEffects;
using Game.Global;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    
    [Tab("Hand")]
    public float drawDuration = 0.5f; // Duration of draw animation
    public float delayBetweenDraws = 0.1f; // Delay between consecutive draws
    public int testCardsToDraw = 3;
    
    [Button("Draw Cards")]
    public void TestDrawButton()
    {
        StartCoroutine(Utility.DrawCardsToHand(testCardsToDraw));
    }
    [EndTab] 
    
    [Tab("Settings")]
    public Transform handCardHolder;
    public Transform deckCardHolder;
    public Transform arenaCardHolder;
    public Transform deckIcon;
    public Transform exhaustIcon;
    [Space] 
    public BoxCollider2D handBounds;
    public BoxCollider2D arenaBounds;
    [Space] 
    [Header("UI Settings")]
    public TextMeshProUGUI attackPowerText;
    [Space] 
    public float resolveSpeed = 1f;

    [EndTab] private float cardResolveScaleSpeed;
    
    private void Awake()
    {
        if  (instance == null) instance = this;
    }

    private void Start()
    {
        cardResolveScaleSpeed = resolveSpeed / 3;
    }
    
    public void ResolveButtonIsPressed()
    {
        StartCoroutine(ResolveBoard());
        
    }
    
    private IEnumerator ResolveBoard() 
    {
        Debug.Log("Resolving board");

        foreach (Transform nextCard in arenaCardHolder)
        {
            //Prepare Card
            Canvas nextCardCanvas = nextCard.GetChild(0).GetComponent<Canvas>();
            int nextCardOriginalSortingOrder = nextCardCanvas.sortingOrder;
            
            //Animate Card
            Card nextCardComponent = nextCard.GetComponent<Card>();
            yield return StartCoroutine(nextCardComponent.AnimateCard(CardAnimations.ResolveStart));
            
            // Get all Effect components on the next Card and resolves them
            Effect[] nextCardEffects = nextCard.GetComponents<Effect>();
            if (nextCardEffects.Length > 0)
            {
                int boardID = nextCard.transform.GetSiblingIndex();
                foreach (Effect effect in nextCardEffects)
                {
                    yield return StartCoroutine(effect.DoEffect(boardID));
                    yield return new WaitForSeconds(1f * Global.timeMult);
                }
                
            }
            
            else
            {
                Debug.Log("No effect");
                yield return new WaitForSeconds(0.5f * Global.timeMult);
            }
            
            
            //Finish Up Card
            yield return StartCoroutine(nextCardComponent.AnimateCard(CardAnimations.ResolveEnd));
        }
        
        yield return new WaitForSeconds(0.5f);
        
        
        
    }
    
    
    
}
