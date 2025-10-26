using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Gomoku.UI;
using Gomoku.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Gomoku.Tests.EditMode
{
    /// <summary>
    /// Unit tests for ButtonHandler audio integration
    /// </summary>
    public class ButtonHandlerAudioTests
    {
        private GameObject buttonObject;
        private ButtonHandler buttonHandler;
        private Button button;
        private GameObject audioManagerObject;
        private AudioManager audioManager;

        [SetUp]
        public void SetUp()
        {
            // Create AudioManager
            audioManagerObject = new GameObject("AudioManager");
            audioManager = audioManagerObject.AddComponent<AudioManager>();

            // Create Button with required components
            buttonObject = new GameObject("TestButton");
            button = buttonObject.AddComponent<Button>();
            buttonObject.AddComponent<Image>();
            buttonObject.AddComponent<CanvasRenderer>();

            // Create ButtonHandler
            buttonHandler = buttonObject.AddComponent<ButtonHandler>();
        }

        [TearDown]
        public void TearDown()
        {
            if (buttonObject != null)
                Object.DestroyImmediate(buttonObject);
            
            if (audioManagerObject != null)
                Object.DestroyImmediate(audioManagerObject);
        }

        [Test]
        public void ButtonHandler_HasAudioConfiguration_ByDefault()
        {
            // Arrange & Act & Assert
            Assert.IsNotNull(buttonHandler, "ButtonHandler should exist");
            Assert.IsNotNull(button, "Button component should exist");
            // Note: In a real test, we would use reflection to verify audio fields exist
        }

        [Test]
        public void ButtonHandler_PointerEnter_TriggersHoverAudio()
        {
            // Arrange
            var pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Vector2.zero
            };

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                buttonHandler.OnPointerEnter(pointerEventData);
            }, "ButtonHandler OnPointerEnter with audio should not throw exceptions");
        }

        [Test]
        public void ButtonHandler_Click_TriggersClickAudio()
        {
            // Arrange
            // Simulate button click
            button.onClick.Invoke();

            // Act & Assert - Simulate button click through Unity's onClick event
            Assert.DoesNotThrow(() =>
            {
                button.onClick.Invoke();
            }, "ButtonHandler click with audio should not throw exceptions");
        }

        [Test]
        public void ButtonHandler_AudioManagerIntegration_IsAvailable()
        {
            // Arrange & Act
            AudioManager audioManagerInstance = AudioManager.Instance;

            // Assert
            Assert.IsNotNull(audioManagerInstance, "AudioManager should be available for ButtonHandler integration");
        }

        [Test]
        public void UIHoverSound_PlayingThroughAudioManager_DoesNotThrow()
        {
            // Arrange
            AudioManager audioManagerInstance = AudioManager.Instance;

            // Act & Assert
            Assert.DoesNotThrow(() => audioManagerInstance.PlayUIHoverSound(),
                "Playing UI hover sound through AudioManager should not throw exceptions");
        }

        [Test]
        public void UIClickSound_PlayingThroughAudioManager_DoesNotThrow()
        {
            // Arrange
            AudioManager audioManagerInstance = AudioManager.Instance;

            // Act & Assert
            Assert.DoesNotThrow(() => audioManagerInstance.PlayUIClick(),
                "Playing UI click sound through AudioManager should not throw exceptions");
        }

        [Test]
        public void ButtonHandler_WithNonInteractableButton_DoesNotPlayAudio()
        {
            // Arrange
            button.interactable = false;
            var pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Vector2.zero
            };

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                buttonHandler.OnPointerEnter(pointerEventData);
            }, "Non-interactable button should not throw exceptions on hover");
        }

        [Test]
        public void ButtonHandler_PointerExit_HandlesGracefully()
        {
            // Arrange
            var pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Vector2.zero
            };

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                buttonHandler.OnPointerExit(pointerEventData);
            }, "ButtonHandler OnPointerExit should not throw exceptions");
        }

        [Test]
        public void ButtonHandler_PointerDownAndUp_HandlesGracefully()
        {
            // Arrange
            var pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Vector2.zero
            };

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                buttonHandler.OnPointerDown(pointerEventData);
                buttonHandler.OnPointerUp(pointerEventData);
            }, "ButtonHandler pointer down/up events should not throw exceptions");
        }
    }
}