using System;
using System.Collections.Generic;

namespace Engine.Monsters
{
    /// <summary>
    /// Represents an elemental type used by monsters and moves.
    /// The order of the values must remain stable because the generator relies on it.
    /// </summary>
    public enum ElementType
    {
        None = -1,
        Flame,
        Aqua,
        Terra,
        Zephyr,
        Spark,
        Frost,
        Shade,
        Light,
        Mind,
        Toxin,
        Metal,
        Flora,
        Spirit,
        Beast,
        Stone,
        Storm
    }

    /// <summary>
    /// Provides deterministic damage multipliers for attacking and defending types.
    /// </summary>
    public static class TypeChart
    {
        private static readonly Dictionary<(ElementType attack, ElementType defend), float> _modifiers;

        static TypeChart()
        {
            _modifiers = new Dictionary<(ElementType attack, ElementType defend), float>();
            void Add(ElementType a, ElementType d, float value) => _modifiers[(a, d)] = value;

            // Simple handcrafted relationships. Symmetry is not required but kept light for clarity.
            Add(ElementType.Flame, ElementType.Flora, 2f);
            Add(ElementType.Flame, ElementType.Frost, 2f);
            Add(ElementType.Flame, ElementType.Aqua, 0.5f);
            Add(ElementType.Aqua, ElementType.Flame, 2f);
            Add(ElementType.Aqua, ElementType.Terra, 2f);
            Add(ElementType.Aqua, ElementType.Spark, 0.5f);
            Add(ElementType.Terra, ElementType.Spark, 2f);
            Add(ElementType.Spark, ElementType.Aqua, 2f);
            Add(ElementType.Spark, ElementType.Terra, 0.5f);
            Add(ElementType.Frost, ElementType.Zephyr, 2f);
            Add(ElementType.Zephyr, ElementType.Terra, 2f);
            Add(ElementType.Toxin, ElementType.Flora, 2f);
            Add(ElementType.Metal, ElementType.Light, 2f);
            Add(ElementType.Light, ElementType.Shade, 2f);
            Add(ElementType.Shade, ElementType.Mind, 2f);
            Add(ElementType.Spirit, ElementType.Spirit, 2f);
            Add(ElementType.Beast, ElementType.Zephyr, 2f);
            Add(ElementType.Stone, ElementType.Flame, 0.5f);
            Add(ElementType.Storm, ElementType.Zephyr, 0.5f);
            Add(ElementType.Storm, ElementType.Spark, 2f);
            Add(ElementType.Flora, ElementType.Terra, 2f);
            Add(ElementType.Flora, ElementType.Flame, 0.5f);

            // Neutral defaults are implied by the lookup method.
        }

        /// <summary>
        /// Returns the damage multiplier for an attacking move against a defending type.
        /// </summary>
        public static float GetModifier(ElementType attackType, ElementType defendType)
        {
            if (attackType == ElementType.None || defendType == ElementType.None)
            {
                return 1f;
            }

            return _modifiers.TryGetValue((attackType, defendType), out var modifier) ? modifier : 1f;
        }

        /// <summary>
        /// Calculates the combined type modifier for a defender with up to two types.
        /// </summary>
        public static float GetCombinedModifier(ElementType attackType, ElementType defendTypeA, ElementType defendTypeB)
        {
            float result = GetModifier(attackType, defendTypeA);
            if (defendTypeB != ElementType.None)
            {
                result *= GetModifier(attackType, defendTypeB);
            }

            return result;
        }
    }
}
