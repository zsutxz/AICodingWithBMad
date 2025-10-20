using UnityEngine;
using UnityEngine.UI;
using Gomoku.Core;

namespace Gomoku
{
    /// <summary>
    /// Handles the visual rendering of the game board using procedural drawing instead of GameObjects
    /// </summary>
    public class BoardRenderer : MonoBehaviour
    {
        [Header("Grid Line Settings")]
        [SerializeField] private Material gridLineMaterial;
        [SerializeField] private Color gridLineColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        [SerializeField] private float gridLineWidth = 0.02f;

        [Header("Background Settings")]
        [SerializeField] private Color boardBackgroundColor = new Color(0.8f, 0.7f, 0.5f, 1.0f); // Dark wood tone

        [Header("Piece Settings")]
        [SerializeField] private Color blackPieceColor = Color.black;
        [SerializeField] private Color whitePieceColor = Color.white;
        [SerializeField] private float pieceSize = 0.8f;

        [Header("Debug Settings")]
        [SerializeField] private bool showGridCoordinates = false;

        private int boardSize;
        private float cellSize;
        private Vector2 boardOffset;

        /// <summary>
        /// Gets the total board width in world units
        /// </summary>
        public float BoardWidth => (boardSize - 1) * cellSize;

        /// <summary>
        /// Gets the total board height in world units
        /// </summary>
        public float BoardHeight => (boardSize - 1) * cellSize;

        // Meshes for rendering
        private Mesh gridMesh;
        private Mesh pieceMesh;
        private Mesh backgroundMesh;

        // Materials for different elements
        private Material gridMaterial;
        private Material backgroundMaterial;
        private MaterialPropertyBlock blackPiecePropertyBlock;
        private MaterialPropertyBlock whitePiecePropertyBlock;

        // Track placed pieces
        private PlayerType[,] placedPieces;

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

            // Initialize rendering resources
            CreateBackgroundMesh();
            CreateGridMesh();
            CreatePieceMesh();
            CreateMaterials();

