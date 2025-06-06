using UnityEngine;

namespace Game.Global
{
    public enum CardLocations
    {
        None,
        Deck,
        Hand,
        Arena,
        Exhaust
    }

    public enum CardAnimations
    {
        None,
        Attack,
        UpdatePower,
        ResolveStart,
        ResolveEnd,
        Exhaust,
        Appear
    }

    public enum BattleCounters
    {
        None,
        Deck,
        Exhaust,
        EnemyHealth
    }

    public enum EnemyActions
    {
        None,
        PLACEHOLDER1,
        PLACEHOLDER2,
        PLACEHOLDER3,
        PLACEHOLDER4,
        PLACEHOLDER5,
        PLACEHOLDER6,
        PLACEHOLDER7,
        PLACEHOLDER8
    }
    
    public enum EnemyUltimate
    {
        None,
        PLACEHOLDER1,
        PLACEHOLDER2,
        PLACEHOLDER3,
        PLACEHOLDER4,
        PLACEHOLDER5,
        PLACEHOLDER6,
        PLACEHOLDER7,
        PLACEHOLDER8
    }
    
    public static class Global
    {
        //Card Container
        public static Transform handCardHolder => 
            BattleHandler.instance?.handCardHolder;
        
        public static Transform deckCardHolder => 
            BattleHandler.instance?.deckCardHolder;
        
        public static Transform arenaCardHolder => 
            BattleHandler.instance?.arenaCardHolder;
        
        public static Transform exhaustCardHolder => 
            BattleHandler.instance?.exhaustCardHolder;
        
        //Time
        public static float timeMult = 1f;

    }
}

