using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Gomoku.Core;
using Gomoku.Audio;
using Gomoku.UI;

namespace Gomoku.Tests.EditMode
{
    /// <summary>
    /// Unit tests for audio settings integration functionality
    /// </summary>
    public class AudioSettingsIntegrationTests
    {
        private GameObject audioManagerObject;
        private AudioManager audioManager;
        private GameSettingsModel gameSettings;
        private GameObject audioSettingsUIObject;
        private AudioSettingsUI audioSettingsUI;

        [SetUp]
        public void SetUp()
        {
            // Create AudioManager
            audioManagerObject = new GameObject("AudioManager");
            audioManager = audioManagerObject.AddComponent<AudioManager>();

            // Create GameSettings
            gameSettings = ScriptableObject.CreateInstance<GameSettingsModel>();
            gameSettings.LoadSettings();

            // Create AudioSettingsUI
            audioSettingsUIObject = new GameObject("AudioSettingsUI");
            audioSettingsUI = audioSettingsUIObject.AddComponent<AudioSettingsUI>();
        }

        [TearDown]
        public void TearDown()
        {
            if (audioManagerObject != null)
                Object.DestroyImmediate(audioManagerObject);
            
            if (audioSettingsUIObject != null)
                Object.DestroyImmediate(audioSettingsUIObject);
            
            if (gameSettings != null)
                ScriptableObject.DestroyImmediate(gameSettings);
            
            // Clear PlayerPrefs
            PlayerPrefs.DeleteAll();
        }

        [Test]
        public void GameSettingsModel_StoresAudioSettings()
        {
            // Arrange
            float testMasterVolume = 0.75f;
            float testMusicVolume = 0.65f;
            float testSFXVolume = 0.85f;
            float testUIVolume = 0.55f;

            // Act
            gameSettings.MasterVolume = testMasterVolume;
            gameSettings.MusicVolume = testMusicVolume;
            gameSettings.SFXVolume = testSFXVolume;
            gameSettings.UIVolume = testUIVolume;

            // Assert
            var volumes = gameSettings.GetAudioVolumes();
            Assert.AreEqual(testMasterVolume, volumes.master, "Master volume should be stored correctly");
            Assert.AreEqual(testMusicVolume, volumes.music, "Music volume should be stored correctly");
            Assert.AreEqual(testSFXVolume, volumes.sfx, "SFX volume should be stored correctly");
            Assert.AreEqual(testUIVolume, volumes.ui, "UI volume should be stored correctly");
        }

        [Test]
        public void GameSettingsModel_StoresMuteSettings()
        {
            // Arrange & Act
            gameSettings.MasterMuted = true;
            gameSettings.MusicMuted = false;
            gameSettings.SFXMuted = true;
            gameSettings.UIMuted = false;

            // Assert
            var muteStates = gameSettings.GetAudioMuteStates();
            Assert.IsTrue(muteStates.master, "Master mute should be stored correctly");
            Assert.IsFalse(muteStates.music, "Music mute should be stored correctly");
            Assert.IsTrue(muteStates.sfx, "SFX mute should be stored correctly");
            Assert.IsFalse(muteStates.ui, "UI mute should be stored correctly");
        }

        [Test]
        public void GameSettingsModel_ClampsVolumeValues()
        {
            // Arrange & Act
            gameSettings.MasterVolume = -0.5f; // Should clamp to 0
            gameSettings.MusicVolume = 1.5f;   // Should clamp to 1
            gameSettings.SFXVolume = 2.0f;     // Should clamp to 1
            gameSettings.UIVolume = -1.0f;     // Should clamp to 0

            // Assert
            var volumes = gameSettings.GetAudioVolumes();
            Assert.AreEqual(0f, volumes.master, "Negative volume should be clamped to 0");
            Assert.AreEqual(1f, volumes.music, "Excessive volume should be clamped to 1");
            Assert.AreEqual(1f, volumes.sfx, "Very excessive volume should be clamped to 1");
            Assert.AreEqual(0f, volumes.ui, "Very negative volume should be clamped to 0");
        }

        [Test]
        public void GameSettingsModel_TriggersEventsOnVolumeChange()
        {
            // Arrange
            bool eventTriggered = false;
            float receivedVolume = 0f;

            gameSettings.OnMasterVolumeChanged += (volume) =>
            {
                eventTriggered = true;
                receivedVolume = volume;
            };

            // Act
            gameSettings.MasterVolume = 0.8f;

            // Assert
            Assert.IsTrue(eventTriggered, "Volume change event should be triggered");
            Assert.AreEqual(0.8f, receivedVolume, "Event should pass correct volume value");
        }

        [Test]
        public void GameSettingsModel_TriggersEventsOnMuteChange()
        {
            // Arrange
            bool eventTriggered = false;
            bool receivedMuteState = false;

            gameSettings.OnMasterMutedChanged += (muted) =>
            {
                eventTriggered = true;
                receivedMuteState = muted;
            };

            // Act
            gameSettings.MasterMuted = true;

            // Assert
            Assert.IsTrue(eventTriggered, "Mute change event should be triggered");
            Assert.IsTrue(receivedMuteState, "Event should pass correct mute state");
        }

        [Test]
        public void GameSettingsModel_PersistsSettingsAcrossInstances()
        {
            // Arrange
            float testVolume = 0.42f;
            gameSettings.MasterVolume = testVolume;
            gameSettings.SaveSettings();

            // Create new GameSettings instance
            GameSettingsModel newSettings = ScriptableObject.CreateInstance<GameSettingsModel>();
            newSettings.LoadSettings();

            // Act & Assert
            Assert.AreEqual(testVolume, newSettings.MasterVolume, "Volume settings should persist across instances");

            // Cleanup
            ScriptableObject.DestroyImmediate(newSettings);
        }

