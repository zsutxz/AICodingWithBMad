using UnityEngine;

namespace Gomoku
{
    /// <summary>
    /// Handles the visual rendering of the game board including grid lines and background
    /// </summary>
    public class BoardRenderer : MonoBehaviour
    {
        [Header("Grid Line Settings")]
        [SerializeField] private Material gridLineMaterial;
        [SerializeField] private Color gridLineColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        [SerializeField] private float gridLineWidth = 0.02f;

        [Header("Background Settings")]
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private Sprite boardBackgroundSprite;
        [SerializeField] private Color boardBackgroundColor = new Color(0.8f, 0.7f, 0.5f, 1.0f); // Dark wood tone

        [Header("Intersection Markers")]
        [SerializeField] private GameObject intersectionMarkerPrefab;
        [SerializeField] private Color intersectionMarkerColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        [SerializeField] private float intersectionMarkerSize = 0.05f;

        [Header("Debug Settings")]
        [SerializeField] private bool showGridCoordinates = false;
        [SerializeField] private GameObject coordinateLabelPrefab;

        private int boardSize;
        private float cellSize;
        private Vector2 boardOffset;
        private LineRenderer[] horizontalLines;
        private LineRenderer[] verticalLines;
        private GameObject[] intersectionMarkers;
        private GameObject[] coordinateLabels;

        /// <summary>
        /// Initializes the board renderer with the specified configuration
        /// </summary>
        /// <param name="boardSize">Size of the board (15x15)</param>
        /// <param name="cellSize">Size of each cell in world units</param>
        /// <param name="boardOffset">Offset of the board in world units</param>
        public void Initialize(int boardSize, float cellSize, Vector2 boardOffset)
        {
            this.boardSize = boardSize;
            this.cellSize = cellSize;
            this.boardOffset = boardOffset;

            ClearExistingVisuals();
            SetupBackground();
            CreateGridLines();
            CreateIntersectionMarkers();
            
            if (showGridCoordinates)
            {
                CreateCoordinateLabels();
            }
        }

        /// <summary>
        /// Clears any existing visual elements
        /// </summary>
        private void ClearExistingVisuals()
        {
            // Clear existing grid lines
            if (horizontalLines != null)
            {
                foreach (var line in horizontalLines)
                {
                    if (line != null) DestroyImmediate(line.gameObject);
                }
            }

            if (verticalLines != null)
            {
                foreach (var line in verticalLines)
                {
                    if (line != null) DestroyImmediate(line.gameObject);
                }
            }

            // Clear intersection markers
            if (intersectionMarkers != null)
            {
                foreach (var marker in intersectionMarkers)
                {
                    if (marker != null) DestroyImmediate(marker);
                }
            }

            // Clear coordinate labels
            if (coordinateLabels != null)
            {
                foreach (var label in coordinateLabels)
                {
                    if (label != null) DestroyImmediate(label);
                }
            }
        }

        /// <summary>
        /// Sets up the board background with traditional Chinese aesthetic
        /// </summary>
        private void SetupBackground()
        {
            if (backgroundRenderer == null)
            {
                // Create background GameObject if not assigned
                GameObject backgroundObject = new GameObject("BoardBackground");
                backgroundObject.transform.SetParent(transform);
                backgroundRenderer = backgroundObject.AddComponent<SpriteRenderer>();
            }

            // Configure background
            backgroundRenderer.sprite = boardBackgroundSprite;
            backgroundRenderer.color = boardBackgroundColor;
            backgroundRenderer.sortingOrder = -10;

            // Position and scale background to fit the board
            float boardWidth = (boardSize - 1) * cellSize;
            float boardHeight = (boardSize - 1) * cellSize;
            Vector3 backgroundPosition = new Vector3(
                boardOffset.x + boardWidth / 2,
                boardOffset.y + boardHeight / 2,
                0
            );

            backgroundRenderer.transform.position = backgroundPosition;
            
            // Scale background to be slightly larger than the board
            float padding = 0.5f;
            backgroundRenderer.transform.localScale = new Vector3(
                boardWidth + padding,
                boardHeight + padding,
                1
            );
        }

        /// <summary>
        /// Creates the horizontal and vertical grid lines
        /// </summary>
        private void CreateGridLines()
        {
            horizontalLines = new LineRenderer[boardSize];
            verticalLines = new LineRenderer[boardSize];

            // Create horizontal lines
            for (int i = 0; i < boardSize; i++)
            {
                GameObject lineObject = new GameObject($"HorizontalLine_{i}");
                lineObject.transform.SetParent(transform);
                
                horizontalLines[i] = lineObject.AddComponent<LineRenderer>();
                ConfigureLineRenderer(horizontalLines[i]);

                // Set line positions
                Vector3 startPos = new Vector3(
                    boardOffset.x,
                    boardOffset.y + i * cellSize,
                    0
                );
                Vector3 endPos = new Vector3(
                    boardOffset.x + (boardSize - 1) * cellSize,
                    boardOffset.y + i * cellSize,
                    0
                );

                horizontalLines[i].positionCount = 2;
                horizontalLines[i].SetPosition(0, startPos);
                horizontalLines[i].SetPosition(1, endPos);
            }

            // Create vertical lines
            for (int i = 0; i < boardSize; i++)
            {
                GameObject lineObject = new GameObject($"VerticalLine_{i}");
                lineObject.transform.SetParent(transform);
                
                verticalLines[i] = lineObject.AddComponent<LineRenderer>();
                ConfigureLineRenderer(verticalLines[i]);

                // Set line positions
                Vector3 startPos = new Vector3(
                    boardOffset.x + i * cellSize,
                    boardOffset.y,
                    0
                );
                Vector3 endPos = new Vector3(
                    boardOffset.x + i * cellSize,
                    boardOffset.y + (boardSize - 1) * cellSize,
                    0
                );

                verticalLines[i].positionCount = 2;
                verticalLines[i].SetPosition(0, startPos);
                verticalLines[i].SetPosition(1, endPos);
            }
        }

