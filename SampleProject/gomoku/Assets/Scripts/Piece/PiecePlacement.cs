using UnityEngine;
using System.Collections.Generic;

namespace Gomoku
{
    /// <summary>
    /// Handles piece placement logic and coordinates with other game systems
    /// </summary>
    public class PiecePlacement : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private GameBoardController gameBoard;
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private IntersectionDetector intersectionDetector;
        
        [Header("Piece Settings")]
        [SerializeField] private GameObject piecePrefab;
        [SerializeField] private Transform pieceContainer;
        [SerializeField] private bool enableVisualFeedback = true;
        
        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip placementSound;
        [SerializeField] private AudioClip invalidPlacementSound;
        
        // Game state
        private PlayerType[,] boardState;
        private List<Piece> placedPieces;
        private List<Vector2Int> moveHistory;
        private bool gameActive;
        
        // Object pooling
        private Queue<Piece> piecePool;
        private const int initialPoolSize = 50;
        
        // Events
        public System.Action<Vector2Int, PlayerType> OnPiecePlaced;
        public System.Action<Vector2Int, PlayerType> OnInvalidPlacement;
        public System.Action<List<Vector2Int>> OnMoveHistoryChanged;
        
        // Properties
        public bool GameActive => gameActive;
        public int MoveCount => moveHistory?.Count ?? 0;
        public PlayerType[,] BoardState => boardState;
        public List<Vector2Int> MoveHistory => new List<Vector2Int>(moveHistory);
        
        private void Awake()
        {
            InitializePiecePlacement();
        }
        
        private void Start()
        {
            SetupReferences();
            InitializeGame();
        }
        
        private void OnDestroy()
        {
            CleanupEvents();
        }
        
