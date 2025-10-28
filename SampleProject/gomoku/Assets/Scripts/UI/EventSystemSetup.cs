using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        private void Start()
        {
            if (setupOnStart)
            {
                SetupEventSystem();
            }
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

            Debug.Log("EventSystem setup complete");
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

            Debug.Log("=== EventSystem Validation ===");
            Debug.Log($"EventSystem: {(hasEventSystem ? "✓" : "✗ MISSING")}");
            Debug.Log($"StandaloneInputModule: {(hasStandaloneInputModule ? "✓" : "✗ MISSING")}");
            Debug.Log($"Physics2DRaycaster: {(hasPhysics2DRaycaster ? "✓" : "✗ MISSING")}");
            Debug.Log($"GraphicRaycaster: {(hasGraphicRaycaster ? "✓" : "✗ MISSING")}");

            if (!hasEventSystem || !hasStandaloneInputModule)
            {
                Debug.LogWarning("Critical EventSystem components missing! UI interaction will not work.");
            }

            if (!hasPhysics2DRaycaster)
            {
                Debug.LogWarning("Physics2DRaycaster missing! 2D collider clicks will not work.");
            }
        }
    }
}