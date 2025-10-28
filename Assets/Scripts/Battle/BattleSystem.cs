using System.Collections;
using UnityEngine;

namespace Engine.Battle
{
    using Engine.Monsters;
    using Engine.Systems;

    /// <summary>
    /// Minimal turn-based battle loop for wild and trainer encounters.
    /// Attach to the Battle scene and assign references via inspector.
    /// </summary>
    public class BattleSystem : MonoBehaviour
    {
        [SerializeField] private Party playerParty;
        [SerializeField] private Party opponentParty;
        [SerializeField] private BattleUI battleUI;

        private System.Random rng;
        private MonsterInstance playerActive;
        private MonsterInstance opponentActive;

        private void Awake()
        {
            rng = new System.Random();
        }

        private void OnEnable()
        {
            BeginBattle();
        }

        public void BeginBattle()
        {
            if (playerParty == null)
            {
                playerParty = EncounterContext.Instance.PlayerParty ?? EncounterContext.Instance.OpponentParty;
            }

            if (opponentParty == null)
            {
                if (EncounterContext.Instance.WildEncounter != null)
                {
                    opponentParty = new Party();
                    opponentParty.AddMonster(EncounterContext.Instance.WildEncounter);
                }
                else
                {
                    opponentParty = EncounterContext.Instance.OpponentParty;
                }
            }

            playerActive = playerParty?.GetFirstHealthyMonster();
            opponentActive = opponentParty?.GetFirstHealthyMonster();

            battleUI.Setup(playerActive, opponentActive);
            if (playerActive != null && opponentActive != null)
            {
                StartCoroutine(BattleLoop());
            }
            else
            {
                battleUI.ShowMessage("Battle could not start.");
            }
        }

        private IEnumerator BattleLoop()
        {
            while (playerActive != null && opponentActive != null)
            {
                yield return PlayerTurn();
                if (opponentActive == null || opponentActive.IsFainted)
                {
                    HandleVictory();
                    yield break;
                }

                yield return OpponentTurn();
                if (playerActive == null || playerActive.IsFainted)
                {
                    HandleDefeat();
                    yield break;
                }
            }
        }

        private IEnumerator PlayerTurn()
        {
            yield return battleUI.PromptPlayerAction(OnPlayerActionSelected);
        }

        private void OnPlayerActionSelected(BattleAction action)
        {
            switch (action.Type)
            {
                case BattleActionType.Fight:
                    ExecuteMove(playerActive, opponentActive, action.Move);
                    break;
                case BattleActionType.Item:
                    if (action.ItemId == "potion" && action.Quantity > 0)
                    {
                        playerActive.Heal(playerActive.MaxHP / 2);
                    }
                    else if (action.ItemType == ItemType.Capture)
                    {
                        TryCapture();
                    }
                    break;
                case BattleActionType.Run:
                    if (rng.NextDouble() < 0.5)
                    {
                        battleUI.ShowMessage("Got away safely!");
                        opponentActive = null;
                    }
                    else
                    {
                        battleUI.ShowMessage("Couldn't escape!");
                    }
                    break;
            }
        }

        private IEnumerator OpponentTurn()
        {
            yield return new WaitForSeconds(0.5f);
            var move = opponentActive.KnownMoves.Count > 0 ? opponentActive.KnownMoves[0] : null;
            ExecuteMove(opponentActive, playerActive, move);
            yield return null;
        }

        private void ExecuteMove(MonsterInstance attacker, MonsterInstance defender, MoveData move)
        {
            if (attacker == null || defender == null || move == null)
            {
                return;
            }

            float hitRoll = (float)rng.NextDouble();
            if (hitRoll > move.Accuracy / 100f)
            {
                battleUI.ShowMessage($"{attacker.Species.SpeciesName}'s {move.MoveName} missed!");
                return;
            }

            float attackStat = move.IsMagical ? attacker.Magic : attacker.Attack;
            float defenseStat = move.IsMagical ? defender.Magic : defender.Defense;
            float baseDamage = (((2 * attacker.Level / 5f) + 2) * move.Power * (attackStat / Mathf.Max(1f, defenseStat))) / 50f + 2;
            float modifier = TypeChart.GetCombinedModifier(move.ElementType, defender.Species.PrimaryType, defender.Species.SecondaryType);
            float random = Mathf.Lerp(0.85f, 1f, (float)rng.NextDouble());
            int damage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * modifier * random));

            defender.ApplyDamage(damage);
            battleUI.UpdateHUD(attacker, defender);
            battleUI.ShowMessage($"{attacker.Species.SpeciesName} used {move.MoveName}! {damage} damage dealt.");

            if (move.Inflicts != StatusEffect.None && rng.NextDouble() < move.StatusChance)
            {
                defender.ApplyStatus(move.Inflicts);
                battleUI.ShowMessage($"{defender.Species.SpeciesName} is now {move.Inflicts}!");
            }

            if (defender.IsFainted)
            {
                HandleFaint(attacker, defender);
            }
        }

        private void HandleFaint(MonsterInstance attacker, MonsterInstance defender)
        {
            battleUI.ShowMessage($"{defender.Species.SpeciesName} fainted!");
            if (defender == opponentActive)
            {
                int xp = Mathf.Max(1, defender.Level * 10);
                attacker.AddExperience(xp);
                battleUI.ShowMessage($"{attacker.Species.SpeciesName} gained {xp} XP!");
                if (attacker.TryEvolve(out var evolved))
                {
                    battleUI.ShowMessage($"{attacker.Species.SpeciesName} evolved into {evolved.SpeciesName}!");
                }

                opponentActive = opponentParty.GetFirstHealthyMonster();
                if (opponentActive != null)
                {
                    battleUI.UpdateHUD(attacker, opponentActive);
                }
            }
            else
            {
                playerActive = playerParty.GetFirstHealthyMonster();
                if (playerActive != null)
                {
                    battleUI.UpdateHUD(playerActive, opponentActive);
                }
            }
        }

        private void TryCapture()
        {
            float healthRatio = (float)opponentActive.CurrentHP / opponentActive.MaxHP;
            float chance = Mathf.Clamp01(1f - healthRatio * 0.7f);
            if (rng.NextDouble() < chance)
            {
                battleUI.ShowMessage("Capture successful!");
                if (playerParty != null && playerParty.Monsters.Count < Party.MaxPartySize)
                {
                    playerParty.AddMonster(opponentActive);
                }
                opponentActive = null;
            }
            else
            {
                battleUI.ShowMessage("Capture failed!");
            }
        }

        private void HandleVictory()
        {
            battleUI.ShowMessage("You won the battle!");
        }

        private void HandleDefeat()
        {
            battleUI.ShowMessage("You blacked out...");
        }
    }
}
