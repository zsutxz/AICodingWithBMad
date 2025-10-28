using UnityEngine;
using Gomoku.Core;
using Gomoku.UI;
using Gomoku;

namespace Gomoku.Diagnostics
{
    /// <summary>
    /// Runtime setup tool for fixing missing game components
    /// Add this to any GameObject in the scene to fix event subscription issues
    /// </summary>
    public class RuntimeGameSetup : MonoBehaviour
    {
        [Header("Setup Options")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private bool showUI = true;

        private void Start()
        {
            Debug.Log("[RuntimeGameSetup] Started");

            if (autoSetupOnStart)
            {
                // Delay setup to ensure all scene objects are loaded
                Invoke("PerformFullSetup", 1f);
            }
        }

        /// <summary>
        /// Performs complete game system setup
        /// </summary>
        [ContextMenu("Perform Full Setup")]
        public void PerformFullSetup()
        {
            Debug.Log("=== PERFORMING FULL GAME SETUP ===");

            // Step 1: Create missing components
            CreateEventSystem();
            CreateGameManagers();

            // Step 2: Setup event subscriptions
            SetupEventSubscriptions();

            // Step 3: Validate everything
            ValidateSetup();

            Debug.Log("=== FULL SETUP COMPLETED ===");
        }

        private void CreateEventSystem()
        {
            Debug.Log("--- Creating Event System ---");

            // Check for EventSystem
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Debug.Log("✓ EventSystem created");
            }
            else
            {
                Debug.Log("✓ EventSystem already exists");
            }

            // Create EventSystemSetup if missing
            if (FindObjectOfType<UI.EventSystemSetup>() == null)
            {
                GameObject setupGO = new GameObject("EventSystemSetup");
                setupGO.AddComponent<UI.EventSystemSetup>();
                Debug.Log("✓ EventSystemSetup created");
            }
            else
            {
                Debug.Log("✓ EventSystemSetup already exists");
            }
        }

        private void CreateGameManagers()
        {
            Debug.Log("--- Creating Game Managers ---");

            // Create TurnManager if missing
            if (FindObjectOfType<TurnManager>() == null)
            {
                GameObject turnManagerGO = new GameObject("TurnManager");
                turnManagerGO.AddComponent<TurnManager>();
                Debug.Log("✓ TurnManager created");
            }
            else
            {
                Debug.Log("✓ TurnManager already exists");
            }

            // Create PiecePlacement if missing
            if (FindObjectOfType<PiecePlacement>() == null)
            {
                GameObject placementGO = new GameObject("PiecePlacement");
                placementGO.AddComponent<PiecePlacement>();
                Debug.Log("✓ PiecePlacement created");
            }
            else
            {
                Debug.Log("✓ PiecePlacement already exists");
            }

            // Create EventSubscriptionDebugger if missing
            if (FindObjectOfType<EventSubscriptionDebugger>() == null)
            {
                GameObject debuggerGO = new GameObject("EventSubscriptionDebugger");
                debuggerGO.AddComponent<EventSubscriptionDebugger>();
                Debug.Log("✓ EventSubscriptionDebugger created");
            }
            else
            {
                Debug.Log("✓ EventSubscriptionDebugger already exists");
            }
        }

        private void SetupEventSubscriptions()
        {
            Debug.Log("--- Setting Up Event Subscriptions ---");

            var piecePlacement = FindObjectOfType<PiecePlacement>();
            if (piecePlacement != null)
            {
                Debug.Log("Calling SetupReferences on PiecePlacement...");
                piecePlacement.SetupReferences();
                Debug.Log("✓ SetupReferences called");
            }
            else
            {
                Debug.LogError("✗ PiecePlacement not found for event subscription!");
            }
        }

        private void ValidateSetup()
        {
            Debug.Log("--- Validating Setup ---");

            var detector = FindObjectOfType<IntersectionDetector>();
            var placement = FindObjectOfType<PiecePlacement>();
            var turnManager = FindObjectOfType<TurnManager>();
            var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();

            Debug.Log($"EventSystem: {(eventSystem != null ? "✓" : "✗")}");
            Debug.Log($"IntersectionDetector: {(detector != null ? "✓" : "✗")}");
            Debug.Log($"PiecePlacement: {(placement != null ? "✓" : "✗")}");
            Debug.Log($"TurnManager: {(turnManager != null ? "✓" : "✗")}");

            if (detector != null && placement != null)
            {
                int listenerCount = detector.OnIntersectionClicked?.GetInvocationList().Length ?? 0;
                Debug.Log($"Event listeners: {listenerCount}");
                Debug.Log($"Click detection: {(listenerCount > 0 ? "✓ WORKING" : "✗ BROKEN")}");

                if (listenerCount == 0)
                {
                    Debug.LogError("🚨 CRITICAL: No event listeners! Clicks will not work!");
                    Debug.LogError("Try clicking the 'Fix Event Subscription' button in the EventSubscriptionDebugger UI");
                }
                else
                {
                    Debug.Log("🎉 SUCCESS: Event subscription is working!");
                }
            }
        }

        private void OnGUI()
        {
            if (!showUI) return;

            GUILayout.BeginArea(new Rect(Screen.width - 260, 10, 250, 400));
            GUILayout.Label("Runtime Game Setup", GUI.skin.box);

            if (GUILayout.Button("Perform Full Setup"))
            {
                PerformFullSetup();
            }

            GUILayout.Space(10);
            GUILayout.Label("Quick Actions:");

            if (GUILayout.Button("Create Missing Components"))
            {
                CreateEventSystem();
                CreateGameManagers();
            }

            if (GUILayout.Button("Fix Event Subscription"))
            {
                SetupEventSubscriptions();
            }

            if (GUILayout.Button("Validate Setup"))
            {
                ValidateSetup();
            }

            GUILayout.Space(10);
            GUILayout.Label("Current Status:");

            var detector = FindObjectOfType<IntersectionDetector>();
            var placement = FindObjectOfType<PiecePlacement>();

            GUILayout.Label($"IntersectionDetector: {(detector != null ? "✓" : "✗")}");
            GUILayout.Label($"PiecePlacement: {(placement != null ? "✓" : "✗")}");

            if (detector != null)
            {
                int listeners = detector.OnIntersectionClicked?.GetInvocationList().Length ?? 0;
                GUILayout.Label($"Event Listeners: {listeners}");

                if (listeners > 0)
                {
                    GUI.color = Color.green;
                    GUILayout.Label("Click Detection: ✓ WORKING");
                    GUI.color = Color.white;
                }
                else
                {
                    GUI.color = Color.red;
                    GUILayout.Label("Click Detection: ✗ BROKEN");
                    GUI.color = Color.white;
                }
            }

            GUILayout.EndArea();
        }
    }
}