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
    //Enemy
    private int currentEnemyHealth;
    
    public static BattleManager instance;
    
    [Tab("Hand")]
    public float delayBetweenDraws = 0.1f; // Delay between consecutive draws
    [EndTab] 
    
    [Tab("Settings")]
    [Header("Card Holder References")]
    public Transform handCardHolder;
    public Transform deckCardHolder;
    public Transform arenaCardHolder;
    public Transform exhaustCardHolder;
    public Transform deckIcon;
    public Transform exhaustIcon;
    [Space] 
    [Header("Area References")]
    public BoxCollider2D handBounds;
    public BoxCollider2D arenaBounds;
    [Space] 
    [Header("UI References")]
    public TextMeshProUGUI attackPowerText;
    public TextMeshProUGUI enemyHealthText;
    [Space] 
    [Header("Component References")]
    public Enemy currentEnemy;
    public Deck currentDeck;
    [EndTab]
    
    private void Awake()
    {
        if  (instance == null) instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartBattle());
    }

    private IEnumerator StartBattle()
    {
        foreach (GameObject nextCardToAdd in currentDeck.cardsInDeck)
        {
            GameObject nextCard = Instantiate(nextCardToAdd, deckCardHolder);
            nextCard.transform.position = new Vector3(-3000,0,0);
            
        }
        currentEnemyHealth = currentEnemy.enemyHealth;
            
        attackPowerText.text = "";
        enemyHealthText.text = currentEnemyHealth.ToString();

        yield break;
    }
    
    public void ResolveButtonIsPressed()
    {
        StartCoroutine(ResolveArena());
    }
    
    private IEnumerator ResolveArena() 
    {
        Debug.Log("Resolving Effects");
        yield return StartCoroutine(ResolveEffects());
        yield return new WaitForSeconds(0.2f * Global.timeMult);
        yield return StartCoroutine(ResolvePower());
        yield return new WaitForSeconds(0.2f * Global.timeMult);
        yield return StartCoroutine(ExhaustCards());


    }
    #region ------------Board Resolve------------//
    
    private IEnumerator ResolveEffects()
    {
        foreach (Transform nextCard in arenaCardHolder)
        {
            
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
                    yield return new WaitForSeconds(0.5f * Global.timeMult);
                }
                
            }
            
            else
            {
                Debug.Log("No effect");
                yield return new WaitForSeconds(0.2f * Global.timeMult);
            }
            
            //Finish Up Card
            yield return StartCoroutine(nextCardComponent.AnimateCard(CardAnimations.ResolveEnd));
        }
        
       
    }

    private IEnumerator ResolvePower()
    {
        int powerSum = 0;
        
        foreach (Transform nextCard in arenaCardHolder)
        {
            int cardPower = 0;
            Card nextCardComponent = nextCard.GetComponent<Card>();
            
            StartCoroutine(nextCardComponent.AnimateCard(CardAnimations.Attack));

            if (nextCard.tag == "Character")
            {
                cardPower = nextCard.GetComponent<Character>().currentPower;  
            }
            
            else if (nextCard.tag == "Gadget")
            {
                cardPower = nextCard.GetComponent<Gadget>().currentPower;  
            }
            
       
            currentEnemyHealth -= cardPower;
            enemyHealthText.text = currentEnemyHealth.ToString();
            
            if (currentEnemyHealth <= 0) WinBattle();
            
            
            yield return new WaitForSeconds(0.5f * Global.timeMult);
        }
    }

    private IEnumerator ExhaustCards()
    {
        foreach (Transform nextCard in arenaCardHolder)
        {
            nextCard.GetComponent<Card>().MoveCard(exhaustIcon.position, CardLocations.Exhaust, false);
            yield return new WaitForSeconds(0.2f * Global.timeMult);
        }
        yield break;
    }
    
    #endregion ------------Board Resolve------------//


    private void WinBattle()
    {
        Debug.Log("Winning Battle");
    }
    
}
