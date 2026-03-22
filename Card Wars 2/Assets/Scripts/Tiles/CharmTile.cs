using System.Collections.Generic;
using AbilityEvents;
using Mirror;
using UnityEngine;

namespace Tiles
{
    public class CharmTile : Tile
    {
        public readonly SyncList<GameObject> charms = new SyncList<GameObject>();
        
        void Awake()
        {
            // Override the base Awake to set custom logical position
            InitializeLogicalPosition();
            GetComponent<TileEventManager>().InitTileEventManager(this);
        }

        void Start()
        {
            SetupNeighbors();
        }

        /// <summary>
        /// Charm tiles have special logical positions (not part of grid)
        /// </summary>
        protected override void InitializeLogicalPosition()
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
                Debug.LogWarning($"Unknown charm middleTile: {myName}");
            }
        }

        // todo look at this ugly functions
        protected override void SetupNeighbors()
        {
            string thisTilesName = gameObject.name;

            if (thisTilesName == "SpellGroup1")
            {
                across = FindTileByName("SpellGroup2");
            }
            else if (thisTilesName == "SpellGroup2")
            {
                across = FindTileByName("SpellGroup1");
            }
            else
            {
                Debug.LogError($"across wasn't set for {gameObject.name}, as the naming is off or spell group with that name simply wasn't found.");
            }
        }

        [Server] // needs to be done on server because sync list
        public void AddCharm(GameObject charm)
        {
            charms.Add(charm);
        }

        [Server]
        public void RemoveCharm(GameObject charm)
        {
            if (charms.Contains(charm))
            {
                charms.Remove(charm);
            }
            else
            {
                Debug.LogError($"Attempt to remove {charm} from charms list failed because {charm} isn't present in the list for some reason.");
            }

        }
    }
}