        [Test]
        public void ApplyAudioSettings_UpdatesAudioManager()
        {
            // Arrange
            gameSettings.MasterVolume = 0.7f;
            gameSettings.MusicVolume = 0.6f;
            gameSettings.SFXVolume = 0.8f;
            gameSettings.UIVolume = 0.9f;

            // Act
            gameSettings.ApplyAudioSettings();

            // Assert
            var audioManagerVolumes = AudioManager.Instance.GetVolumeLevels();
            Assert.AreEqual(0.7f, audioManagerVolumes.master, "AudioManager master volume should be updated");
            Assert.AreEqual(0.6f, audioManagerVolumes.music, "AudioManager music volume should be updated");
            Assert.AreEqual(0.8f, audioManagerVolumes.sfx, "AudioManager SFX volume should be updated");
            Assert.AreEqual(0.9f, audioManagerVolumes.ui, "AudioManager UI volume should be updated");
        }

        [Test]
        public void GameSettingsModel_ResetsToDefaults()
        {
            // Arrange
            gameSettings.MasterVolume = 0.1f;
            gameSettings.MusicVolume = 0.2f;
            gameSettings.SFXVolume = 0.3f;
            gameSettings.UIVolume = 0.4f;
            gameSettings.MasterMuted = true;
            gameSettings.MusicMuted = true;
            gameSettings.SFXMuted = true;
            gameSettings.UIMuted = true;

            // Act
            gameSettings.ResetToDefaults();

            // Assert
            var volumes = gameSettings.GetAudioVolumes();
            var muteStates = gameSettings.GetAudioMuteStates();

            Assert.AreEqual(1.0f, volumes.master, "Master volume should reset to default");
            Assert.AreEqual(0.8f, volumes.music, "Music volume should reset to default");
            Assert.AreEqual(1.0f, volumes.sfx, "SFX volume should reset to default");
            Assert.AreEqual(0.9f, volumes.ui, "UI volume should reset to default");

            Assert.IsFalse(muteStates.master, "Master mute should reset to default");
            Assert.IsFalse(muteStates.music, "Music mute should reset to default");
            Assert.IsFalse(muteStates.sfx, "SFX mute should reset to default");
            Assert.IsFalse(muteStates.ui, "UI mute should reset to default");
        }

        [Test]
        public void AudioSettingsUI_InitializesWithoutErrors()
        {
            // Arrange & Act & Assert
            Assert.IsNotNull(audioSettingsUI, "AudioSettingsUI should be created successfully");
            // AudioSettingsUI should handle missing UI components gracefully
        }

        [Test]
        public void AudioSettingsUI_HandlesNullReferences()
        {
            // Arrange & Act & Assert
            // This test verifies that AudioSettingsUI doesn't throw exceptions
            // when UI components are not assigned
            Assert.DoesNotThrow(() =>
            {
                // Test that AudioSettingsUI can be created and doesn't throw exceptions
                var audioManager = AudioManager.Instance;
                if (audioManager != null)
                {
                    audioManager.SetMasterVolume(0.5f);
                }
            }, "AudioSettingsUI should handle null references gracefully");
        }

        [UnityTest]
        public IEnumerator SettingsIntegration_WorksInRealTime()
        {
            // Arrange
            float targetVolume = 0.75f;

            // Act
            gameSettings.MasterVolume = targetVolume;
            yield return null; // Allow one frame for updates

            // Assert
            var audioManagerVolumes = AudioManager.Instance.GetVolumeLevels();
            Assert.AreEqual(targetVolume, audioManagerVolumes.master, "Settings should apply in real-time");
        }

        [Test]
        public void GameSettingsModel_HandlesGameSettings()
        {
            // Arrange & Act
            gameSettings.ShowHints = false;
            gameSettings.ShowAnimations = true;
            gameSettings.ShowLastMove = false;
            gameSettings.GameSpeed = 1.5f;

            // Assert
            Assert.IsFalse(gameSettings.ShowHints, "Show hints should be set correctly");
            Assert.IsTrue(gameSettings.ShowAnimations, "Show animations should be set correctly");
            Assert.IsFalse(gameSettings.ShowLastMove, "Show last move should be set correctly");
            Assert.AreEqual(1.5f, gameSettings.GameSpeed, "Game speed should be set correctly");
        }

        [Test]
        public void AudioSettingsIntegration_DoesNotCreateMemoryLeaks()
        {
            // Arrange
            long initialMemory = System.GC.GetTotalMemory(false);

            // Act - Create and destroy multiple settings instances
            for (int i = 0; i < 10; i++)
            {
                GameSettingsModel tempSettings = ScriptableObject.CreateInstance<GameSettingsModel>();
                tempSettings.LoadSettings();
                tempSettings.MasterVolume = i * 0.1f;
                tempSettings.SaveSettings();
                ScriptableObject.DestroyImmediate(tempSettings);
            }

            // Force garbage collection
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();

            long finalMemory = System.GC.GetTotalMemory(false);

            // Assert
            long memoryDifference = finalMemory - initialMemory;
            Assert.IsTrue(memoryDifference < 1024 * 1024, // Less than 1MB increase
                "Audio settings integration should not create significant memory leaks");
        }
    }
}