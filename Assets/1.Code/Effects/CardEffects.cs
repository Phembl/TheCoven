using System.Collections;
using UnityEngine;
using Game.Global;

namespace Game.CardEffects
{
    public static class CardEffects
    {
        private static float waitAfterEffect = 0.2f;
        public static IEnumerator DoEffect (CardEffectData cardEffectData)
        {
            GameObject targetCard = null;
            
            //Find targetCard gameObject
            if (cardEffectData.cardEffectTarget != CardEffectTargets.None)
            {
                //targetCard = BattleManager.instance.GetCardTarget
                targetCard = Utility.GetCardEffectTarget
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
            }
            
            yield return new WaitForSeconds(waitAfterEffect);
        }

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
        
        
        
        
        
        
        
    }
}

