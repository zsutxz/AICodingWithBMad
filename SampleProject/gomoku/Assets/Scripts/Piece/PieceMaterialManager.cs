using UnityEngine;
using System.Collections.Generic;
using Gomoku.Core;

namespace Gomoku
{
    /// <summary>
    /// Manages materials for game pieces with optimized rendering and visual effects
    /// </summary>
    public class PieceMaterialManager : MonoBehaviour
    {
        [Header("Material Settings")]
        [SerializeField] private Material blackPieceMaterial;
        [SerializeField] private Material whitePieceMaterial;
        [SerializeField] private Material shadowMaterial;

        [Header("Visual Settings")]
        [SerializeField] private float blackPieceGlossiness = 0.8f;
        [SerializeField] private float whitePieceGlossiness = 0.6f;
        [SerializeField] private Color blackPieceEmission = Color.black;
        [SerializeField] private Color whitePieceEmission = new Color(0.1f, 0.1f, 0.1f);
        [SerializeField] private Texture2D pieceTexture;

        [Header("Optimization")]
        [SerializeField] private bool enableMaterialPooling = true;
        [SerializeField] private int initialPoolSize = 20;

        // Material pools
        private Dictionary<PlayerType, Queue<Material>> blackMaterialPool;
        private Dictionary<PlayerType, Queue<Material>> whiteMaterialPool;
        private Queue<Material> shadowMaterialPool;

        // Cached materials
        private static Dictionary<PlayerType, Material> cachedMaterials = new Dictionary<PlayerType, Material>();
        private static Material cachedShadowMaterial;

        private void Awake()
        {
            InitializeMaterialManager();
        }

        /// <summary>
        /// Initializes the material manager and creates base materials
        /// </summary>
        private void InitializeMaterialManager()
        {
            // Initialize pools if enabled
            if (enableMaterialPooling)
            {
                blackMaterialPool = new Dictionary<PlayerType, Queue<Material>>();
                whiteMaterialPool = new Dictionary<PlayerType, Queue<Material>>();
                shadowMaterialPool = new Queue<Material>();

                blackMaterialPool[PlayerType.PlayerOne] = new Queue<Material>();
                blackMaterialPool[PlayerType.PlayerTwo] = new Queue<Material>();
                whiteMaterialPool[PlayerType.PlayerOne] = new Queue<Material>();
                whiteMaterialPool[PlayerType.PlayerTwo] = new Queue<Material>();

                // Pre-populate pools
                for (int i = 0; i < initialPoolSize; i++)
                {
                    CreatePooledMaterials();
                }
            }

            // Create base materials if not assigned
            CreateBaseMaterials();

            Debug.Log("PieceMaterialManager initialized");
        }

        /// <summary>
        /// Creates base materials if not assigned
        /// </summary>
        private void CreateBaseMaterials()
        {
            // Create black piece material
            if (blackPieceMaterial == null)
            {
                blackPieceMaterial = CreatePieceMaterial(Color.black, blackPieceGlossiness, blackPieceEmission);
            }

            // Create white piece material
            if (whitePieceMaterial == null)
            {
                whitePieceMaterial = CreatePieceMaterial(Color.white, whitePieceGlossiness, whitePieceEmission);
            }

            // Create shadow material
            if (shadowMaterial == null)
            {
                shadowMaterial = CreateShadowMaterial();
            }
        }

        /// <summary>
        /// Creates a piece material with specified properties
        /// </summary>
        /// <param name="baseColor">Base color of the material</param>
        /// <param name="glossiness">Glossiness value</param>
        /// <param name="emission">Emission color</param>
        /// <returns>Created material</returns>
        private Material CreatePieceMaterial(Color baseColor, float glossiness, Color emission)
        {
            Material mat = new Material(Shader.Find("Standard"));

            mat.color = baseColor;
            mat.SetFloat("_Metallic", 0.1f);
            mat.SetFloat("_Glossiness", glossiness);
            mat.SetColor("_EmissionColor", emission);
            mat.mainTexture = pieceTexture;

            // Enable smooth rendering
            mat.SetFloat("_SmoothnessTextureChannel", 1);

            return mat;
        }

        /// <summary>
        /// Creates a shadow material
        /// </summary>
        /// <returns>Created shadow material</returns>
        private Material CreateShadowMaterial()
        {
            Material mat = new Material(Shader.Find("Standard"));

            Color shadowColor = new Color(0f, 0f, 0f, 0.3f);
            mat.color = shadowColor;
            mat.SetFloat("_Mode", 3); // Transparent
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;

            return mat;
        }

        /// <summary>
        /// Creates pooled materials for optimization
        /// </summary>
        private void CreatePooledMaterials()
        {
            // Create material instances for pooling
            Material blackMat = new Material(blackPieceMaterial);
            Material whiteMat = new Material(whitePieceMaterial);
            Material shadowMat = new Material(shadowMaterial);

            blackMaterialPool[PlayerType.PlayerOne].Enqueue(blackMat);
            blackMaterialPool[PlayerType.PlayerTwo].Enqueue(blackMat);
            whiteMaterialPool[PlayerType.PlayerOne].Enqueue(whiteMat);
            whiteMaterialPool[PlayerType.PlayerTwo].Enqueue(whiteMat);
            shadowMaterialPool.Enqueue(shadowMat);
        }

