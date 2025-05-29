using System.Text;
using UnityEngine;

namespace Game.CardEffects
{
    public enum CardEffectTypes
    {
        None,       
        Individual,
        Buff,
        CardDraw,
        Tinker,
        Augment,
        Hex
    }
        
    public enum CardEffectTargets
    {
        None,
        Random,
        Self,
        Right,
        Left,
        ArenaAll,
        DeckRandom,
        DeckAll,
        HandRandom,
        HandAll
    }
    
    public enum GadgetTargets
    {
        None,
        Random,
        Right,
        Left,
        FarRight,
        FarLeft
    }
    
    public enum GadgetTypes
    {
        None,
        Bomb,
        Placeholder
    }
    
}

