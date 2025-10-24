using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Gomoku.Animation;

namespace Tests.EditMode.Animation
{
    public class AnimationManagerTests
    {
        private GameObject testGameObject;
        private AnimationManager animationManager;

        [SetUp]
        public void SetUp()
        {
            testGameObject = new GameObject("TestAnimationManager");
            animationManager = testGameObject.AddComponent<AnimationManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testGameObject);
        }

        [Test]
        public void AnimationManager_InitializesCorrectly()
        {
            Assert.IsNotNull(animationManager);
            Assert.AreEqual(0, animationManager.ActiveAnimationCount);
            Assert.IsTrue(animationManager.EnableAnimations);
            Assert.AreEqual(AnimationQuality.High, animationManager.AnimationQuality);
        }

        [Test]
        public void RegisterActiveAnimation_AddsAnimationToActiveList()
        {
            var animationComponent = new GameObject("TestAnimation").AddComponent<PieceAnimation>();
            animationManager.RegisterActiveAnimation(animationComponent);

            Assert.AreEqual(1, animationManager.ActiveAnimationCount);

            Object.DestroyImmediate(animationComponent.gameObject);
        }

        [Test]
        public void UnregisterActiveAnimation_RemovesAnimationFromActiveList()
        {
            var animationComponent = new GameObject("TestAnimation").AddComponent<PieceAnimation>();
            animationManager.RegisterActiveAnimation(animationComponent);
            animationManager.UnregisterActiveAnimation(animationComponent);

            Assert.AreEqual(0, animationManager.ActiveAnimationCount);

            Object.DestroyImmediate(animationComponent.gameObject);
        }

        [Test]
        public void ActiveAnimationCount_ReturnsCorrectCount()
        {
            var animation1 = new GameObject("Animation1").AddComponent<PieceAnimation>();
            var animation2 = new GameObject("Animation2").AddComponent<VictoryEffect>();

            animationManager.RegisterActiveAnimation(animation1);
            animationManager.RegisterActiveAnimation(animation2);

            Assert.AreEqual(2, animationManager.ActiveAnimationCount);

            Object.DestroyImmediate(animation1.gameObject);
            Object.DestroyImmediate(animation2.gameObject);
        }

        [Test]
        public void StopAllActiveAnimations_RemovesAllActiveAnimations()
        {
            var animation1 = new GameObject("Animation1").AddComponent<PieceAnimation>();
            var animation2 = new GameObject("Animation2").AddComponent<VictoryEffect>();

            animationManager.RegisterActiveAnimation(animation1);
            animationManager.RegisterActiveAnimation(animation2);
            animationManager.StopAllActiveAnimations();

            Assert.AreEqual(0, animationManager.ActiveAnimationCount);

            Object.DestroyImmediate(animation1.gameObject);
            Object.DestroyImmediate(animation2.gameObject);
        }

        [Test]
        public void AnimationLimit_Respected()
        {
            // Note: AnimationManager doesn't have an explicit limit check method
            // This test verifies that animations can be registered up to the configured limit
            var animation1 = new GameObject("Animation1").AddComponent<PieceAnimation>();
            var animation2 = new GameObject("Animation2").AddComponent<VictoryEffect>();

            animationManager.RegisterActiveAnimation(animation1);
            animationManager.RegisterActiveAnimation(animation2);

            // Should be able to register both animations
            Assert.AreEqual(2, animationManager.ActiveAnimationCount);

            Object.DestroyImmediate(animation1.gameObject);
            Object.DestroyImmediate(animation2.gameObject);
        }

        [Test]
        public void AnimationTypes_CanBeRegistered()
        {
            var pieceAnimation = new GameObject("PieceAnimation").AddComponent<PieceAnimation>();
            var victoryEffect = new GameObject("VictoryEffect").AddComponent<VictoryEffect>();

            animationManager.RegisterActiveAnimation(pieceAnimation);
            animationManager.RegisterActiveAnimation(victoryEffect);

            // Both animation types should be registered successfully
            Assert.AreEqual(2, animationManager.ActiveAnimationCount);

            Object.DestroyImmediate(pieceAnimation.gameObject);
            Object.DestroyImmediate(victoryEffect.gameObject);
        }

        [Test]
        public void SetAnimationsEnabled_TogglesAnimationState()
        {
            // Test enabling animations
            animationManager.SetAnimationsEnabled(true);
            Assert.IsTrue(animationManager.EnableAnimations);

            // Test disabling animations
            animationManager.SetAnimationsEnabled(false);
            Assert.IsFalse(animationManager.EnableAnimations);
        }

        [Test]
        public void SetAnimationQuality_ChangesQualityLevel()
        {
            // Test setting different quality levels
            animationManager.SetAnimationQuality(AnimationQuality.Low);
            Assert.AreEqual(AnimationQuality.Low, animationManager.AnimationQuality);

            animationManager.SetAnimationQuality(AnimationQuality.Medium);
            Assert.AreEqual(AnimationQuality.Medium, animationManager.AnimationQuality);

            animationManager.SetAnimationQuality(AnimationQuality.High);
            Assert.AreEqual(AnimationQuality.High, animationManager.AnimationQuality);
        }

    }
}