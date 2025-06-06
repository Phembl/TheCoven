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

public class BattleHandler : MonoBehaviour
{
    //Enemy
    [HideInInspector] public int currentEnemyHealth;
    [HideInInspector] public int currentDeckSize;
    [HideInInspector] public int currentExhaustSize;
    
    public static BattleHandler instance;
    
    [Tab("Battle")]
    public GameObject currentEnemy;
    public Deck currentDeck;
    [EndTab]
    
    [Tab("Hand")]
    public float delayBetweenDraws = 0.1f * Global.timeMult; // Delay between consecutive draws
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
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI deckSizeText;
    public TextMeshProUGUI exhaustSizeText;
    [EndTab] 
    
    private float timeMult = Global.timeMult;
    
    
    private void Awake()
    {
        if  (instance == null) instance = this;
    }

    private void Start()
    {
        InitializeBattle();
    }
    
#region ------------Battle Setup------------//

    private void InitializeBattle()
    {
        attackPowerText.text = "";
        
        InitializeDeck();
        InitializeEnemy();
    }

    private void InitializeDeck()
    {
        UpdateCounter(BattleCounters.Deck, currentDeck.cardsInDeck.Length);
        UpdateCounter(BattleCounters.Exhaust, 0);
        
        foreach (GameObject nextCardToAdd in currentDeck.cardsInDeck)
        {
            GameObject nextCard = Instantiate(nextCardToAdd, deckCardHolder);
            nextCard.transform.position = new Vector3(-3000,0,0);
            
            UpdateCounter(BattleCounters.Deck, 1);
        }
    }

    private void InitializeEnemy()
    {
        Debug.Log("Initializing Enemy");
        
        GameObject nextEnemy = Instantiate(currentEnemy);
        Enemy nextEnemyComponent = nextEnemy.GetComponent<Enemy>();
        nextEnemy.name = ($"Enemy ({nextEnemyComponent.enemyName})");
        
        UpdateCounter(BattleCounters.EnemyHealth, nextEnemyComponent.enemyHealth);
        enemyNameText.text = nextEnemyComponent.enemyName;
        
        
        //INIT ENEMY ACTIONS
    }
    
#endregion ------------Battle Setup------------//
    

#region ------------Board Resolve------------//
    
    public void ResolveButtonIsPressed()
    {
        StartCoroutine(ResolveArena());
    }
    
    private IEnumerator ResolveArena() 
    {
        Debug.Log("Resolving Effects");
        yield return StartCoroutine(ResolveEffects());
        yield return new WaitForSeconds(0.2f * timeMult);
        yield return StartCoroutine(ResolvePower());
        yield return new WaitForSeconds(0.2f * timeMult);
        yield return StartCoroutine(ExhaustCards());


    }
    
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
                    yield return new WaitForSeconds(0.5f * timeMult);
                }
                
            }
            
            else
            {
                Debug.Log("No effect");
                yield return new WaitForSeconds(0.2f * timeMult);
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
            
            // Animate Attack
            StartCoroutine(nextCardComponent.AnimateCard(CardAnimations.Attack));

            // Get Power
            if (nextCard.tag == "Character")
            {
                cardPower = nextCard.GetComponent<Character>().currentPower;  
            }
            
            else if (nextCard.tag == "Gadget")
            {
                cardPower = nextCard.GetComponent<Gadget>().currentPower;  
            }
            
            // Attack Enemy
            currentEnemyHealth -= cardPower;
            enemyHealthText.text = currentEnemyHealth.ToString();
            
            // Check Win
            if (currentEnemyHealth <= 0) WinBattle();
            
            
            yield return new WaitForSeconds(0.5f * timeMult);
        }
    }

    private IEnumerator ExhaustCards()
    {
        Debug.Log(arenaCardHolder.childCount);
        
        foreach (Transform nextCard in arenaCardHolder)
        {
            StartCoroutine(nextCard.GetComponent<Card>().AnimateCard(CardAnimations.Exhaust));
            
            UpdateCounter(BattleCounters.Exhaust, 1);
            
            yield return new WaitForSeconds(0.5f * timeMult);
        }

        foreach (Transform nextCard in arenaCardHolder)
        {
            Utility.AddCardToExhaust(nextCard.gameObject);
        }
      
    }
    
#endregion ------------Board Resolve------------//
    
#region ------------Battle States------------//

    public void UpdateCounter(BattleCounters counter, int changeValue)
    {
        TextMeshProUGUI textField = null;
        int newValue = 0;
        
        switch (counter)
        {
            case BattleCounters.Deck:
                textField = deckSizeText;
                newValue = currentDeckSize + changeValue;
                currentDeckSize = newValue;
                break;
            
            case BattleCounters.Exhaust:
                textField = exhaustSizeText;
                newValue = currentExhaustSize + changeValue;
                currentExhaustSize = newValue;
                break;
            
            case BattleCounters.EnemyHealth:
                textField = enemyHealthText;
                newValue = currentEnemyHealth + changeValue;
                currentEnemyHealth = newValue;
                break;
        }
        
        textField.text = newValue.ToString();
    }
    
    #endregion ------------Battle States------------//


    private void WinBattle()
    {
        Debug.Log("Winning Battle");
    }
    
}
