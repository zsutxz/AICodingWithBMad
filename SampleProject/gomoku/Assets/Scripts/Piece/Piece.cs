using UnityEngine;

namespace Gomoku
{
    /// <summary>
    /// Represents a single game piece on the board
    /// </summary>
    public class Piece : MonoBehaviour
    {
        [Header("Piece Settings")]
        [SerializeField] private PlayerType pieceType = PlayerType.None;
        [SerializeField] private float placementAnimationDuration = 0.2f;
        [SerializeField] private AnimationCurve placementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Visual Settings")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite blackPieceSprite;
        [SerializeField] private Sprite whitePieceSprite;
        [SerializeField] private Color blackPieceColor = Color.black;
        [SerializeField] private Color whitePieceColor = Color.white;
        
        // Board position
        private Vector2Int boardPosition;
        private bool isAnimating;
        
        // Animation
        private Coroutine placementAnimation;
        
        // Events
        public System.Action<Piece> OnPiecePlaced;
        public System.Action<Piece> OnPieceAnimationComplete;
        
        // Properties
        public PlayerType PieceType => pieceType;
        public Vector2Int BoardPosition => boardPosition;
        public bool IsBlackPiece => pieceType == PlayerType.Black;
        public bool IsWhitePiece => pieceType == PlayerType.White;
        public bool IsAnimating => isAnimating;
        
        private void Awake()
        {
            InitializePiece();
        }
        
        /// <summary>
        /// Initializes the piece component
        /// </summary>
        private void InitializePiece()
        {
            // Get sprite renderer if not assigned
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                }
            }
            
            // Add Rigidbody2D for physics (kinematic)
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = false; // No physics simulation needed
            
            // Add CircleCollider2D for potential future interactions
            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<CircleCollider2D>();
                collider.isTrigger = true; // Just for detection, not physics
            }
        }
        
        /// <summary>
        /// Sets up the piece with specified type and position
        /// </summary>
        /// <param name="type">Type of piece (Black or White)</param>
        /// <param name="boardPos">Position on the board</param>
        /// <param name="worldPos">World position to place the piece</param>
        /// <param name="animate">Whether to animate the placement</param>
        public void SetupPiece(PlayerType type, Vector2Int boardPos, Vector3 worldPos, bool animate = true)
        {
            if (!TurnManager.IsValidPlayer(type))
            {
                Debug.LogError($"Invalid piece type: {type}");
                return;
            }
            
            pieceType = type;
            boardPosition = boardPos;
            
            // Set visual appearance
            UpdateVisualAppearance();
            
            // Set position
            if (animate)
            {
                StartPlacementAnimation(worldPos);
            }
            else
            {
                transform.position = worldPos;
                OnPiecePlaced?.Invoke(this);
            }
            
            Debug.Log($"{type} piece placed at board position ({boardPos.x}, {boardPos.y})");
        }
        
        /// <summary>
        /// Updates the visual appearance based on piece type
        /// </summary>
        private void UpdateVisualAppearance()
        {
            if (spriteRenderer == null) return;
            
            switch (pieceType)
            {
                case PlayerType.Black:
                    spriteRenderer.sprite = blackPieceSprite;
                    spriteRenderer.color = blackPieceColor;
                    break;
                    
                case PlayerType.White:
                    spriteRenderer.sprite = whitePieceSprite;
                    spriteRenderer.color = whitePieceColor;
                    break;
                    
                default:
                    spriteRenderer.sprite = null;
                    spriteRenderer.color = Color.clear;
                    Debug.LogWarning($"Unknown piece type: {pieceType}");
                    break;
            }
        }
        
        /// <summary>
        /// Starts the placement animation
        /// </summary>
        /// <param name="targetPosition">Target world position</param>
        private void StartPlacementAnimation(Vector3 targetPosition)
        {
            if (placementAnimation != null)
            {
                StopCoroutine(placementAnimation);
            }
            
            placementAnimation = StartCoroutine(AnimatePlacement(targetPosition));
        }
        
        /// <summary>
        /// Animates the piece placement
        /// </summary>
        /// <param name="targetPosition">Target position</param>
        /// <returns>Coroutine enumerator</returns>
        private System.Collections.IEnumerator AnimatePlacement(Vector3 targetPosition)
        {
            isAnimating = true;
            
            // Start position (slightly above target for drop effect)
            Vector3 startPosition = targetPosition + Vector3.up * 2f;
            Vector3 startScale = Vector3.zero;
            Vector3 targetScale = Vector3.one;
            
            transform.position = startPosition;
            transform.localScale = startScale;
            
            float elapsed = 0f;
            
            while (elapsed < placementAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / placementAnimationDuration;
                float curveValue = placementCurve.Evaluate(t);
                
                // Animate position and scale
                transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);
                transform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
                
                yield return null;
            }
            
            // Ensure final values are exact
            transform.position = targetPosition;
            transform.localScale = targetScale;
            
            isAnimating = false;
            placementAnimation = null;
            
            // Notify that placement is complete
            OnPiecePlaced?.Invoke(this);
            OnPieceAnimationComplete?.Invoke(this);
        }
        
        /// <summary>
        /// Resets the piece to be reused by object pooling
        /// </summary>
        public void ResetPiece()
        {
            // Stop any ongoing animations
            if (placementAnimation != null)
            {
                StopCoroutine(placementAnimation);
                placementAnimation = null;
            }
            
            isAnimating = false;
            pieceType = PlayerType.None;
            boardPosition = Vector2Int.zero;
            
            // Reset transform
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
            
            // Clear visual
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = null;
                spriteRenderer.color = Color.clear;
            }
            
            // Deactivate
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Sets the piece sprites for black and white pieces
        /// </summary>
        /// <param name="blackSprite">Sprite for black pieces</param>
        /// <param name="whiteSprite">Sprite for white pieces</param>
        public void SetPieceSprites(Sprite blackSprite, Sprite whiteSprite)
        {
            blackPieceSprite = blackSprite;
            whitePieceSprite = whiteSprite;

            // Update current appearance if already set
            if (pieceType != PlayerType.None)
            {
                UpdateVisualAppearance();
            }
        }
        
        /// <summary>
        /// Sets the piece colors for black and white pieces
        /// </summary>
        /// <param name="blackColor">Color for black pieces</param>
        /// <param name="whiteColor">Color for white pieces</param>
        public void SetPieceColors(Color blackColor, Color whiteColor)
        {
            blackPieceColor = blackColor;
            whitePieceColor = whiteColor;

            // Update current appearance if already set
            if (pieceType != PlayerType.None)
            {
                UpdateVisualAppearance();
            }
        }
        
        /// <summary>
        /// Gets a string representation of the piece
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return $"{pieceType} piece at ({boardPosition.x}, {boardPosition.y})";
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for debugging piece state
        /// </summary>
        [ContextMenu("Log Piece Info")]
        private void LogPieceInfo()
        {
            Debug.Log($"Piece Info: {ToString()}, Animating: {isAnimating}");
        }
        #endif
    }
}