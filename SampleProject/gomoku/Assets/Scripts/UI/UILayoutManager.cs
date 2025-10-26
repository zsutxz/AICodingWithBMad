using UnityEngine;

namespace Gomoku.UI
{
    /// <summary>
    /// Manages the positioning of UI elements to ensure non-intrusive layout
    /// </summary>
    public class UILayoutManager : MonoBehaviour
    {
        [Header("UI Element References")]
        [SerializeField] private RectTransform turnIndicator;
        [SerializeField] private RectTransform moveCounter;
        [SerializeField] private RectTransform pauseButton;
        [SerializeField] private RectTransform gameBoardContainer;

        [Header("Layout Settings")]
        [SerializeField] private float screenMargin = 20f;
        [SerializeField] private Vector2 turnIndicatorPosition = new Vector2(0.05f, 0.95f); // Top-left
        [SerializeField] private Vector2 moveCounterPosition = new Vector2(0.05f, 0.05f); // Bottom-left
        [SerializeField] private Vector2 pauseButtonPosition = new Vector2(0.95f, 0.95f); // Top-right

        [Header("Safe Area Settings")]
        [SerializeField] private bool useSafeArea = true;
        [SerializeField] private float safeAreaMargin = 10f;

        private Rect safeArea;
        private Vector2 screenSize;

        private void Awake()
        {
            InitializeLayoutManager();
        }

        private void Start()
        {
            UpdateUILayout();
        }

        /// <summary>
        /// Initializes the layout manager
        /// </summary>
        private void InitializeLayoutManager()
        {
            // Get screen information
            screenSize = new Vector2(Screen.width, Screen.height);
            safeArea = Screen.safeArea;

            Debug.Log("UILayoutManager initialized");
        }

        /// <summary>
        /// Updates the UI layout based on current screen configuration
        /// </summary>
        public void UpdateUILayout()
        {
            // Update screen information
            screenSize = new Vector2(Screen.width, Screen.height);
            safeArea = Screen.safeArea;

            // Position UI elements
            PositionTurnIndicator();
            PositionMoveCounter();
            PositionPauseButton();

            Debug.Log("UI layout updated");
        }

        /// <summary>
        /// Positions the turn indicator in the top-left corner
        /// </summary>
        private void PositionTurnIndicator()
        {
            if (turnIndicator != null)
            {
                Vector2 position = CalculateScreenPosition(turnIndicatorPosition);
                turnIndicator.anchorMin = new Vector2(0, 1);
                turnIndicator.anchorMax = new Vector2(0, 1);
                turnIndicator.pivot = new Vector2(0, 1);
                turnIndicator.anchoredPosition = position;
            }
        }

        /// <summary>
        /// Positions the move counter in the bottom-left corner
        /// </summary>
        private void PositionMoveCounter()
        {
            if (moveCounter != null)
            {
                Vector2 position = CalculateScreenPosition(moveCounterPosition);
                moveCounter.anchorMin = new Vector2(0, 0);
                moveCounter.anchorMax = new Vector2(0, 0);
                moveCounter.pivot = new Vector2(0, 0);
                moveCounter.anchoredPosition = position;
            }
        }

        /// <summary>
        /// Positions the pause button in the top-right corner
        /// </summary>
        private void PositionPauseButton()
        {
            if (pauseButton != null)
            {
                Vector2 position = CalculateScreenPosition(pauseButtonPosition);
                pauseButton.anchorMin = new Vector2(1, 1);
                pauseButton.anchorMax = new Vector2(1, 1);
                pauseButton.pivot = new Vector2(1, 1);
                pauseButton.anchoredPosition = position;
            }
        }

        /// <summary>
        /// Calculates screen position based on normalized coordinates
        /// </summary>
        /// <param name="normalizedPosition">Normalized position (0-1)</param>
        /// <returns>Screen position with margins applied</returns>
        private Vector2 CalculateScreenPosition(Vector2 normalizedPosition)
        {
            Vector2 position = new Vector2(
                normalizedPosition.x * screenSize.x,
                normalizedPosition.y * screenSize.y
            );

            // Apply margins
            if (normalizedPosition.x <= 0.5f)
                position.x += screenMargin;
            else
                position.x -= screenMargin;

            if (normalizedPosition.y <= 0.5f)
                position.y += screenMargin;
            else
                position.y -= screenMargin;

            // Apply safe area adjustments
            if (useSafeArea)
            {
                position = ApplySafeArea(position);
            }

            return position;
        }

