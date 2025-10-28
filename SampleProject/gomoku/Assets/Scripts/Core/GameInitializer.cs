using UnityEngine;

namespace Gomoku.Core
{
    /// <summary>
    /// Automatically initializes all required game systems on scene start
    /// </summary>
    public class GameInitializer : MonoBehaviour
    {
        [Header("Auto Initialization")]
        [SerializeField] private bool initializeOnStart = true;
        [SerializeField] private bool destroyAfterInitialization = true;

        private void Start()
        {
            if (initializeOnStart)
            {
                InitializeGameSystems();
            }
        }

        /// <summary>
        /// Initializes all required game systems
        /// </summary>
        [ContextMenu("Initialize Game Systems")]
        public void InitializeGameSystems()
        {
            Debug.Log("=== Game Systems Initialization Started ===");

            // Step 1: Create EventSystemSetup if needed
            CreateEventSystemSetup();

            // Step 2: Wait a frame and call setup
            StartCoroutine(DelayedSetup());

            Debug.Log("=== Game Systems Initialization Scheduled ===");
        }

        private void CreateEventSystemSetup()
        {
            UI.EventSystemSetup eventSystemSetup = FindObjectOfType<UI.EventSystemSetup>();
            if (eventSystemSetup == null)
            {
                GameObject eventSystemGO = new GameObject("EventSystemSetup");
                eventSystemSetup = eventSystemGO.AddComponent<UI.EventSystemSetup>();
                Debug.Log("EventSystemSetup created automatically");
            }
            else
            {
                Debug.Log("EventSystemSetup already exists");
            }
        }

        private System.Collections.IEnumerator DelayedSetup()
        {
            // Wait one frame to ensure all components are initialized
            yield return null;

            Debug.Log("--- Performing delayed setup ---");

            // Find and call setup
            UI.EventSystemSetup eventSystemSetup = FindObjectOfType<UI.EventSystemSetup>();
            if (eventSystemSetup != null)
            {
                eventSystemSetup.SetupEventSystem();
                Debug.Log("EventSystemSetup.SetupEventSystem() called");

                // Wait another frame and validate
                yield return null;
                eventSystemSetup.ValidateSetup();
                Debug.Log("EventSystemSetup.ValidateSetup() called");
            }
            else
            {
                Debug.LogError("EventSystemSetup not found even after creation!");
            }

            if (destroyAfterInitialization)
            {
                Debug.Log("GameInitializer completed, destroying self");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("GameInitializer completed, keeping object for debugging");
            }
        }

        /// <summary>
        /// Manual validation of all game systems
        /// </summary>
        [ContextMenu("Validate All Systems")]
        public void ValidateAllSystems()
        {
            Debug.Log("=== Manual System Validation ===");

            UI.EventSystemSetup eventSystemSetup = FindObjectOfType<UI.EventSystemSetup>();
            if (eventSystemSetup != null)
            {
                eventSystemSetup.ValidateSetup();
            }
            else
            {
                Debug.LogError("EventSystemSetup not found!");
            }

            // Additional validation
            bool hasIntersectionDetector = FindObjectOfType<IntersectionDetector>() != null;
            bool hasPiecePlacement = FindObjectOfType<PiecePlacement>() != null;

            Debug.Log($"IntersectionDetector: {(hasIntersectionDetector ? "✓" : "✗ MISSING")}");
            Debug.Log($"PiecePlacement: {(hasPiecePlacement ? "✓" : "✗ MISSING")}");

            if (hasIntersectionDetector && hasPiecePlacement)
            {
                var detector = FindObjectOfType<IntersectionDetector>();
                int listenerCount = detector.OnIntersectionClicked?.GetInvocationList().Length ?? 0;
                Debug.Log($"Event subscription status: {(listenerCount > 0 ? $"✓ {listenerCount} listeners" : "✗ NO LISTENERS")}");
            }
        }
    }
}