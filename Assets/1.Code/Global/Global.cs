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
        Exhaust
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
        //Time
        public static float timeMult = 1f;

    }
}

