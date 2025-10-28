using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Gomoku;
using Gomoku.UI;
using Gomoku.Core;

namespace Gomoku.Diagnostics
{
    /// <summary>
    /// Validates the complete click detection chain from UI to game logic
    /// </summary>
    public class ClickChainValidator : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool runTestsOnStart = true;
        [SerializeField] private KeyCode testKey = KeyCode.C;

        private IntersectionDetector intersectionDetector;
        private PiecePlacement piecePlacement;
        private TurnManager turnManager;
        private GameBoardController gameBoard;

        private void Start()
        {
            if (runTestsOnStart)
            {
                Invoke("RunValidation", 1f); // Delay to allow components to initialize
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(testKey))
            {
                RunValidation();
            }
        }

        /// <summary>
        /// Runs complete validation of the click detection chain
        /// </summary>
        [ContextMenu("Validate Click Chain")]
        public void RunValidation()
        {
            Debug.Log("=== Click Detection Chain Validation Started ===");

            // Step 1: Find all required components
            FindComponents();

            // Step 2: Check EventSystem
            ValidateEventSystem();

            // Step 3: Check Collider setup
            ValidateColliderSetup();

            // Step 4: Check event subscriptions
            ValidateEventSubscriptions();

            // Step 5: Test manual piece placement
            TestManualPlacement();

            Debug.Log("=== Click Detection Chain Validation Complete ===");
        }

        private void FindComponents()
        {
            Debug.Log("--- Step 1: Component Discovery ---");

            intersectionDetector = FindObjectOfType<IntersectionDetector>();
            piecePlacement = FindObjectOfType<PiecePlacement>();
            turnManager = FindObjectOfType<TurnManager>();
            gameBoard = FindObjectOfType<GameBoardController>();

            Debug.Log($"IntersectionDetector: {(intersectionDetector != null ? "✓ Found" : "✗ MISSING")}");
            Debug.Log($"PiecePlacement: {(piecePlacement != null ? "✓ Found" : "✗ MISSING")}");
            Debug.Log($"TurnManager: {(turnManager != null ? "✓ Found" : "✗ MISSING")}");
            Debug.Log($"GameBoardController: {(gameBoard != null ? "✓ Found" : "✗ MISSING")}");

            if (intersectionDetector == null || piecePlacement == null || turnManager == null || gameBoard == null)
            {
                Debug.LogError("Critical components missing! Click detection cannot work.");
                return;
            }
        }

        private void ValidateEventSystem()
        {
            Debug.Log("--- Step 2: EventSystem Validation ---");

            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            StandaloneInputModule inputModule = FindObjectOfType<StandaloneInputModule>();
            Physics2DRaycaster physics2DRaycaster = FindObjectOfType<Physics2DRaycaster>();
            GraphicRaycaster graphicRaycaster = FindObjectOfType<GraphicRaycaster>();

            Debug.Log($"EventSystem: {(eventSystem != null ? "✓ Found" : "✗ MISSING")}");
            Debug.Log($"StandaloneInputModule: {(inputModule != null ? "✓ Found" : "✗ MISSING")}");
            Debug.Log($"Physics2DRaycaster: {(physics2DRaycaster != null ? "✓ Found" : "✗ MISSING")}");
            Debug.Log($"GraphicRaycaster: {(graphicRaycaster != null ? "✓ Found" : "✗ MISSING")}");

            if (eventSystem == null || inputModule == null)
            {
                Debug.LogError("EventSystem or StandaloneInputModule missing! UI events will not work.");
            }

            if (physics2DRaycaster == null)
            {
                Debug.LogWarning("Physics2DRaycaster missing! 2D collider clicks may not work.");
            }
        }

        private void ValidateColliderSetup()
        {
            Debug.Log("--- Step 3: Collider Validation ---");

            if (intersectionDetector != null)
            {
                BoxCollider2D collider = intersectionDetector.GetComponent<BoxCollider2D>();
                Debug.Log($"BoxCollider2D on IntersectionDetector: {(collider != null ? "✓ Found" : "✗ MISSING")}");

                if (collider != null)
                {
                    Debug.Log($"Collider Size: {collider.size}");
                    Debug.Log($"Collider Offset: {collider.offset}");
                    Debug.Log($"Is Trigger: {collider.isTrigger}");
                    Debug.Log($"Enabled: {collider.enabled}");
                }
            }
        }

        private void ValidateEventSubscriptions()
        {
            Debug.Log("--- Step 4: Event Subscription Validation ---");

            if (intersectionDetector != null && piecePlacement != null)
            {
                // Check if PiecePlacement is subscribed to IntersectionDetector events
                int listenerCount = intersectionDetector.OnIntersectionClicked?.GetInvocationList().Length ?? 0;
                Debug.Log($"OnIntersectionClicked listeners: {listenerCount}");

                // Test event subscription by triggering a manual event
                bool eventReceived = false;
                System.Action<int, int> testHandler = (x, y) => eventReceived = true;
                intersectionDetector.OnIntersectionClicked += testHandler;

                // Trigger test event
                intersectionDetector.OnIntersectionClicked?.Invoke(0, 0);

                if (eventReceived)
                {
                    Debug.Log("✓ Event system working correctly");
                }
                else
                {
                    Debug.LogError("✗ Event system not working!");
                }

                // Clean up test handler
                intersectionDetector.OnIntersectionClicked -= testHandler;
            }
        }

        private void TestManualPlacement()
        {
            Debug.Log("--- Step 5: Manual Placement Test ---");

            if (piecePlacement != null && turnManager != null && gameBoard != null)
            {
                Debug.Log($"Game Active: {piecePlacement.GameActive}");
                Debug.Log($"Current Player: {turnManager.CurrentPlayer}");
                Debug.Log($"Board Size: {gameBoard.BoardSize}");

                // Try to place a piece at center
                int center = gameBoard.BoardSize / 2;
                bool placementResult = piecePlacement.TryPlacePiece(center, center, turnManager.CurrentPlayer);

                Debug.Log($"Manual placement at ({center}, {center}): {(placementResult ? "✓ Success" : "✗ Failed")}");

                if (placementResult)
                {
                    Debug.Log("✓ Piece placement logic working correctly");
                }
                else
                {
                    Debug.LogError("✗ Piece placement logic failed!");
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 220, 300, 150));
            GUILayout.Label("Click Chain Validator", GUI.skin.box);
            GUILayout.Label($"Press '{testKey}' to validate");

            if (intersectionDetector != null && piecePlacement != null && turnManager != null && gameBoard != null)
            {
                GUILayout.Label("All components: ✓ Found");
                GUILayout.Label($"Game Active: {piecePlacement.GameActive}");
                GUILayout.Label($"Current Player: {turnManager.CurrentPlayer}");
            }
            else
            {
                GUI.color = Color.red;
                GUILayout.Label("Components: ✗ Missing");
                GUI.color = Color.white;
            }

            GUILayout.EndArea();
        }
    }
}