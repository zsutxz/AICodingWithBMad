using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Gomoku.Animation;

namespace Tests.EditMode.Animation
{
    public class AnimationPerformanceMonitorTests
    {
        private GameObject testGameObject;
        private AnimationPerformanceMonitor performanceMonitor;

        [SetUp]
        public void SetUp()
        {
            testGameObject = new GameObject("TestPerformanceMonitor");
            performanceMonitor = testGameObject.AddComponent<AnimationPerformanceMonitor>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testGameObject);
        }

        [Test]
        public void AnimationPerformanceMonitor_InitializesCorrectly()
        {
            Assert.IsNotNull(performanceMonitor);
            Assert.IsTrue(performanceMonitor.EnablePerformanceMonitoring);
            Assert.AreEqual(0f, performanceMonitor.AverageFrameTime);
            Assert.AreEqual(0f, performanceMonitor.CurrentFrameRate);
        }

        [Test]
        public void RecordAnimationPerformance_TracksAnimationData()
        {
            // Record animation performance
            performanceMonitor.RecordAnimationPerformance("TestAnimation", 0.1f);

            // Verify performance data is recorded
            var performanceData = performanceMonitor.GetAnimationPerformanceData("TestAnimation");
            Assert.IsNotNull(performanceData);
            Assert.AreEqual(1, performanceData.ExecutionCount);
            Assert.AreEqual(0.1f, performanceData.TotalDuration);
        }

        [Test]
        public void RecordFrameTime_TracksFramePerformance()
        {
            // Record frame time
            performanceMonitor.RecordFrameTime(0.016f, 0.005f);

            // Verify frame time data is recorded
            Assert.Greater(performanceMonitor.AverageFrameTime, 0f);
            Assert.Greater(performanceMonitor.CurrentFrameRate, 0f);
        }

        [Test]
        public void GetAllPerformanceData_ReturnsAllRecordedData()
        {
            // Record multiple performance entries
            performanceMonitor.RecordAnimationPerformance("Animation1", 0.1f);
            performanceMonitor.RecordAnimationPerformance("Animation2", 0.2f);

            // Get all performance data
            var allData = performanceMonitor.GetAllPerformanceData();
            Assert.IsNotNull(allData);
            Assert.AreEqual(2, allData.Count);
        }

        [Test]
        public void ResetPerformanceData_ClearsAllData()
        {
            // Record some data
            performanceMonitor.RecordAnimationPerformance("TestAnimation", 0.1f);
            performanceMonitor.RecordFrameTime(0.016f, 0.005f);

            // Reset data
            performanceMonitor.ResetPerformanceData();

            // Verify data is cleared
            var performanceData = performanceMonitor.GetAnimationPerformanceData("TestAnimation");
            Assert.IsNull(performanceData);
        }

        [Test]
        public void SetPerformanceMonitoringEnabled_ControlsMonitoring()
        {
            // Disable monitoring
            performanceMonitor.SetPerformanceMonitoringEnabled(false);
            Assert.IsFalse(performanceMonitor.EnablePerformanceMonitoring);

            // Enable monitoring
            performanceMonitor.SetPerformanceMonitoringEnabled(true);
            Assert.IsTrue(performanceMonitor.EnablePerformanceMonitoring);
        }

        [Test]
        public void SetAutoOptimization_ConfiguresOptimization()
        {
            // Set auto optimization
            performanceMonitor.SetAutoOptimization(true, 30);

            // Note: This test verifies the method exists and doesn't throw errors
            // The actual optimization behavior would be tested in integration tests
        }

        [Test]
        public void AnimationPerformanceData_CalculatesAveragesCorrectly()
        {
            // Create performance data instance
            var performanceData = new AnimationPerformanceData();

            // Record multiple durations
            performanceData.RecordDuration(0.1f);
            performanceData.RecordDuration(0.2f);
            performanceData.RecordDuration(0.3f);

            // Verify calculations
            Assert.AreEqual(3, performanceData.ExecutionCount);
            Assert.AreEqual(0.6f, performanceData.TotalDuration);
            Assert.AreEqual(0.2f, performanceData.AverageDuration);
            Assert.AreEqual(0.1f, performanceData.MinDuration);
            Assert.AreEqual(0.3f, performanceData.MaxDuration);
        }

        [UnityTest]
        public IEnumerator PerformanceMonitor_HandlesRealTimeUpdates()
        {
            // Record initial performance
            performanceMonitor.RecordAnimationPerformance("TestAnimation", 0.1f);

            // Wait a frame
            yield return null;

            // Verify performance data is still accessible
            var performanceData = performanceMonitor.GetAnimationPerformanceData("TestAnimation");
            Assert.IsNotNull(performanceData);
        }

        [Test]
        public void SingletonInstance_ProvidesAccessToMonitor()
        {
            // Note: Singleton instance is set up in Awake method
            // This test verifies the singleton pattern is implemented
            var instance = AnimationPerformanceMonitor.Instance;
            Assert.IsNotNull(instance);
        }
    }
}