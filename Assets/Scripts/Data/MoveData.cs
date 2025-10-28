using UnityEngine;

namespace Engine.Monsters
{
    /// <summary>
    /// Defines a move that a monster can execute during battle.
    /// </summary>
    [CreateAssetMenu(menuName = "Monsters/Move", fileName = "NewMove")]
    public class MoveData : ScriptableObject
    {
        [SerializeField] private string moveName = "New Move";
        [TextArea]
        [SerializeField] private string description = "";
        [SerializeField] private ElementType elementType = ElementType.Flame;
        [SerializeField] private int power = 40;
        [SerializeField] private int accuracy = 95;
        [SerializeField] private bool isMagical;
        [SerializeField] private StatusEffect inflicts = StatusEffect.None;
        [SerializeField] private float statusChance = 0f;

        public string MoveName => moveName;
        public string Description => description;
        public ElementType ElementType => elementType;
        public int Power => power;
        public int Accuracy => Mathf.Clamp(accuracy, 1, 100);
        public bool IsMagical => isMagical;
        public StatusEffect Inflicts => inflicts;
        public float StatusChance => Mathf.Clamp01(statusChance);
    }
}
