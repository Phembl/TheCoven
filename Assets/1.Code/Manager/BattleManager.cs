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
    [Space] 
    public BoxCollider2D handBounds;
    public BoxCollider2D arenaBounds;
    [EndTab] 
    
    private void Awake()
    {
        if  (instance == null) instance = this;
    }
    
    

    private void Start()
    {
        
    }
    
}
