using UnityEngine;
using Gomoku.Core;
using System.Collections;

namespace Gomoku
{
    /// <summary>
    /// Controls visual effects for pieces including animations, materials, and special effects
    /// </summary>
    public class PieceVisualController : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private Piece piece;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform shadowTransform;
        [SerializeField] private SpriteRenderer shadowRenderer;

        [Header("Visual Effects")]
        [SerializeField] private bool enableGlowEffect = true;
        [SerializeField] private bool enableScaleEffect = true;
        [SerializeField] private bool enableRotationEffect = false;
        [SerializeField] private float glowIntensity = 1.5f;
        [SerializeField] private float glowDuration = 0.3f;
        [SerializeField] private Color glowColor = Color.yellow;

        [Header("Highlight Effects")]
        [SerializeField] private bool enableSelectionHighlight = true;
        [SerializeField] private Color selectionColor = Color.cyan;
        [SerializeField] private Color lastMoveColor = Color.green;
        [SerializeField] private float highlightIntensity = 1.2f;
        [SerializeField] private float highlightDuration = 0.2f;

        [Header("Win Effects")]
        [SerializeField] private bool enableWinEffects = true;
        [SerializeField] private float winPulseAmplitude = 0.3f;
        [SerializeField] private float winPulseFrequency = 3f;
        [SerializeField] private Color winHighlightColor = new Color(1f, 0.843f, 0f);

        // State
        private bool isGlowing = false;
        private bool isSelected = false;
        private bool isLastMove = false;
        private bool isWinningPiece = false;
        private Coroutine glowCoroutine;
        private Coroutine highlightCoroutine;
        private Coroutine winEffectCoroutine;
        private Material originalMaterial;

        // Effects
        private Material glowMaterial;
        private Material highlightMaterial;

        private void Awake()
        {
            InitializeVisualController();
        }

        private void Start()
        {
            SetupVisualEffects();
        }

        /// <summary>
        /// Initializes the visual controller
        /// </summary>
        private void InitializeVisualController()
        {
            // Get components if not assigned
            if (piece == null)
                piece = GetComponent<Piece>();

            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            // Find shadow components
            if (shadowTransform == null)
                shadowTransform = transform.Find("Shadow");

            if (shadowRenderer == null && shadowTransform != null)
                shadowRenderer = shadowTransform.GetComponent<SpriteRenderer>();

            // Cache original material
            if (spriteRenderer != null)
                originalMaterial = spriteRenderer.material;

            // Create effect materials
            CreateEffectMaterials();

            Debug.Log($"PieceVisualController initialized for {piece?.PieceType} piece");
        }

        /// <summary>
        /// Creates materials for visual effects
        /// </summary>
        private void CreateEffectMaterials()
        {
            if (originalMaterial == null) return;

            // Create glow material
            if (enableGlowEffect)
            {
                glowMaterial = new Material(Shader.Find("Standard"));
                glowMaterial.CopyPropertiesFromMaterial(originalMaterial);
                glowMaterial.EnableKeyword("_EMISSION");
                glowMaterial.SetColor("_EmissionColor", glowColor * glowIntensity);
            }

            // Create highlight material
            if (enableSelectionHighlight)
            {
                highlightMaterial = new Material(Shader.Find("Standard"));
                highlightMaterial.CopyPropertiesFromMaterial(originalMaterial);
                highlightMaterial.SetColor("_EmissionColor", selectionColor * highlightIntensity);
                highlightMaterial.EnableKeyword("_EMISSION");
            }
        }

        /// <summary>
        /// Sets up initial visual effects
        /// </summary>
        private void SetupVisualEffects()
        {
            // Set initial visual state based on piece type
            if (piece != null)
            {
                UpdatePieceVisuals();
            }
        }

        /// <summary>
        /// Updates piece visuals based on current state
        /// </summary>
        private void UpdatePieceVisuals()
        {
            if (spriteRenderer == null || piece == null) return;

            // Apply appropriate material based on state
            if (isWinningPiece && enableWinEffects)
            {
                // Winning pieces get special treatment
                spriteRenderer.material = highlightMaterial;
                if (highlightMaterial != null)
                {
                    highlightMaterial.SetColor("_EmissionColor", winHighlightColor * highlightIntensity);
                }
            }
            else if (isSelected)
            {
                spriteRenderer.material = highlightMaterial;
            }
            else if (isGlowing)
            {
                spriteRenderer.material = glowMaterial;
            }
            else
            {
                spriteRenderer.material = originalMaterial;
            }
        }

        /// <summary>
        /// Triggers a glow effect on the piece
        /// </summary>
        /// <param name="duration">Duration of the glow effect</param>
        /// <param name="color">Glow color</param>
        public void TriggerGlowEffect(float duration = -1, Color? color = null)
        {
            if (!enableGlowEffect) return;

            if (duration < 0) duration = glowDuration;
            Color glowCol = color ?? glowColor;

            if (glowCoroutine != null)
            {
                StopCoroutine(glowCoroutine);
            }

            glowCoroutine = StartCoroutine(GlowEffectCoroutine(duration, glowCol));
        }

        /// <summary>
        /// Coroutine for glow effect animation
        /// </summary>
        /// <param name="duration">Duration of effect</param>
        /// <param name="color">Glow color</param>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator GlowEffectCoroutine(float duration, Color color)
        {
            isGlowing = true;
            UpdatePieceVisuals();

            if (glowMaterial != null)
            {
                glowMaterial.SetColor("_EmissionColor", color * glowIntensity);
            }

            float elapsed = 0f;
            float startIntensity = glowIntensity;
            float targetIntensity = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Fade glow effect
                float currentIntensity = Mathf.Lerp(startIntensity, targetIntensity, t);
                if (glowMaterial != null)
                {
                    glowMaterial.SetColor("_EmissionColor", color * currentIntensity);
                }

                // Pulse effect
                if (enableScaleEffect)
                {
                    float pulse = 1f + Mathf.Sin(t * Mathf.PI * 4f) * 0.05f;
                    transform.localScale = Vector3.one * pulse;
                }

                yield return null;
            }

