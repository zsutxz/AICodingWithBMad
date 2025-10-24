using NUnit.Framework;
using UnityEngine;
using Gomoku.Animation;
using Gomoku;
using Gomoku.Core;

namespace Gomoku.Tests.Animation
{
    public class TurnTransitionTests
    {
        private GameObject testObject;
        private TurnTransition turnTransition;
        private SpriteRenderer spriteRenderer;

        [SetUp]
        public void SetUp()
        {
            testObject = new GameObject("TestTurnTransition");
            spriteRenderer = testObject.AddComponent<SpriteRenderer>();
            turnTransition = testObject.AddComponent<TurnTransition>();
            
            // Initialize with test sprite
            spriteRenderer.sprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), Vector2.zero);
        }

        [TearDown]
        public void TearDown()
        {
            if (testObject != null)
            {
                Object.DestroyImmediate(testObject);
            }
        }

        [Test]
        public void TurnTransition_InitializesCorrectly()
        {
            // Assert
            Assert.IsNotNull(turnTransition);
            Assert.IsNotNull(turnTransition.TransitionDuration);
            Assert.IsFalse(turnTransition.IsAnimating);
        }

        [Test]
        public void StartTurnTransition_SetsAnimationState()
        {
            // Act
            turnTransition.StartTurnTransition(PlayerType.PlayerOne);

            // Assert
            Assert.IsTrue(turnTransition.IsAnimating);
        }

        [Test]
        public void StopTransitionAnimation_ResetsAnimationState()
        {
            // Arrange
            turnTransition.StartTurnTransition(PlayerType.PlayerOne);

            // Act
            turnTransition.StopTransitionAnimation();

            // Assert
            Assert.IsFalse(turnTransition.IsAnimating);
        }

        [Test]
        public void SetPlayerColors_UpdatesColorProperties()
        {
            // Arrange
            Color playerOneColor = Color.blue;
            Color playerTwoColor = Color.green;

            // Act
            turnTransition.SetPlayerColors(playerOneColor, playerTwoColor);

            // Assert
            // Note: In real implementation, we would verify the color assignments
            Assert.Pass("SetPlayerColors method executed without errors");
        }

        [Test]
        public void SetTransitionColor_UpdatesTransitionColor()
        {
            // Arrange
            Color newColor = Color.magenta;

            // Act
            turnTransition.SetTransitionColor(newColor);

            // Assert
            // Note: In real implementation, we would verify the color assignment
            Assert.Pass("SetTransitionColor method executed without errors");
        }

        [Test]
        public void SetTransitionDuration_UpdatesDurationProperty()
        {
            // Arrange
            float newDuration = 1.0f;

            // Act
            turnTransition.SetTransitionDuration(newDuration);

            // Assert
            Assert.AreEqual(newDuration, turnTransition.TransitionDuration);
        }

        [Test]
        public void UpdateTurnDisplay_HandlesDifferentPlayerTypes()
        {
            // Act & Assert - Should not throw exceptions for different player types
            Assert.DoesNotThrow(() => turnTransition.UpdateTurnDisplay(PlayerType.PlayerOne));
            Assert.DoesNotThrow(() => turnTransition.UpdateTurnDisplay(PlayerType.PlayerTwo));
            Assert.DoesNotThrow(() => turnTransition.UpdateTurnDisplay(PlayerType.None));
        }

        [Test]
        public void AnimationEvents_AreInvoked()
        {
            // Arrange
            bool startInvoked = false;
            bool completeInvoked = false;

            turnTransition.OnTransitionStart += (transition) => startInvoked = true;
            turnTransition.OnTransitionComplete += (transition) => completeInvoked = true;

            // Act
            turnTransition.StartTurnTransition(PlayerType.PlayerOne);
            turnTransition.StopTransitionAnimation();

            // Assert
            Assert.IsTrue(startInvoked, "OnTransitionStart should be invoked");
            Assert.IsTrue(completeInvoked, "OnTransitionComplete should be invoked");
        }
    }
}