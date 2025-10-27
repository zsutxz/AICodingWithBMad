using UnityEngine;
using Gomoku.Core;
using Gomoku.Systems;
using Gomoku.UI;

namespace Gomoku
{
    /// <summary>
    /// Manages preview pieces when hovering over board intersections
    /// </summary>
    public class PiecePreviewManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameBoardController gameBoard;
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private IntersectionDetector intersectionDetector;
        [SerializeField] private PiecePlacement piecePlacement;

        [Header("Preview Settings")]
        [SerializeField] private GameObject previewPiecePrefab;
        [SerializeField] private Color blackPreviewColor = new Color(0f, 0f, 0f, 0.6f);
        [SerializeField] private Color whitePreviewColor = new Color(1f, 1f, 1f, 0.6f);
        [SerializeField] private float previewScale = 0.9f;
        [SerializeField] private bool enablePreview = true;

        [Header("Animation Settings")]
        [SerializeField] private float fadeInDuration = 0.15f;
        [SerializeField] private float fadeOutDuration = 0.1f;
        [SerializeField] private float pulseAmplitude = 0.05f;
        [SerializeField] private float pulseFrequency = 2f;

        // Preview state
        private GameObject currentPreview;
        private Vector2Int currentPreviewPosition = new Vector2Int(-1, -1);
        private bool isPreviewActive = false;
        private Coroutine previewAnimation;

        private void Awake()
        {
            InitializePreviewManager();
        }

        private void Start()
        {
            SetupReferences();
        }

        private void OnDestroy()
        {
            CleanupEvents();
        }

        /// <summary>
        /// Initializes the preview manager
        /// </summary>
        private void InitializePreviewManager()
        {
            // Auto-create preview prefab if not assigned
            if (previewPiecePrefab == null)
            {
                CreateDefaultPreviewPrefab();
            }
        }

        /// <summary>
        /// Sets up component references and event subscriptions
        /// </summary>
        private void SetupReferences()
        {
            // Auto-find components if not assigned
            if (gameBoard == null)
                gameBoard = FindObjectOfType<GameBoardController>();

            if (turnManager == null)
                turnManager = FindObjectOfType<TurnManager>();

            if (intersectionDetector == null)
                intersectionDetector = FindObjectOfType<IntersectionDetector>();

            if (piecePlacement == null)
                piecePlacement = FindObjectOfType<PiecePlacement>();

            // Subscribe to intersection detector events
            if (intersectionDetector != null)
            {
                intersectionDetector.OnHoverStart += OnHoverStart;
                intersectionDetector.OnHoverMove += OnHoverMove;
                intersectionDetector.OnHoverEnd += OnHoverEnd;
            }

            // Subscribe to turn manager events
            if (turnManager != null)
            {
                turnManager.OnPlayerTurnChanged.AddListener(OnTurnChanged);
            }

            Debug.Log("PiecePreviewManager initialized");
        }

        /// <summary>
        /// Creates a default preview prefab
        /// </summary>
        private void CreateDefaultPreviewPrefab()
        {
            GameObject previewObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            previewObj.name = "DefaultPreviewPiece";

            // Configure renderer
            Renderer renderer = previewObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Create a simple material with transparency
                Material previewMaterial = new Material(Shader.Find("Standard"));
                previewMaterial.color = blackPreviewColor;
                previewMaterial.SetFloat("_Mode", 3); // Transparent mode
                previewMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                previewMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                previewMaterial.SetInt("_ZWrite", 0);
                previewMaterial.DisableKeyword("_ALPHATEST_ON");
                previewMaterial.EnableKeyword("_ALPHABLEND_ON");
                previewMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                previewMaterial.renderQueue = 3000;

                renderer.material = previewMaterial;
            }

            // Remove collider
            Collider collider = previewObj.GetComponent<Collider>();
            if (collider != null)
            {
                DestroyImmediate(collider);
            }

            previewPiecePrefab = previewObj;
            previewObj.SetActive(false);

