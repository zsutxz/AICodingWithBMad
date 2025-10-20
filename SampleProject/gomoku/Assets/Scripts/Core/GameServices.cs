using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gomoku.Core
{
    /// <summary>
    /// Service locator pattern implementation for accessing global game systems.
    /// This provides a centralized way to register and retrieve game services
    /// without tight coupling or reliance on FindObjectOfType.
    ///
    /// Usage:
    /// - Register a service: GameServices.Register<ITurnManager>(turnManager);
    /// - Get a service: var turnManager = GameServices.Get<ITurnManager>();
    /// - Check if service exists: if (GameServices.Has<ITurnManager>()) { ... }
    /// </summary>
    public static class GameServices
    {
        // Dictionary to store registered services by type
        private static Dictionary<Type, object> services = new Dictionary<Type, object>();

        /// <summary>
        /// Registers a service with the service locator.
        /// </summary>
        /// <typeparam name="T">The interface type of the service</typeparam>
        /// <param name="service">The service instance to register</param>
        public static void Register<T>(T service) where T : class
        {
            Type serviceType = typeof(T);

            if (services.ContainsKey(serviceType))
            {
                Debug.LogWarning($"Service of type {serviceType.Name} is already registered. Overwriting...");
            }

            services[serviceType] = service;
            Debug.Log($"Service registered: {serviceType.Name}");
        }

        /// <summary>
        /// Retrieves a registered service.
        /// </summary>
        /// <typeparam name="T">The interface type of the service to retrieve</typeparam>
        /// <returns>The registered service instance, or null if not found</returns>
        public static T Get<T>() where T : class
        {
            Type serviceType = typeof(T);

            if (services.TryGetValue(serviceType, out object service))
            {
                return service as T;
            }

            Debug.LogError($"Service of type {serviceType.Name} not found. Make sure it has been registered.");
            return null;
        }

        /// <summary>
        /// Checks if a service of the specified type has been registered.
        /// </summary>
        /// <typeparam name="T">The interface type to check</typeparam>
        /// <returns>True if the service is registered, false otherwise</returns>
        public static bool Has<T>() where T : class
        {
            return services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Unregisters a service.
        /// </summary>
        /// <typeparam name="T">The interface type of the service to unregister</typeparam>
        public static void Unregister<T>() where T : class
        {
            Type serviceType = typeof(T);

            if (services.Remove(serviceType))
            {
                Debug.Log($"Service unregistered: {serviceType.Name}");
            }
            else
            {
                Debug.LogWarning($"Attempted to unregister service of type {serviceType.Name} that was not registered.");
            }
        }

        /// <summary>
        /// Clears all registered services.
        /// This should only be used when completely resetting the game state.
        /// </summary>
        public static void Clear()
        {
            services.Clear();
            Debug.Log("All services cleared from service locator");
        }
    }
    /// <summary>
    /// Interface for the Turn Manager service
    /// </summary>
    public interface ITurnManager
    {
        PlayerType CurrentPlayer { get; }
        int TurnCount { get; }
        void EndTurn();
        void ResetTurns();
        PlayerType GetNextPlayer(PlayerType current);
        bool IsPlayerTurn(PlayerType player);
        void SetStartingPlayer(PlayerType player);
    }

    /// <summary>
    /// Interface for the Game State Manager service
    /// </summary>
    public interface IGameStateManager
    {
        GameState CurrentState { get; }
        void SetState(GameState newState);
        void PauseGame();
        void ResumeGame();
        void RestartGame();
        void ReturnToMainMenu();
    }

    /// <summary>
    /// Interface for the Game Board Controller service
    /// </summary>
    public interface IGameBoardController
    {
        int BoardSize { get; }
        float CellSize { get; }
        Vector2 BoardOffset { get; }
        bool IsInitialized { get; }
        bool IsValidCoordinate(int x, int y);
        Vector3 BoardToWorldPosition(int x, int y);
        bool WorldToBoardPosition(Vector3 worldPosition, out int x, out int y);
        bool PlacePiece(PlayerType pieceType, int x, int y);
        bool RemovePiece(int x, int y);
        void ClearBoard();
        void UpdateBoardSize(int newSize);
        void UpdateCellSize(float newSize);
        void UpdateBoardOffset(Vector2 newOffset);
    }

    /// <summary>
    /// Interface for the Piece Placement service
    /// </summary>
    public interface IPiecePlacement
    {
        bool GameActive { get; }
        int MoveCount { get; }
        PlayerType[,] BoardState { get; }
        List<Vector2Int> MoveHistory { get; }
        void InitializeGame();
        void StopGame();
        void ResumeGame();
        void ClearAllPieces();
        PlayerType GetPieceAt(int x, int y);
        bool IsPositionOccupied(int x, int y);
    }
}