using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Gomoku.Audio;

namespace Gomoku.Tests.EditMode
{
    /// <summary>
    /// Unit tests for AudioManager volume control functionality
    /// </summary>
    public class AudioManagerVolumeTests
    {
        private GameObject audioManagerObject;
        private AudioManager audioManager;

        [SetUp]
        public void SetUp()
        {
            audioManagerObject = new GameObject("TestAudioManager");
            audioManager = audioManagerObject.AddComponent<AudioManager>();
        }

        [TearDown]
        public void TearDown()
        {
            if (audioManagerObject != null)
            {
                Object.DestroyImmediate(audioManagerObject);
            }
            
            // Clear PlayerPrefs after each test
            PlayerPrefs.DeleteAll();
        }

        [Test]
        public void SetMasterVolume_ClampsValueBetweenZeroAndOne()
        {
            // Arrange
            float validVolume = 0.5f;

            // Act
            audioManager.SetMasterVolume(validVolume);
            var volumes = audioManager.GetVolumeLevels();

            // Assert
            Assert.AreEqual(validVolume, volumes.master, "Master volume should be set correctly");
        }

        [Test]
        public void SetMasterVolume_ClampsInvalidValues()
        {
            // Arrange & Act
            audioManager.SetMasterVolume(-0.5f); // Should be clamped to 0
            var volumesAfterNegative = audioManager.GetVolumeLevels();

            audioManager.SetMasterVolume(1.5f); // Should be clamped to 1
            var volumesAfterExcessive = audioManager.GetVolumeLevels();

            // Assert
            Assert.AreEqual(0f, volumesAfterNegative.master, "Negative volume should be clamped to 0");
            Assert.AreEqual(1f, volumesAfterExcessive.master, "Excessive volume should be clamped to 1");
        }

        [Test]
        public void SetMusicVolume_WorksCorrectly()
        {
            // Arrange
            float testVolume = 0.7f;

            // Act
            audioManager.SetMusicVolume(testVolume);
            var volumes = audioManager.GetVolumeLevels();

            // Assert
            Assert.AreEqual(testVolume, volumes.music, "Music volume should be set correctly");
        }

        [Test]
        public void SetSFXVolume_WorksCorrectly()
        {
            // Arrange
            float testVolume = 0.9f;

            // Act
            audioManager.SetSFXVolume(testVolume);
            var volumes = audioManager.GetVolumeLevels();

            // Assert
            Assert.AreEqual(testVolume, volumes.sfx, "SFX volume should be set correctly");
        }

        [Test]
        public void SetUIVolume_WorksCorrectly()
        {
            // Arrange
            float testVolume = 0.6f;

            // Act
            audioManager.SetUIVolume(testVolume);
            var volumes = audioManager.GetVolumeLevels();

            // Assert
            Assert.AreEqual(testVolume, volumes.ui, "UI volume should be set correctly");
        }

        [Test]
        public void ToggleMasterMute_ChangesMuteState()
        {
            // Arrange
            var initialMuteStates = audioManager.GetMuteStates();

            // Act
            audioManager.ToggleMasterMute();
            var afterToggle = audioManager.GetMuteStates();

            // Assert
            Assert.AreNotEqual(initialMuteStates.master, afterToggle.master, "Master mute state should change");
        }

        [Test]
        public void ToggleMusicMute_ChangesMuteState()
        {
            // Arrange
            var initialMuteStates = audioManager.GetMuteStates();

            // Act
            audioManager.ToggleMusicMute();
            var afterToggle = audioManager.GetMuteStates();

            // Assert
            Assert.AreNotEqual(initialMuteStates.music, afterToggle.music, "Music mute state should change");
        }

        [Test]
        public void ToggleSFXMute_ChangesMuteState()
        {
            // Arrange
            var initialMuteStates = audioManager.GetMuteStates();

            // Act
            audioManager.ToggleSFXMute();
            var afterToggle = audioManager.GetMuteStates();

            // Assert
            Assert.AreNotEqual(initialMuteStates.sfx, afterToggle.sfx, "SFX mute state should change");
        }

