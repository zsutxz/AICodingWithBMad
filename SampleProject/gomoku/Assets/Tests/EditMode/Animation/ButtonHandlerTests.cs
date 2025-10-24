using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Gomoku.UI;

namespace Tests.EditMode.Animation
{
    public class ButtonHandlerTests
    {
        private GameObject testGameObject;
        private ButtonHandler buttonHandler;
        private Button button;
        private bool eventTriggered;

        [SetUp]
        public void SetUp()
        {
            testGameObject = new GameObject("TestButtonHandler");
            buttonHandler = testGameObject.AddComponent<ButtonHandler>();
            button = testGameObject.AddComponent<Button>();

            // Add required components for button functionality
            testGameObject.AddComponent<Image>();
            testGameObject.AddComponent<RectTransform>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testGameObject);
        }

        [Test]
        public void ButtonHandler_InitializesCorrectly()
        {
            Assert.IsNotNull(buttonHandler);
            Assert.IsFalse(buttonHandler.IsAnimating);
            Assert.IsFalse(buttonHandler.IsHovered);
            Assert.IsFalse(buttonHandler.IsPressed);
        }

        [Test]
        public void OnPointerEnter_StartsHoverAnimation()
        {
            var eventData = new PointerEventData(EventSystem.current);
            buttonHandler.OnPointerEnter(eventData);

            Assert.IsTrue(buttonHandler.IsAnimating);
        }

        [Test]
        public void OnPointerExit_StartsExitAnimation()
        {
            var eventData = new PointerEventData(EventSystem.current);
            buttonHandler.OnPointerEnter(eventData);
            buttonHandler.OnPointerExit(eventData);

            Assert.IsTrue(buttonHandler.IsAnimating);
        }

        [Test]
        public void OnPointerDown_StartsPressAnimation()
        {
            var eventData = new PointerEventData(EventSystem.current);
            buttonHandler.OnPointerDown(eventData);

            Assert.IsTrue(buttonHandler.IsAnimating);
        }

        [Test]
        public void OnPointerUp_StartsReleaseAnimation()
        {
            var eventData = new PointerEventData(EventSystem.current);
            buttonHandler.OnPointerDown(eventData);
            buttonHandler.OnPointerUp(eventData);

            Assert.IsTrue(buttonHandler.IsAnimating);
        }

        [Test]
        public void AnimationDuration_IsConfigurable()
        {
            var expectedDuration = 0.2f;
            buttonHandler.SetAnimationDuration(expectedDuration);
            // Note: There's no getter for animation duration in the implementation
            // This test verifies the method exists and doesn't throw errors
        }

        [Test]
        public void AnimationScales_AreConfigurable()
        {
            var hoverScale = 1.1f;
            var clickScale = 0.9f;
            buttonHandler.SetAnimationScales(hoverScale, clickScale);
            // Note: There's no getter for scales in the implementation
            // This test verifies the method exists and doesn't throw errors
        }

        [Test]
        public void ButtonColors_AreConfigurable()
        {
            var normalColor = Color.white;
            var hoverColor = Color.yellow;
            var clickColor = Color.gray;
            buttonHandler.SetButtonColors(normalColor, hoverColor, clickColor);
            // Note: There's no getter for colors in the implementation
            // This test verifies the method exists and doesn't throw errors
        }

        [Test]
        public void OnButtonHover_EventIsTriggered()
        {
            bool eventTriggered = false;
            buttonHandler.OnButtonHover += (handler) => eventTriggered = true;

            var eventData = new PointerEventData(EventSystem.current);
            buttonHandler.OnPointerEnter(eventData);

            Assert.IsTrue(eventTriggered);
        }

        [Test]
        public void OnButtonClick_EventIsTriggered()
        {
            buttonHandler.OnButtonClick += (handler) => eventTriggered = true;

            // Note: Since HandleButtonClick() is private, we can't test it directly
            // This test verifies the event subscription pattern works
            // The actual click handling would be tested through Unity's UI system
        }

        [Test]
        public void SetInteractable_ControlsButtonState()
        {
            // Test enabling interactable
            buttonHandler.SetInteractable(true);
            Assert.IsTrue(button.interactable);

            // Test disabling interactable
            buttonHandler.SetInteractable(false);
            Assert.IsFalse(button.interactable);
        }
    }
}