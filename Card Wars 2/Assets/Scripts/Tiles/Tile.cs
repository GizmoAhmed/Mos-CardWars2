using Mirror;
using UnityEngine;

namespace Tiles
{
    public abstract class Tile : NetworkBehaviour
    {
        [Header("Logical Position (Game Logic)")]
        [Tooltip("Row in the grid: 0 = bottom row (L1-L4), 1 = top row (L5-L8)")]
        public int row;
        
        [Tooltip("Column in the grid: 0-3 (left to right)")]
        public int column;
        
        [Tooltip("Which player's side: 0 = Player 1, 1 = Player 2")]
        public int playerSide; // Will be set based on tileOwner
        
        [Header("Ownership")]
        [Tooltip("True if this player's side, false if opponent's side")]
        public bool tileOwner;
        
        [Header("Visual Neighbors (for rendering/mirroring)")]
        public GameObject across;

        protected abstract void InitializeLogicalPosition();

        protected abstract void SetupNeighbors();
        
        protected GameObject FindTileByName(string tileName)
        {
            var found = GameObject.Find(tileName);
            if (found == null)
                Debug.LogWarning($"Could not find {tileName} for {gameObject.name}");
            return found;
        }

        /// <summary>
        /// Check if another middleTile is at the same logical position
        /// </summary>
        protected bool IsSameLogicalPosition(MiddleTile otherMiddleTile)
        {
            return row == otherMiddleTile.row &&
                   column == otherMiddleTile.column &&
                   playerSide == otherMiddleTile.playerSide;
        }
    }
}