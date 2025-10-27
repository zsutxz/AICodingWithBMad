using UnityEngine;
using static Gomoku.TurnManager;
using Gomoku.Core;

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
        [SerializeField] private Color blackPieceColor = Color.black;
        [SerializeField] private Color whitePieceColor = Color.white;
        [SerializeField] private float shadowOffset = 0.05f;
        [SerializeField] private float shadowOpacity = 0.3f;

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
        public bool IsBlackPiece => pieceType == PlayerType.PlayerOne;
        public bool IsWhitePiece => pieceType == PlayerType.PlayerTwo;
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

            // Update visual appearance
            UpdatePieceVisual();

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
        /// Updates the piece's visual appearance based on its type
        /// </summary>
        private void UpdatePieceVisual()
        {
            // Update main renderer
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color targetColor = pieceType == PlayerType.PlayerOne ? blackPieceColor : whitePieceColor;
                spriteRenderer.color = targetColor;
            }

            // Create shadow effect
            CreateShadowEffect();

            Debug.Log($"Updated piece visual for {pieceType} piece");
        }

        /// <summary>
        /// Creates a shadow effect for the piece
        /// </summary>
        private void CreateShadowEffect()
        {
            // Check if shadow already exists
            Transform shadowTransform = transform.Find("Shadow");
            if (shadowTransform != null) return;

            // Create shadow object
            GameObject shadowObj = new GameObject("Shadow");
            shadowObj.transform.SetParent(transform);
            shadowObj.transform.localPosition = new Vector3(shadowOffset, -shadowOffset, 0.01f);
            shadowObj.transform.localScale = Vector3.one;

            // Add shadow renderer
            SpriteRenderer shadowRenderer = shadowObj.AddComponent<SpriteRenderer>();
            SpriteRenderer mainRenderer = GetComponent<SpriteRenderer>();

            if (mainRenderer != null)
            {
                shadowRenderer.sprite = mainRenderer.sprite;
                shadowRenderer.color = new Color(0f, 0f, 0f, shadowOpacity);
                shadowRenderer.sortingOrder = mainRenderer.sortingOrder - 1;
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

            // Start position (higher for better drop effect)
            Vector3 startPosition = targetPosition + Vector3.up * 3f;
            Vector3 startScale = Vector3.zero;
            Vector3 targetScale = Vector3.one;

            transform.position = startPosition;
            transform.localScale = startScale;

            float elapsed = 0f;
            float bounceHeight = 0.5f;
            float bounceDuration = placementAnimationDuration * 0.3f;
            float scaleDuration = placementAnimationDuration * 0.7f;

            // Phase 1: Drop with bounce
            while (elapsed < bounceDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / bounceDuration;
                float dropCurve = placementCurve.Evaluate(Mathf.Min(t * 2f, 1f));

                // Animate position with slight overshoot for bounce
                Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, dropCurve);

                // Add bounce effect
                if (t < 0.5f)
                {
                    currentPos.y += Mathf.Sin(t * Mathf.PI) * bounceHeight;
                }

                transform.position = currentPos;

                // Scale up quickly at the start
                float scaleT = Mathf.Min(t / 0.3f, 1f);
                transform.localScale = Vector3.Lerp(startScale, targetScale * 1.2f, scaleT);

                yield return null;
            }

            // Phase 2: Settle and finalize scale
            float settleElapsed = 0f;
            Vector3 settleStartPos = transform.position;
            Vector3 settleStartScale = transform.localScale;

            while (settleElapsed < scaleDuration)
            {
                settleElapsed += Time.deltaTime;
                float settleT = settleElapsed / scaleDuration;
                float settleCurve = 1f - Mathf.Pow(1f - settleT, 3f); // Ease out cubic

                // Settle into final position
                transform.position = Vector3.Lerp(settleStartPos, targetPosition, settleCurve);

                // Scale down to final size
                transform.localScale = Vector3.Lerp(settleStartScale, targetScale, settleCurve);

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

            // Deactivate
            gameObject.SetActive(false);
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