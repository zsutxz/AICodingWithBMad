using UnityEngine;
using TMPro;

namespace Gomoku.UI
{
    /// <summary>
    /// Manages consistent visual design language across all UI elements
    /// </summary>
    public class UIStyleManager : MonoBehaviour
    {
        [Header("Typography Settings")]
        [SerializeField] private TMP_FontAsset primaryFont;
        [SerializeField] private TMP_FontAsset secondaryFont;
        [SerializeField] private int headingFontSize = 32;
        [SerializeField] private int bodyFontSize = 24;
        [SerializeField] private int smallFontSize = 18;

        [Header("Color Palette")]
        [SerializeField] private Color primaryColor = new Color(0.2f, 0.1f, 0.05f); // Dark wood tone
        [SerializeField] private Color secondaryColor = new Color(0.8f, 0.7f, 0.6f); // Light wood tone
        [SerializeField] private Color accentColor = new Color(0.9f, 0.8f, 0.7f); // Stone color
        [SerializeField] private Color textColor = Color.white;
        [SerializeField] private Color disabledColor = new Color(0.5f, 0.5f, 0.5f);

        [Header("UI Element Colors")]
        [SerializeField] private Color buttonNormalColor = Color.white;
        [SerializeField] private Color buttonHoverColor = new Color(0.8f, 0.8f, 0.8f);
        [SerializeField] private Color buttonPressedColor = new Color(0.6f, 0.6f, 0.6f);

        [Header("Spacing and Layout")]
        [SerializeField] private float elementSpacing = 20f;
        [SerializeField] private float padding = 15f;
        [SerializeField] private float cornerRadius = 8f;

        [Header("Animation Settings")]
        [SerializeField] private float defaultAnimationDuration = 0.2f;
        [SerializeField] private float hoverAnimationDuration = 0.15f;
        [SerializeField] private float clickAnimationDuration = 0.1f;

        // Singleton instance
        private static UIStyleManager instance;

        #region Singleton Pattern

        /// <summary>
        /// Gets the singleton instance of the UIStyleManager
        /// </summary>
        public static UIStyleManager Instance
        {
            get
            {
                if (instance == null)
                {
                    // Try to find existing instance in the scene
                    instance = FindObjectOfType<UIStyleManager>();

                    // If no instance exists, create a new one
                    if (instance == null)
                    {
                        GameObject managerObject = new GameObject("UIStyleManager");
                        instance = managerObject.AddComponent<UIStyleManager>();
                        DontDestroyOnLoad(managerObject);
                    }
                }
                return instance;
            }
        }

        #endregion

        #region MonoBehaviour Methods

