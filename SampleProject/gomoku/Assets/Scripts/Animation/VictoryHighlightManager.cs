using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gomoku;
using Gomoku.Core;

namespace Gomoku.Animation
{
    /// <summary>
    /// Manages victory highlight effects and coordinates between WinDetector and VictoryEffect components
    /// </summary>
    public class VictoryHighlightManager : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private WinDetector winDetector;
        [SerializeField] private PiecePlacement piecePlacement;
        
        [Header("Victory Effect Settings")]
        [SerializeField] private float highlightSequenceDelay = 0.1f;
        [SerializeField] private Color playerOneGlowColor = Color.yellow;
        [SerializeField] private Color playerTwoGlowColor = Color.cyan;
        
        [Header("Performance Settings")]
        [SerializeField] private bool enableVictoryEffects = true;
        
        // Victory effect tracking
        private Dictionary<Vector2Int, VictoryEffect> victoryEffects;
        private List<Vector2Int> winningPositions;
        private Coroutine victorySequenceCoroutine;
        
        // Events
        public System.Action<List<Vector2Int>, PlayerType> OnVictoryHighlightStart;
        public System.Action<List<Vector2Int>, PlayerType> OnVictoryHighlightComplete;
        
        // Properties
        public bool IsHighlighting { get; private set; }
        public bool EnableVictoryEffects => enableVictoryEffects;
        
        private void Awake()
        {
            Initialize();
        }
        
        /// <summary>
        /// Initializes the victory highlight manager
        /// </summary>
        private void Initialize()
        {
            victoryEffects = new Dictionary<Vector2Int, VictoryEffect>();
            winningPositions = new List<Vector2Int>();
            
            // Auto-find components if not assigned
            if (winDetector == null)
            {
                winDetector = FindObjectOfType<WinDetector>();
            }
            
            if (piecePlacement == null)
            {
                piecePlacement = FindObjectOfType<PiecePlacement>();
            }
            
            // Subscribe to win detection events
            if (winDetector != null)
            {
                winDetector.onWinDetected.AddListener(HandleWinDetected);
            }
            else
            {
                Debug.LogError("VictoryHighlightManager: WinDetector reference not found");
            }
            
            if (piecePlacement == null)
            {
                Debug.LogError("VictoryHighlightManager: PiecePlacement reference not found");
            }
        }
        
        /// <summary>
        /// Handles win detection events
        /// </summary>
        /// <param name="winningPlayer">The player who won</param>
        private void HandleWinDetected(PlayerType winningPlayer)
        {
            if (!enableVictoryEffects) return;
            
            // Get winning positions (this would need to be implemented in WinDetector)
            // For now, we'll simulate finding winning positions
            FindWinningPositions(winningPlayer);
            
            // Start victory highlight sequence
            StartVictoryHighlightSequence(winningPlayer);
        }
        
        /// <summary>
        /// Finds the winning positions (placeholder implementation)
        /// In a real implementation, this would query WinDetector for the actual winning line
        /// </summary>
        /// <param name="winningPlayer">The winning player</param>
        private void FindWinningPositions(PlayerType winningPlayer)
        {
            winningPositions.Clear();
            
            // This is a simplified implementation
            // In a real game, the WinDetector would provide the actual winning positions
            
            // For demonstration, we'll find all pieces of the winning player
            if (piecePlacement != null && piecePlacement.BoardState != null)
            {
                int boardSize = piecePlacement.BoardState.GetLength(0);
                
                for (int x = 0; x < boardSize; x++)
                {
                    for (int y = 0; y < boardSize; y++)
                    {
                        if (piecePlacement.BoardState[x, y] == winningPlayer)
                        {
                            winningPositions.Add(new Vector2Int(x, y));
                        }
                    }
                }
            }
            
            // Limit to first 5 positions for demonstration
            if (winningPositions.Count > 5)
            {
                winningPositions = winningPositions.GetRange(0, 5);
            }
            
            Debug.Log($"Found {winningPositions.Count} potential winning positions for {winningPlayer}");
        }
        
        /// <summary>
        /// Starts the victory highlight sequence
        /// </summary>
        /// <param name="winningPlayer">The winning player</param>
        public void StartVictoryHighlightSequence(PlayerType winningPlayer)
        {
            if (IsHighlighting)
            {
                StopVictoryHighlightSequence();
            }
            
            victorySequenceCoroutine = StartCoroutine(AnimateVictoryHighlightSequence(winningPlayer));
        }
        
        /// <summary>
        /// Animates the victory highlight sequence
        /// </summary>
        /// <param name="winningPlayer">The winning player</param>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator AnimateVictoryHighlightSequence(PlayerType winningPlayer)
        {
            IsHighlighting = true;
            OnVictoryHighlightStart?.Invoke(winningPositions, winningPlayer);
            
            // Get the appropriate glow color
            Color glowColor = winningPlayer == PlayerType.PlayerOne ? playerOneGlowColor : playerTwoGlowColor;
            
            // Create victory effects for winning positions
            foreach (Vector2Int position in winningPositions)
            {
                if (CreateVictoryEffectAtPosition(position, glowColor))
                {
                    yield return new WaitForSeconds(highlightSequenceDelay);
                }
            }
            
            // Wait for all effects to complete
            yield return new WaitForSeconds(2f);
            
            // Clean up victory effects
            CleanupVictoryEffects();
            
            IsHighlighting = false;
            OnVictoryHighlightComplete?.Invoke(winningPositions, winningPlayer);
            
            Debug.Log($"Victory highlight sequence completed for {winningPlayer}");
        }
        
