using UnityEngine;
using UnityEngine.UI;

namespace Gomoku.UI
{
    /// <summary>
    /// Provides visual feedback for pause state
    /// </summary>
    public class PauseVisualIndicator : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private Color pausedOverlayColor = new Color(0, 0, 0, 0.5f);
        [SerializeField] private float fadeDuration = 0.3f;
        
        [Header("UI References")]
        [SerializeField] private Canvas pauseCanvas;
        [SerializeField] private Image pauseOverlay;
        [SerializeField] private Text pauseText;
        
        private GameStateManager gameStateManager;
        private CanvasGroup canvasGroup;
        private GameStateEnum lastState;
        
        void Start()
        {
            gameStateManager = GameStateManager.Instance;
            lastState = gameStateManager.GetCurrentState();
            
            // Setup visual components if not assigned
            SetupVisualComponents();
            
            // Initial state
            UpdateVisualState(lastState);
        }
        
        void Update()
        {
            GameStateEnum currentState = gameStateManager.GetCurrentState();
            
            if (currentState != lastState)
            {
                UpdateVisualState(currentState);
                lastState = currentState;
            }
        }
        
        private void SetupVisualComponents()
        {
            if (pauseCanvas == null)
            {
                // Create canvas for pause overlay
                GameObject canvasObject = new GameObject("PauseCanvas");
                canvasObject.transform.SetParent(transform);
                
                pauseCanvas = canvasObject.AddComponent<Canvas>();
                pauseCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                pauseCanvas.sortingOrder = 100; // Ensure it's on top
                
                CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                
                GraphicRaycaster raycaster = canvasObject.AddComponent<GraphicRaycaster>();
                
                canvasGroup = canvasObject.AddComponent<CanvasGroup>();
            }
            else
            {
                canvasGroup = pauseCanvas.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = pauseCanvas.gameObject.AddComponent<CanvasGroup>();
                }
            }
            
            if (pauseOverlay == null)
            {
                // Create overlay image
                GameObject overlayObject = new GameObject("PauseOverlay");
                overlayObject.transform.SetParent(pauseCanvas.transform, false);
                
                RectTransform overlayRect = overlayObject.AddComponent<RectTransform>();
                overlayRect.anchorMin = Vector2.zero;
                overlayRect.anchorMax = Vector2.one;
                overlayRect.sizeDelta = Vector2.zero;
                overlayRect.anchoredPosition = Vector2.zero;
                
                pauseOverlay = overlayObject.AddComponent<Image>();
                pauseOverlay.color = pausedOverlayColor;
            }
            
            if (pauseText == null)
            {
                // Create PAUSED text
                GameObject textObject = new GameObject("PauseText");
                textObject.transform.SetParent(pauseCanvas.transform, false);
                
                RectTransform textRect = textObject.AddComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0.4f, 0.6f);
                textRect.anchorMax = new Vector2(0.6f, 0.7f);
                textRect.sizeDelta = Vector2.zero;
                textRect.anchoredPosition = Vector2.zero;
                
                pauseText = textObject.AddComponent<Text>();
                pauseText.text = "PAUSED";
                pauseText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                pauseText.fontSize = 48;
                pauseText.color = Color.white;
                pauseText.alignment = TextAnchor.MiddleCenter;
                pauseText.fontStyle = FontStyle.Bold;
            }
        }
        
        private void UpdateVisualState(GameStateEnum state)
        {
            switch (state)
            {
                case GameStateEnum.Paused:
                    ShowPauseVisuals();
                    break;
                    
                default:
                    HidePauseVisuals();
                    break;
            }
        }
        
        private void ShowPauseVisuals()
        {
            if (pauseCanvas != null)
            {
                pauseCanvas.gameObject.SetActive(true);
            }
            
            if (canvasGroup != null)
            {
                // Fade in effect
                StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, fadeDuration));
            }
        }
        
        private void HidePauseVisuals()
        {
            if (canvasGroup != null)
            {
                // Fade out effect
                StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0f, fadeDuration));
            }
            else if (pauseCanvas != null)
            {
                pauseCanvas.gameObject.SetActive(false);
            }
        }
        
        private System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup group, float start, float end, float duration)
        {
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime; // Use unscaled time to respect pause state
                group.alpha = Mathf.Lerp(start, end, elapsed / duration);
                yield return null;
            }
            
            group.alpha = end;
            
            // Hide canvas when fully faded out
            if (end == 0f && pauseCanvas != null)
            {
                pauseCanvas.gameObject.SetActive(false);
            }
        }
    }
}