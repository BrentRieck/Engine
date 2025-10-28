using System.IO;
using UnityEngine;

namespace Engine.Systems
{
    using Engine.Monsters;

    /// <summary>
    /// Saves and loads the player state to JSON in Application.persistentDataPath.
    /// </summary>
    public static class SaveSystem
    {
        private const string SaveFileName = "savegame.json";

        [System.Serializable]
        private class SaveData
        {
            public Party party;
            public Inventory inventory;
            public int badges;
        }

        public static void Save(Party party, Inventory inventory, int badges)
        {
            var data = new SaveData
            {
                party = party,
                inventory = inventory,
                badges = badges
            };

            string json = JsonUtility.ToJson(data, true);
            string path = Path.Combine(Application.persistentDataPath, SaveFileName);
            File.WriteAllText(path, json);
            Debug.Log($"Saved game to {path}");
        }

        public static bool TryLoad(out Party party, out Inventory inventory, out int badges)
        {
            string path = Path.Combine(Application.persistentDataPath, SaveFileName);
            if (!File.Exists(path))
            {
                party = null;
                inventory = null;
                badges = 0;
                return false;
            }

            string json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<SaveData>(json);
            party = data.party ?? new Party();
            inventory = data.inventory ?? new Inventory();
            badges = data.badges;
            return true;
        }
    }
}
