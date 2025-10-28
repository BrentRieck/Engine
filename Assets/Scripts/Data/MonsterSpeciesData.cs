using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Monsters
{
    /// <summary>
    /// Scriptable representation of a monster species.
    /// </summary>
    [CreateAssetMenu(menuName = "Monsters/Species", fileName = "NewMonster")]
    public class MonsterSpeciesData : ScriptableObject
    {
        [SerializeField] private string speciesName = "Unnamed";
        [SerializeField] private ElementType primaryType = ElementType.Flame;
        [SerializeField] private ElementType secondaryType = ElementType.None;
        [SerializeField] private int baseHP = 50;
        [SerializeField] private int baseAttack = 50;
        [SerializeField] private int baseDefense = 50;
        [SerializeField] private int baseSpeed = 50;
        [SerializeField] private int baseMagic = 50;
        [SerializeField] private int evolutionLevel = 0;
        [SerializeField] private MonsterSpeciesData evolutionTarget;
        [SerializeField] private List<LevelMove> learnset = new List<LevelMove>();
        [SerializeField] private Sprite frontSprite;
        [SerializeField] private Sprite backSprite;

        public string SpeciesName => speciesName;
        public ElementType PrimaryType => primaryType;
        public ElementType SecondaryType => secondaryType;
        public int BaseHP => Mathf.Max(1, baseHP);
        public int BaseAttack => Mathf.Max(1, baseAttack);
        public int BaseDefense => Mathf.Max(1, baseDefense);
        public int BaseSpeed => Mathf.Max(1, baseSpeed);
        public int BaseMagic => Mathf.Max(1, baseMagic);
        public int EvolutionLevel => evolutionLevel;
        public MonsterSpeciesData EvolutionTarget => evolutionTarget;
        public IReadOnlyList<LevelMove> Learnset => learnset;
        public Sprite FrontSprite => frontSprite;
        public Sprite BackSprite => backSprite;

        public void SetEvolution(MonsterSpeciesData target, int level)
        {
            evolutionTarget = target;
            evolutionLevel = level;
        }

        public void SetSprites(Sprite front, Sprite back)
        {
            frontSprite = front;
            backSprite = back;
        }

        public void SetLearnset(List<LevelMove> moves)
        {
            learnset = moves;
        }
    }

    [Serializable]
    public struct LevelMove
    {
        public int level;
        public MoveData move;
    }
}
