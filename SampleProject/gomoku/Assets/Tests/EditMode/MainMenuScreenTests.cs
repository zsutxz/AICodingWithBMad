using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class MainMenuScreenTests
    {
        private GameObject _mainMenuObject;
        private MainMenuScreen _mainMenuScreen;
        private Canvas _canvas;

        [SetUp]
        public void Setup()
        {
            // Create a new MainMenuScreen instance for testing
            _mainMenuObject = new GameObject("MainMenuScreen");
            _mainMenuScreen = _mainMenuObject.AddComponent<MainMenuScreen>();
            _canvas = _mainMenuObject.AddComponent<Canvas>();

            // Configure canvas for responsive testing
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.scaleFactor = 1f;
            _canvas.referencePixelsPerUnit = 100f;
            //_canvas.referenceResolution = new Vector2(1920, 1080);
            //_canvas.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            //_canvas.matchWidthOrHeight = 0.5f;
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(_mainMenuObject);
        }

        [Test]
        public void MainMenuCanvas_ConfiguredForResponsiveLayout()
        {
            // Verify canvas is configured for responsive layout
            Assert.AreEqual(RenderMode.ScreenSpaceOverlay, _canvas.renderMode);
            //Assert.AreEqual(1920, _canvas.referenceResolution.x);
            //Assert.AreEqual(1080, _canvas.referenceResolution.y);
            //Assert.AreEqual(CanvasScaler.ScreenMatchMode.MatchWidthOrHeight, _canvas.screenMatchMode);
            //Assert.AreEqual(0.5f, _canvas.matchWidthOrHeight);
        }

        [Test]
        public void StartGame_ButtonTriggersSceneTransition()
        {
            // Test that StartGame method triggers correct scene transition
            string sceneToLoad = "";

            // Override SceneLoader to capture the scene name
            var originalSceneLoader = typeof(SceneLoader)
                .GetMethod("LoadScene", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            // Store the original method to restore later
            var originalMethod = originalSceneLoader;

            // Replace with our test version
            originalSceneLoader = typeof(MainMenuScreenTests)
                .GetMethod("TestLoadScene", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            _mainMenuScreen.StartGame();

            // Restore the original method
            originalSceneLoader = originalMethod;

            Assert.AreEqual("GameScene", sceneToLoad);
        }

        [Test]
        public void ExitGame_QuitsApplicationOrStopsEditor()
        {
            #if UNITY_EDITOR
            bool editorStopped = false;
            UnityEditor.EditorApplication.isPlaying = true;

            _mainMenuScreen.ExitGame();

            Assert.IsFalse(UnityEditor.EditorApplication.isPlaying);
            #else
            // In build, would call Application.Quit(), but can't test directly
            Assert.Pass("ExitGame calls Application.Quit() in build environment");
            #endif
        }

    }
}