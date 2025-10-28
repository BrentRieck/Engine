using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Monsters
{
    /// <summary>
    /// Runtime representation of a monster in the player's party or in battle.
    /// </summary>
    [Serializable]
    public class MonsterInstance
    {
        [SerializeField] private MonsterSpeciesData species;
        [SerializeField] private int level;
        [SerializeField] private int currentHP;
        [SerializeField] private List<MoveData> knownMoves = new List<MoveData>();
        [SerializeField] private StatusEffect status = StatusEffect.None;
        [SerializeField] private int experience;

        public MonsterSpeciesData Species => species;
        public int Level => level;
        public int CurrentHP => currentHP;
        public IReadOnlyList<MoveData> KnownMoves => knownMoves;
        public StatusEffect Status => status;
        public int Experience => experience;

        public int MaxHP => CalculateStat(species.BaseHP, level);
        public int Attack => CalculateStat(species.BaseAttack, level);
        public int Defense => CalculateStat(species.BaseDefense, level);
        public int Speed => CalculateStat(species.BaseSpeed, level);
        public int Magic => CalculateStat(species.BaseMagic, level);

        public MonsterInstance(MonsterSpeciesData species, int level)
        {
            this.species = species;
            this.level = Mathf.Clamp(level, 1, 100);
            currentHP = MaxHP;
            experience = CalculateTotalExpForLevel(this.level);
            RefreshMovesFromLearnset();
        }

        public void RefreshMovesFromLearnset()
        {
            knownMoves.Clear();
            foreach (var entry in species.Learnset)
            {
                if (entry.level <= level && entry.move != null)
                {
                    knownMoves.Add(entry.move);
                }
            }

            while (knownMoves.Count > 4)
            {
                knownMoves.RemoveAt(0);
            }
        }

        public void ApplyDamage(int amount)
        {
            currentHP = Mathf.Max(0, currentHP - Mathf.Max(0, amount));
        }

        public void Heal(int amount)
        {
            currentHP = Mathf.Min(MaxHP, currentHP + Mathf.Max(0, amount));
        }

        public void Restore()
        {
            currentHP = MaxHP;
            status = StatusEffect.None;
        }

        public bool IsFainted => currentHP <= 0;

        public void AddExperience(int amount)
        {
            experience += Mathf.Max(0, amount);
            int newLevel = CalculateLevelFromTotalExp(experience);
            if (newLevel > level)
            {
                level = Mathf.Min(newLevel, 100);
                currentHP = MaxHP;
                RefreshMovesFromLearnset();
            }
        }

        public bool TryEvolve(out MonsterSpeciesData evolved)
        {
            evolved = null;
            if (species.EvolutionTarget != null && level >= species.EvolutionLevel)
            {
                species = species.EvolutionTarget;
                RefreshMovesFromLearnset();
                currentHP = MaxHP;
                evolved = species;
                return true;
            }

            return false;
        }

        public void ApplyStatus(StatusEffect effect)
        {
            if (status == StatusEffect.None)
            {
                status = effect;
            }
        }

        public void ClearStatus()
        {
            status = StatusEffect.None;
        }

        private static int CalculateStat(int baseStat, int level)
        {
            return Mathf.FloorToInt((baseStat * 2f * level) / 100f) + level + 10;
        }

        public static int CalculateTotalExpForLevel(int level)
        {
            return Mathf.FloorToInt(Mathf.Pow(level, 3));
        }

        public static int CalculateLevelFromTotalExp(int totalExp)
        {
            int level = 1;
            while (CalculateTotalExpForLevel(level + 1) <= totalExp && level < 100)
            {
                level++;
            }

            return level;
        }
    }
}
