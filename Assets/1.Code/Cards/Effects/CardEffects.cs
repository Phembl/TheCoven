using System.Collections;
using UnityEngine;

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
                targetCard = BattleManager.instance.GetCardTarget
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
            targetCard.GetComponent<Character>().UpdatePower(buffStrength);
            yield break;
        }

        private static IEnumerator EffectCardDraw(int drawAmount)
        {
            Debug.Log($"Drawing {drawAmount} cards to hand.");
            HandManager.instance.DrawCards(drawAmount);
            yield break;
        }

        private static IEnumerator EffectTinker(GameObject targetCard)
        {
            yield break;
        }
        
        
        
        
        
        
        
    }
}

