using UnityEngine;
using Game.Global;

namespace Game.CardEffects
{
    public static class CardEffectText
    {
        public static string GetCardEffectText (CardEffectData cardEffectData)

        {
            string cardEffectFullText = "";
            string cardEffectTargetText = "";
            string cardEffectRepeatText = "";
            
            if (cardEffectData.cardEffectTarget != CardEffectTargets.None)
                cardEffectTargetText = ConstructCardEffectTargetText(cardEffectData.cardEffectTarget);

            if (cardEffectData.gadgetTarget != GadgetTargets.None) 
                cardEffectTargetText =  ConstructGadgetTargetText(cardEffectData.gadgetTarget);
            
                
            
            string cardEffectText = ConstructEffectText
                (
                    cardEffectData.cardEffectType, 
                    cardEffectTargetText, 
                    cardEffectData.cardEffectStrength,
                    cardEffectData.gadgetName
                
                );
            
            if (cardEffectData.cardEffectRepeatCount > 0)
                cardEffectRepeatText = ConstructRepeatText(cardEffectData.cardEffectRepeatCount);
            
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

        private static string ConstructGadgetTargetText(GadgetTargets gadgetTarget, bool startSentence = false)
        {
            string gadgetTargetText = "";

            switch (gadgetTarget)
            {
                case GadgetTargets.Random:
                    gadgetTargetText = startSentence ? "At a random position" : "at a random position";
                    break;
                
                case GadgetTargets.Right:
                    gadgetTargetText = startSentence ? "To the right of this card" : "to the right of this card";
                    break;
                
                case GadgetTargets.Left:
                    gadgetTargetText = startSentence ? "To the left of this card" : "to the left of this card";
                    break;
                
                case GadgetTargets.FarRight:
                    gadgetTargetText = startSentence ? "To the most right" : "to the most right";
                    break;
                
                case GadgetTargets.FarLeft:
                    gadgetTargetText = startSentence ? "To the most left" : "to the most left";
                    break;
                
                default:
                    gadgetTargetText = "<color=red>PLACEHOLDER</color>";
                    break;
            }

            return gadgetTargetText;
        }
        private static string ConstructEffectText(CardEffectTypes effectType, string targetText, int effectStrength, string gadgetName)
        {
            string effectText = "";
            
            switch (effectType)
            {
                case CardEffectTypes.Buff:
                    effectText = 
                        $"Increase the power of {targetText} by <color=green>{effectStrength}</color>.";
                    break;
                
                case CardEffectTypes.CardDraw:
                    if (effectStrength > 1)
                    {
                        effectText = 
                            $"Draw <color=green>{effectStrength}</color> cards.";
                    }

                    else
                    {
                        effectText = 
                            $"Draw <color=green>{effectStrength}</color> card.";
                    }

                   
                    break;
                
                case CardEffectTypes.Tinker:
                    effectText = 
                        $"Create a <color=#FF8C00>{gadgetName}</color> {targetText}.";
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
