using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Global;

namespace Game.CharacterClasses
{
    public enum CharacterClasses
    {
        None,
        Brawler,
        Medic,
        Soldier,
        Hacker,
        Sniper,
        Trickster,
        Tinkerer,
        Technician,
        Hexxe
    }

        public static class CharacterClassHandler
    {
        // Tracks the number of characters of each class currently in the arena
        private static Dictionary<CharacterClasses, int> classCounts = new();

        // Event: Notifies listeners (e.g., cards, UI) when the strength of a class effect changes
        // int value represents the new effect strength: (number of characters - 1)
        public static event Action<CharacterClasses, int> OnClassEffectChanged;

        // Called by BattleHandler to apply changes in class composition in the arena
        // classChanges contains a delta for each class (e.g., +1 when a card is added, -1 when removed)
        public static void UpdateClassEffects(Dictionary<CharacterClasses, int> classChanges)
        {
            foreach (var change in classChanges)
            {
                CharacterClasses classType = change.Key;
                int delta = change.Value;

                // Get the previous count for this class (default to 0 if not found)
                int previousCount = classCounts.ContainsKey(classType) ? classCounts[classType] : 0;

                // Apply the delta, making sure count doesn't go below 0
                int updatedCount = Mathf.Max(0, previousCount + delta);
                classCounts[classType] = updatedCount;

                // Calculate old and new effect strength as (count - 1), minimum 0
                int previousStrength = Mathf.Max(0, previousCount - 1);
                int newStrength = Mathf.Max(0, updatedCount - 1);

                // Only update effects and notify listeners if the strength has changed
                if (previousStrength != newStrength)
                {
                    Debug.Log($"[ClassHandler] {classType} effect updated: Strength {previousStrength} -> {newStrength}");

                    // Notify all subscribed listeners
                    OnClassEffectChanged?.Invoke(classType, newStrength);

                    // Apply logic for the specific effect update
                    ApplyEffectChange(classType, newStrength);
                }
            }
        }

        // Returns the current strength of a class effect (0 if not present or only 1 card)
        public static int GetClassStrength(CharacterClasses classType)
        {
            return classCounts.ContainsKey(classType) ? Mathf.Max(0, classCounts[classType] - 1) : 0;
        }

        // Placeholder for applying or updating the class effect
        // For example: update stats, trigger animations, or refresh card descriptions
        private static void ApplyEffectChange(CharacterClasses classType, int strength)
        {
            // TODO: Add actual logic for applying this class effect to relevant cards/systems
            Debug.Log($"[ClassHandler] Applying effect for {classType} with strength {strength}");
        }
    }
} 