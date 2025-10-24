using UnityEngine;
using System.Collections.Generic;

namespace Gomoku.UI
{
    /// <summary>
    /// Manages button animations across the UI and provides centralized control
    /// </summary>
    public class ButtonAnimationManager : MonoBehaviour
    {
        [Header("Global Button Settings")]
        [SerializeField] private bool enableButtonAnimations = true;
        [SerializeField] private float globalHoverScale = 1.1f;
        [SerializeField] private float globalClickScale = 0.9f;
        [SerializeField] private float globalAnimationDuration = 0.2f;
        
        [Header("Color Settings")]
        [SerializeField] private Color globalNormalColor = Color.white;
        [SerializeField] private Color globalHoverColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        [SerializeField] private Color globalClickColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        
        // Button tracking
        private List<ButtonHandler> registeredButtons;
        
        // Events
        public System.Action<ButtonHandler> OnButtonAnimationStart;
        public System.Action<ButtonHandler> OnButtonAnimationComplete;
        
        // Properties
        public bool EnableButtonAnimations => enableButtonAnimations;
        public int RegisteredButtonCount => registeredButtons?.Count ?? 0;
        
        // Singleton instance
        private static ButtonAnimationManager instance;
        public static ButtonAnimationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ButtonAnimationManager>();
                    if (instance == null)
                    {
                        GameObject managerObject = new GameObject("ButtonAnimationManager");
                        instance = managerObject.AddComponent<ButtonAnimationManager>();
                        DontDestroyOnLoad(managerObject);
                    }
                }
                return instance;
            }
        }
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Initializes the button animation manager
        /// </summary>
        private void Initialize()
        {
            registeredButtons = new List<ButtonHandler>();
            
            // Auto-register existing buttons
            AutoRegisterButtons();
            
            Debug.Log("ButtonAnimationManager initialized");
        }
        
        /// <summary>
        /// Auto-registers existing buttons in the scene
        /// </summary>
        private void AutoRegisterButtons()
        {
            ButtonHandler[] existingButtons = FindObjectsOfType<ButtonHandler>();
            foreach (ButtonHandler button in existingButtons)
            {
                RegisterButton(button);
            }
            
            Debug.Log($"Auto-registered {existingButtons.Length} buttons");
        }
        
        /// <summary>
        /// Registers a button with the animation manager
        /// </summary>
        /// <param name="button">Button to register</param>
        public void RegisterButton(ButtonHandler button)
        {
            if (button != null && !registeredButtons.Contains(button))
            {
                registeredButtons.Add(button);
                
                // Apply global settings to the button
                ApplyGlobalSettingsToButton(button);
                
                // Subscribe to button events
                button.OnButtonHover += HandleButtonHover;
                button.OnButtonClick += HandleButtonClick;
                
                Debug.Log($"Registered button: {button.name}");
            }
        }
        
        /// <summary>
        /// Unregisters a button from the animation manager
        /// </summary>
        /// <param name="button">Button to unregister</param>
        public void UnregisterButton(ButtonHandler button)
        {
            if (button != null && registeredButtons.Contains(button))
            {
                registeredButtons.Remove(button);
                
                // Unsubscribe from button events
                button.OnButtonHover -= HandleButtonHover;
                button.OnButtonClick -= HandleButtonClick;
                
                Debug.Log($"Unregistered button: {button.name}");
            }
        }
        
        /// <summary>
        /// Applies global settings to a button
        /// </summary>
        /// <param name="button">Button to configure</param>
        private void ApplyGlobalSettingsToButton(ButtonHandler button)
        {
            if (button == null) return;
            
            button.SetButtonColors(globalNormalColor, globalHoverColor, globalClickColor);
            button.SetAnimationScales(globalHoverScale, globalClickScale);
            button.SetAnimationDuration(globalAnimationDuration);
            
            // Set interactable state based on global setting
            button.SetInteractable(enableButtonAnimations);
        }
        
        /// <summary>
        /// Handles button hover events
        /// </summary>
        /// <param name="button">The hovered button</param>
        private void HandleButtonHover(ButtonHandler button)
        {
            OnButtonAnimationStart?.Invoke(button);
        }
        
        /// <summary>
        /// Handles button click events
        /// </summary>
        /// <param name="button">The clicked button</param>
        private void HandleButtonClick(ButtonHandler button)
        {
            OnButtonAnimationComplete?.Invoke(button);
        }
        
        /// <summary>
        /// Sets whether button animations are enabled globally
        /// </summary>
        /// <param name="enabled">Whether animations are enabled</param>
        public void SetButtonAnimationsEnabled(bool enabled)
        {
            enableButtonAnimations = enabled;
            
            // Update all registered buttons
            foreach (ButtonHandler button in registeredButtons)
            {
                if (button != null)
                {
                    button.SetInteractable(enabled);
                }
            }
            
            Debug.Log($"Button animations {(enabled ? "enabled" : "disabled")} globally");
        }
        
        /// <summary>
        /// Sets global animation scales
        /// </summary>
        /// <param name="hoverScale">Hover scale multiplier</param>
        /// <param name="clickScale">Click scale multiplier</param>
        public void SetGlobalAnimationScales(float hoverScale, float clickScale)
        {
            globalHoverScale = Mathf.Max(1f, hoverScale);
            globalClickScale = Mathf.Max(0.5f, clickScale);
            
            // Update all registered buttons
            foreach (ButtonHandler button in registeredButtons)
            {
                if (button != null)
                {
                    button.SetAnimationScales(globalHoverScale, globalClickScale);
                }
            }
        }
        
        /// <summary>
        /// Sets global animation duration
        /// </summary>
        /// <param name="duration">Animation duration</param>
        public void SetGlobalAnimationDuration(float duration)
        {
            globalAnimationDuration = Mathf.Max(0.1f, duration);
            
            // Update all registered buttons
            foreach (ButtonHandler button in registeredButtons)
            {
                if (button != null)
                {
                    button.SetAnimationDuration(globalAnimationDuration);
                }
            }
        }
        
        /// <summary>
        /// Sets global button colors
        /// </summary>
        /// <param name="normal">Normal color</param>
        /// <param name="hover">Hover color</param>
        /// <param name="click">Click color</param>
        public void SetGlobalButtonColors(Color normal, Color hover, Color click)
        {
            globalNormalColor = normal;
            globalHoverColor = hover;
            globalClickColor = click;
            
            // Update all registered buttons
            foreach (ButtonHandler button in registeredButtons)
            {
                if (button != null)
                {
                    button.SetButtonColors(globalNormalColor, globalHoverColor, globalClickColor);
                }
            }
        }
        
        /// <summary>
        /// Gets all registered buttons
        /// </summary>
        /// <returns>List of registered buttons</returns>
        public List<ButtonHandler> GetRegisteredButtons()
        {
            return new List<ButtonHandler>(registeredButtons);
        }
        
        /// <summary>
        /// Resets all buttons to their normal state
        /// </summary>
        public void ResetAllButtons()
        {
            foreach (ButtonHandler button in registeredButtons)
            {
                if (button != null)
                {
                    // This would call a reset method on the button
                    // For now, we'll just ensure it's in a clean state
                    button.SetInteractable(enableButtonAnimations);
                }
            }
            
            Debug.Log("All buttons reset to normal state");
        }
        
        /// <summary>
        /// Cleans up the button animation manager
        /// </summary>
        private void OnDestroy()
        {
            if (instance == this)
            {
                // Unsubscribe from all button events
                foreach (ButtonHandler button in registeredButtons)
                {
                    if (button != null)
                    {
                        button.OnButtonHover -= HandleButtonHover;
                        button.OnButtonClick -= HandleButtonClick;
                    }
                }
                
                registeredButtons.Clear();
            }
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method for testing button animations
        /// </summary>
        [ContextMenu("Test Button Animation Manager")]
        private void TestButtonAnimationManager()
        {
            if (Application.isPlaying)
            {
                Debug.Log($"Button Animation Manager Status:");
                Debug.Log($"- Animations Enabled: {enableButtonAnimations}");
                Debug.Log($"- Registered Buttons: {registeredButtons.Count}");
                Debug.Log($"- Global Hover Scale: {globalHoverScale}");
                Debug.Log($"- Global Click Scale: {globalClickScale}");
                Debug.Log($"- Global Duration: {globalAnimationDuration}");
                
                // List registered buttons
                foreach (ButtonHandler button in registeredButtons)
                {
                    if (button != null)
                    {
                        Debug.Log($"  - {button.name}: {(button.IsAnimating ? "Animating" : "Idle")}");
                    }
                }
            }
            else
            {
                Debug.Log("Button animation manager test can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for toggling button animations
        /// </summary>
        [ContextMenu("Toggle Button Animations")]
        private void ToggleButtonAnimations()
        {
            if (Application.isPlaying)
            {
                SetButtonAnimationsEnabled(!enableButtonAnimations);
            }
            else
            {
                Debug.Log("Button animation toggle can only be run in Play mode");
            }
        }
        
        /// <summary>
        /// Editor-only method for resetting all buttons
        /// </summary>
        [ContextMenu("Reset All Buttons")]
        private void ResetAllButtonsEditor()
        {
            if (Application.isPlaying)
            {
                ResetAllButtons();
            }
            else
            {
                Debug.Log("Button reset can only be run in Play mode");
            }
        }
        #endif
    }
}