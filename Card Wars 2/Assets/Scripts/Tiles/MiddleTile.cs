using AbilityEvents;
using Mirror;
using UnityEngine;

namespace Tiles
{
    public class MiddleTile : Tile
    {
        [Header("Visual Occupancy, Client specific")]
        public GameObject creatureVisual;
        public GameObject buildingVisual;

        [Header("Logical Occupancy (Server Authority - Game Logic)")]
        [Tooltip("Logical reference to creature - same on server and all clients")]
        [SyncVar]
        public GameObject logicalCreature;

        [Tooltip("Logical reference to building - same on server and all clients")] [SyncVar]
        public GameObject logicalBuilding;
        
        [Header("Neighbors")]
        public GameObject adjacentLeft;
        public GameObject adjacentRight;
        public GameObject diagonalLeft;
        public GameObject diagonalRight;

        void Awake()
        {
            // Extract row and column from name (L1-L8)
            InitializeLogicalPosition();
            GetComponent<TileEventManager>().InitTileEventManager(this);
        }

        void Start()
        {
            SetupNeighbors();
        }

        /// <summary>
        /// Extract logical row/column from middleTile name
        /// </summary>
        protected override void InitializeLogicalPosition()
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
                playerSide = clientTileOwner ? 0 : 1;
            }
        }

        protected override void SetupNeighbors()
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
    }
}