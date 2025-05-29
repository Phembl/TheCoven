using UnityEngine;

namespace Game.CardEffects
{
    public static class CardEffectText
    {
        public static string GetCardEffectText(CardEffectTypes effectType, CardEffectTargets effectTarget, int effectStrength,
            int repeatCount)
        {
            string cardEffectFullText = "";
            string cardEffectTargetText = "";
            string cardEffectRepeatText = "";
            
            if (effectTarget != CardEffectTargets.None)
                cardEffectTargetText = ConstructCardEffectTargetText(effectTarget);
            
            string cardEffectText = ConstructEffectText(effectType, cardEffectTargetText, effectStrength);
            
            if (repeatCount > 0)
                cardEffectRepeatText = ConstructRepeatText(repeatCount);
            
            cardEffectFullText = cardEffectText + cardEffectRepeatText;
            return cardEffectFullText;
        }

        private static string ConstructCardEffectTargetText(CardEffectTargets effectTarget, bool startSentence = false)
        {
            string targetText = "";

            switch (effectTarget)
            {
                case CardEffectTargets.Random:
                    targetText = startSentence ? "A random card in the arena" : "a random card in the arena";
                    break;

                case CardEffectTargets.Self:
                    targetText = startSentence ? "This card" : "this card";
                    break;

                case CardEffectTargets.Right:
                    targetText = startSentence ? "The card to the right" : "the card to the right";
                    break;

                case CardEffectTargets.Left:
                    targetText = startSentence ? "The card to the left" : "the card to the left";
                    break;

                case CardEffectTargets.ArenaAll:
                    targetText = startSentence ? "All cards in the arena" : "all cards in the arena";
                    break;

                case CardEffectTargets.DeckRandom:
                    targetText = startSentence ? "A random card in the deck" : "a random card in the deck";

                    break;

                case CardEffectTargets.DeckAll:
                    targetText = startSentence ? "All cards in the deck" : "all cards in the deck";
                    break;

                case CardEffectTargets.HandRandom:
                    targetText = startSentence ? "A random card in hand" : "a random card in hand";
                    break;

                case CardEffectTargets.HandAll:
                    targetText = startSentence ? "All cards in hand" : "all cards in hand";
                    break;


                default:
                    targetText = "<color=red>PLACEHOLDER</color>";
                    break;
            }

            return targetText;
        }

        private static string ConstructEffectText(CardEffectTypes effectType, string targetText, int effectStrengh)
        {
            string effectText = "";
            
            switch (effectType)
            {
                case CardEffectTypes.Buff:
                    effectText = 
                        $"Increase the power of {targetText} by <color=green>{effectStrengh}</color>.";
                    break;
                
                case CardEffectTypes.CardDraw:
                    effectText = 
                        $"Draw <color=green>{effectStrengh}</color> cards.";
                    break;
                
                default:
                    effectText = "<color=red>PLACEHOLDER</color>";
                    break;
                    
            }

            return effectText;
        }

        private static string ConstructRepeatText(int repeatCount)
        {
            string repeatText = "";
            if (repeatCount != 0)
            {
                switch (repeatCount)
                {
                    case 1:
                        repeatText = " (Repeat once).";
                        break;
                    case 2:
                        repeatText = " (Repeat twice).";
                        break;
                    default:
                        repeatText = $" (Repeat {repeatCount} times).";
                        break;
                }
            }

            return repeatText;

        }

    }
    
}
