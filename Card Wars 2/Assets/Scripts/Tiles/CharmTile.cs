using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Tiles
{
    public class CharmTile : Tile
    {
        [SyncVar]
        public List<GameObject> InUseCharms = new List<GameObject>();

        public override bool IsOccupied => true; // Always "occupied" but can hold multiple charms

        void Awake()
        {
            // Override the base Awake to set custom logical position
            InitializeCharmTilePosition();
        }

        /// <summary>
        /// Charm tiles have special logical positions (not part of grid)
        /// </summary>
        private void InitializeCharmTilePosition()
        {
            string myName = gameObject.name;

            if (myName == "SpellGroup1")
            {
                // Player 1's charm zone
                row = -1;        // Special value: not in grid
                column = -1;     // Special value: charm zone
                playerSide = 0;  // Player 1
            }
            else if (myName == "SpellGroup2")
            {
                // Player 2's charm zone
                row = -1;        // Special value: not in grid
                column = -1;     // Special value: charm zone
                playerSide = 1;  // Player 2
            }
            else
            {
                Debug.LogWarning($"Unknown charm tile: {myName}");
            }

            // Debug.Log($"{myName}: CharmTile - Row={row}, Column={column}, PlayerSide={playerSide}");
        }

        protected override void SetupNeighbors()
        {
            // Setup visual mirroring
            if (across == null)
            {
                if (gameObject.name == "SpellGroup1")
                {
                    across = GameObject.Find("SpellGroup2");
                }
                else if (gameObject.name == "SpellGroup2")
                {
                    across = GameObject.Find("SpellGroup1");
                }
            }
        }
    }
}