        /// <summary>
        /// Creates a victory effect at the specified position
        /// </summary>
        /// <param name="position">Board position</param>
        /// <param name="glowColor">Glow color</param>
        /// <returns>True if effect was created successfully</returns>
        private bool CreateVictoryEffectAtPosition(Vector2Int position, Color glowColor)
        {
            if (piecePlacement == null || !piecePlacement.IsPositionOccupied(position.x, position.y))
            {
                return false;
            }
            
            // Find the piece at this position
            Piece piece = FindPieceAtPosition(position);
            if (piece == null)
            {
                return false;
            }
            
            // Add VictoryEffect component to the piece
            VictoryEffect victoryEffect = piece.GetComponent<VictoryEffect>();
            if (victoryEffect == null)
            {
                victoryEffect = piece.gameObject.AddComponent<VictoryEffect>();
            }
            
            // Configure the victory effect
            victoryEffect.SetGlowColor(glowColor);
            
            // Start the victory highlight
            victoryEffect.StartVictoryHighlight();
            
            // Track the effect
            victoryEffects[position] = victoryEffect;
            
            return true;
        }
        
        /// <summary>
        /// Finds the piece at a specific board position
        /// </summary>
        /// <param name="position">Board position</param>
        /// <returns>The piece at the position, or null if not found</returns>
        private Piece FindPieceAtPosition(Vector2Int position)
        {
            // This is a simplified implementation
            // In a real game, you would have a more efficient way to find pieces
            
            Piece[] allPieces = FindObjectsOfType<Piece>();
            foreach (Piece piece in allPieces)
            {
                if (piece.BoardPosition == position)
                {
                    return piece;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Cleans up victory effects
        /// </summary>
        private void CleanupVictoryEffects()
        {
            foreach (var kvp in victoryEffects)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.StopVictoryAnimation();
                    
                    // Optionally remove the component
                    // Destroy(kvp.Value);
                }
            }
            
            victoryEffects.Clear();
        }
        
        /// <summary>
        /// Stops the victory highlight sequence
        /// </summary>
        public void StopVictoryHighlightSequence()
        {
            if (victorySequenceCoroutine != null)
            {
                StopCoroutine(victorySequenceCoroutine);
                victorySequenceCoroutine = null;
            }
            
            CleanupVictoryEffects();
            IsHighlighting = false;
            
            Debug.Log("Victory highlight sequence stopped");
        }
        
        /// <summary>
        /// Sets whether victory effects are enabled
        /// </summary>
        /// <param name="enabled">Whether effects are enabled</param>
        public void SetVictoryEffectsEnabled(bool enabled)
        {
            enableVictoryEffects = enabled;
            
            if (!enabled && IsHighlighting)
            {
                StopVictoryHighlightSequence();
            }
        }
        
        /// <summary>
        /// Sets the glow colors for players
        /// </summary>
        /// <param name="playerOneColor">Player one glow color</param>
        /// <param name="playerTwoColor">Player two glow color</param>
        public void SetPlayerGlowColors(Color playerOneColor, Color playerTwoColor)
        {
            playerOneGlowColor = playerOneColor;
            playerTwoGlowColor = playerTwoColor;
        }
        
        /// <summary>
        /// Sets the highlight sequence delay
        /// </summary>
        /// <param name="delay">Delay between highlights</param>
        public void SetHighlightSequenceDelay(float delay)
        {
            highlightSequenceDelay = Mathf.Max(0f, delay);
        }
        
        /// <summary>
        /// Resets the victory highlight manager
        /// </summary>
        public void ResetManager()
        {
            StopVictoryHighlightSequence();
            winningPositions.Clear();
            victoryEffects.Clear();
        }
        
        private void OnDestroy()
        {
            // Clean up event subscriptions
            if (winDetector != null)
            {
                winDetector.onWinDetected.RemoveListener(HandleWinDetected);
            }
            
            StopVictoryHighlightSequence();
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for testing victory highlights
        /// </summary>
        [ContextMenu("Test Victory Highlight - Player One")]
        private void TestVictoryHighlightPlayerOne()
        {
            if (Application.isPlaying)
            {
                StartVictoryHighlightSequence(PlayerType.PlayerOne);
            }
            else
            {
                Debug.Log("Victory highlight test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for testing victory highlights
        /// </summary>
        [ContextMenu("Test Victory Highlight - Player Two")]
        private void TestVictoryHighlightPlayerTwo()
        {
            if (Application.isPlaying)
            {
                StartVictoryHighlightSequence(PlayerType.PlayerTwo);
            }
            else
            {
                Debug.Log("Victory highlight test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for stopping victory highlights
        /// </summary>
        [ContextMenu("Stop Victory Highlight")]
        private void StopVictoryHighlight()
        {
            if (Application.isPlaying)
            {
                StopVictoryHighlightSequence();
            }
            else
            {
                Debug.Log("Victory highlight stop can only be run in Play mode");
            }
        }
        #endif
    }
}