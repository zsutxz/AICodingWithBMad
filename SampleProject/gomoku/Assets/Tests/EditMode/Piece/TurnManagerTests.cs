using NUnit.Framework;
using UnityEngine;

namespace Gomoku.Tests
{
    [TestFixture]
    public class TurnManagerTests
    {
        private TurnManager turnManager;

        [SetUp]
        public void SetUp()
        {
            GameObject go = new GameObject("TurnManager");
            turnManager = go.AddComponent<TurnManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(turnManager.gameObject);
        }

        [Test]
        public void TurnManager_Initializes_With_Black_As_Starting_Player_By_Default()
        {
            // TurnManager initializes automatically in Awake
            Assert.AreEqual(PlayerType.Black, turnManager.CurrentPlayer);
        }

        [Test]
        public void EndTurn_Switches_From_Black_To_White()
        {
            turnManager.EndTurn();
            Assert.AreEqual(PlayerType.White, turnManager.CurrentPlayer);
        }

        [Test]
        public void EndTurn_Switches_From_White_To_Black()
        {
            // Set starting player to White first
            turnManager.SetStartingPlayer(PlayerType.White);
            turnManager.EndTurn();
            Assert.AreEqual(PlayerType.Black, turnManager.CurrentPlayer);
        }

        [Test]
        public void ResetTurns_Resets_To_Starting_Player()
        {
            turnManager.EndTurn(); // B -> W
            turnManager.EndTurn(); // W -> B
            turnManager.ResetTurns();
            Assert.AreEqual(PlayerType.Black, turnManager.CurrentPlayer);
        }
    }
}