        /// <summary>
        /// Initializes the piece placement system
        /// </summary>
        private void InitializePiecePlacement()
        {
            placedPieces = new List<Piece>();
            moveHistory = new List<Vector2Int>();
            piecePool = new Queue<Piece>();
            gameActive = false;
            
            // Create piece container if not assigned
            if (pieceContainer == null)
            {
                GameObject container = new GameObject("Piece Container");
                container.transform.SetParent(transform);
                pieceContainer = container.transform;
            }
            
            // Setup audio source
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.volume = 0.7f;
            }
        }
        
        /// <summary>
        /// Sets up component references
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
            
            // Validate references
            if (gameBoard == null)
            {
                Debug.LogError("GameBoard reference not found!");
                return;
            }
            
            if (turnManager == null)
            {
                Debug.LogError("TurnManager reference not found!");
                return;
            }
            
            if (intersectionDetector == null)
            {
                Debug.LogError("IntersectionDetector reference not found!");
                return;
            }
            
            // Subscribe to events
            intersectionDetector.OnIntersectionClicked += HandleIntersectionClick;
            
            Debug.Log("PiecePlacement references setup complete");
        }
        
        /// <summary>
        /// Initializes the game state
        /// </summary>
        public void InitializeGame()
        {
            if (gameBoard == null) return;
            
            // Initialize board state
            int boardSize = gameBoard.BoardSize;
            boardState = new PlayerType[boardSize, boardSize];
            
            // Clear board state
            for (int x = 0; x < boardSize; x++)
            {
                for (int y = 0; y < boardSize; y++)
                {
                    boardState[x, y] = PlayerType.None;
                }
            }
            
            // Initialize object pool
            InitializePiecePool();
            
            // Clear existing pieces and history
            ClearAllPieces();
            moveHistory.Clear();
            
            gameActive = true;
            
            Debug.Log($"Game initialized with {boardSize}x{boardSize} board");
        }
        
        /// <summary>
        /// Initializes the object pool for pieces
        /// </summary>
        private void InitializePiecePool()
        {
            if (piecePrefab == null)
            {
                Debug.LogError("Piece prefab not assigned!");
                return;
            }
            
            // Clear existing pool
            while (piecePool.Count > 0)
            {
                Piece piece = piecePool.Dequeue();
                if (piece != null)
                {
                    DestroyImmediate(piece.gameObject);
                }
            }
            
            // Create initial pool
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreatePooledPiece();
            }
            
            Debug.Log($"Piece pool initialized with {initialPoolSize} pieces");
        }
        
        /// <summary>
        /// Creates a new pooled piece
        /// </summary>
        /// <returns>New piece component</returns>
        private Piece CreatePooledPiece()
        {
            GameObject pieceObj = Instantiate(piecePrefab, pieceContainer);
            Piece piece = pieceObj.GetComponent<Piece>();
            
            if (piece == null)
            {
                piece = pieceObj.AddComponent<Piece>();
            }
            
            piece.ResetPiece();
            piecePool.Enqueue(piece);
            
            return piece;
        }
        
        /// <summary>
        /// Gets a piece from the pool
        /// </summary>
        /// <returns>Available piece</returns>
        private Piece GetPooledPiece()
        {
            if (piecePool.Count == 0)
            {
                return CreatePooledPiece();
            }
            
            Piece piece = piecePool.Dequeue();
            piece.gameObject.SetActive(true);
            return piece;
        }
        
        /// <summary>
        /// Returns a piece to the pool
        /// </summary>
        /// <param name="piece">Piece to return</param>
        private void ReturnPieceToPool(Piece piece)
        {
            if (piece != null)
            {
                piece.ResetPiece();
                piecePool.Enqueue(piece);
            }
        }
        
        /// <summary>
        /// Handles intersection click events
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        private void HandleIntersectionClick(int x, int y)
        {
            if (!gameActive)
            {
                Debug.Log("Game is not active");
                return;
            }
            
            if (turnManager == null)
            {
                Debug.LogError("TurnManager is null");
                return;
            }
            
            TryPlacePiece(x, y, turnManager.CurrentPlayer);
        }
        
        /// <summary>
        /// Attempts to place a piece at the specified coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="playerType">Type of piece to place</param>
        /// <returns>True if piece was placed successfully</returns>
        public bool TryPlacePiece(int x, int y, PlayerType playerType)
        {
            // Validate coordinates
            if (!IsValidCoordinate(x, y))
            {
                Debug.Log($"Invalid coordinates: ({x}, {y})");
                PlayInvalidPlacementFeedback(x, y, playerType);
                return false;
            }
            
            // Check if position is occupied
            if (boardState[x, y] != PlayerType.None)
            {
                Debug.Log($"Position ({x}, {y}) is already occupied by {boardState[x, y]}");
                PlayInvalidPlacementFeedback(x, y, playerType);
                return false;
            }
            
            // Validate player type
            if (!TurnManager.IsValidPlayer(playerType))
            {
                Debug.LogError($"Invalid player type: {playerType}");
                return false;
            }
            
            // Place the piece
            PlacePiece(x, y, playerType);
            return true;
        }
        
        /// <summary>
        /// Places a piece at the specified coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="playerType">Type of piece to place</param>
        private void PlacePiece(int x, int y, PlayerType playerType)
        {
            // Update board state
            boardState[x, y] = playerType;
            
            // Add to move history
            Vector2Int move = new Vector2Int(x, y);
            moveHistory.Add(move);
            
            // Create visual piece
            CreateVisualPiece(x, y, playerType);
            
            // Play placement feedback
            PlayPlacementFeedback(x, y, playerType);
            
            // Notify listeners
            OnPiecePlaced?.Invoke(move, playerType);
            OnMoveHistoryChanged?.Invoke(moveHistory);
            
            // Switch turns
            if (turnManager != null)
            {
                turnManager.EndTurn();
            }
            
            Debug.Log($"Placed {playerType} piece at ({x}, {y}). Move #{moveHistory.Count}");
        }
        
        /// <summary>
        /// Creates the visual representation of a piece
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="playerType">Type of piece</param>
        private void CreateVisualPiece(int x, int y, PlayerType playerType)
        {
            if (gameBoard == null) return;
            
            // Get world position
            Vector3 worldPosition = gameBoard.BoardToWorldPosition(x, y);
            
            // Get piece from pool
            Piece piece = GetPooledPiece();
            
            // Setup piece
            Vector2Int boardPos = new Vector2Int(x, y);
            piece.SetupPiece(playerType, boardPos, worldPosition, true);
            
            // Add to placed pieces list
            placedPieces.Add(piece);
            
            // Subscribe to piece events
            piece.OnPiecePlaced += OnPieceVisualPlaced;
        }
        
        /// <summary>
        /// Handles when a piece visual is placed
        /// </summary>
        /// <param name="piece">Placed piece</param>
        private void OnPieceVisualPlaced(Piece piece)
        {
            // Unsubscribe from event
            piece.OnPiecePlaced -= OnPieceVisualPlaced;
            
            Debug.Log($"Visual piece placement complete: {piece}");
        }
        
        /// <summary>
        /// Plays placement feedback (audio and visual)
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="playerType">Type of piece placed</param>
        private void PlayPlacementFeedback(int x, int y, PlayerType playerType)
        {
            // Play placement sound
            if (audioSource != null && placementSound != null)
            {
                audioSource.PlayOneShot(placementSound);
            }
            
            // Visual feedback
            if (enableVisualFeedback && intersectionDetector != null)
            {
                intersectionDetector.HighlightIntersection(x, y, true);
            }
        }
        
        /// <summary>
        /// Plays invalid placement feedback
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="playerType">Type of piece attempted</param>
        private void PlayInvalidPlacementFeedback(int x, int y, PlayerType playerType)
        {
            // Play invalid sound
            if (audioSource != null && invalidPlacementSound != null)
            {
                audioSource.PlayOneShot(invalidPlacementSound);
            }
            
            // Visual feedback
            if (enableVisualFeedback && intersectionDetector != null)
            {
                intersectionDetector.HighlightIntersection(x, y, false);
            }
            
            // Notify listeners
            OnInvalidPlacement?.Invoke(new Vector2Int(x, y), playerType);
        }
        
        /// <summary>
        /// Validates if coordinates are within board bounds
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>True if valid</returns>
        private bool IsValidCoordinate(int x, int y)
        {
            if (gameBoard == null) return false;
            return gameBoard.IsValidCoordinate(x, y);
        }
        
        /// <summary>
        /// Clears all pieces from the board
        /// </summary>
        public void ClearAllPieces()
        {
            // Return all pieces to pool
            foreach (Piece piece in placedPieces)
            {
                ReturnPieceToPool(piece);
            }
            
            placedPieces.Clear();
            
            // Clear board state
            if (boardState != null)
            {
                int boardSize = gameBoard != null ? gameBoard.BoardSize : 15;
                for (int x = 0; x < boardSize; x++)
                {
                    for (int y = 0; y < boardSize; y++)
                    {
                        boardState[x, y] = PlayerType.None;
                    }
                }
            }
            
            // Clear move history
            moveHistory.Clear();
            OnMoveHistoryChanged?.Invoke(moveHistory);
            
            Debug.Log("All pieces cleared");
        }
        
        /// <summary>
        /// Stops the game
        /// </summary>
        public void StopGame()
        {
            gameActive = false;
            Debug.Log("Game stopped");
        }
        
        /// <summary>
        /// Resumes the game
        /// </summary>
        public void ResumeGame()
        {
            gameActive = true;
            Debug.Log("Game resumed");
        }
        
        /// <summary>
        /// Gets the piece type at specific coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>Piece type at position</returns>
        public PlayerType GetPieceAt(int x, int y)
        {
            if (!IsValidCoordinate(x, y)) return PlayerType.None;
            return boardState[x, y];
        }
        
        /// <summary>
        /// Checks if a position is occupied
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>True if occupied</returns>
        public bool IsPositionOccupied(int x, int y)
        {
            return GetPieceAt(x, y) != PlayerType.None;
        }
        
        /// <summary>
        /// Cleans up event subscriptions
        /// </summary>
        private void CleanupEvents()
        {
            if (intersectionDetector != null)
            {
                intersectionDetector.OnIntersectionClicked -= HandleIntersectionClick;
            }
            
            // Cleanup piece events
            foreach (Piece piece in placedPieces)
            {
                if (piece != null)
                {
                    piece.OnPiecePlaced -= OnPieceVisualPlaced;
                }
            }
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for debugging game state
        /// </summary>
        [ContextMenu("Log Game State")]
        private void LogGameState()
        {
            Debug.Log($"Game Active: {gameActive}, Moves: {MoveCount}, Placed Pieces: {placedPieces.Count}");
            if (turnManager != null)
            {
                Debug.Log($"Current Player: {turnManager.CurrentPlayer}");
            }
        }
        #endif
    }
}