        [Test]
        public void ToggleUIMute_ChangesMuteState()
        {
            // Arrange
            var initialMuteStates = audioManager.GetMuteStates();

            // Act
            audioManager.ToggleUIMute();
            var afterToggle = audioManager.GetMuteStates();

            // Assert
            Assert.AreNotEqual(initialMuteStates.ui, afterToggle.ui, "UI mute state should change");
        }

        [Test]
        public void GetVolumeLevels_ReturnsAllVolumes()
        {
            // Arrange
            audioManager.SetMasterVolume(0.8f);
            audioManager.SetMusicVolume(0.6f);
            audioManager.SetSFXVolume(0.9f);
            audioManager.SetUIVolume(0.7f);

            // Act
            var volumes = audioManager.GetVolumeLevels();

            // Assert
            Assert.AreEqual(0.8f, volumes.master, "Master volume should be correct");
            Assert.AreEqual(0.6f, volumes.music, "Music volume should be correct");
            Assert.AreEqual(0.9f, volumes.sfx, "SFX volume should be correct");
            Assert.AreEqual(0.7f, volumes.ui, "UI volume should be correct");
        }

        [Test]
        public void GetMuteStates_ReturnsAllMuteStates()
        {
            // Arrange
            audioManager.ToggleMasterMute();
            audioManager.ToggleMusicMute();

            // Act
            var muteStates = audioManager.GetMuteStates();

            // Assert
            Assert.IsTrue(muteStates.master, "Master should be muted");
            Assert.IsTrue(muteStates.music, "Music should be muted");
            Assert.IsFalse(muteStates.sfx, "SFX should not be muted");
            Assert.IsFalse(muteStates.ui, "UI should not be muted");
        }

        [Test]
        public void ResetVolumeSettings_ResetsToDefaults()
        {
            // Arrange
            audioManager.SetMasterVolume(0.3f);
            audioManager.SetMusicVolume(0.2f);
            audioManager.SetSFXVolume(0.4f);
            audioManager.SetUIVolume(0.5f);
            audioManager.ToggleMasterMute();
            audioManager.ToggleMusicMute();

            // Act
            audioManager.ResetVolumeSettings();
            var volumes = audioManager.GetVolumeLevels();
            var muteStates = audioManager.GetMuteStates();

            // Assert
            Assert.AreEqual(1.0f, volumes.master, "Master volume should reset to 1.0");
            Assert.AreEqual(0.8f, volumes.music, "Music volume should reset to 0.8");
            Assert.AreEqual(1.0f, volumes.sfx, "SFX volume should reset to 1.0");
            Assert.AreEqual(0.9f, volumes.ui, "UI volume should reset to 0.9");

            Assert.IsFalse(muteStates.master, "Master should not be muted");
            Assert.IsFalse(muteStates.music, "Music should not be muted");
            Assert.IsFalse(muteStates.sfx, "SFX should not be muted");
            Assert.IsFalse(muteStates.ui, "UI should not be muted");
        }

        [Test]
        public void VolumeSettings_PersistAcrossAudioManagerInstances()
        {
            // Arrange
            float testMasterVolume = 0.75f;
            float testMusicVolume = 0.55f;

            // Act
            audioManager.SetMasterVolume(testMasterVolume);
            audioManager.SetMusicVolume(testMusicVolume);

            // Create new AudioManager instance
            GameObject newAudioManagerObject = new GameObject("NewAudioManager");
            AudioManager newAudioManager = newAudioManagerObject.AddComponent<AudioManager>();

            // Assert
            var newVolumes = newAudioManager.GetVolumeLevels();
            Assert.AreEqual(testMasterVolume, newVolumes.master, "Master volume should persist");
            Assert.AreEqual(testMusicVolume, newVolumes.music, "Music volume should persist");

            // Cleanup
            Object.DestroyImmediate(newAudioManagerObject);
        }
    }
}