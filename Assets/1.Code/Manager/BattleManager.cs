using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using VInspector;
using Random = UnityEngine.Random;
using Game.CardEffects;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    
    //Card Holders
    public Transform handCardHolder;
    public Transform deckCardHolder;
    public Transform arenaCardHolder;
    
    private void Awake()
    {
        if  (instance == null) instance = this;
    }

    private void Start()
    {
        
    }

    public GameObject GetCardTarget(CardEffectTargets effectTarget, int resolverBoardID)
    {
        GameObject targetCard = null;

        int randomTarget;

        switch (effectTarget)
        {
            case CardEffectTargets.Random:
                randomTarget = Random.Range(0, arenaCardHolder.childCount);
                targetCard = arenaCardHolder.GetChild(randomTarget).gameObject;
                break;
            case CardEffectTargets.Self:
                targetCard = handCardHolder.GetChild(resolverBoardID).gameObject;
                break;
            case CardEffectTargets.Right:
                if (resolverBoardID + 1 == arenaCardHolder.childCount) break; //Checks if the resolved card is not the most-right
                targetCard = arenaCardHolder.GetChild(resolverBoardID + 1).gameObject;
                break;
        }
        return targetCard;
    }

}
