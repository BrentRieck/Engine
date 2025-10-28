using System.Collections.Generic;
using UnityEngine;

namespace Engine.Monsters
{
    /// <summary>
    /// Loads all generated monster species and move data from Resources.
    /// </summary>
    public static class MonsterDatabase
    {
        private static MonsterSpeciesData[] species;
        private static MoveData[] moves;

        public static IReadOnlyList<MonsterSpeciesData> Species
        {
            get
            {
                EnsureLoaded();
                return species;
            }
        }

        public static IReadOnlyList<MoveData> Moves
        {
            get
            {
                EnsureLoaded();
                return moves;
            }
        }

        private static void EnsureLoaded()
        {
            if (species != null && moves != null)
            {
                return;
            }

            moves = Resources.LoadAll<MoveData>("Moves");
            species = Resources.LoadAll<MonsterSpeciesData>("Monsters");
        }
    }
}