        /// <summary>
        /// Applies safe area constraints to a position
        /// </summary>
        /// <param name="position">Original position</param>
        /// <returns>Position adjusted for safe area</returns>
        private Vector2 ApplySafeArea(Vector2 position)
        {
            // Ensure position is within safe area bounds
            position.x = Mathf.Clamp(position.x, safeArea.xMin + safeAreaMargin, safeArea.xMax - safeAreaMargin);
            position.y = Mathf.Clamp(position.y, safeArea.yMin + safeAreaMargin, safeArea.yMax - safeAreaMargin);

            return position;
        }

        /// <summary>
        /// Checks if UI elements are obstructing the game board
        /// </summary>
        /// <returns>True if UI elements are not obstructing the board</returns>
        public bool IsBoardVisible()
        {
            if (gameBoardContainer == null) return true;

            // Get board bounds in screen space
            Vector3[] boardCorners = new Vector3[4];
            gameBoardContainer.GetWorldCorners(boardCorners);

            // Check if any UI element overlaps with board
            bool turnIndicatorOverlaps = IsOverlapping(turnIndicator, boardCorners);
            bool moveCounterOverlaps = IsOverlapping(moveCounter, boardCorners);
            bool pauseButtonOverlaps = IsOverlapping(pauseButton, boardCorners);

            return !(turnIndicatorOverlaps || moveCounterOverlaps || pauseButtonOverlaps);
        }

        /// <summary>
        /// Checks if a UI element overlaps with the game board
        /// </summary>
        /// <param name="element">UI element to check</param>
        /// <param name="boardCorners">Board corners in world space</param>
        /// <returns>True if the element overlaps with the board</returns>
        private bool IsOverlapping(RectTransform element, Vector3[] boardCorners)
        {
            if (element == null) return false;

            // Get element bounds in screen space
            Vector3[] elementCorners = new Vector3[4];
            element.GetWorldCorners(elementCorners);

            // Simple AABB collision check
            Rect elementRect = new Rect(elementCorners[0].x, elementCorners[0].y,
                                      elementCorners[2].x - elementCorners[0].x,
                                      elementCorners[2].y - elementCorners[0].y);

            Rect boardRect = new Rect(boardCorners[0].x, boardCorners[0].y,
                                    boardCorners[2].x - boardCorners[0].x,
                                    boardCorners[2].y - boardCorners[0].y);

            return elementRect.Overlaps(boardRect);
        }

        /// <summary>
        /// Adjusts layout for different screen orientations
        /// </summary>
        /// <param name="isLandscape">True if landscape orientation</param>
        public void AdjustForOrientation(bool isLandscape)
        {
            if (isLandscape)
            {
                // Landscape adjustments
                turnIndicatorPosition = new Vector2(0.02f, 0.95f);
                moveCounterPosition = new Vector2(0.02f, 0.05f);
                pauseButtonPosition = new Vector2(0.98f, 0.95f);
            }
            else
            {
                // Portrait adjustments
                turnIndicatorPosition = new Vector2(0.05f, 0.95f);
                moveCounterPosition = new Vector2(0.05f, 0.05f);
                pauseButtonPosition = new Vector2(0.95f, 0.95f);
            }

            UpdateUILayout();
        }

        /// <summary>
        /// Sets the reference to a UI element
        /// </summary>
        /// <param name="elementType">Type of UI element</param>
        /// <param name="element">The UI element</param>
        public void SetUIElement(UIElementType elementType, RectTransform element)
        {
            switch (elementType)
            {
                case UIElementType.TurnIndicator:
                    turnIndicator = element;
                    break;
                case UIElementType.MoveCounter:
                    moveCounter = element;
                    break;
                case UIElementType.PauseButton:
                    pauseButton = element;
                    break;
                case UIElementType.GameBoard:
                    gameBoardContainer = element;
                    break;
            }

            UpdateUILayout();
        }

        /// <summary>
        /// Gets the current screen size
        /// </summary>
        /// <returns>Current screen size</returns>
        public Vector2 GetScreenSize()
        {
            return screenSize;
        }

        /// <summary>
        /// Gets the current safe area
        /// </summary>
        /// <returns>Current safe area</returns>
        public Rect GetSafeArea()
        {
            return safeArea;
        }
    }

    /// <summary>
    /// Types of UI elements managed by the layout manager
    /// </summary>
    public enum UIElementType
    {
        TurnIndicator,
        MoveCounter,
        PauseButton,
        GameBoard
    }
}