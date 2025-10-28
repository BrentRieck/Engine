using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Systems
{
    using Engine.Monsters;

    /// <summary>
    /// Defines the encounters available in a given overworld area.
    /// </summary>
    [CreateAssetMenu(menuName = "Game/EncounterTable", fileName = "NewEncounterTable")]
    public class EncounterTable : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public MonsterSpeciesData species;
            public Vector2Int levelRange = new Vector2Int(1, 5);
            public float weight = 1f;
        }

        [SerializeField] private List<Entry> entries = new List<Entry>();

        public MonsterInstance GenerateEncounter(System.Random rng)
        {
            if (entries.Count == 0)
            {
                return null;
            }

            float totalWeight = 0f;
            foreach (var entry in entries)
            {
                totalWeight += Mathf.Max(0.01f, entry.weight);
            }

            float roll = (float)(rng.NextDouble() * totalWeight);
            foreach (var entry in entries)
            {
                float weight = Mathf.Max(0.01f, entry.weight);
                if (roll <= weight)
                {
                    int level = rng.Next(entry.levelRange.x, entry.levelRange.y + 1);
                    return new MonsterInstance(entry.species, level);
                }

                roll -= weight;
            }

            var fallback = entries[entries.Count - 1];
            int fallbackLevel = rng.Next(fallback.levelRange.x, fallback.levelRange.y + 1);
            return new MonsterInstance(fallback.species, fallbackLevel);
        }
    }
}
