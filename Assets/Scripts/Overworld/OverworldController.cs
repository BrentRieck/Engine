using UnityEngine;

namespace Engine.Overworld
{
    using Engine.Systems;

    /// <summary>
    /// Simplified overworld controller handling player movement and encounters.
    /// </summary>
    public class OverworldController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private EncounterTable grassEncounterTable;
        [SerializeField] private Engine.Systems.Party playerParty;
        [SerializeField] private string battleSceneName = "Battle";

        private Rigidbody2D body;
        private Vector2 input;
        private float encounterTimer;
        private System.Random rng = new System.Random();

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            playerParty ??= new Engine.Systems.Party();
        }

        private void Update()
        {
            playerParty ??= EncounterContext.Instance.PlayerParty ?? new Engine.Systems.Party();
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            input = Vector2.ClampMagnitude(input, 1f);

            if (input.sqrMagnitude > 0.01f)
            {
                encounterTimer += Time.deltaTime;
                if (encounterTimer > 3f && IsInEncounterZone())
                {
                    encounterTimer = 0f;
                    TriggerEncounter();
                }
            }
            else
            {
                encounterTimer = 0f;
            }
        }

        private void FixedUpdate()
        {
            body.MovePosition(body.position + input * moveSpeed * Time.fixedDeltaTime);
        }

        private bool IsInEncounterZone()
        {
            // Placeholder: rely on layer or trigger detection.
            return true;
        }

        private void TriggerEncounter()
        {
            var instance = grassEncounterTable?.GenerateEncounter(rng);
            if (instance != null)
            {
                // Set encounter data in a global context, then load battle scene.
                EncounterContext.Instance.ConfigureWildEncounter(playerParty, instance);
                UnityEngine.SceneManagement.SceneManager.LoadScene(battleSceneName);
            }
        }
    }
}
