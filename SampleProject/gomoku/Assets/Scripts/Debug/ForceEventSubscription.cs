using UnityEngine;
using Gomoku;
using Gomoku.Core;
using Gomoku.UI;

namespace Gomoku.Diagnostics
{
    /// <summary>
    /// Forces event subscription by directly calling PiecePlacement when IntersectionDetector is clicked
    /// This is a brute-force solution to ensure click events work
    /// </summary>
    public class ForceEventSubscription : MonoBehaviour
    {
        [Header("Force Subscription Settings")]
        [SerializeField] private bool enableForceMode = true;
        [SerializeField] private bool showDebugLogs = true;
        [SerializeField] private bool autoFindAndSubscribe = true;

        private IntersectionDetector intersectionDetector;
        private PiecePlacement piecePlacement;
        private TurnManager turnManager;

        private void Start()
        {
            if (autoFindAndSubscribe)
            {
                // Delay to ensure all components are initialized
                Invoke("ForceSetup", 0.1f);
            }
        }

        /// <summary>
        /// Force setup all components and event subscriptions
        /// </summary>
        [ContextMenu("Force Setup")]
        public void ForceSetup()
        {
            Debug.Log("=== FORCE EVENT SUBSCRIPTION SETUP ===");

            // Find all components
            intersectionDetector = FindObjectOfType<IntersectionDetector>();
            piecePlacement = FindObjectOfType<PiecePlacement>();
            turnManager = FindObjectOfType<TurnManager>();

            Debug.Log($"IntersectionDetector: {intersectionDetector != null}");
            Debug.Log($"PiecePlacement: {piecePlacement != null}");
            Debug.Log($"TurnManager: {turnManager != null}");

            if (intersectionDetector == null || piecePlacement == null)
            {
                Debug.LogError("Missing required components!");
                return;
            }

            // Try to subscribe normally first
            TryNormalSubscription();

            // If normal subscription fails, use reflection
            if (!IsSubscriptionWorking())
            {
                Debug.LogWarning("Normal subscription failed, trying reflection method...");
                TryReflectionSubscription();
            }

            // Final check
            ValidateSubscription();
        }

        private void TryNormalSubscription()
        {
            Debug.Log("Attempting normal event subscription...");

            // Force setup PiecePlacement references first
            if (piecePlacement != null)
            {
                piecePlacement.SetupReferences();

                // Wait a moment for setup to complete
                StartCoroutine(DelayedSubscriptionCheck());
            }
        }

        private System.Collections.IEnumerator DelayedSubscriptionCheck()
        {
            yield return new WaitForSeconds(0.1f);

            int listenerCount = intersectionDetector.OnIntersectionClicked?.GetInvocationList().Length ?? 0;
            Debug.Log($"Delayed subscription check - Listeners: {listenerCount}");
        }

