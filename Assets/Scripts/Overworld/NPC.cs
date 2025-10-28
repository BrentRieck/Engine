using UnityEngine;

namespace Engine.Overworld
{
    using Engine.Systems;

    /// <summary>
    /// Basic NPC with optional trainer battle and dialog.
    /// </summary>
    public class NPC : MonoBehaviour
    {
        [TextArea]
        [SerializeField] private string dialogue = "Hello there!";
        [SerializeField] private bool isTrainer;
        [SerializeField] private Party trainerParty;
        [SerializeField] private string battleSceneName = "Battle";

        public void Interact()
        {
            if (isTrainer)
            {
                EncounterContext.Instance.ConfigureTrainerBattle(null, trainerParty);
                UnityEngine.SceneManagement.SceneManager.LoadScene(battleSceneName);
            }
            else
            {
                Debug.Log(dialogue);
            }
        }
    }
}
