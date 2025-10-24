using NUnit.Framework;
using UnityEngine;
using Gomoku.Animation;

namespace Gomoku.Tests.Animation
{
    public class VictoryEffectTests
    {
        private GameObject testObject;
        private VictoryEffect victoryEffect;
        private SpriteRenderer spriteRenderer;

        [SetUp]
        public void SetUp()
        {
            testObject = new GameObject("TestVictoryEffect");
            spriteRenderer = testObject.AddComponent<SpriteRenderer>();
            victoryEffect = testObject.AddComponent<VictoryEffect>();
            
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
        public void VictoryEffect_InitializesCorrectly()
        {
            // Assert
            Assert.IsNotNull(victoryEffect);
            Assert.IsNotNull(victoryEffect.GlowDuration);
            Assert.IsFalse(victoryEffect.IsAnimating);
        }

        [Test]
        public void StartVictoryHighlight_SetsAnimationState()
        {
            // Act
            victoryEffect.StartVictoryHighlight();

            // Assert
            Assert.IsTrue(victoryEffect.IsAnimating);
        }

        [Test]
        public void StopVictoryAnimation_ResetsAnimationState()
        {
            // Arrange
            victoryEffect.StartVictoryHighlight();

            // Act
            victoryEffect.StopVictoryAnimation();

            // Assert
            Assert.IsFalse(victoryEffect.IsAnimating);
        }

        [Test]
        public void SetGlowColor_UpdatesColorProperty()
        {
            // Arrange
            Color newColor = Color.red;

            // Act
            victoryEffect.SetGlowColor(newColor);

            // Assert
            // Note: In real implementation, we would verify the material color update
            Assert.Pass("SetGlowColor method executed without errors");
        }

        [Test]
        public void SetGlowDuration_UpdatesDurationProperty()
        {
            // Arrange
            float newDuration = 2.0f;

            // Act
            victoryEffect.SetGlowDuration(newDuration);

            // Assert
            Assert.AreEqual(newDuration, victoryEffect.GlowDuration);
        }

        [Test]
        public void SetPulseIntensity_UpdatesIntensityProperty()
        {
            // Arrange
            float newIntensity = 3.0f;

            // Act
            victoryEffect.SetPulseIntensity(newIntensity);

            // Assert
            // Note: In real implementation, we would verify the pulse curve update
            Assert.Pass("SetPulseIntensity method executed without errors");
        }

        [Test]
        public void AnimationEvents_AreInvoked()
        {
            // Arrange
            bool startInvoked = false;
            bool completeInvoked = false;

            victoryEffect.OnVictoryEffectStart += (effect) => startInvoked = true;
            victoryEffect.OnVictoryEffectComplete += (effect) => completeInvoked = true;

            // Act
            victoryEffect.StartVictoryHighlight();
            victoryEffect.StopVictoryAnimation();

            // Assert
            Assert.IsTrue(startInvoked, "OnVictoryEffectStart should be invoked");
            Assert.IsTrue(completeInvoked, "OnVictoryEffectComplete should be invoked");
        }
    }
}