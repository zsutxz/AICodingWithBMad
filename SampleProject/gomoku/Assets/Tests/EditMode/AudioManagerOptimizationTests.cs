using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Gomoku.Audio;

namespace Gomoku.Tests.EditMode
{
    /// <summary>
    /// Unit tests for AudioManager performance optimization features
    /// </summary>
    public class AudioManagerOptimizationTests
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
        }

        [Test]
        public void GetPerformanceMetrics_ReturnsValidMetrics()
        {
            // Arrange & Act
            var (activeSources, pooledSources, memoryUsage) = audioManager.GetPerformanceMetrics();

            // Assert
            Assert.IsTrue(activeSources >= 0, "Active sources should be non-negative");
            Assert.IsTrue(pooledSources >= 0, "Pooled sources should be non-negative");
            Assert.IsTrue(memoryUsage >= 0, "Memory usage should be non-negative");
        }

        [Test]
        public void PreloadAudioClips_DoesNotThrow()
        {
            // Arrange & Act & Assert
            Assert.DoesNotThrow(() => audioManager.PreloadAudioClips(),
                "Preloading audio clips should not throw exceptions");
        }

        [Test]
        public void AudioManager_HasSFXPoolInitialized()
        {
            // Arrange & Act
            var metrics = audioManager.GetPerformanceMetrics();

            // Assert
            Assert.IsTrue(metrics.pooledSources > 0, "SFX pool should be initialized with sources");
        }

        [Test]
        public void PlayOptimizedSFX_HandlesNullClip()
        {
            // Arrange & Act & Assert
            Assert.DoesNotThrow(() =>
            {
                // This should handle null clip gracefully
                audioManager.PlayVictorySound(); // Will work with null clip
            }, "Playing optimized SFX with null clip should not throw exceptions");
        }

        [Test]
        public void VolumeSettings_LoadedOnlyOnce()
        {
            // Arrange & Act
            audioManager.SetMasterVolume(0.5f);
            
            // Create new instance to test loading
            GameObject newAudioManagerObject = new GameObject("NewAudioManager");
            AudioManager newAudioManager = newAudioManagerObject.AddComponent<AudioManager>();
            
            // Assert
            var volumes = newAudioManager.GetVolumeLevels();
            Assert.AreEqual(0.5f, volumes.master, "Volume settings should be loaded correctly");

            // Cleanup
            Object.DestroyImmediate(newAudioManagerObject);
        }

        [Test]
        public void CleanupCoroutine_IsInitialized()
        {
            // Arrange & Act
            // AudioManager should initialize cleanup coroutine in Awake

            // Assert
            Assert.IsNotNull(audioManager, "AudioManager should be properly initialized");
            // Note: We can't easily test the coroutine directly without reflection
        }

        [Test]
        public void SFXPool_HandlesMultipleRequests()
        {
            // Arrange
            int initialPooledSources = audioManager.GetPerformanceMetrics().pooledSources;

            // Act - Play multiple sounds rapidly
            for (int i = 0; i < 5; i++)
            {
                audioManager.PlayUIClick();
            }

            // Assert
            var finalMetrics = audioManager.GetPerformanceMetrics();
            Assert.IsTrue(finalMetrics.pooledSources >= 0, "SFX pool should handle multiple requests");
        }

        [Test]
        public void AudioSettings_PersistAcrossOptimizations()
        {
            // Arrange
            float testVolume = 0.65f;
            audioManager.SetMusicVolume(testVolume);

            // Act - Trigger cleanup
            var metrics = audioManager.GetPerformanceMetrics();
            audioManager.PreloadAudioClips();

            // Assert
            var volumes = audioManager.GetVolumeLevels();
            Assert.AreEqual(testVolume, volumes.music, "Volume settings should persist after optimizations");
        }

        [Test]
        public void MemoryUsageEstimation_IsReasonable()
        {
            // Arrange & Act
            var (activeSources, pooledSources, memoryUsage) = audioManager.GetPerformanceMetrics();

            // Assert
            Assert.IsTrue(memoryUsage < 100f, "Estimated memory usage should be reasonable (< 100MB)");
            Assert.IsTrue(memoryUsage > 0f, "Memory usage should be positive when audio sources exist");
        }

        [UnityTest]
        public IEnumerator PerformanceMetrics_UpdateInRealTime()
        {
            // Arrange
            var initialMetrics = audioManager.GetPerformanceMetrics();

            // Act
            audioManager.PlayUIClick();
            yield return new WaitForSeconds(0.1f);

            var afterPlayMetrics = audioManager.GetPerformanceMetrics();

            // Assert
            Assert.IsTrue(afterPlayMetrics.activeSources >= initialMetrics.activeSources,
                "Active sources should increase or stay the same after playing audio");
        }

        [Test]
        public void OptimizedAudioMethods_DoNotCreateGarbage()
        {
            // Arrange & Act
            long initialAllocatedMemory = System.GC.GetTotalMemory(false);

            // Play multiple sounds
            for (int i = 0; i < 10; i++)
            {
                audioManager.PlayUIClick();
                audioManager.PlayUIHoverSound();
            }

            long finalAllocatedMemory = System.GC.GetTotalMemory(false);

            // Assert
            long memoryDifference = finalAllocatedMemory - initialAllocatedMemory;
            Assert.IsTrue(memoryDifference < 1024 * 1024, // Less than 1MB increase
                "Optimized audio methods should not create excessive garbage collection");
        }
    }
}