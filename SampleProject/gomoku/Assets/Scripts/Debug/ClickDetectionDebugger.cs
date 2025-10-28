using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Gomoku;
using Gomoku.UI;
using Gomoku.Core;

namespace Gomoku.Diagnostics
{
    /// <summary>
    /// Debug utility to test click detection functionality
    /// </summary>
    public class ClickDetectionDebugger : MonoBehaviour, IPointerClickHandler
    {
        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private KeyCode debugKey = KeyCode.D;

        private IntersectionDetector intersectionDetector;
        private PiecePlacement piecePlacement;
        private GameBoardController gameBoard;

        private void Start()
        {
            // Find required components
            intersectionDetector = FindObjectOfType<IntersectionDetector>();
            piecePlacement = FindObjectOfType<PiecePlacement>();
            gameBoard = FindObjectOfType<GameBoardController>();

            LogDebug("=== Click Detection Debugger Started ===");
            LogDebug($"IntersectionDetector found: {intersectionDetector != null}");
            LogDebug($"PiecePlacement found: {piecePlacement != null}");
            LogDebug($"GameBoardController found: {gameBoard != null}");

            // Check EventSystem
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            LogDebug($"EventSystem found: {eventSystem != null}");

            // Check GraphicRaycaster (for UI)
            GraphicRaycaster graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
            LogDebug($"GraphicRaycaster found: {graphicRaycaster != null}");

            // Check Physics2DRaycaster (for 2D objects)
            Physics2DRaycaster physics2DRaycaster = FindObjectOfType<Physics2DRaycaster>();
            LogDebug($"Physics2DRaycaster found: {physics2DRaycaster != null}");

            // Subscribe to events
            if (intersectionDetector != null)
            {
                intersectionDetector.OnIntersectionClicked += OnIntersectionClicked;
                intersectionDetector.OnValidIntersectionDetected += OnValidIntersectionDetected;
                LogDebug("Subscribed to IntersectionDetector events");
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(debugKey))
            {
                LogDebug("=== Manual Debug Trigger ===");
                TestManualPlacement();
            }

            // Log mouse position for debugging
            if (Input.GetMouseButtonDown(0) && enableDebugLogs)
            {
                Vector3 mousePos = Input.mousePosition;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
                LogDebug($"Mouse click detected at Screen: {mousePos}, World: {worldPos}");
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            LogDebug($"=== Direct Click Event Received ===");
            LogDebug($"Button: {eventData.button}");
            LogDebug($"Position: {eventData.position}");
            LogDebug($"Press Position: {eventData.pressPosition}");

            Vector3 worldPosition = GetWorldPositionFromPointer(eventData);
            LogDebug($"World Position: {worldPosition}");

            if (intersectionDetector != null)
            {
                if (intersectionDetector.TryGetIntersectionFromWorldPosition(worldPosition, out int x, out int y))
                {
                    LogDebug($"Intersection found at: ({x}, {y})");
                }
                else
                {
                    LogDebug("No valid intersection found at this position");
                }
            }
        }

        private void OnIntersectionClicked(int x, int y)
        {
            LogDebug($"=== Intersection Clicked Event Received ===");
            LogDebug($"Coordinates: ({x}, {y})");
            LogDebug($"Current player: {piecePlacement?.GameActive}");

            if (piecePlacement != null)
            {
                LogDebug($"Game active: {piecePlacement.GameActive}");
                if (piecePlacement.TryPlacePiece(x, y, piecePlacement.gameObject.GetComponent<TurnManager>()?.CurrentPlayer ?? PlayerType.PlayerOne))
                {
                    LogDebug($"Piece successfully placed at ({x}, {y})");
                }
                else
                {
                    LogDebug($"Failed to place piece at ({x}, {y})");
                }
            }
        }

        private void OnValidIntersectionDetected(int x, int y)
        {
            LogDebug($"Valid intersection detected at: ({x}, {y})");
        }

        private void TestManualPlacement()
        {
            if (piecePlacement != null && gameBoard != null)
            {
                // Try to place a piece at center of board
                int centerCoord = gameBoard.BoardSize / 2;
                bool placed = piecePlacement.TryPlacePiece(centerCoord, centerCoord, Gomoku.Core.PlayerType.PlayerOne);
                LogDebug($"Manual placement at ({centerCoord}, {centerCoord}): {placed}");
            }
        }

        private Vector3 GetWorldPositionFromPointer(PointerEventData eventData)
        {
            Camera camera = eventData.pressEventCamera ?? Camera.main;
            if (camera == null)
            {
                LogDebug("No camera found!");
                return Vector3.zero;
            }

            return camera.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, 10));
        }

        private void LogDebug(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[ClickDebugger] {message}");
            }
        }

        private void OnDestroy()
        {
            if (intersectionDetector != null)
            {
                intersectionDetector.OnIntersectionClicked -= OnIntersectionClicked;
                intersectionDetector.OnValidIntersectionDetected -= OnValidIntersectionDetected;
            }
        }

        private void OnGUI()
        {
            if (!enableDebugLogs) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("Click Detection Debug Info", GUI.skin.box);
            GUILayout.Label($"Press '{debugKey}' for manual test");
            GUILayout.Label($"EventSystem: {FindObjectOfType<EventSystem>() != null}");
            GUILayout.Label($"GraphicRaycaster: {FindObjectOfType<GraphicRaycaster>() != null}");
            GUILayout.Label($"Physics2DRaycaster: {FindObjectOfType<Physics2DRaycaster>() != null}");

            if (intersectionDetector != null)
            {
                GUILayout.Label("IntersectionDetector: Connected");
            }
            else
            {
                GUILayout.Label("IntersectionDetector: NOT FOUND", GUI.skin.label);
            }
            GUILayout.EndArea();
        }
    }
}