using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Gomoku.Animation;

namespace Tests.EditMode.Animation
{
    public class AnimationIntegrationTests
    {
        private GameObject testManagerObject;
        private AnimationManager animationManager;
        private AnimationSettings animationSettings;
        private AnimationPerformanceMonitor performanceMonitor;

        [SetUp]
        public void SetUp()
        {
            testManagerObject = new GameObject("TestAnimationSystem");
            animationManager = testManagerObject.AddComponent<AnimationManager>();
            // AnimationSettings is a ScriptableObject, so we use CreateInstance instead of AddComponent
            animationSettings = ScriptableObject.CreateInstance<AnimationSettings>();
            performanceMonitor = testManagerObject.AddComponent<AnimationPerformanceMonitor>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testManagerObject);
            Object.DestroyImmediate(animationSettings);
        }

        [Test]
        public void AnimationSystem_ComponentsWorkTogether()
        {
            // Verify all components are properly initialized
            Assert.IsNotNull(animationManager);
            Assert.IsNotNull(animationSettings);
            Assert.IsNotNull(performanceMonitor);

            // Verify initial state
            Assert.IsTrue(animationSettings.EnableAnimations);
            Assert.AreEqual(0, animationManager.ActiveAnimationCount);
            Assert.IsTrue(performanceMonitor.EnablePerformanceMonitoring);
        }

        [Test]
        public void AnimationRegistration_UpdatesAnimationManager()
        {
            var pieceAnimation = new GameObject("PieceAnimation").AddComponent<PieceAnimation>();

            // Register animation
            animationManager.RegisterActiveAnimation(pieceAnimation);

            // Verify animation manager is updated
            Assert.AreEqual(1, animationManager.ActiveAnimationCount);

            Object.DestroyImmediate(pieceAnimation.gameObject);
        }

        [Test]
        public void AnimationUnregistration_UpdatesAnimationManager()
        {
            var pieceAnimation = new GameObject("PieceAnimation").AddComponent<PieceAnimation>();

            // Register then unregister animation
            animationManager.RegisterActiveAnimation(pieceAnimation);
            animationManager.UnregisterActiveAnimation(pieceAnimation);

            // Verify animation manager is updated
            Assert.AreEqual(0, animationManager.ActiveAnimationCount);

            Object.DestroyImmediate(pieceAnimation.gameObject);
        }

        [Test]
        public void AnimationSettings_ControlAnimationBehavior()
        {
            var pieceAnimation = new GameObject("PieceAnimation").AddComponent<PieceAnimation>();

            // Disable animations
            animationSettings.SetAnimationsEnabled(false);

            // Verify animations are disabled
            Assert.IsFalse(animationSettings.EnableAnimations);

            Object.DestroyImmediate(pieceAnimation.gameObject);
        }

        [Test]
        public void PerformanceMonitor_TracksAnimationPerformance()
        {
            var animation1 = new GameObject("Animation1").AddComponent<PieceAnimation>();

            // Register animation and record performance
            animationManager.RegisterActiveAnimation(animation1);
            performanceMonitor.RecordAnimationPerformance("PieceAnimation", 0.1f);

            // Verify performance data is recorded
            var performanceData = performanceMonitor.GetAnimationPerformanceData("PieceAnimation");
            Assert.IsNotNull(performanceData);
            Assert.AreEqual(1, performanceData.ExecutionCount);

            Object.DestroyImmediate(animation1.gameObject);
        }

        [Test]
        public void AnimationEvents_PropagateThroughAnimationManager()
        {
            bool managerEventTriggered = false;

            // Set up event handler

            animationManager.OnAnimationRegistered += (animation) => managerEventTriggered = true;

            // Trigger animation registration
            var animationComponent = new GameObject("TestAnimation").AddComponent<PieceAnimation>();
            animationManager.RegisterActiveAnimation(animationComponent);

            // Verify event was triggered
            Assert.IsTrue(managerEventTriggered);

            Object.DestroyImmediate(animationComponent.gameObject);
        }

        [Test]
        public void QualitySettings_ControlAnimationBehavior()
        {
            // Test animation settings configuration
            animationSettings.SetAnimationsEnabled(true);
            Assert.IsTrue(animationSettings.EnableAnimations);

            animationSettings.SetAnimationsEnabled(false);
            Assert.IsFalse(animationSettings.EnableAnimations);
        }

        [Test]
        public void AnimationManager_HandlesMultipleAnimations()
        {
            var pieceAnimation = new GameObject("PieceAnimation").AddComponent<PieceAnimation>();
            var victoryEffect = new GameObject("VictoryEffect").AddComponent<VictoryEffect>();

            // Register multiple animations
            animationManager.RegisterActiveAnimation(pieceAnimation);
            animationManager.RegisterActiveAnimation(victoryEffect);

            // Verify multiple animations are tracked
            Assert.AreEqual(2, animationManager.ActiveAnimationCount);

            Object.DestroyImmediate(pieceAnimation.gameObject);
            Object.DestroyImmediate(victoryEffect.gameObject);
        }

        [Test]
        public void AnimationPerformanceMonitor_TracksFrameTime()
        {
            // Record frame time
            performanceMonitor.RecordFrameTime(0.016f, 0.005f);

            // Verify frame time data is recorded
            Assert.Greater(performanceMonitor.AverageFrameTime, 0f);
            Assert.Greater(performanceMonitor.CurrentFrameRate, 0f);
        }

        [Test]
        public void PerformanceMonitor_ProvidesPerformanceData()
        {
            // Record some performance data
            performanceMonitor.RecordAnimationPerformance("TestAnimation", 0.1f);
            performanceMonitor.RecordFrameTime(0.016f, 0.005f);

            // Get performance data
            var performanceData = performanceMonitor.GetAllPerformanceData();
            Assert.IsNotNull(performanceData);
        }

        [Test]
        public void AnimationSystem_HandlesMultipleAnimationTypes()
        {
            var pieceAnimation = new GameObject("PieceAnimation").AddComponent<PieceAnimation>();
            var victoryEffect = new GameObject("VictoryEffect").AddComponent<VictoryEffect>();
            var turnTransition = new GameObject("TurnTransition").AddComponent<TurnTransition>();

            // Register multiple animation types
            animationManager.RegisterActiveAnimation(pieceAnimation);
            animationManager.RegisterActiveAnimation(victoryEffect);
            animationManager.RegisterActiveAnimation(turnTransition);

            // Verify all animations are tracked
            Assert.AreEqual(3, animationManager.ActiveAnimationCount);

            Object.DestroyImmediate(pieceAnimation.gameObject);
            Object.DestroyImmediate(victoryEffect.gameObject);
            Object.DestroyImmediate(turnTransition.gameObject);
        }

        [UnityTest]
        public IEnumerator AnimationSystem_HandlesRealTimeUpdates()
        {
            var pieceAnimation = new GameObject("PieceAnimation").AddComponent<PieceAnimation>();
            animationManager.RegisterActiveAnimation(pieceAnimation);

            // Wait a frame
            yield return null;

            // Verify system is still functioning
            Assert.AreEqual(1, animationManager.ActiveAnimationCount);

            Object.DestroyImmediate(pieceAnimation.gameObject);
        }

        [Test]
        public void AnimationSystem_ValidatesSettingsCorrectly()
        {
            // Test animation settings functionality
            animationSettings.SetAnimationsEnabled(true);
            Assert.IsTrue(animationSettings.EnableAnimations);

            animationSettings.SetAnimationsEnabled(false);
            Assert.IsFalse(animationSettings.EnableAnimations);
        }
    }
}