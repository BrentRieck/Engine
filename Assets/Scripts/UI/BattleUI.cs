using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Battle
{
    using Engine.Monsters;
    using Engine.Systems;

    public enum BattleActionType
    {
        Fight,
        Item,
        Capture,
        Run
    }

    public struct BattleAction
    {
        public BattleActionType Type;
        public MoveData Move;
        public string ItemId;
        public ItemType ItemType;
        public int Quantity;
    }

    /// <summary>
    /// Presents battle information and gathers player input.
    /// </summary>
    public class BattleUI : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private Text playerName;
        [SerializeField] private Slider playerHP;
        [SerializeField] private Text playerStatus;
        [SerializeField] private Text opponentName;
        [SerializeField] private Slider opponentHP;
        [SerializeField] private Text opponentStatus;
        [SerializeField] private Text messageText;

        private Action<BattleAction> onActionSelected;

        public void Setup(MonsterInstance player, MonsterInstance opponent)
        {
            UpdateHUD(player, opponent);
        }

        public void UpdateHUD(MonsterInstance player, MonsterInstance opponent)
        {
            if (player != null)
            {
                playerName.text = $"{player.Species.SpeciesName} Lv.{player.Level}";
                playerHP.maxValue = player.MaxHP;
                playerHP.value = player.CurrentHP;
                playerStatus.text = player.Status.ToString();
            }

            if (opponent != null)
            {
                opponentName.text = $"{opponent.Species.SpeciesName} Lv.{opponent.Level}";
                opponentHP.maxValue = opponent.MaxHP;
                opponentHP.value = opponent.CurrentHP;
                opponentStatus.text = opponent.Status.ToString();
            }
        }

        public IEnumerator PromptPlayerAction(Action<BattleAction> callback)
        {
            onActionSelected = callback;
            messageText.text = "What will you do?";
            yield return new WaitUntil(() => onActionSelected == null);
        }

        public void ChooseMove(MoveData move)
        {
            onActionSelected?.Invoke(new BattleAction { Type = BattleActionType.Fight, Move = move });
            onActionSelected = null;
        }

        public void ChooseItem(string itemId, ItemType itemType, int quantity)
        {
            onActionSelected?.Invoke(new BattleAction { Type = BattleActionType.Item, ItemId = itemId, ItemType = itemType, Quantity = quantity });
            onActionSelected = null;
        }

        public void ChooseRun()
        {
            onActionSelected?.Invoke(new BattleAction { Type = BattleActionType.Run });
            onActionSelected = null;
        }

        public void ShowMessage(string message)
        {
            messageText.text = message;
        }
    }
}
