using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Gomoku.Core;
using Gomoku;

namespace Gomoku.UI
{
    /// <summary>
    /// Automatically sets up EventSystem and required raycasters for UI interaction
    /// </summary>
    public class EventSystemSetup : MonoBehaviour
    {
        [Header("Auto Setup")]
        [SerializeField] private bool setupOnStart = true;
        [SerializeField] private bool addPhysics2DRaycaster = true;
        [SerializeField] private bool addGraphicRaycaster = true;
        [SerializeField] private bool createGameManagers = true;

        private void Start()
        {
            Debug.Log("[EventSystemSetup] Start() called");

            if (setupOnStart)
            {
                SetupEventSystem();
            }
        }

        private void Awake()
        {
            Debug.Log("[EventSystemSetup] Awake() called");
        }

        /// <summary>
        /// Sets up EventSystem and required components for UI interaction
        /// </summary>
        [ContextMenu("Setup Event System")]
        public void SetupEventSystem()
        {
            // Check if EventSystem exists
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemGO = new GameObject("EventSystem");
                eventSystem = eventSystemGO.AddComponent<EventSystem>();
                eventSystemGO.AddComponent<StandaloneInputModule>();

                Debug.Log("EventSystem created automatically");
            }
            else
            {
                Debug.Log("EventSystem already exists");
            }

            // Add Physics2DRaycaster if needed (for 2D colliders)
            if (addPhysics2DRaycaster)
            {
                Physics2DRaycaster physics2DRaycaster = FindObjectOfType<Physics2DRaycaster>();
                if (physics2DRaycaster == null && Camera.main != null)
                {
                    Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
                    Debug.Log("Physics2DRaycaster added to main camera");
                }
            }

            // Add GraphicRaycaster if needed (for UI elements)
            if (addGraphicRaycaster)
            {
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    GraphicRaycaster graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
                    if (graphicRaycaster == null)
                    {
                        canvas.gameObject.AddComponent<GraphicRaycaster>();
                        Debug.Log("GraphicRaycaster added to canvas");
                    }
                }
            }

            // Create game management components if needed
            if (createGameManagers)
            {
                SetupGameManagers();
            }

            Debug.Log("EventSystem setup complete");
        }

        /// <summary>
        /// Sets up required game management components
        /// </summary>
        private void SetupGameManagers()
        {
            Debug.Log("--- Setting up Game Managers ---");

            // Check and create TurnManager if needed
            TurnManager turnManager = FindObjectOfType<TurnManager>();
            if (turnManager == null)
            {
                GameObject turnManagerGO = new GameObject("TurnManager");
                turnManager = turnManagerGO.AddComponent<TurnManager>();
                Debug.Log("TurnManager created automatically");
            }
            else
            {
                Debug.Log("TurnManager already exists");
            }

            // Check and create PiecePlacement if needed
            PiecePlacement piecePlacement = FindObjectOfType<PiecePlacement>();
            if (piecePlacement == null)
            {
                GameObject piecePlacementGO = new GameObject("PiecePlacement");
                piecePlacement = piecePlacementGO.AddComponent<PiecePlacement>();
                Debug.Log("PiecePlacement created automatically");
            }
            else
            {
                Debug.Log("PiecePlacement already exists");
            }

            // Check and create GameBoardController if needed
            GameBoardController gameBoard = FindObjectOfType<GameBoardController>();
            if (gameBoard == null)
            {
                Debug.LogWarning("GameBoardController not found! Please add it to the scene manually.");
            }
            else
            {
                Debug.Log("GameBoardController found");
            }

            // Force setup references for all PiecePlacement components
            PiecePlacement[] allPiecePlacements = FindObjectsOfType<PiecePlacement>();
            foreach (var pp in allPiecePlacements)
            {
                pp.SetupReferences();
                Debug.Log("Force-called SetupReferences on PiecePlacement");
            }
        }

        /// <summary>
        /// Validates that all required components for UI interaction are present
        /// </summary>
        [ContextMenu("Validate Setup")]
        public void ValidateSetup()
        {
            bool hasEventSystem = FindObjectOfType<EventSystem>() != null;
            bool hasStandaloneInputModule = FindObjectOfType<StandaloneInputModule>() != null;
            bool hasPhysics2DRaycaster = FindObjectOfType<Physics2DRaycaster>() != null;
            bool hasGraphicRaycaster = FindObjectOfType<GraphicRaycaster>() != null;

            // Check game managers
            bool hasTurnManager = FindObjectOfType<TurnManager>() != null;
            bool hasPiecePlacement = FindObjectOfType<PiecePlacement>() != null;
            bool hasGameBoardController = FindObjectOfType<GameBoardController>() != null;

            Debug.Log("=== Complete System Validation ===");
            Debug.Log("--- Event System ---");
            Debug.Log($"EventSystem: {(hasEventSystem ? "✓" : "✗ MISSING")}");
            Debug.Log($"StandaloneInputModule: {(hasStandaloneInputModule ? "✓" : "✗ MISSING")}");
            Debug.Log($"Physics2DRaycaster: {(hasPhysics2DRaycaster ? "✓" : "✗ MISSING")}");
            Debug.Log($"GraphicRaycaster: {(hasGraphicRaycaster ? "✓" : "✗ MISSING")}");

            Debug.Log("--- Game Managers ---");
            Debug.Log($"TurnManager: {(hasTurnManager ? "✓" : "✗ MISSING")}");
            Debug.Log($"PiecePlacement: {(hasPiecePlacement ? "✓" : "✗ MISSING")}");
            Debug.Log($"GameBoardController: {(hasGameBoardController ? "✓" : "✗ MISSING")}");

            // Check event subscription
            if (hasPiecePlacement)
            {
                var intersectionDetector = FindObjectOfType<IntersectionDetector>();
                if (intersectionDetector != null)
                {
                    int listenerCount = intersectionDetector.OnIntersectionClicked?.GetInvocationList().Length ?? 0;
                    Debug.Log($"Event listeners: {(listenerCount > 0 ? $"✓ {listenerCount} listeners" : "✗ NO LISTENERS")}");

                    if (listenerCount == 0)
                    {
                        Debug.LogError("CRITICAL: PiecePlacement is not subscribed to IntersectionDetector events! Clicks will not work.");
                    }
                }
            }

            if (!hasEventSystem || !hasStandaloneInputModule)
            {
                Debug.LogWarning("Critical EventSystem components missing! UI interaction will not work.");
            }

            if (!hasPhysics2DRaycaster)
            {
                Debug.LogWarning("Physics2DRaycaster missing! 2D collider clicks will not work.");
            }

            if (!hasTurnManager || !hasPiecePlacement || !hasGameBoardController)
            {
                Debug.LogWarning("Game management components missing! Game logic will not work properly.");
            }
        }
    }
}