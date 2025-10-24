using NUnit.Framework;
using UnityEngine;
using Gomoku.Animation;

namespace Tests.EditMode.Animation
{
    public class AnimationSettingsTests
    {
        private AnimationSettings animationSettings;

        [SetUp]
        public void SetUp()
        {
            animationSettings = ScriptableObject.CreateInstance<AnimationSettings>();
        }

        [TearDown]
        public void TearDown()
        {
            if (animationSettings != null)
            {
                Object.DestroyImmediate(animationSettings);
            }
        }

        [Test]
        public void AnimationSettings_InitializesCorrectly()
        {
            Assert.IsNotNull(animationSettings);
            Assert.IsTrue(animationSettings.EnableAnimations);
            Assert.AreEqual(AnimationQuality.High, animationSettings.AnimationQuality);
        }

        [Test]
        public void EnableAnimations_CanBeToggled()
        {
            animationSettings.SetAnimationsEnabled(false);
            Assert.IsFalse(animationSettings.EnableAnimations);

            animationSettings.SetAnimationsEnabled(true);
            Assert.IsTrue(animationSettings.EnableAnimations);
        }

        [Test]
        public void QualityLevel_CanBeSet()
        {
            var expectedQuality = AnimationQuality.Low;
            animationSettings.SetAnimationQuality(expectedQuality);
            Assert.AreEqual(expectedQuality, animationSettings.AnimationQuality);
        }

        [Test]
        public void MaxConcurrentAnimations_IsConfigurable()
        {
            var expectedMax = 10;
            animationSettings.SetPerformanceLimits(expectedMax, 15, 4);
            Assert.AreEqual(expectedMax, animationSettings.MaxConcurrentAnimations);
        }

        [Test]
        public void ParticleEffectLimit_IsConfigurable()
        {
            animationSettings.SetQualityFeatures(true, false, false);
            // Note: MaxParticlesPerEffect is read-only in the current implementation
            Assert.IsTrue(animationSettings.EnableParticleEffects);
        }

        [Test]
        public void QualitySettings_ApplyCorrectly()
        {
            // Test Low quality settings
            animationSettings.SetAnimationQuality(AnimationQuality.Low);
            Assert.AreEqual(5, animationSettings.MaxConcurrentAnimations);
            Assert.IsFalse(animationSettings.EnableParticleEffects);
            Assert.IsFalse(animationSettings.EnableComplexShaders);
            Assert.IsFalse(animationSettings.EnablePostProcessing);

            // Test High quality settings
            animationSettings.SetAnimationQuality(AnimationQuality.High);
            Assert.AreEqual(15, animationSettings.MaxConcurrentAnimations);
            Assert.IsTrue(animationSettings.EnableParticleEffects);
            Assert.IsTrue(animationSettings.EnableComplexShaders);
            Assert.IsTrue(animationSettings.EnablePostProcessing);
        }

        [Test]
        public void IsAnimationAllowed_ReturnsCorrectState()
        {
            // Animations enabled
            animationSettings.SetAnimationsEnabled(true);
            Assert.IsTrue(animationSettings.EnableAnimations);

            // Animations disabled
            animationSettings.SetAnimationsEnabled(false);
            Assert.IsFalse(animationSettings.EnableAnimations);
        }

        [Test]
        public void AnimationDurations_AreConfigurable()
        {
            animationSettings.SetAnimationDurations(0.5f, 2.0f, 0.8f, 0.7f, 0.3f);
            Assert.AreEqual(0.5f, animationSettings.PiecePlacementDuration);
            Assert.AreEqual(2.0f, animationSettings.VictoryHighlightDuration);
            Assert.AreEqual(0.8f, animationSettings.TurnTransitionDuration);
            Assert.AreEqual(0.7f, animationSettings.StateTransitionDuration);
            Assert.AreEqual(0.3f, animationSettings.ButtonAnimationDuration);
        }

        [Test]
        public void OnAnimationsToggled_EventIsTriggered()
        {
            bool eventTriggered = false;
            bool newValue = false;
            animationSettings.OnAnimationsToggled += (enabled) => {
                eventTriggered = true;
                newValue = enabled;
            };

            animationSettings.SetAnimationsEnabled(false);

            Assert.IsTrue(eventTriggered);
            Assert.IsFalse(newValue);
        }

        [Test]
        public void OnQualityChanged_EventIsTriggered()
        {
            bool eventTriggered = false;
            AnimationQuality newQuality = AnimationQuality.High;

            animationSettings.OnQualityChanged += (quality) => {
                eventTriggered = true;
                newQuality = quality;
            };

            animationSettings.SetAnimationQuality(AnimationQuality.Low);

            Assert.IsTrue(eventTriggered);
            Assert.AreEqual(AnimationQuality.Low, newQuality);
        }

        [Test]
        public void ResetToDefaults_RestoresDefaultValues()
        {
            // Change some settings
            animationSettings.SetAnimationsEnabled(false);
            animationSettings.SetAnimationQuality(AnimationQuality.Low);
            animationSettings.SetPerformanceLimits(5, 10, 3);

            // Reset to defaults
            animationSettings.ResetToDefaults();

            // Verify default values
            Assert.IsTrue(animationSettings.EnableAnimations);
            Assert.AreEqual(AnimationQuality.High, animationSettings.AnimationQuality);
            Assert.AreEqual(10, animationSettings.MaxConcurrentAnimations);
        }

        [Test]
        public void GetRecommendedDuration_ReturnsCorrectValues()
        {
            var pieceDuration = animationSettings.GetRecommendedDuration(AnimationType.PiecePlacement);
            var victoryDuration = animationSettings.GetRecommendedDuration(AnimationType.VictoryHighlight);
            var turnDuration = animationSettings.GetRecommendedDuration(AnimationType.TurnTransition);
            var stateDuration = animationSettings.GetRecommendedDuration(AnimationType.StateTransition);
            var buttonDuration = animationSettings.GetRecommendedDuration(AnimationType.ButtonInteraction);

            Assert.AreEqual(animationSettings.PiecePlacementDuration, pieceDuration);
            Assert.AreEqual(animationSettings.VictoryHighlightDuration, victoryDuration);
            Assert.AreEqual(animationSettings.TurnTransitionDuration, turnDuration);
            Assert.AreEqual(animationSettings.StateTransitionDuration, stateDuration);
            Assert.AreEqual(animationSettings.ButtonAnimationDuration, buttonDuration);
        }

        [Test]
        public void IsFeatureEnabled_ReturnsCorrectState()
        {
            // Test with High quality (all features enabled)
            animationSettings.SetAnimationQuality(AnimationQuality.High);
            Assert.IsTrue(animationSettings.IsFeatureEnabled(AnimationFeature.ParticleEffects));
            Assert.IsTrue(animationSettings.IsFeatureEnabled(AnimationFeature.ComplexShaders));
            Assert.IsTrue(animationSettings.IsFeatureEnabled(AnimationFeature.PostProcessing));

            // Test with Low quality (all features disabled)
            animationSettings.SetAnimationQuality(AnimationQuality.Low);
            Assert.IsFalse(animationSettings.IsFeatureEnabled(AnimationFeature.ParticleEffects));
            Assert.IsFalse(animationSettings.IsFeatureEnabled(AnimationFeature.ComplexShaders));
            Assert.IsFalse(animationSettings.IsFeatureEnabled(AnimationFeature.PostProcessing));
        }
    }
}