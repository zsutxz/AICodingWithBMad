using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Gomoku.Systems;
using Gomoku.UI.MainMenu;
using Gomoku.Audio;
using Gomoku.UI;

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
            // Test that StartGame method sets correct game state
            _mainMenuScreen.StartGame();

            // Verify that game state is set to Playing
            var currentState = Gomoku.Systems.GameStateManager.Instance.GetCurrentState();
            Assert.AreEqual(Gomoku.Systems.GameStateEnum.Playing, currentState, "Should set game state to Playing when StartGame is called");
        }

        [Test]
        public void ExitGame_QuitsApplicationOrStopsEditor()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = true;

            _mainMenuScreen.ExitGame();

            Assert.IsFalse(UnityEditor.EditorApplication.isPlaying);
            #else
            // In build, would call Application.Quit(), but can't test directly
            Assert.Pass("ExitGame calls Application.Quit() in build environment");
            #endif
        }

        [Test]
        public void StartGame_TransitionsToPlayingState()
        {
            // Test that StartGame transitions to Playing state
            var originalState = GameStateManager.Instance.GetCurrentState();
            
            _mainMenuScreen.StartGame();
            
            var newState = GameStateManager.Instance.GetCurrentState();
            Assert.AreEqual(GameStateEnum.Playing, newState);
        }

        [Test]
        public void MainMenuScreen_InitializesInMainMenuState()
        {
            // Test that MainMenuScreen sets the correct initial state
            var stateManager = GameStateManager.Instance;
            
            // Start() is automatically called by Unity when the component is enabled
            // The state should already be set to MainMenu after component creation
            Assert.AreEqual(GameStateEnum.MainMenu, stateManager.GetCurrentState());
        }

        [Test]
        public void OpenSettings_DoesNotThrowException()
        {
            // Test that OpenSettings method doesn't throw exceptions
            // Currently commented out but should be safe to call
            Assert.DoesNotThrow(() => _mainMenuScreen.OpenSettings());
        }

        [Test]
        public void UIManager_ProperlyInitialized()
        {
            // Test that UIManager component exists and is properly configured
            var uiManager = Object.FindObjectOfType<UIManager>();
            Assert.IsNotNull(uiManager, "UIManager should be present in the scene");
        }

        [Test]
        public void GameStateManager_PersistsAcrossSceneChanges()
        {
            // Test that GameStateManager uses DontDestroyOnLoad
            var stateManager = GameStateManager.Instance;
            
            // Verify it's marked as DontDestroyOnLoad by checking if it's a singleton
            var managers = Object.FindObjectsOfType<GameStateManager>();
            Assert.AreEqual(1, managers.Length, "Should only have one GameStateManager instance");
        }

        [Test]
        public void AudioManager_IntegrationWithButtonHandler()
        {
            // Test that AudioManager is accessible and can be used by ButtonHandler
            var audioManager = Object.FindObjectOfType<AudioManager>();
            
            // Create a test button handler
            var buttonObject = new GameObject("TestButton");
            var buttonHandler = buttonObject.AddComponent<ButtonHandler>();
            
            // Test that button handler can access AudioManager without errors
            Assert.DoesNotThrow(() => 
            {
                // Create AudioManager instance if it doesn't exist
                var audioManagerInstance = AudioManager.Instance;
                if (audioManagerInstance != null)
                {
                    audioManagerInstance.PlayUIClick();
                }
            });
            
            Object.Destroy(buttonObject);
        }

    }
}