        private void TryReflectionSubscription()
        {
            try
            {
                Debug.Log("Using reflection to force event subscription...");

                // Get the private HandleIntersectionClick method
                var handleMethod = typeof(PiecePlacement).GetMethod("HandleIntersectionClick",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (handleMethod != null)
                {
                    // Create a delegate that calls the private method
                    System.Action<int, int> handler = (x, y) =>
                    {
                        if (showDebugLogs)
                            Debug.Log($"[ForceSubscription] Calling HandleIntersectionClick({x}, {y})");

                        try
                        {
                            handleMethod.Invoke(piecePlacement, new object[] { x, y });
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"[ForceSubscription] Error invoking HandleIntersectionClick: {e.Message}");
                        }
                    };

                    // Subscribe to the event
                    intersectionDetector.OnIntersectionClicked += handler;

                    int listenerCount = intersectionDetector.OnIntersectionClicked?.GetInvocationList().Length ?? 0;
                    Debug.Log($"Reflection subscription completed. Listeners: {listenerCount}");

                    if (listenerCount > 0)
                    {
                        Debug.Log("✓ Force subscription successful!");
                    }
                }
                else
                {
                    Debug.LogError("Could not find HandleIntersectionClick method via reflection!");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Reflection subscription failed: {e.Message}");
            }
        }

        private bool IsSubscriptionWorking()
        {
            int listenerCount = intersectionDetector.OnIntersectionClicked?.GetInvocationList().Length ?? 0;
            return listenerCount > 0;
        }

        private void ValidateSubscription()
        {
            int listenerCount = intersectionDetector.OnIntersectionClicked?.GetInvocationList().Length ?? 0;
            Debug.Log($"=== FINAL VALIDATION ===");
            Debug.Log($"Event listeners: {listenerCount}");
            Debug.Log($"Status: {(listenerCount > 0 ? "✓ WORKING" : "✗ STILL BROKEN")}");

            if (listenerCount == 0)
            {
                Debug.LogError("🚨 CRITICAL: Event subscription still not working!");
                Debug.LogError("This indicates a deeper problem with the component setup.");
            }
        }

        /// <summary>
        /// Manual test of the click system
        /// </summary>
        [ContextMenu("Test Click System")]
        public void TestClickSystem()
        {
            Debug.Log("=== TESTING CLICK SYSTEM ===");

            if (intersectionDetector != null && piecePlacement != null && turnManager != null)
            {
                Debug.Log("Simulating a click at position (7, 7)...");

                // Manually invoke the event
                intersectionDetector.OnIntersectionClicked?.Invoke(7, 7);

                Debug.Log("Click simulation completed");
            }
            else
            {
                Debug.LogError("Cannot test - missing components");
            }
        }

        /// <summary>
        /// Emergency fix that creates a completely new PiecePlacement if needed
        /// </summary>
        [ContextMenu("Emergency Fix")]
        public void EmergencyFix()
        {
            Debug.Log("=== EMERGENCY FIX ===");

            // Destroy existing PiecePlacement if it's broken
            var existingPlacements = FindObjectsOfType<PiecePlacement>();
            foreach (var placement in existingPlacements)
            {
                Debug.Log("Destroying existing PiecePlacement");
                DestroyImmediate(placement.gameObject);
            }

            // Create new components
            CreateNewComponents();

            // Setup everything
            ForceSetup();
        }

        private void CreateNewComponents()
        {
            Debug.Log("Creating fresh components...");

            // Create TurnManager if needed
            if (FindObjectOfType<TurnManager>() == null)
            {
                GameObject turnManagerGO = new GameObject("TurnManager");
                turnManagerGO.AddComponent<TurnManager>();
                Debug.Log("✓ Created new TurnManager");
            }

            // Create new PiecePlacement
            GameObject piecePlacementGO = new GameObject("PiecePlacement");
            var newPiecePlacement = piecePlacementGO.AddComponent<PiecePlacement>();
            Debug.Log("✓ Created new PiecePlacement");

            // Initialize the new piece placement
            newPiecePlacement.InitializeGame();
        }

        private void OnGUI()
        {
            if (!showDebugLogs) return;

            GUILayout.BeginArea(new Rect(10, 320, 300, 200));
            GUILayout.Label("Force Event Subscription", GUI.skin.box);

            if (GUILayout.Button("Force Setup"))
            {
                ForceSetup();
            }

            if (GUILayout.Button("Test Click System"))
            {
                TestClickSystem();
            }

            if (GUILayout.Button("Emergency Fix"))
            {
                EmergencyFix();
            }

            // Show current status
            int listeners = 0;
            if (intersectionDetector != null)
            {
                listeners = intersectionDetector.OnIntersectionClicked?.GetInvocationList().Length ?? 0;
            }

            GUILayout.Label($"Listeners: {listeners}");
            if (listeners > 0)
            {
                GUI.color = Color.green;
                GUILayout.Label("Status: ✓ WORKING");
                GUI.color = Color.white;
            }
            else
            {
                GUI.color = Color.red;
                GUILayout.Label("Status: ✗ BROKEN");
                GUI.color = Color.white;
            }

            GUILayout.EndArea();
        }
    }
}