            // Initialize piece tracking
            placedPieces = new PlayerType[boardSize, boardSize];
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    placedPieces[x, y] = PlayerType.None;
                }
            }

            // Initialize UI elements
            InitializeUI();

            Debug.Log($"BoardRenderer initialized with size {boardSize}x{boardSize}");
        }

        /// <summary>
        /// Creates a mesh for the board background
        /// </summary>
        private void CreateBackgroundMesh()
        {
            if (backgroundMesh != null)
            {
                DestroyImmediate(backgroundMesh);
            }

            backgroundMesh = new Mesh();

            // Create a quad for the background
            Vector3[] vertices = {
                new Vector3(boardOffset.x, boardOffset.y, 0),
                new Vector3(boardOffset.x + BoardWidth, boardOffset.y, 0),
                new Vector3(boardOffset.x, boardOffset.y + BoardHeight, 0),
                new Vector3(boardOffset.x + BoardWidth, boardOffset.y + BoardHeight, 0)
            };

            int[] triangles = {
                0, 2, 1, // First triangle
                1, 2, 3  // Second triangle
            };

            Vector2[] uv = {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };

            backgroundMesh.vertices = vertices;
            backgroundMesh.triangles = triangles;
            backgroundMesh.uv = uv;
            backgroundMesh.RecalculateNormals();
            backgroundMesh.RecalculateBounds();
        }

        /// <summary>
        /// Creates a mesh for the grid lines
        /// </summary>
        private void CreateGridMesh()
        {
            if (gridMesh != null)
            {
                DestroyImmediate(gridMesh);
            }

            gridMesh = new Mesh();

            // Calculate vertices for all grid lines
            int horizontalLineCount = boardSize;
            int verticalLineCount = boardSize;
            int totalLines = horizontalLineCount + verticalLineCount;
            int verticesPerLine = 2; // Start and end points

            Vector3[] vertices = new Vector3[totalLines * verticesPerLine];
            int[] triangles = new int[totalLines * verticesPerLine * 3]; // Each line needs 2 triangles for width
            Vector2[] uv = new Vector2[totalLines * verticesPerLine];

            int vertexIndex = 0;
            int triangleIndex = 0;

            // Create horizontal lines
            for (int i = 0; i < boardSize; i++)
            {
                float y = boardOffset.y + i * cellSize;

                // Left end of horizontal line
                vertices[vertexIndex] = new Vector3(boardOffset.x, y, 0);
                uv[vertexIndex] = new Vector2(0, 0);

                // Right end of horizontal line
                vertices[vertexIndex + 1] = new Vector3(boardOffset.x + (boardSize - 1) * cellSize, y, 0);
                uv[vertexIndex + 1] = new Vector2(1, 0);

                // Create triangles for line width
                if (vertexIndex > 0)
                {
                    // Create quad between this line and previous line
                    int v0 = vertexIndex - 2;
                    int v1 = vertexIndex - 1;
                    int v2 = vertexIndex;
                    int v3 = vertexIndex + 1;

                    triangles[triangleIndex++] = v0;
                    triangles[triangleIndex++] = v1;
                    triangles[triangleIndex++] = v2;

                    triangles[triangleIndex++] = v1;
                    triangles[triangleIndex++] = v3;
                    triangles[triangleIndex++] = v2;
                }

                vertexIndex += 2;
            }

            // Create vertical lines
            for (int i = 0; i < boardSize; i++)
            {
                float x = boardOffset.x + i * cellSize;

                // Bottom end of vertical line
                vertices[vertexIndex] = new Vector3(x, boardOffset.y, 0);
                uv[vertexIndex] = new Vector2(0, 0);

                // Top end of vertical line
                vertices[vertexIndex + 1] = new Vector3(x, boardOffset.y + (boardSize - 1) * cellSize, 0);
                uv[vertexIndex + 1] = new Vector2(1, 0);

                // Create triangles for line width
                if (vertexIndex > 0)
                {
                    // Create quad between this line and previous line
                    int v0 = vertexIndex - 2;
                    int v1 = vertexIndex - 1;
                    int v2 = vertexIndex;
                    int v3 = vertexIndex + 1;

                    triangles[triangleIndex++] = v0;
                    triangles[triangleIndex++] = v1;
                    triangles[triangleIndex++] = v2;

                    triangles[triangleIndex++] = v1;
                    triangles[triangleIndex++] = v3;
                    triangles[triangleIndex++] = v2;
                }

                vertexIndex += 2;
            }

            // Resize arrays to actual size used
            System.Array.Resize(ref triangles, triangleIndex);

            // Assign to mesh
            gridMesh.vertices = vertices;
            gridMesh.triangles = triangles;
            gridMesh.uv = uv;
            gridMesh.RecalculateNormals();
            gridMesh.RecalculateBounds();
        }

        /// <summary>
        /// Creates a simple quad mesh for pieces
        /// </summary>
        private void CreatePieceMesh()
        {
            if (pieceMesh != null)
            {
                DestroyImmediate(pieceMesh);
            }

            pieceMesh = new Mesh();

            // Create a simple quad for pieces
            Vector3[] vertices = {
                new Vector3(-pieceSize/2, -pieceSize/2, 0),
                new Vector3(pieceSize/2, -pieceSize/2, 0),
                new Vector3(-pieceSize/2, pieceSize/2, 0),
                new Vector3(pieceSize/2, pieceSize/2, 0)
            };

            int[] triangles = {
                0, 2, 1, // First triangle
                1, 2, 3  // Second triangle
            };

            Vector2[] uv = {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };

            pieceMesh.vertices = vertices;
            pieceMesh.triangles = triangles;
            pieceMesh.uv = uv;
            pieceMesh.RecalculateNormals();
            pieceMesh.RecalculateBounds();
        }

        /// <summary>
        /// Creates materials for rendering
        /// </summary>
        private void CreateMaterials()
        {
            // Create grid material
            gridMaterial = new Material(gridLineMaterial);
            gridMaterial.color = gridLineColor;

            // Create background material
            backgroundMaterial = new Material(gridLineMaterial);
            backgroundMaterial.color = boardBackgroundColor;

            // Create property blocks for pieces to avoid material instances
            blackPiecePropertyBlock = new MaterialPropertyBlock();
            blackPiecePropertyBlock.SetColor("_Color", blackPieceColor);

            whitePiecePropertyBlock = new MaterialPropertyBlock();
            whitePiecePropertyBlock.SetColor("_Color", whitePieceColor);
        }

        /// <summary>
        /// Reference to the canvas that will contain the board UI elements
        /// </summary>
        [SerializeField] private Canvas canvas;

        /// <summary>
        /// Reference to the panel that will act as the board container
        /// </summary>
        [SerializeField] private RectTransform boardPanel;

        /// <summary>
        /// Prefab for grid lines
        /// </summary>
        [SerializeField] private GameObject gridLinePrefab;

        /// <summary>
        /// Prefab for game pieces
        /// </summary>
        [SerializeField] private GameObject piecePrefab;

        /// <summary>
        /// Container for all grid lines
        /// </summary>
        private GameObject gridContainer;

        /// <summary>
        /// Container for all pieces
        /// </summary>
        private GameObject piecesContainer;

        /// <summary>
        /// Array of piece game objects for quick access
        /// </summary>
        private GameObject[,] pieceObjects;

        private void Awake()
        {
            InitializeUI();
        }

        /// <summary>
        /// Initializes the UI elements for the board
        /// </summary>
        private void InitializeUI()
        {
            // Get or create canvas
            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
                if (canvas == null)
                {
                    // Create a new canvas
                    GameObject canvasObj = new GameObject("BoardCanvas");
                    canvas = canvasObj.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.transform.SetParent(transform);
                    canvas.transform.localPosition = Vector3.zero;
                    canvas.transform.localRotation = Quaternion.identity;
                    canvas.transform.localScale = Vector3.one;
                }
            }

            // Get or create board panel
            if (boardPanel == null)
            {
                // Create a panel to contain the board
                GameObject panelObj = new GameObject("BoardPanel");
                boardPanel = panelObj.AddComponent<RectTransform>();
                boardPanel.SetParent(canvas.transform);

                // Set size to match board dimensions
                float width = (boardSize - 1) * cellSize;
                float height = (boardSize - 1) * cellSize;
                boardPanel.sizeDelta = new Vector2(width, height);
                boardPanel.anchoredPosition = Vector2.zero;

                // Add Image component for background
                Image backgroundImage = panelObj.AddComponent<Image>();
                backgroundImage.color = boardBackgroundColor;
            }

            // Create containers for grid lines and pieces
            gridContainer = new GameObject("GridLines");
            gridContainer.transform.SetParent(boardPanel);

            piecesContainer = new GameObject("Pieces");
            piecesContainer.transform.SetParent(boardPanel);

            // Initialize piece objects array
            pieceObjects = new GameObject[boardSize, boardSize];

            // Create grid lines
            CreateGridLines();

            Debug.Log("BoardRenderer UI initialized");
        }

        /// <summary>
        /// Creates the grid lines for the board
        /// </summary>
        private void CreateGridLines()
        {
            // Clear existing grid lines
            foreach (Transform child in gridContainer.transform)
            {
                Destroy(child.gameObject);
            }

            float width = (boardSize - 1) * cellSize;
            float height = (boardSize - 1) * cellSize;

            // Create horizontal lines
            for (int i = 0; i < boardSize; i++)
            {
                float yPos = i * cellSize;
                CreateGridLine(new Vector2(0, yPos), new Vector2(width, yPos));
            }

            // Create vertical lines
            for (int i = 0; i < boardSize; i++)
            {
                float xPos = i * cellSize;
                CreateGridLine(new Vector2(xPos, 0), new Vector2(xPos, height));
            }
        }

        /// <summary>
        /// Creates a single grid line between two points
        /// </summary>
        /// <param name="start">Start point in local space</param>
        /// <param name="end">End point in local space</param>
        private void CreateGridLine(Vector2 start, Vector2 end)
        {
            GameObject line = Instantiate(gridLinePrefab != null ? gridLinePrefab : new GameObject("GridLine"), gridContainer.transform);
            RectTransform rectTransform = line.GetComponent<RectTransform>();

            if (rectTransform == null)
            {
                rectTransform = line.AddComponent<RectTransform>();
            }

            // Calculate position and rotation
            Vector2 center = (start + end) / 2;
            float length = Vector2.Distance(start, end);
            float angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;

            rectTransform.anchoredPosition = center;
            rectTransform.sizeDelta = new Vector2(length, gridLineWidth);
            rectTransform.localEulerAngles = new Vector3(0, 0, angle);

            // Apply color
            Image image = line.GetComponent<Image>();
            if (image == null)
            {
                image = line.AddComponent<Image>();
            }
            image.color = gridLineColor;
        }

        /// <summary>
        /// Updates the grid line color
        /// </summary>
        /// <param name="newColor">New grid line color</param>
        public void UpdateGridLineColor(Color newColor)
        {
            gridLineColor = newColor;
            if (gridMaterial != null)
            {
                gridMaterial.color = gridLineColor;
            }
        }

        /// <summary>
        /// Updates the board background color
        /// </summary>
        /// <param name="newColor">New background color</param>
        public void UpdateBackgroundColor(Color newColor)
        {
            boardBackgroundColor = newColor;
            if (backgroundMaterial != null)
            {
                backgroundMaterial.color = boardBackgroundColor;
            }
            // Recreate the background mesh if needed
            if (backgroundMesh != null)
            {
                CreateBackgroundMesh();
            }
        }

        /// <summary>
        /// Places a piece on the board
        /// </summary>
        /// <param name="pieceType">The type of piece to place</param>
        /// <param name="x">X coordinate on the board</param>
        /// <param name="y">Y coordinate on the board</param>
        public void PlacePiece(PlayerType pieceType, int x, int y)
        {
            if (x >= 0 && x < boardSize && y >= 0 && y < boardSize)
            {
                // Update the internal state
                placedPieces[x, y] = pieceType;

                // Create or update the piece UI element
                if (pieceObjects[x, y] == null)
                {
                    // Create a new piece
                    GameObject piece = Instantiate(piecePrefab != null ? piecePrefab : new GameObject("Piece"), piecesContainer.transform);
                    RectTransform rectTransform = piece.GetComponent<RectTransform>();

                    if (rectTransform == null)
                    {
                        rectTransform = piece.AddComponent<RectTransform>();
                    }

                    // Position the piece
                    float posX = x * cellSize;
                    float posY = y * cellSize;
                    rectTransform.anchoredPosition = new Vector2(posX, posY);
                    rectTransform.sizeDelta = new Vector2(pieceSize, pieceSize);

                    // Apply color based on piece type
                    Image image = piece.GetComponent<Image>();
                    if (image == null)
                    {
                        image = piece.AddComponent<Image>();
                    }
                    image.color = pieceType == PlayerType.PlayerOne ? blackPieceColor : whitePieceColor;

                    // Store reference
                    pieceObjects[x, y] = piece;
                }
            }
        }

        /// <summary>
        /// Removes a piece from the board
        /// </summary>
        /// <param name="x">X coordinate on the board</param>
        /// <param name="y">Y coordinate on the board</param>
        public void RemovePiece(int x, int y)
        {
            if (x >= 0 && x < boardSize && y >= 0 && y < boardSize)
            {
                // Update the internal state
                placedPieces[x, y] = PlayerType.None;

                // Remove the UI element if it exists
                if (pieceObjects[x, y] != null)
                {
                    Destroy(pieceObjects[x, y]);
                    pieceObjects[x, y] = null;
                }
            }
        }

        /// <summary>
        /// Cleans up resources
        /// </summary>
        private void OnDestroy()
        {
            // Clean up procedural meshes and materials
            if (gridMesh != null)
            {
                DestroyImmediate(gridMesh);
            }

            if (pieceMesh != null)
            {
                DestroyImmediate(pieceMesh);
            }

            if (backgroundMesh != null)
            {
                DestroyImmediate(backgroundMesh);
            }

            if (gridMaterial != null)
            {
                DestroyImmediate(gridMaterial);
            }

            if (backgroundMaterial != null)
            {
                DestroyImmediate(backgroundMaterial);
            }

            // Clean up UI elements
            if (gridContainer != null)
            {
                Destroy(gridContainer);
            }

            if (piecesContainer != null)
            {
                Destroy(piecesContainer);
            }

            if (boardPanel != null)
            {
                Destroy(boardPanel.gameObject);
            }

            if (canvas != null && canvas.gameObject.name == "BoardCanvas")
            {
                Destroy(canvas.gameObject);
            }
        }

        /// <summary>
        /// Toggles the display of grid coordinates
        /// </summary>
        /// <param name="show">Whether to show grid coordinates</param>
        public void ToggleGridCoordinates(bool show)
        {
            showGridCoordinates = show;

            // If we need to visualize coordinates, we'd need to implement that
            // For now, just store the setting
            Debug.Log($"Grid coordinates visualization: {(show ? "enabled" : "disabled")}");
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