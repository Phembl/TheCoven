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
        ResolveEnd
    }
    
    public static class Global
    {
        //Time
        public static float timeMult = 1f;

    }
}

