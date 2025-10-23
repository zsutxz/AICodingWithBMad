using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Gomoku.UI.MainMenu
{
    public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float clickScale = 0.95f;
        [SerializeField] private float animationSpeed = 10f;

        private Transform buttonTransform;
        private Vector3 originalScale;
        private bool isHovered = false;

        void Awake()
        {
            buttonTransform = transform;
            originalScale = buttonTransform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
            StopAllCoroutines();
            StartCoroutine(ScaleTo(hoverScale));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            StopAllCoroutines();
            //StartCoroutine(ScaleTo(originalScale));
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //// Play UI click sound
            //if (AudioManager.Instance != null)
            //{
            //    AudioManager.Instance.PlayUIClick();
            //}

            StopAllCoroutines();
            StartCoroutine(ClickAnimation());
        }

        private IEnumerator ScaleTo(float targetScale)
        {
            float currentTime = 0;
            Vector3 startScale = buttonTransform.localScale;
            Vector3 targetVector = new Vector3(targetScale, targetScale, targetScale);

            while (currentTime < 1)
            {
                currentTime += Time.deltaTime * animationSpeed;
                buttonTransform.localScale = Vector3.Lerp(startScale, targetVector, currentTime);
                yield return null;
            }
        }

        private IEnumerator ClickAnimation()
        {
            // Scale down
            yield return ScaleTo(clickScale);
            //// Scale back to appropriate scale (hover or original)
            //float target = isHovered ? hoverScale : originalScale;
            //yield return ScaleTo(target);
        }
    }
}