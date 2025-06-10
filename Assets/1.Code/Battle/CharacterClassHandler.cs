using System.Collections.Generic;
using UnityEngine;
using Game.Global;

namespace Game.CharacterClasses
{
    public static class CharacterClassHandler
    {
        public static Dictionary<Global.CharacterClasses, int> currentlyActiveClasses 
            = new Dictionary<Global.CharacterClasses, int>();
        
        public static void UpdateCharacterClassEffects(Dictionary<Global.CharacterClasses, int> classesInArena)
        {
            foreach (var entry in classesInArena)
            {
                currentlyActiveClasses.TryGetValue(entry.Key, out int previousCount);
                if (entry.Value != previousCount)
                {
                    // New class or changed count – apply effect
                    Debug.Log($"Updating class effect for {entry.Key}: was {previousCount}, now {entry.Value}");
                    ApplyEffect(entry.Key, entry.Value);
                }
            }

            // Optionally: remove effects for classes that are no longer in the arena
            foreach (var entry in currentlyActiveClasses)
            {
                if (!classesInArena.ContainsKey(entry.Key))
                {
                    Debug.Log($"Removing class effect for {entry.Key}");
                    RemoveEffect(entry.Key);
                }
            }

            // Finally, update the internal dictionary to match the current state
            currentlyActiveClasses = new Dictionary<Global.CharacterClasses, int>(classesInArena);
        }

        private static void ApplyEffect(Global.CharacterClasses characterClass, int count)
        {
            // Your effect logic here
        }

        private static void RemoveEffect(Global.CharacterClasses characterClass)
        {
            // Your cleanup logic here
        }


    }
}