        /// <summary>
        /// Configures a LineRenderer with common settings
        /// </summary>
        /// <param name="lineRenderer">LineRenderer to configure</param>
        private void ConfigureLineRenderer(LineRenderer lineRenderer)
        {
            lineRenderer.material = gridLineMaterial;
            lineRenderer.startColor = gridLineColor;
            lineRenderer.endColor = gridLineColor;
            lineRenderer.startWidth = gridLineWidth;
            lineRenderer.endWidth = gridLineWidth;
            lineRenderer.useWorldSpace = true;
            lineRenderer.sortingOrder = -5;
        }

        /// <summary>
        /// Creates visual markers at each grid intersection
        /// </summary>
        private void CreateIntersectionMarkers()
        {
            intersectionMarkers = new GameObject[boardSize * boardSize];

            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    Vector3 position = new Vector3(
                        boardOffset.x + x * cellSize,
                        boardOffset.y + y * cellSize,
                        0
                    );

                    GameObject marker;
                    if (intersectionMarkerPrefab != null)
                    {
                        marker = Instantiate(intersectionMarkerPrefab, position, Quaternion.identity, transform);
                    }
                    else
                    {
                        // Create a simple sphere marker if no prefab is assigned
                        marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        marker.transform.SetParent(transform);
                        marker.transform.position = position;
                        marker.transform.localScale = Vector3.one * intersectionMarkerSize;
                        
                        Renderer renderer = marker.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            renderer.material.color = intersectionMarkerColor;
                        }
                    }

                    marker.name = $"Intersection_{x}_{y}";
                    intersectionMarkers[x * boardSize + y] = marker;
                }
            }
        }

        /// <summary>
        /// Creates coordinate labels for debugging purposes
        /// </summary>
        private void CreateCoordinateLabels()
        {
            if (coordinateLabelPrefab == null) return;

            coordinateLabels = new GameObject[boardSize * 2]; // Horizontal and vertical labels
            int labelIndex = 0;

            // Create horizontal coordinate labels (A-O)
            for (int i = 0; i < boardSize; i++)
            {
                char coordinateChar = (char)('A' + i);
                Vector3 position = new Vector3(
                    boardOffset.x + i * cellSize,
                    boardOffset.y - 0.2f,
                    0
                );

                GameObject label = Instantiate(coordinateLabelPrefab, position, Quaternion.identity, transform);
                // Note: In a real implementation, you would set the text component here
                label.name = $"Coordinate_H_{coordinateChar}";
                coordinateLabels[labelIndex++] = label;
            }

            // Create vertical coordinate labels (1-15)
            for (int i = 0; i < boardSize; i++)
            {
                Vector3 position = new Vector3(
                    boardOffset.x - 0.2f,
                    boardOffset.y + i * cellSize,
                    0
                );

                GameObject label = Instantiate(coordinateLabelPrefab, position, Quaternion.identity, transform);
                // Note: In a real implementation, you would set the text component here
                label.name = $"Coordinate_V_{i + 1}";
                coordinateLabels[labelIndex++] = label;
            }
        }

        /// <summary>
        /// Toggles the display of grid coordinates
        /// </summary>
        /// <param name="show">Whether to show coordinates</param>
        public void ToggleGridCoordinates(bool show)
        {
            showGridCoordinates = show;

            if (coordinateLabels != null)
            {
                foreach (var label in coordinateLabels)
                {
                    if (label != null) label.SetActive(show);
                }
            }

            if (show && coordinateLabels == null)
            {
                CreateCoordinateLabels();
            }
        }

        /// <summary>
        /// Updates the board background color
        /// </summary>
        /// <param name="newColor">New background color</param>
        public void UpdateBackgroundColor(Color newColor)
        {
            boardBackgroundColor = newColor;
            if (backgroundRenderer != null)
            {
                backgroundRenderer.color = boardBackgroundColor;
            }
        }

        /// <summary>
        /// Updates the grid line color
        /// </summary>
        /// <param name="newColor">New grid line color</param>
        public void UpdateGridLineColor(Color newColor)
        {
            gridLineColor = newColor;
            
            if (horizontalLines != null)
            {
                foreach (var line in horizontalLines)
                {
                    if (line != null)
                    {
                        line.startColor = gridLineColor;
                        line.endColor = gridLineColor;
                    }
                }
            }

            if (verticalLines != null)
            {
                foreach (var line in verticalLines)
                {
                    if (line != null)
                    {
                        line.startColor = gridLineColor;
                        line.endColor = gridLineColor;
                    }
                }
            }
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            // Update visuals when properties change in the inspector
            if (Application.isPlaying && boardSize > 0)
            {
                Initialize(boardSize, cellSize, boardOffset);
            }
        }
        #endif
    }
}