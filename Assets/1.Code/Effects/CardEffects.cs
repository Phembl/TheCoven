using System.Collections;
using UnityEngine;
using Game.Global;

namespace Game.CardEffects
{
    public static class CardEffects
    {
        
        private static Transform arenaCardHolder = BattleManager.instance.arenaCardHolder;
        
        public static IEnumerator DoEffect (CardEffectData cardEffectData)
        {
            
            //float waitAfterEffect = 0.2f * Global.timeMult;
            
            GameObject targetCard = null;
            
            //Find targetCard gameObject
            if (cardEffectData.cardEffectTarget != CardEffectTargets.None)
            {
                //targetCard = BattleManager.instance.GetCardTarget
                targetCard = GetCardEffectTarget
                    (
                        cardEffectData.cardEffectTarget,
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
                    yield return EffectCardDraw(cardEffectData.cardEffectStrength);
                    break;
                    
            }
            
            yield return new WaitForSeconds(0.5f);
        }
      
#region ------------Card Effect Targets------------//
        
        /// <summary>
        /// This function takes a CardEffectTarget and the siblingID of the effect user in the Arena.
        /// It returns a target GameObject.
        /// </summary>
        public static GameObject GetCardEffectTarget(CardEffectTargets effectTarget, int resolverBoardID)
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
                    targetCard = arenaCardHolder.GetChild(resolverBoardID).gameObject;
                    break;
                case CardEffectTargets.Right:
                    if (resolverBoardID + 1 == arenaCardHolder.childCount) break; //Checks if the resolved card is not the most-right
                    targetCard = arenaCardHolder.GetChild(resolverBoardID + 1).gameObject;
                    break;
            }
            return targetCard;
        } 
#endregion ------------Card Effect Targets------------// 
        
#region ------------Card Effects------------//
        private static IEnumerator EffectBuff(GameObject targetCard, int buffStrength)
        {
            Debug.Log($"Buff {targetCard.name} for {buffStrength}.");
            if (targetCard.tag == "Character") targetCard.GetComponent<Character>().UpdatePower(buffStrength); 
            else if (targetCard.tag == "Gadget") targetCard.GetComponent<Gadget>().UpdatePower(buffStrength); 
            yield break;
        }

        private static IEnumerator EffectCardDraw(int drawAmount)
        {
            yield return Utility.DrawCardsToHand(drawAmount);
            
        }

        private static IEnumerator EffectTinker(GameObject targetCard)
        {
            yield break;
        }
        
#endregion ------------Card Effects------------//    
        
        
        
        
        
        
    }
}

