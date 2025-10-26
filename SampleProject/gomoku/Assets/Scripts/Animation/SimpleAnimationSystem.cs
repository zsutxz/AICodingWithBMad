using UnityEngine;
using System.Collections;

namespace Gomoku.Animation
{
    /// <summary>
    /// Simplified animation system for handling UI and game animations
    /// </summary>
    public class SimpleAnimationSystem : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private bool enableAnimations = true;
        [SerializeField] private float defaultAnimationSpeed = 1f;

        // Singleton instance
        private static SimpleAnimationSystem instance;
        public static SimpleAnimationSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("SimpleAnimationSystem");
                    instance = managerObject.AddComponent<SimpleAnimationSystem>();
                    DontDestroyOnLoad(managerObject);
                }
                return instance;
            }
        }

        public bool EnableAnimations => enableAnimations;
        public float DefaultAnimationSpeed => defaultAnimationSpeed;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Animate scale of a transform
        /// </summary>
        public IEnumerator AnimateScale(Transform target, Vector3 from, Vector3 to, float duration)
        {
            if (!enableAnimations) yield break;

            float elapsed = 0;
            while (elapsed < duration)
            {
                target.localScale = Vector3.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime * defaultAnimationSpeed;
                yield return null;
            }
            target.localScale = to;
        }

        /// <summary>
        /// Animate position of a transform
        /// </summary>
        public IEnumerator AnimatePosition(Transform target, Vector3 from, Vector3 to, float duration)
        {
            if (!enableAnimations) yield break;

            float elapsed = 0;
            while (elapsed < duration)
            {
                target.position = Vector3.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime * defaultAnimationSpeed;
                yield return null;
            }
            target.position = to;
        }

        /// <summary>
        /// Animate color of a UI image
        /// </summary>
        public IEnumerator AnimateColor(UnityEngine.UI.Image target, Color from, Color to, float duration)
        {
            if (!enableAnimations) yield break;

            float elapsed = 0;
            while (elapsed < duration)
            {
                target.color = Color.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime * defaultAnimationSpeed;
                yield return null;
            }
            target.color = to;
        }

        /// <summary>
        /// Simple fade effect for UI elements
        /// </summary>
        public IEnumerator FadeCanvasGroup(CanvasGroup target, float from, float to, float duration)
        {
            if (!enableAnimations) yield break;

            float elapsed = 0;
            while (elapsed < duration)
            {
                target.alpha = Mathf.Lerp(from, to, elapsed / duration);
                elapsed += Time.deltaTime * defaultAnimationSpeed;
                yield return null;
            }
            target.alpha = to;
        }

        /// <summary>
        /// Toggle animations on/off
        /// </summary>
        public void SetAnimationsEnabled(bool enabled)
        {
            enableAnimations = enabled;
            Debug.Log($"Animations {(enabled ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// Set animation speed
        /// </summary>
        public void SetAnimationSpeed(float speed)
        {
            defaultAnimationSpeed = Mathf.Max(0.1f, speed);
        }
    }
}