            isGlowing = false;
            UpdatePieceVisuals();
            transform.localScale = Vector3.one;

            glowCoroutine = null;
        }

        /// <summary>
        /// Sets the piece as selected
        /// </summary>
        /// <param name="selected">Whether the piece is selected</param>
        public void SetSelected(bool selected)
        {
            if (!enableSelectionHighlight) return;

            isSelected = selected;
            UpdatePieceVisuals();

            if (selected)
            {
                if (highlightCoroutine != null)
                {
                    StopCoroutine(highlightCoroutine);
                }
                highlightCoroutine = StartCoroutine(SelectionHighlightCoroutine());
            }
            else
            {
                if (highlightCoroutine != null)
                {
                    StopCoroutine(highlightCoroutine);
                    highlightCoroutine = null;
                }
            }
        }

        /// <summary>
        /// Coroutine for selection highlight effect
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator SelectionHighlightCoroutine()
        {
            float elapsed = 0f;

            while (isSelected)
            {
                elapsed += Time.deltaTime;

                // Pulsing selection effect
                float pulse = 1f + Mathf.Sin(elapsed * 3f) * 0.1f;
                transform.localScale = Vector3.one * pulse;

                yield return null;
            }

            transform.localScale = Vector3.one;
            highlightCoroutine = null;
        }

        /// <summary>
        /// Sets the piece as the last move
        /// </summary>
        /// <param name="isLastMove">Whether this is the last move</param>
        public void SetLastMove(bool isLastMove)
        {
            this.isLastMove = isLastMove;

            if (isLastMove)
            {
                TriggerGlowEffect(1f, lastMoveColor);
            }
        }

        /// <summary>
        /// Sets the piece as part of a winning combination
        /// </summary>
        /// <param name="isWinning">Whether this is a winning piece</param>
        public void SetWinningPiece(bool isWinning)
        {
            if (!enableWinEffects) return;

            isWinningPiece = isWinning;
            UpdatePieceVisuals();

            if (isWinning)
            {
                if (winEffectCoroutine != null)
                {
                    StopCoroutine(winEffectCoroutine);
                }
                winEffectCoroutine = StartCoroutine(WinEffectCoroutine());
            }
            else
            {
                if (winEffectCoroutine != null)
                {
                    StopCoroutine(winEffectCoroutine);
                    winEffectCoroutine = null;
                }
            }
        }

        /// <summary>
        /// Coroutine for winning piece effect
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator WinEffectCoroutine()
        {
            float elapsed = 0f;
            Vector3 originalScale = transform.localScale;

            while (isWinningPiece)
            {
                elapsed += Time.deltaTime;

                // Winning pulse effect
                float pulse = 1f + Mathf.Sin(elapsed * winPulseFrequency * 2f * Mathf.PI) * winPulseAmplitude;
                transform.localScale = originalScale * pulse;

                // Rotation effect
                if (enableRotationEffect)
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Sin(elapsed * 2f) * 5f);
                }

                yield return null;
            }

            transform.localScale = originalScale;
            transform.rotation = Quaternion.identity;
            winEffectCoroutine = null;
        }

        /// <summary>
        /// Updates the visual materials
        /// </summary>
        /// <param name="newMaterial">New material to use</param>
        public void UpdateMaterial(Material newMaterial)
        {
            if (newMaterial == null) return;

            originalMaterial = newMaterial;

            // Recreate effect materials with new base
            CreateEffectMaterials();

            // Update current visual state
            UpdatePieceVisuals();
        }

        /// <summary>
        /// Resets all visual effects
        /// </summary>
        public void ResetVisualEffects()
        {
            // Stop all coroutines
            if (glowCoroutine != null)
            {
                StopCoroutine(glowCoroutine);
                glowCoroutine = null;
            }

            if (highlightCoroutine != null)
            {
                StopCoroutine(highlightCoroutine);
                highlightCoroutine = null;
            }

            if (winEffectCoroutine != null)
            {
                StopCoroutine(winEffectCoroutine);
                winEffectCoroutine = null;
            }

            // Reset states
            isGlowing = false;
            isSelected = false;
            isLastMove = false;
            isWinningPiece = false;

            // Reset transform
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;

            // Update visuals
            UpdatePieceVisuals();
        }

        private void OnDestroy()
        {
            // Cleanup coroutines
            if (glowCoroutine != null)
                StopCoroutine(glowCoroutine);

            if (highlightCoroutine != null)
                StopCoroutine(highlightCoroutine);

            if (winEffectCoroutine != null)
                StopCoroutine(winEffectCoroutine);

            // Cleanup materials
            if (glowMaterial != null)
                DestroyImmediate(glowMaterial);

            if (highlightMaterial != null)
                DestroyImmediate(highlightMaterial);
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method to test glow effect
        /// </summary>
        [ContextMenu("Test Glow Effect")]
        private void TestGlowEffect()
        {
            TriggerGlowEffect();
        }

        /// <summary>
        /// Editor-only method to test selection
        /// </summary>
        [ContextMenu("Test Selection")]
        private void TestSelection()
        {
            SetSelected(!isSelected);
        }

        /// <summary>
        /// Editor-only method to test win effect
        /// </summary>
        [ContextMenu("Test Win Effect")]
        private void TestWinEffect()
        {
            SetWinningPiece(!isWinningPiece);
        }
        #endif
    }
}