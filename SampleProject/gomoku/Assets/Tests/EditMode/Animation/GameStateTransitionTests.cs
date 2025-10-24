using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Gomoku.Animation;

namespace Tests.EditMode.Animation
{
    public class GameStateTransitionTests
    {
        private GameObject testGameObject;
        private GameStateTransition gameStateTransition;

        [SetUp]
        public void SetUp()
        {
            testGameObject = new GameObject("TestGameStateTransition");
            gameStateTransition = testGameObject.AddComponent<GameStateTransition>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testGameObject);
        }

        [Test]
        public void GameStateTransition_InitializesCorrectly()
        {
            Assert.IsNotNull(gameStateTransition);
            Assert.IsFalse(gameStateTransition.IsTransitioning);
        }

        [Test]
        public void StartFadeTransition_SetsTransitioningState()
        {
            gameStateTransition.StartFadeTransition(true);
            Assert.IsTrue(gameStateTransition.IsTransitioning);
        }

        [Test]
        public void StopCurrentTransition_ResetsTransitioningState()
        {
            gameStateTransition.StartFadeTransition(true);
            gameStateTransition.StopCurrentTransition();
            Assert.IsFalse(gameStateTransition.IsTransitioning);
        }

        [Test]
        public void TransitionDuration_IsConfigurable()
        {
            var expectedDuration = 0.5f;
            gameStateTransition.SetFadeDuration(expectedDuration);
            Assert.AreEqual(expectedDuration, gameStateTransition.FadeDuration);
        }

        [Test]
        public void FadeColor_CanBeSet()
        {
            var expectedColor = Color.red;
            gameStateTransition.SetFadeColor(expectedColor);
            // Note: There's no getter for fade color in the implementation
            // This test verifies the method exists and doesn't throw errors
        }

        [Test]
        public void OnTransitionStarted_EventIsTriggered()
        {
            bool eventTriggered = false;
            gameStateTransition.OnTransitionStart += (transition) => eventTriggered = true;

            gameStateTransition.StartFadeTransition(true);

            Assert.IsTrue(eventTriggered);
        }

        [Test]
        public void OnTransitionCompleted_EventIsTriggered()
        {
            bool eventTriggered = false;
            gameStateTransition.OnTransitionComplete += (transition) => eventTriggered = true;

            gameStateTransition.StartFadeTransition(true);
            gameStateTransition.StopCurrentTransition();

            Assert.IsTrue(eventTriggered);
        }

        [Test]
        public void IsTransitioning_ReturnsCorrectState()
        {
            Assert.IsFalse(gameStateTransition.IsTransitioning);

            gameStateTransition.StartFadeTransition(true);
            Assert.IsTrue(gameStateTransition.IsTransitioning);

            gameStateTransition.StopCurrentTransition();
            Assert.IsFalse(gameStateTransition.IsTransitioning);
        }

        [Test]
        public void StopCurrentTransition_ReturnsToInitialState()
        {
            gameStateTransition.StartFadeTransition(true);
            gameStateTransition.StopCurrentTransition();

            Assert.IsFalse(gameStateTransition.IsTransitioning);
        }

        [UnityTest]
        public IEnumerator FadeTransition_CompletesSuccessfully()
        {
            gameStateTransition.StartFadeTransition(true);

            yield return new WaitForSeconds(gameStateTransition.FadeDuration + 0.1f);

            Assert.IsFalse(gameStateTransition.IsTransitioning);
        }
    }
}