using UnityEngine;
using UnityEngine.EventSystems;

namespace Gomoku.Diagnostics
{
    /// <summary>
    /// Simple click tester to verify basic input detection
    /// </summary>
    public class SimpleClickTester : MonoBehaviour, IPointerClickHandler
    {
        [Header("Test Settings")]
        [SerializeField] private bool enableLogs = true;
        [SerializeField] private KeyCode testKey = KeyCode.T;

        private void Start()
        {
            // Check EventSystem
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            Log($"EventSystem found: {eventSystem != null}");

            // Check Physics2DRaycaster
            Physics2DRaycaster raycaster = FindObjectOfType<Physics2DRaycaster>();
            Log($"Physics2DRaycaster found: {raycaster != null}");

            // Check if we have a collider
            Collider2D collider = GetComponent<Collider2D>();
            Log($"Collider2D found: {collider != null}");

            if (collider != null)
            {
                Log($"Collider type: {collider.GetType().Name}");
                Log($"Collider enabled: {collider.enabled}");
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(testKey))
            {
                Log("=== Manual Test Triggered ===");
                TestClickDetection();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Log($"=== CLICK DETECTED ===");
            Log($"Button: {eventData.button}");
            Log($"Position: {eventData.position}");
            Log($"Click works!");
        }

        private void TestClickDetection()
        {
            // Try to find main game components
            var intersectionDetector = FindObjectOfType<Gomoku.IntersectionDetector>();
            var piecePlacement = FindObjectOfType<Gomoku.PiecePlacement>();

            Log($"IntersectionDetector found: {intersectionDetector != null}");
            Log($"PiecePlacement found: {piecePlacement != null}");

            if (intersectionDetector != null && piecePlacement != null)
            {
                // Test if the event system is working
                Log("Both main components found - click detection should work");
            }
            else
            {
                Log("WARNING: Main components missing - this may be why clicks aren't working");
            }
        }

        private void Log(string message)
        {
            if (enableLogs)
            {
                Debug.Log($"[SimpleClickTester] {message}");
            }
        }

        private void OnGUI()
        {
            if (!enableLogs) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 100));
            GUILayout.Label("Simple Click Tester", GUI.skin.box);
            GUILayout.Label($"Press '{testKey}' to test");
            GUILayout.Label("Click on this object to test");
            GUILayout.EndArea();
        }
    }
}