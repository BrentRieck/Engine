using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.UI
{
    using Engine.Monsters;
    using Engine.Systems;

    /// <summary>
    /// Handles the pause menu, monster dex, party management and save/load options.
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private GameObject monsterDexPanel;
        [SerializeField] private GameObject partyPanel;
        [SerializeField] private InputField dexSearchField;
        [SerializeField] private RectTransform dexListContainer;
        [SerializeField] private GameObject dexListEntryPrefab;
        [SerializeField] private Text partyInfoText;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;

        private Party party;
        private List<MonsterSpeciesData> seenSpecies = new List<MonsterSpeciesData>();

        private void Awake()
        {
            root.SetActive(false);
            saveButton.onClick.AddListener(OnSaveClicked);
            loadButton.onClick.AddListener(OnLoadClicked);
        }

        private void Start()
        {
            if (party == null)
            {
                party = EncounterContext.Instance.PlayerParty ?? new Party();
            }

            if (seenSpecies.Count == 0)
            {
                seenSpecies.AddRange(MonsterDatabase.Species);
            }

            RefreshPartyUI();
            PopulateDexList(string.Empty);
        }

        public void Initialize(Party party, IEnumerable<MonsterSpeciesData> knownSpecies)
        {
            this.party = party;
            seenSpecies.Clear();
            seenSpecies.AddRange(knownSpecies);
            RefreshPartyUI();
            PopulateDexList(string.Empty);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            root.SetActive(!root.activeSelf);
            if (root.activeSelf)
            {
                PopulateDexList(dexSearchField != null ? dexSearchField.text : string.Empty);
            }
        }

        public void OnDexSearchChanged(string search)
        {
            PopulateDexList(search);
        }

        private void PopulateDexList(string search)
        {
            if (dexListContainer == null)
            {
                return;
            }

            foreach (Transform child in dexListContainer)
            {
                Destroy(child.gameObject);
            }

            string lower = search.ToLowerInvariant();
            foreach (var species in seenSpecies)
            {
                if (species == null || (!string.IsNullOrEmpty(lower) && !species.SpeciesName.ToLowerInvariant().Contains(lower)))
                {
                    continue;
                }

                if (dexListEntryPrefab != null)
                {
                    var entry = Instantiate(dexListEntryPrefab, dexListContainer);
                    var label = entry.GetComponentInChildren<Text>();
                    if (label != null)
                    {
                        label.text = species.SpeciesName;
                    }
                }
            }
        }

        private void RefreshPartyUI()
        {
            if (party == null)
            {
                partyInfoText.text = "No party";
                return;
            }

            var builder = new StringBuilder();
            foreach (var monster in party.Monsters)
            {
                builder.AppendLine($"{monster.Species.SpeciesName} Lv.{monster.Level} HP {monster.CurrentHP}/{monster.MaxHP}");
            }

            partyInfoText.text = builder.ToString();
        }

        private void OnSaveClicked()
        {
            SaveSystem.Save(party, new Inventory(), 0);
        }

        private void OnLoadClicked()
        {
            if (SaveSystem.TryLoad(out var loadedParty, out var inventory, out var badges))
            {
                party = loadedParty;
                RefreshPartyUI();
            }
        }
    }
}
