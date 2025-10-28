using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Systems
{
    using Engine.Monsters;

    /// <summary>
    /// Manages the player's active party of monsters.
    /// </summary>
    [Serializable]
    public class Party
    {
        [SerializeField] private List<MonsterInstance> monsters = new List<MonsterInstance>();
        public IReadOnlyList<MonsterInstance> Monsters => monsters;
        public const int MaxPartySize = 6;

        [field: NonSerialized]
        public event Action Changed;

        public void AddMonster(MonsterInstance instance)
        {
            if (monsters.Count >= MaxPartySize)
            {
                throw new InvalidOperationException("Party is full");
            }

            monsters.Add(instance);
            Changed?.Invoke();
        }

        public void RemoveMonster(MonsterInstance instance)
        {
            if (monsters.Remove(instance))
            {
                Changed?.Invoke();
            }
        }

        public MonsterInstance GetFirstHealthyMonster()
        {
            foreach (var monster in monsters)
            {
                if (!monster.IsFainted)
                {
                    return monster;
                }
            }

            return null;
        }

        public bool HasHealthyMonster()
        {
            return GetFirstHealthyMonster() != null;
        }

        public void Swap(int indexA, int indexB)
        {
            if (indexA < 0 || indexA >= monsters.Count || indexB < 0 || indexB >= monsters.Count)
            {
                return;
            }

            (monsters[indexA], monsters[indexB]) = (monsters[indexB], monsters[indexA]);
            Changed?.Invoke();
        }
    }
}