        /// <summary>
        /// Initialize the UI style manager
        /// </summary>
        private void Awake()
        {
            // Ensure only one instance exists
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("UIStyleManager initialized");
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        #endregion

        #region Public Methods - Typography

        /// <summary>
        /// Applies heading style to a TextMeshPro component
        /// </summary>
        /// <param name="textComponent">The TextMeshPro component to style</param>
        public void ApplyHeadingStyle(TextMeshProUGUI textComponent)
        {
            if (textComponent != null)
            {
                textComponent.font = primaryFont;
                textComponent.fontSize = headingFontSize;
                textComponent.color = textColor;
                textComponent.fontStyle = FontStyles.Bold;
            }
        }

        /// <summary>
        /// Applies body style to a TextMeshPro component
        /// </summary>
        /// <param name="textComponent">The TextMeshPro component to style</param>
        public void ApplyBodyStyle(TextMeshProUGUI textComponent)
        {
            if (textComponent != null)
            {
                textComponent.font = primaryFont;
                textComponent.fontSize = bodyFontSize;
                textComponent.color = textColor;
                textComponent.fontStyle = FontStyles.Normal;
            }
        }

        /// <summary>
        /// Applies small text style to a TextMeshPro component
        /// </summary>
        /// <param name="textComponent">The TextMeshPro component to style</param>
        public void ApplySmallStyle(TextMeshProUGUI textComponent)
        {
            if (textComponent != null)
            {
                textComponent.font = secondaryFont ?? primaryFont;
                textComponent.fontSize = smallFontSize;
                textComponent.color = textColor;
                textComponent.fontStyle = FontStyles.Normal;
            }
        }

        #endregion

        #region Public Methods - Colors

        /// <summary>
        /// Gets the primary color
        /// </summary>
        /// <returns>Primary color</returns>
        public Color GetPrimaryColor()
        {
            return primaryColor;
        }

        /// <summary>
        /// Gets the secondary color
        /// </summary>
        /// <returns>Secondary color</returns>
        public Color GetSecondaryColor()
        {
            return secondaryColor;
        }

        /// <summary>
        /// Gets the accent color
        /// </summary>
        /// <returns>Accent color</returns>
        public Color GetAccentColor()
        {
            return accentColor;
        }

        /// <summary>
        /// Gets the text color
        /// </summary>
        /// <returns>Text color</returns>
        public Color GetTextColor()
        {
            return textColor;
        }

        /// <summary>
        /// Gets the disabled color
        /// </summary>
        /// <returns>Disabled color</returns>
        public Color GetDisabledColor()
        {
            return disabledColor;
        }

        /// <summary>
        /// Gets the button normal color
        /// </summary>
        /// <returns>Button normal color</returns>
        public Color GetButtonNormalColor()
        {
            return buttonNormalColor;
        }

        /// <summary>
        /// Gets the button hover color
        /// </summary>
        /// <returns>Button hover color</returns>
        public Color GetButtonHoverColor()
        {
            return buttonHoverColor;
        }

        /// <summary>
        /// Gets the button pressed color
        /// </summary>
        /// <returns>Button pressed color</returns>
        public Color GetButtonPressedColor()
        {
            return buttonPressedColor;
        }

        #endregion

        #region Public Methods - Layout

        /// <summary>
        /// Gets the element spacing
        /// </summary>
        /// <returns>Element spacing</returns>
        public float GetElementSpacing()
        {
            return elementSpacing;
        }

        /// <summary>
        /// Gets the padding
        /// </summary>
        /// <returns>Padding value</returns>
        public float GetPadding()
        {
            return padding;
        }

        /// <summary>
        /// Gets the corner radius
        /// </summary>
        /// <returns>Corner radius</returns>
        public float GetCornerRadius()
        {
            return cornerRadius;
        }

        #endregion

        #region Public Methods - Animation

        /// <summary>
        /// Gets the default animation duration
        /// </summary>
        /// <returns>Default animation duration</returns>
        public float GetDefaultAnimationDuration()
        {
            return defaultAnimationDuration;
        }

        /// <summary>
        /// Gets the hover animation duration
        /// </summary>
        /// <returns>Hover animation duration</returns>
        public float GetHoverAnimationDuration()
        {
            return hoverAnimationDuration;
        }

        /// <summary>
        /// Gets the click animation duration
        /// </summary>
        /// <returns>Click animation duration</returns>
        public float GetClickAnimationDuration()
        {
            return clickAnimationDuration;
        }

        #endregion

        #region Public Methods - Fonts

        /// <summary>
        /// Gets the primary font
        /// </summary>
        /// <returns>Primary font</returns>
        public TMP_FontAsset GetPrimaryFont()
        {
            return primaryFont;
        }

        /// <summary>
        /// Gets the secondary font
        /// </summary>
        /// <returns>Secondary font</returns>
        public TMP_FontAsset GetSecondaryFont()
        {
            return secondaryFont;
        }

        #endregion

        /// <summary>
        /// Validates that all required references are set
        /// </summary>
        /// <returns>True if all required references are valid</returns>
        public bool ValidateReferences()
        {
            if (primaryFont == null)
            {
                Debug.LogWarning("UIStyleManager: Primary font is not assigned");
                return false;
            }

            return true;
        }
    }
}