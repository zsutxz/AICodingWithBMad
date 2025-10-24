using NUnit.Framework;
using UnityEngine;
using Gomoku.Animation;

namespace Gomoku.Tests.Animation
{
    public class PieceAnimationTests
    {
        private GameObject testObject;
        private PieceAnimation pieceAnimation;
        private SpriteRenderer spriteRenderer;

        [SetUp]
        public void SetUp()
        {
            testObject = new GameObject("TestPiece");
            spriteRenderer = testObject.AddComponent<SpriteRenderer>();
            pieceAnimation = testObject.AddComponent<PieceAnimation>();
            
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
        public void PieceAnimation_InitializesCorrectly()
        {
            // Assert
            Assert.IsNotNull(pieceAnimation);
            Assert.IsNotNull(pieceAnimation.FadeInDuration);
            Assert.IsNotNull(pieceAnimation.ScaleDuration);
            Assert.IsFalse(pieceAnimation.IsAnimating);
        }

        [Test]
        public void StartPlacementAnimation_SetsAnimationState()
        {
            // Act
            pieceAnimation.StartPlacementAnimation();

            // Assert
            Assert.IsTrue(pieceAnimation.IsAnimating);
        }

        [Test]
        public void StopCurrentAnimation_ResetsAnimationState()
        {
            // Arrange
            pieceAnimation.StartPlacementAnimation();

            // Act
            pieceAnimation.StopCurrentAnimation();

            // Assert
            Assert.IsFalse(pieceAnimation.IsAnimating);
        }

        [Test]
        public void ResetAnimation_RestoresOriginalState()
        {
            // Arrange
            pieceAnimation.StartPlacementAnimation();

            // Act
            pieceAnimation.ResetAnimation();

            // Assert
            Assert.IsFalse(pieceAnimation.IsAnimating);
            Assert.AreEqual(Vector3.one, testObject.transform.localScale);
            Assert.AreEqual(1f, spriteRenderer.color.a);
        }

        [Test]
        public void SetAnimationDurations_UpdatesProperties()
        {
            // Arrange
            float newFadeDuration = 0.5f;
            float newScaleDuration = 0.7f;

            // Act
            pieceAnimation.SetAnimationDurations(newFadeDuration, newScaleDuration);

            // Assert
            Assert.AreEqual(newFadeDuration, pieceAnimation.FadeInDuration);
            Assert.AreEqual(newScaleDuration, pieceAnimation.ScaleDuration);
        }

        [Test]
        public void AnimationEvents_AreInvoked()
        {
            // Arrange
            bool startInvoked = false;
            bool completeInvoked = false;

            pieceAnimation.OnAnimationStart += (anim) => startInvoked = true;
            pieceAnimation.OnAnimationComplete += (anim) => completeInvoked = true;

            // Act
            pieceAnimation.StartPlacementAnimation();
            pieceAnimation.StopCurrentAnimation();

            // Assert
            Assert.IsTrue(startInvoked, "OnAnimationStart should be invoked");
            Assert.IsTrue(completeInvoked, "OnAnimationComplete should be invoked");
        }
    }
}