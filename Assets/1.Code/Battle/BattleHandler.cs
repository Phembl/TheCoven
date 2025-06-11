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
using Game.CharacterClasses;
using TMPro;

public class BattleHandler : MonoBehaviour
{
   
    [HideInInspector] public int currentEnemyHealth;
    [HideInInspector] public int currentDeckSize;
    [HideInInspector] public int currentExhaustSize;
    [HideInInspector] public bool isCurrentlyResolving;
    private Dictionary<CharacterClasses, int> activeClasses;
    
    public static BattleHandler instance;
    
    [Tab("Battle")]
    public GameObject currentEnemy;
    public Deck currentDeck;
    [Space] 
    [SerializeField] private int cardDrawStart;
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
    public Transform resolveIndicator;
    [Space] 
    [Header("UI References")]
    public TextMeshProUGUI attackPowerText;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI deckSizeText;
    public TextMeshProUGUI exhaustSizeText;
    [EndTab] 
    
    private float timeMult = Global.timeMult;

    private int cardsResolvedThisAttack;
    
    
    
    private void Awake()
    {
        if  (instance == null) instance = this;
    }

    private void Start()
    {
        activeClasses = new Dictionary<CharacterClasses, int>();
        StartCoroutine(InitializeBattle());
    }
    
#region ------------Battle Setup------------//

    private IEnumerator InitializeBattle()
    {
        attackPowerText.text = "";
        
        Debug.Log("---- BattleHandler: Initializing Deck Now ----");
        yield return StartCoroutine(InitializeDeck());
        
        Debug.Log("---- BattleHandler: Initializing Enemy Now ----");
        yield return StartCoroutine(InitializeEnemy());
        
        Debug.Log("---- BattleHandler: Initializing First Round Now ----");
        yield return StartCoroutine(StartNewRound(cardDrawStart));
    }

    private IEnumerator InitializeDeck()
    {
        UpdateCounter(BattleCounters.Deck, currentDeck.cardsInDeck.Length);
        UpdateCounter(BattleCounters.Exhaust, 0);
        
        foreach (GameObject nextCardToAdd in currentDeck.cardsInDeck)
        {
            GameObject nextCard = Instantiate(nextCardToAdd, deckCardHolder);
            yield return StartCoroutine
                (nextCard.GetComponent<Character>().InitializeCharacter());
            
            nextCard.transform.position = new Vector3(-3000,0,0);
            
            UpdateCounter(BattleCounters.Deck, 1);
        }
        
        yield break;
    }

    private IEnumerator InitializeEnemy()
    {
        GameObject nextEnemy = Instantiate(currentEnemy);
        Enemy nextEnemyComponent = nextEnemy.GetComponent<Enemy>();
        nextEnemy.name = ($"Enemy ({nextEnemyComponent.enemyName})");
        
        UpdateCounter(BattleCounters.EnemyHealth, nextEnemyComponent.enemyHealth);
        enemyNameText.text = nextEnemyComponent.enemyName;
        
        
        //INIT ENEMY ACTIONS
        
        
        yield break;
    }

    private IEnumerator StartNewRound(int cardsToDraw)
    {
        yield return StartCoroutine
            (Utility.DrawCardsToHand(cardsToDraw));
        
        cardsResolvedThisAttack = 0;
        isCurrentlyResolving = false;
        activeClasses.Clear();
        
    }
    
#endregion ------------Battle Setup------------//

#region ------------Board Resolve------------//
    
    public void ResolveButtonIsPressed()
    {
        StartCoroutine(ResolveArena());
    }
    
    private IEnumerator ResolveArena() 
    {
        isCurrentlyResolving = true;
        Debug.Log("---- BattleHandler: Resolving Card Effects Now ----");
        yield return StartCoroutine(ResolveCards());
        yield return new WaitForSeconds(0.2f * timeMult);
        
        Debug.Log("---- BattleHandler: Resolving Card Attacks Now ----");
        yield return StartCoroutine(ResolvePower());
        yield return new WaitForSeconds(0.2f * timeMult);
        
        Debug.Log("---- BattleHandler: Exhausting Card Attacks Now ----");
        yield return StartCoroutine(ExhaustCards());
        yield return new WaitForSeconds(0.2f * timeMult);
        
        Debug.Log("---- BattleHandler: Starting New Round Now ----");
        yield return StartCoroutine(StartNewRound(cardsResolvedThisAttack));
        yield return new WaitForSeconds(0.2f * timeMult);
        
    }

    private IEnumerator ResolveCards()
    {
        int index = 0;
        int cardsToResolve = arenaCardHolder.childCount;

        while (index < cardsToResolve)
        {
            int currentIndex = index;
            GameObject currentCard = arenaCardHolder.GetChild(index).gameObject;
            
            Vector3 indicatorPos = new Vector3(currentCard.transform.position.x, resolveIndicator.position.y, resolveIndicator.position.z);
            resolveIndicator.position = indicatorPos;

            yield return StartCoroutine(currentCard.GetComponent<Card>().ResolveCardEffects());
            
            int updatedIndex = currentCard.transform.GetSiblingIndex();

            // If new cards were inserted before this one, adjust index accordingly
            if (updatedIndex > currentIndex)
            {
                index = updatedIndex + 1;
            }
            else
            {
                index++; // move to next card as usual
            }

            cardsToResolve = arenaCardHolder.childCount; // in case more were added
            
            yield return new WaitForSeconds(0.3f * timeMult);
        }
        
        Vector3 indicatorPosIdle = new Vector3(3000, resolveIndicator.position.y, resolveIndicator.position.z);
        resolveIndicator.position = indicatorPosIdle;
        
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
        
        while (arenaCardHolder.childCount > 0)
        {
            GameObject nextCard = arenaCardHolder.GetChild(0).gameObject;
            
            yield return StartCoroutine(nextCard.GetComponent<Card>().AnimateCard(CardAnimations.Exhaust));
            Utility.AddCardToExhaust(nextCard.gameObject);
            
            yield return new WaitForSeconds(0.3f * timeMult);

            cardsResolvedThisAttack++;
        }
        
       
      
    }
    
#endregion ------------Board Resolve------------//
    
#region ------------Battle States------------//

    public void UpdateClassesInArena(GameObject lastPlayedCard)
    {
        if (!lastPlayedCard.CompareTag("Character")) return;
        Character character = lastPlayedCard.GetComponent<Character>();
        
        // Check all 3 class slots
        AddClassToDictionary(activeClasses, character.characterClass1);
        AddClassToDictionary(activeClasses, character.characterClass2);
        AddClassToDictionary(activeClasses, character.characterClass3);
        
        Debug.Log("---- BattleHandler: The Arena has the following classes:----");
        foreach (var entry in activeClasses)
        {
            Debug.Log($"{entry.Key}: {entry.Value}");
        }
        
        //CharacterClassHandler.UpdateCharacterClassEffects(activeClasses);
    }
    
    private void AddClassToDictionary(Dictionary<CharacterClasses, int> dict, CharacterClasses characterClass)
    {
        if (characterClass == CharacterClasses.None) return;

        if (dict.ContainsKey(characterClass))
            dict[characterClass]++;
        else
            dict[characterClass] = 1;
    }

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
