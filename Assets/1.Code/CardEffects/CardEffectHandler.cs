using System.Collections;
using UnityEngine;
using Game.Global;
using Game.CardEffects;


public class CardEffectHandler : MonoBehaviour
{
    public static CardEffectHandler instance;
    
    float timeMult = Global.timeMult;
    
    private void Awake()
    {
        if  (instance == null) instance = this;
    }
    
    public IEnumerator DoEffect (CardEffectData cardEffectData)
    {
        
        GameObject targetCard = null;
        Vector3 gadgetTargetPos = Vector3.zero;
        int gadgetTargetSiblingID = 0;
        
        //Find targetCard gameObject
        if (cardEffectData.cardEffectTarget != CardEffectTargets.None)
        {
           
            targetCard = GetCardEffectTarget
                (
                    cardEffectData.cardEffectTarget,
                    cardEffectData.cardEffectUserBoardID
                );
        }

        if (cardEffectData.gadgetTarget != GadgetTargets.None)
        {
            (gadgetTargetPos, gadgetTargetSiblingID) = GetGadgetTarget
                (
                    cardEffectData.gadgetTarget,
                    cardEffectData.cardEffectUserBoardID
                );
        }
        
        switch (cardEffectData.cardEffectType)
        {
            case CardEffectTypes.Buff:
                yield return EffectBuff(targetCard, cardEffectData.cardEffectStrength);
                break;
            
            case CardEffectTypes.CardDraw:
                yield return EffectCardDraw(cardEffectData.cardEffectStrength);
                break;
            
            case CardEffectTypes.Tinker:
                yield return EffectTinker(cardEffectData.gadget.transform, gadgetTargetPos, gadgetTargetSiblingID);
                break;
                
        }
    }
  
#region ------------Card Effect Targets------------//
    
    /// <summary>
    /// This function takes a CardEffectTarget and the siblingID of the effect user in the Arena.
    /// It returns a target GameObject.
    /// </summary>
    private GameObject GetCardEffectTarget(CardEffectTargets effectTarget, int resolverBoardID)
    {
        GameObject targetCard = null;

        int randomTarget;

        switch (effectTarget)
        {
            case CardEffectTargets.Random:
                randomTarget = Random.Range(0, Global.arenaCardHolder.childCount);
                targetCard = Global.arenaCardHolder.GetChild(randomTarget).gameObject;
                break;
            case CardEffectTargets.Self:
                targetCard = Global.arenaCardHolder.GetChild(resolverBoardID).gameObject;
                break;
            case CardEffectTargets.Right:
                if (resolverBoardID + 1 == Global.arenaCardHolder.childCount) break; //Checks if the resolved card is not the most-right
                targetCard = Global.arenaCardHolder.GetChild(resolverBoardID + 1).gameObject;
                break;
        }
        return targetCard;
    }

    private (Vector3 gadgetTargetPos, int gadgetTargetSiblingID) 
        GetGadgetTarget(GadgetTargets gadgetTarget, int resolverBoardID)
    {
        Vector3 gadgetTargetPos = new Vector3(0, Utility.arenaBounds.offset.y, 0);
        int gadgetTargetSiblingID = 0;
        
        float[] newArenaPositions = Utility.CalculateCardPositions
        (
            Global.arenaCardHolder.childCount + 1, 
            Utility.arenaBounds.size.x,
            Utility.ARENA_CARDSPACING
        );

        switch (gadgetTarget)
        {
            case GadgetTargets.Random:
                gadgetTargetSiblingID = Random.Range(0, Global.arenaCardHolder.childCount);
                break;
            
            case GadgetTargets.Right:
                gadgetTargetSiblingID = resolverBoardID + 1;
                break;
        }
        
        gadgetTargetPos.x = newArenaPositions[gadgetTargetSiblingID];
        
        return (gadgetTargetPos, gadgetTargetSiblingID);
        
    }
    
#endregion ------------Card Effect Targets------------// 
    
#region ------------Card Effects------------//
    private IEnumerator EffectBuff(GameObject targetCard, int buffStrength)
    {
        if (targetCard == null) //No Target found
        {
            Debug.Log("No Buff target found.");
            yield break;
        }
        else
        {
            Debug.Log($"Buff {targetCard.name} for {buffStrength}.");
            if (targetCard.tag == "Character") 
                targetCard.GetComponent<Character>().UpdatePower(buffStrength); 
            
            else if (targetCard.tag == "Gadget") 
                targetCard.GetComponent<Gadget>().UpdatePower(buffStrength); 
        }
        
        
    }

    private IEnumerator EffectCardDraw(int drawAmount)
    {
        yield return Utility.DrawCardsToHand(drawAmount);
        
    }

    private IEnumerator EffectTinker
        (
            Transform gadgetCard, 
            Vector3 gadgetTargetPos, 
            int gadgetTargetSiblingID
        )
    
    {
        //Prepare gadget for Arena Placement
        gadgetCard.SetParent(Global.arenaCardHolder);
        gadgetCard.SetSiblingIndex(gadgetTargetSiblingID);
        gadgetCard.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
        
        //Set gadget into arena (invisible)
        yield return StartCoroutine (gadgetCard.gameObject.GetComponent<Card>().MoveCard
            (gadgetTargetPos, CardLocations.Arena, true));
        
        Utility.UpdateCardOrder(CardLocations.Arena, true);
        
        yield return StartCoroutine
            (gadgetCard.GetComponent<Card>().AnimateCard(CardAnimations.Appear));
        
    }
    
#endregion ------------Card Effects------------//    
    
    
    
    
    
    
}