        /// <summary>
        /// Gets a material for the specified player type
        /// </summary>
        /// <param name="playerType">Type of player</param>
        /// <param name="useBlackMaterial">Whether to use black material base</param>
        /// <returns>Material for the piece</returns>
        public Material GetMaterialForPlayer(PlayerType playerType, bool useBlackMaterial = true)
        {
            Material baseMaterial = useBlackMaterial ? blackPieceMaterial : whitePieceMaterial;

            // Use caching if pooling is disabled
            if (!enableMaterialPooling)
            {
                string key = $"{playerType}_{(useBlackMaterial ? "black" : "white")}";
                if (cachedMaterials.ContainsKey(playerType))
                {
                    return cachedMaterials[playerType];
                }

                Material material = new Material(baseMaterial);
                cachedMaterials[playerType] = material;
                return material;
            }

            // Use pooling
            Dictionary<PlayerType, Queue<Material>> pool = useBlackMaterial ? blackMaterialPool : whiteMaterialPool;

            if (pool[playerType].Count > 0)
            {
                return pool[playerType].Dequeue();
            }

            // Create new material if pool is empty
            return new Material(baseMaterial);
        }

        /// <summary>
        /// Gets a shadow material
        /// </summary>
        /// <returns>Shadow material</returns>
        public Material GetShadowMaterial()
        {
            if (!enableMaterialPooling)
            {
                if (cachedShadowMaterial == null)
                {
                    cachedShadowMaterial = new Material(shadowMaterial);
                }
                return cachedShadowMaterial;
            }

            if (shadowMaterialPool.Count > 0)
            {
                return shadowMaterialPool.Dequeue();
            }

            return new Material(shadowMaterial);
        }

        /// <summary>
        /// Returns a material to the pool
        /// </summary>
        /// <param name="material">Material to return</param>
        /// <param name="playerType">Type of player the material was used for</param>
        /// <param name="useBlackMaterial">Whether it was based on black material</param>
        public void ReturnMaterialToPool(Material material, PlayerType playerType, bool useBlackMaterial = true)
        {
            if (!enableMaterialPooling || material == null) return;

            Dictionary<PlayerType, Queue<Material>> pool = useBlackMaterial ? blackMaterialPool : whiteMaterialPool;
            pool[playerType].Enqueue(material);
        }

        /// <summary>
        /// Returns a shadow material to the pool
        /// </summary>
        /// <param name="material">Shadow material to return</param>
        public void ReturnShadowMaterialToPool(Material material)
        {
            if (!enableMaterialPooling || material == null) return;

            shadowMaterialPool.Enqueue(material);
        }

        /// <summary>
        /// Updates material properties
        /// </summary>
        /// <param name="blackColor">New black piece color</param>
        /// <param name="whiteColor">New white piece color</param>
        /// <param name="blackGloss">New black glossiness</param>
        /// <param name="whiteGloss">New white glossiness</param>
        public void UpdateMaterialProperties(Color blackColor, Color whiteColor, float blackGloss, float whiteGloss)
        {
            // Update base materials
            if (blackPieceMaterial != null)
            {
                blackPieceMaterial.color = blackColor;
                blackPieceMaterial.SetFloat("_Glossiness", blackGloss);
            }

            if (whitePieceMaterial != null)
            {
                whitePieceMaterial.color = whiteColor;
                whitePieceMaterial.SetFloat("_Glossiness", whiteGloss);
            }

            // Clear cached materials to force recreation
            cachedMaterials.Clear();
        }

        /// <summary>
        /// Updates texture for all materials
        /// </summary>
        /// <param name="newTexture">New texture to apply</param>
        public void UpdateTexture(Texture2D newTexture)
        {
            pieceTexture = newTexture;

            if (blackPieceMaterial != null)
                blackPieceMaterial.mainTexture = newTexture;

            if (whitePieceMaterial != null)
                whitePieceMaterial.mainTexture = newTexture;

            // Clear cached materials
            cachedMaterials.Clear();
        }

        /// <summary>
        /// Gets material count information for debugging
        /// </summary>
        /// <returns>String with material count information</returns>
        public string GetMaterialCountInfo()
        {
            if (!enableMaterialPooling) return "Material pooling is disabled";

            string info = "Material Pool Status:\n";
            info += $"Black Materials (Player 1): {blackMaterialPool[PlayerType.PlayerOne].Count}\n";
            info += $"Black Materials (Player 2): {blackMaterialPool[PlayerType.PlayerTwo].Count}\n";
            info += $"White Materials (Player 1): {whiteMaterialPool[PlayerType.PlayerOne].Count}\n";
            info += $"White Materials (Player 2): {whiteMaterialPool[PlayerType.PlayerTwo].Count}\n";
            info += $"Shadow Materials: {shadowMaterialPool.Count}\n";
            info += $"Cached Materials: {cachedMaterials.Count}";

            return info;
        }

        private void OnDestroy()
        {
            // Cleanup pooled materials
            if (enableMaterialPooling)
            {
                foreach (var pool in blackMaterialPool.Values)
                {
                    while (pool.Count > 0)
                    {
                        Material mat = pool.Dequeue();
                        if (mat != null) DestroyImmediate(mat);
                    }
                }

                foreach (var pool in whiteMaterialPool.Values)
                {
                    while (pool.Count > 0)
                    {
                        Material mat = pool.Dequeue();
                        if (mat != null) DestroyImmediate(mat);
                    }
                }

                while (shadowMaterialPool.Count > 0)
                {
                    Material mat = shadowMaterialPool.Dequeue();
                    if (mat != null) DestroyImmediate(mat);
                }
            }

            // Cleanup cached materials
            foreach (var mat in cachedMaterials.Values)
            {
                if (mat != null) DestroyImmediate(mat);
            }

            if (cachedShadowMaterial != null)
            {
                DestroyImmediate(cachedShadowMaterial);
            }
        }

        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method to log material status
        /// </summary>
        [ContextMenu("Log Material Status")]
        private void LogMaterialStatus()
        {
            Debug.Log(GetMaterialCountInfo());
        }
        #endif
    }
}