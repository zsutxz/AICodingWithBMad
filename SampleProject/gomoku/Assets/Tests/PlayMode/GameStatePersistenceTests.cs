using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Gomoku;
using System.Collections;

namespace Tests
{
    [TestFixture]
    public class GameStatePersistenceTests
    {
        private GameStateManager _gameStateManager;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            // Load main menu scene for testing
            yield return SceneManager.LoadSceneAsync("MainMenu");

            // Ensure GameStateManager exists and is persistent
            _gameStateManager = GameObject.FindObjectOfType<GameStateManager>();
            Assert.IsNotNull(_gameStateManager, "GameStateManager should exist in the scene");
        }

        [UnityTest]
        public IEnumerator GameState_PersistsThroughSceneTransitions()
        {
            // Start in MainMenu state
            _gameStateManager.SetState(GameStateEnum.MainMenu);
            Assert.AreEqual(GameStateEnum.MainMenu, _gameStateManager.GetCurrentState(),
                "Should start in MainMenu state");

            // Transition to GameScene
            _gameStateManager.SetState(GameStateEnum.Playing);
            yield return SceneManager.LoadSceneAsync("GameScene");

            // Check state after transition
            Assert.AreEqual(GameStateEnum.Playing, _gameStateManager.GetCurrentState(),
                "GameState should be Playing after transition to GameScene");

            // Transition back to MainMenu
            _gameStateManager.SetState(GameStateEnum.MainMenu);
            yield return SceneManager.LoadSceneAsync("MainMenu");

            // Check state persists after returning
            Assert.AreEqual(GameStateEnum.MainMenu, _gameStateManager.GetCurrentState(),
                "GameState should return to MainMenu after transition");

            yield break;
        }

        [UnityTest]
        public IEnumerator GameStateManager_PersistsAcrossScenes()
        {
            // Get original reference
            var originalManager = _gameStateManager;
            Assert.IsNotNull(originalManager, "Original GameStateManager should exist");

            // Transition to another scene
            yield return SceneManager.LoadSceneAsync("GameScene");

            // Check manager still exists
            var newManager = GameObject.FindObjectOfType<GameStateManager>();
            Assert.IsNotNull(newManager, "GameStateManager should persist in new scene");
            Assert.AreEqual(originalManager, newManager,
                "Same GameStateManager instance should persist across scenes");

            // Check state preservation
            Assert.AreEqual(GameStateEnum.Playing, newManager.GetCurrentState(),
                "GameState should be preserved when transitioning to GameScene");

            yield break;
        }
    }
}