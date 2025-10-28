using UnityEngine;

namespace Engine.Systems
{
    using Engine.Monsters;

    /// <summary>
    /// Stores encounter data between scene transitions.
    /// </summary>
    public class EncounterContext : ScriptableObject
    {
        private static EncounterContext instance;

        public static EncounterContext Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = CreateInstance<EncounterContext>();
                    DontDestroyOnLoad(instance);
                }

                return instance;
            }
        }

        public MonsterInstance WildEncounter { get; private set; }
        public Party PlayerParty { get; private set; }
        public Party OpponentParty { get; private set; }
        public Inventory PlayerInventory { get; private set; }

        public void ConfigureWildEncounter(Party player, MonsterInstance monster)
        {
            PlayerParty = player;
            WildEncounter = monster;
            OpponentParty = new Party();
            OpponentParty.AddMonster(monster);
        }

        public void ConfigureTrainerBattle(Party player, Party opponent, Inventory inventory = null)
        {
            PlayerParty = player;
            OpponentParty = opponent;
            PlayerInventory = inventory;
            WildEncounter = null;
        }
    }
}
