using UnityEngine;
using UnityEngine.UI;

namespace Engine.UI
{
    using Engine.Systems;

    /// <summary>
    /// Minimal shop UI allowing purchase of capture items and potions.
    /// </summary>
    public class ShopController : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private Text currencyText;
        [SerializeField] private int startingCurrency = 100;
        [SerializeField] private int captureCost = 50;
        [SerializeField] private int potionCost = 30;

        private int currency;

        private void Awake()
        {
            currency = startingCurrency;
            RefreshUI();
            inventory ??= EncounterContext.Instance.PlayerInventory ?? new Inventory();
        }

        public void BuyCaptureItem()
        {
            if (currency >= captureCost)
            {
                currency -= captureCost;
                inventory?.AddItem("capture-orb", ItemType.Capture, 1);
                RefreshUI();
            }
        }

        public void BuyPotion()
        {
            if (currency >= potionCost)
            {
                currency -= potionCost;
                inventory?.AddItem("potion", ItemType.Potion, 1);
                RefreshUI();
            }
        }

        private void RefreshUI()
        {
            currencyText.text = $"$ {currency}";
        }
    }
}
