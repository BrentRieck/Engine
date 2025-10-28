using UnityEngine;
using UnityEngine.SceneManagement;

namespace Engine.UI
{
    using Engine.Systems;

    /// <summary>
    /// Handles title screen buttons for new or continue game.
    /// </summary>
    public class TitleScreenController : MonoBehaviour
    {
        [SerializeField] private string overworldSceneName = "Overworld";

        public void NewGame()
        {
            var party = new Party();
            var inventory = new Inventory();
            SaveSystem.Save(party, inventory, 0);
            EncounterContext.Instance.ConfigureTrainerBattle(party, null, inventory);
            SceneManager.LoadScene(overworldSceneName);
        }

        public void ContinueGame()
        {
            if (!SaveSystem.TryLoad(out var party, out var inventory, out var badges))
            {
                party = new Party();
                inventory = new Inventory();
            }

            EncounterContext.Instance.ConfigureTrainerBattle(party, null, inventory);
            SceneManager.LoadScene(overworldSceneName);
        }
    }
}
