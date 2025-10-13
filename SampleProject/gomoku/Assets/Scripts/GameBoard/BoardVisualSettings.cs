using UnityEngine;

namespace Gomoku
{
    /// <summary>
    /// Configuration class for board visual settings with traditional Chinese aesthetic
    /// </summary>
    [CreateAssetMenu(fileName = "BoardVisualSettings", menuName = "Gomoku/Board Visual Settings")]
    public class BoardVisualSettings : ScriptableObject
    {
        [Header("Traditional Chinese Aesthetic")]
        [SerializeField] private Color boardBackgroundColor = new Color(0.8f, 0.7f, 0.5f, 1.0f); // Dark wood tone
        [SerializeField] private Color gridLineColor = new Color(0.2f, 0.2f, 0.2f, 1.0f); // Dark grid lines
        [SerializeField] private Color intersectionMarkerColor = new Color(0.3f, 0.3f, 0.3f, 0.8f); // Subtle intersection markers
        
        [Header("Grid Line Settings")]
        [SerializeField] private float gridLineWidth = 0.02f;
        [SerializeField] private Material gridLineMaterial;
        
        [Header("Intersection Markers")]
        [SerializeField] private float intersectionMarkerSize = 0.05f;
        [SerializeField] private GameObject intersectionMarkerPrefab;
        
        [Header("Background Texture")]
        [SerializeField] private Texture2D boardBackgroundTexture;
        [SerializeField] private Sprite boardBackgroundSprite;
        [SerializeField] private Vector2 backgroundTiling = Vector2.one;
        
        [Header("Board Dimensions")]
        [SerializeField] private int boardSize = 15;
        [SerializeField] private float cellSize = 1.0f;
        [SerializeField] private Vector2 boardOffset = new Vector2(-7, -7);

        // Public properties for access
        public Color BoardBackgroundColor => boardBackgroundColor;
        public Color GridLineColor => gridLineColor;
        public Color IntersectionMarkerColor => intersectionMarkerColor;
        public float GridLineWidth => gridLineWidth;
        public Material GridLineMaterial => gridLineMaterial;
        public float IntersectionMarkerSize => intersectionMarkerSize;
        public GameObject IntersectionMarkerPrefab => intersectionMarkerPrefab;
        public Texture2D BoardBackgroundTexture => boardBackgroundTexture;
        public Sprite BoardBackgroundSprite => boardBackgroundSprite;
        public Vector2 BackgroundTiling => backgroundTiling;
        public int BoardSize => boardSize;
        public float CellSize => cellSize;
        public Vector2 BoardOffset => boardOffset;

        /// <summary>
        /// Applies these visual settings to a BoardRenderer component
        /// </summary>
        /// <param name="boardRenderer">BoardRenderer to apply settings to</param>
        public void ApplyToBoardRenderer(BoardRenderer boardRenderer)
        {
            if (boardRenderer == null) return;

            // Initialize the board renderer with these settings
            boardRenderer.Initialize(boardSize, cellSize, boardOffset);
            
            // Apply visual properties
            boardRenderer.UpdateBackgroundColor(boardBackgroundColor);
            boardRenderer.UpdateGridLineColor(gridLineColor);
        }

        /// <summary>
        /// Creates a default instance with traditional Chinese aesthetic
        /// </summary>
        /// <returns>Default BoardVisualSettings instance</returns>
        public static BoardVisualSettings CreateDefault()
        {
            BoardVisualSettings settings = CreateInstance<BoardVisualSettings>();
            
            // Set traditional Chinese aesthetic colors
            settings.boardBackgroundColor = new Color(0.8f, 0.7f, 0.5f, 1.0f); // Dark wood tone
            settings.gridLineColor = new Color(0.2f, 0.2f, 0.2f, 1.0f); // Dark grid lines
            settings.intersectionMarkerColor = new Color(0.3f, 0.3f, 0.3f, 0.8f); // Subtle markers
            
            // Set dimensions for 15x15 board
            settings.boardSize = 15;
            settings.cellSize = 1.0f;
            settings.boardOffset = new Vector2(-7, -7);
            
            return settings;
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            // Ensure values are within valid ranges
            gridLineWidth = Mathf.Max(0.001f, gridLineWidth);
            intersectionMarkerSize = Mathf.Max(0.01f, intersectionMarkerSize);
            boardSize = Mathf.Max(3, boardSize);
            cellSize = Mathf.Max(0.1f, cellSize);
        }
        #endif
    }
}