            Debug.Log("Created default preview piece prefab");
        }

        /// <summary>
        /// Handles hover start event
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        private void OnHoverStart(int x, int y)
        {
            if (!enablePreview) return;

            ShowPreview(x, y);
        }

        /// <summary>
        /// Handles hover move event
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        private void OnHoverMove(int x, int y)
        {
            if (!enablePreview) return;

            UpdatePreview(x, y);
        }

        /// <summary>
        /// Handles hover end event
        /// </summary>
        private void OnHoverEnd()
        {
            HidePreview();
        }

        /// <summary>
        /// Handles turn change event
        /// </summary>
        /// <param name="newPlayer">New current player</param>
        private void OnTurnChanged(PlayerType newPlayer)
        {
            // Update preview color if currently showing
            if (isPreviewActive && currentPreviewPosition.x >= 0)
            {
                UpdatePreviewColor(newPlayer);
            }
        }

        /// <summary>
        /// Shows preview at specified position
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        private void ShowPreview(int x, int y)
        {
            if (!enablePreview || gameBoard == null) return;

            // Check if position is valid and not occupied
            if (!gameBoard.IsValidCoordinate(x, y)) return;
            if (piecePlacement != null && piecePlacement.IsPositionOccupied(x, y)) return;

            currentPreviewPosition = new Vector2Int(x, y);

            // Create preview if not exists
            if (currentPreview == null)
            {
                currentPreview = Instantiate(previewPiecePrefab, transform);
                currentPreview.name = "PreviewPiece";
            }

            // Position the preview
            Vector3 worldPosition = gameBoard.BoardToWorldPosition(x, y);
            currentPreview.transform.position = worldPosition;
            currentPreview.transform.localScale = Vector3.one * previewScale;

            // Set color based on current player
            UpdatePreviewColor(turnManager?.CurrentPlayer ?? PlayerType.PlayerOne);

            // Start animation
            StartPreviewAnimation();

            isPreviewActive = true;
        }

        /// <summary>
        /// Updates preview position
        /// </summary>
        /// <param name="x">New X coordinate</param>
        /// <param name="y">New Y coordinate</param>
        private void UpdatePreview(int x, int y)
        {
            if (!isPreviewActive || currentPreview == null) return;

            // Check if position actually changed
            if (currentPreviewPosition.x == x && currentPreviewPosition.y == y) return;

            // Check if new position is valid and not occupied
            if (!gameBoard.IsValidCoordinate(x, y) ||
                (piecePlacement != null && piecePlacement.IsPositionOccupied(x, y)))
            {
                HidePreview();
                return;
            }

            currentPreviewPosition = new Vector2Int(x, y);

            // Update position
            Vector3 worldPosition = gameBoard.BoardToWorldPosition(x, y);
            currentPreview.transform.position = worldPosition;
        }

        /// <summary>
        /// Hides the preview
        /// </summary>
        private void HidePreview()
        {
            if (previewAnimation != null)
            {
                StopCoroutine(previewAnimation);
                previewAnimation = null;
            }

            if (currentPreview != null)
            {
                currentPreview.SetActive(false);
            }

            currentPreviewPosition = new Vector2Int(-1, -1);
            isPreviewActive = false;
        }

        /// <summary>
        /// Updates preview color based on player type
        /// </summary>
        /// <param name="playerType">Current player type</param>
        private void UpdatePreviewColor(PlayerType playerType)
        {
            if (currentPreview == null) return;

            Color targetColor = playerType == PlayerType.PlayerOne ? blackPreviewColor : whitePreviewColor;
            Renderer renderer = currentPreview.GetComponent<Renderer>();

            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = targetColor;
            }
        }

        /// <summary>
        /// Starts preview animation
        /// </summary>
        private void StartPreviewAnimation()
        {
            if (previewAnimation != null)
            {
                StopCoroutine(previewAnimation);
            }

            previewAnimation = StartCoroutine(AnimatePreview());
        }

        /// <summary>
        /// Animates the preview piece
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        private System.Collections.IEnumerator AnimatePreview()
        {
            if (currentPreview == null) yield break;

            currentPreview.SetActive(true);

            float time = 0f;
            Vector3 baseScale = Vector3.one * previewScale;

            while (isPreviewActive)
            {
                time += Time.deltaTime;

                // Pulse animation
                float pulseFactor = 1f + Mathf.Sin(time * pulseFrequency * 2f * Mathf.PI) * pulseAmplitude;
                currentPreview.transform.localScale = baseScale * pulseFactor;

                yield return null;
            }
        }

        /// <summary>
        /// Enables or disables preview
        /// </summary>
        /// <param name="enable">Whether to enable preview</param>
        public void SetPreviewEnabled(bool enable)
        {
            enablePreview = enable;
            if (!enable)
            {
                HidePreview();
            }
        }

        /// <summary>
        /// Updates preview colors
        /// </summary>
        /// <param name="blackColor">New black preview color</param>
        /// <param name="whiteColor">New white preview color</param>
        public void UpdatePreviewColors(Color blackColor, Color whiteColor)
        {
            blackPreviewColor = blackColor;
            whitePreviewColor = whiteColor;

            // Update current preview if active
            if (isPreviewActive)
            {
                UpdatePreviewColor(turnManager?.CurrentPlayer ?? PlayerType.PlayerOne);
            }
        }

        /// <summary>
        /// Cleans up event subscriptions
        /// </summary>
        private void CleanupEvents()
        {
            if (intersectionDetector != null)
            {
                intersectionDetector.OnHoverStart -= OnHoverStart;
                intersectionDetector.OnHoverMove -= OnHoverMove;
                intersectionDetector.OnHoverEnd -= OnHoverEnd;
            }

            if (turnManager != null)
            {
                turnManager.OnPlayerTurnChanged.RemoveListener(OnTurnChanged);
            }

            if (previewAnimation != null)
            {
                StopCoroutine(previewAnimation);
            }
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for testing preview
        /// </summary>
        [ContextMenu("Test Preview")]
        private void TestPreview()
        {
            if (gameBoard != null)
            {
                int center = gameBoard.BoardSize / 2;
                ShowPreview(center, center);
            }
        }

        /// <summary>
        /// Editor-only method for hiding preview
        /// </summary>
        [ContextMenu("Hide Preview")]
        private void HidePreviewTest()
        {
            HidePreview();
        }
        #endif
    }
}