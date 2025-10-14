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
            turnManager.StartGame();
            Assert.AreEqual(TurnManager.PlayerType.Black, turnManager.CurrentPlayer);
        }

        [Test]
        public void NextTurn_Switches_From_Black_To_White()
        {
            turnManager.StartGame();
            turnManager.NextTurn();
            Assert.AreEqual(TurnManager.PlayerType.White, turnManager.CurrentPlayer);
        }

        [Test]
        public void NextTurn_Switches_From_White_To_Black()
        {
            turnManager.StartGame();
            turnManager.SetCurrentPlayer(TurnManager.PlayerType.White);
            turnManager.NextTurn();
            Assert.AreEqual(TurnManager.PlayerType.Black, turnManager.CurrentPlayer);
        }

        [Test]
        public void ResetTurns_Resets_To_Starting_Player()
        {
            turnManager.StartGame();
            turnManager.NextTurn(); // B -> W
            turnManager.NextTurn(); // W -> B
            turnManager.ResetTurns();
            Assert.AreEqual(TurnManager.PlayerType.Black, turnManager.CurrentPlayer);
        }
    }
}
