using UnityEngine;

namespace Engine.Systems
{
    /// <summary>
    /// Tracks a simple quest: defeat the gym leader.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private string questId = "defeat-gym";
        [SerializeField] private bool isComplete;

        public bool IsComplete => isComplete;

        public void MarkComplete()
        {
            isComplete = true;
        }

        public void Load(int badges)
        {
            isComplete = badges > 0;
        }
    }
}
