using UnityEngine;
using Gomoku.Core;

namespace Gomoku.Diagnostics
{
    /// <summary>
    /// Emergency debugger to fix event subscription issues at runtime
    /// </summary>
    public class EventSubscriptionDebugger : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool autoFixOnStart = true;
        [SerializeField] private bool showDebugUI = true;

        private void Start()
        {
            if (autoFixOnStart)
            {
                // Wait a moment for all systems to initialize
                Invoke("FixEventSubscription", 0.5f);
            }
        }

        /// <summary>
        /// Emergency fix for event subscription issues
        /// </summary>
        [ContextMenu("Fix Event Subscription")]
        public void FixEventSubscription()
        {
            Debug.Log("=== EMERGENCY EVENT SUBSCRIPTION FIX ===");

            // Find all components
            var intersectionDetector = FindObjectOfType<IntersectionDetector>();
            var piecePlacement = FindObjectOfType<PiecePlacement>();

            Debug.Log($"IntersectionDetector found: {intersectionDetector != null}");
            Debug.Log($"PiecePlacement found: {piecePlacement != null}");

            if (intersectionDetector == null)
            {
                Debug.LogError("IntersectionDetector not found in scene!");
                return;
            }

            if (piecePlacement == null)
            {
                Debug.LogError("PiecePlacement not found in scene! Creating one...");
                CreateMissingComponents();
                piecePlacement = FindObjectOfType<PiecePlacement>();
            }

            if (intersectionDetector != null && piecePlacement != null)
            {
                // Force setup references
                Debug.Log("Force-calling SetupReferences on PiecePlacement...");
                piecePlacement.SetupReferences();

                // Check subscription status
                int listenerCount = intersectionDetector.OnIntersectionClicked?.GetInvocationList().Length ?? 0;
                Debug.Log($"Event listener count after fix: {listenerCount}");

                if (listenerCount == 0)
                {
                    Debug.LogWarning("Still no listeners! Attempting manual subscription...");
                    TryManualSubscription(intersectionDetector, piecePlacement);
                }
                else
                {
                    Debug.Log("✓ Event subscription fixed successfully!");
                }
            }

            ValidateCompleteSystem();
        }

        private void CreateMissingComponents()
        {
            Debug.Log("Creating missing game components...");

            // Create TurnManager if missing
            if (FindObjectOfType<TurnManager>() == null)
            {
                GameObject turnManagerGO = new GameObject("TurnManager");
                turnManagerGO.AddComponent<TurnManager>();
                Debug.Log("Created TurnManager");
            }

            // Create PiecePlacement if missing
            if (FindObjectOfType<PiecePlacement>() == null)
            {
                GameObject piecePlacementGO = new GameObject("PiecePlacement");
                piecePlacementGO.AddComponent<PiecePlacement>();
                Debug.Log("Created PiecePlacement");
            }
        }

        private void TryManualSubscription(IntersectionDetector detector, PiecePlacement placement)
        {
            try
            {
                Debug.Log("Attempting manual event subscription...");

                // Get the private method using reflection
                var handleMethod = typeof(PiecePlacement).GetMethod("HandleIntersectionClick",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (handleMethod != null)
                {
                    // Create delegate and subscribe manually
                    System.Action<int, int> handler = (x, y) => handleMethod.Invoke(placement, new object[] { x, y });
                    detector.OnIntersectionClicked += handler;

                    int listenerCount = detector.OnIntersectionClicked?.GetInvocationList().Length ?? 0;
                    Debug.Log($"Manual subscription completed. Listener count: {listenerCount}");

                    if (listenerCount > 0)
                    {
                        Debug.Log("✓ Manual subscription successful!");
                    }
                }
                else
                {
                    Debug.LogError("Could not find HandleIntersectionClick method!");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Manual subscription failed: {e.Message}");
            }
        }

        private void ValidateCompleteSystem()
        {
            Debug.Log("=== Complete System Validation ===");

            var detector = FindObjectOfType<IntersectionDetector>();
            var placement = FindObjectOfType<PiecePlacement>();
            var turnManager = FindObjectOfType<TurnManager>();
            var gameBoard = FindObjectOfType<GameBoardController>();

            Debug.Log($"IntersectionDetector: {detector != null}");
            Debug.Log($"PiecePlacement: {placement != null}");
            Debug.Log($"TurnManager: {turnManager != null}");
            Debug.Log($"GameBoardController: {gameBoard != null}");

            if (detector != null)
            {
                int listenerCount = detector.OnIntersectionClicked?.GetInvocationList().Length ?? 0;
                Debug.Log($"Event listeners: {listenerCount}");
                Debug.Log($"Event system working: {listenerCount > 0}");
            }
        }

        private void OnGUI()
        {
            if (!showDebugUI) return;

            GUILayout.BeginArea(new Rect(10, 10, 350, 300));
            GUILayout.Label("Event Subscription Debugger", GUI.skin.box);

            if (GUILayout.Button("Fix Event Subscription"))
            {
                FixEventSubscription();
            }

            if (GUILayout.Button("Validate System"))
            {
                ValidateCompleteSystem();
            }

            // Show current status
            var detector = FindObjectOfType<IntersectionDetector>();
            var placement = FindObjectOfType<PiecePlacement>();

            GUILayout.Label($"IntersectionDetector: {(detector != null ? "✓" : "✗")}");
            GUILayout.Label($"PiecePlacement: {(placement != null ? "✓" : "✗")}");

            if (detector != null)
            {
                int listeners = detector.OnIntersectionClicked?.GetInvocationList().Length ?? 0;
                GUILayout.Label($"Event Listeners: {listeners}");
                GUILayout.Label($"Status: {(listeners > 0 ? "✓ Working" : "✗ Broken")}",
                    listeners > 0 ? Color.green : Color.red);
            }

            GUILayout.EndArea();
        }
    }
}