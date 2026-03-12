using Mirror;
using UnityEngine;

namespace Tiles
{
    public class Tile : NetworkBehaviour
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

        [Header("Occupancy")]
        public GameObject creature;
        public GameObject building;
        
        public virtual bool IsOccupied => creature != null; 

        [Header("Visual Neighbors (for rendering/mirroring)")]
        public GameObject across;
        public GameObject adjacentLeft;
        public GameObject adjacentRight;
        public GameObject diagonalLeft;
        public GameObject diagonalRight;

        void Awake()
        {
            // Extract row and column from name (L1-L8)
            InitializeLogicalPosition();
        }

        void Start()
        {
            SetupNeighbors();
        }

        /// <summary>
        /// Extract logical row/column from tile name
        /// </summary>
        private void InitializeLogicalPosition()
        {
            string myName = gameObject.name; // e.g. "L2"

            if (myName.StartsWith("L") && int.TryParse(myName.Substring(1), out int num))
            {
                // Calculate row: L1-L4 = row 0, L5-L8 = row 1
                row = (num - 1) / 4; // 0 or 1
                
                // Calculate column: 0-3 (left to right)
                column = (num - 1) % 4; // 0, 1, 2, or 3
                
                // PlayerSide will be set by game manager or network spawn
                // For now, you can determine it from tileOwner
                playerSide = tileOwner ? 0 : 1;
                
                // Debug.Log($"{myName}: Row={row}, Column={column}, PlayerSide={playerSide}");
            }
            else
            {
                // Debug.LogError($"Tile {myName} doesn't follow L# naming convention!");
            }
        }

        protected virtual void SetupNeighbors()
        {
            string myName = gameObject.name; // e.g. "L2"

            if (myName.StartsWith("L") && int.TryParse(myName.Substring(1), out int num))
            {
                int col = (num - 1) % 4; // 0..3
                bool isBottom = num <= 4;

                // Across (same column, other row)
                int acrossNum = isBottom ? num + 4 : num - 4;
                across = FindTileByName("L" + acrossNum);

                // Adjacent left
                if (col > 0)
                    adjacentLeft = FindTileByName("L" + (num - 1));

                // Adjacent right
                if (col < 3)
                    adjacentRight = FindTileByName("L" + (num + 1));

                // Diagonal left
                if (col > 0)
                    diagonalLeft = FindTileByName("L" + (acrossNum - 1));

                // Diagonal right
                if (col < 3)
                    diagonalRight = FindTileByName("L" + (acrossNum + 1));
            }
        }

        private GameObject FindTileByName(string tileName)
        {
            var found = GameObject.Find(tileName);
            if (found == null)
                Debug.LogWarning($"Could not find {tileName} for {gameObject.name}");
            return found;
        }
        
        /// <summary>
        /// Check if another tile is at the same logical position
        /// </summary>
        public bool IsSameLogicalPosition(Tile otherTile)
        {
            return row == otherTile.row && 
                   column == otherTile.column && 
                   playerSide == otherTile.playerSide;
        }
    }
}