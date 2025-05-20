using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    
    //Card Holders
    public Transform handCardHolder;
    public Transform deckCardHolder;
    public Transform arenaCardHolder;
    
    private enum CardTargets
    {
        Random,
        Self,
        Right,
        Left,
        All,
        DeckRandom,
        DeckAll,
        HandRandom,
        HandAll
    }
    
    private void Awake()
    {
        if  (instance == null) instance = this;
    }

    private void Start()
    {
        
    }

    public GameObject GetCardTarget(int targetID, int resolverBoardID)
    {
        GameObject target = null;
        CardTargets targetType = (CardTargets)targetID;

        int randomTarget;

        switch (targetType)
        {
            case CardTargets.Random:
                randomTarget = Random.Range(0, arenaCardHolder.childCount);
                target = arenaCardHolder.GetChild(randomTarget).gameObject;
                break;
            case CardTargets.Self:
                target = handCardHolder.GetChild(resolverBoardID).gameObject;
                break;
            case CardTargets.Right:
                if (resolverBoardID + 1 == arenaCardHolder.childCount) break; //Checks if the resolved card is not the most-right
                target = arenaCardHolder.GetChild(resolverBoardID + 1).gameObject;
                break;
        }
        return target;
    }

}
