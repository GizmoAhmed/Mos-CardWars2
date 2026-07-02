using System.Collections.Generic;
using AbilityEvents;
using CardScripts.CardMovements;
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
                serverPlayerSide = 0;  // Player 1
            }
            else if (myName == "SpellGroup2")
            {
                // Player 2's charm zone
                row = -1;        // Special value: not in grid
                column = -1;     // Special value: charm zone
                serverPlayerSide = 1;  // Player 2
            }
            else
            {
                Debug.LogWarning($"Unknown charm tile: {myName}");
            }
        }

        // todo look at this ugly ah function
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
            // Debug.LogWarning($"Adding charm {charm.name} to {gameObject.name} SyncList...");
            charms.Add(charm);
        }

        [Server]
        public void RemoveCharm(GameObject charm)
        {
            Debug.LogWarning($"Attempting to remove {charm} from {gameObject.name}...");
            
            if (charms.Contains(charm))
            {
                charms.Remove(charm);
                //Debug.Log($"<color=green>...Successfully removed {charm} from {gameObject.name}</color>");
            }
            else
            {
                /*Debug.LogError($"...Attempt to remove {charm} from {gameObject.name} failed because {charm} isn't present in the list." +
                               $"\nList Count: {charms.Count}");*/
            }
        }
        
        /// <summary>
        /// Clear this tile
        /// </summary>
        public override void DestroyAllCardsOnTile()
        {
            // TIL: sync lists (and other containers in the C family),
            // don't work when you modify the list (in this case, remove from)
            // because it uses an iterator that removes at each index
            // so when you remove the first index, it goes to the second...
            // ...but since the first is removed, and there is no longer a second, it assumes its done
            // claude recommends moving backwards, which makes sense
            for (int i = charms.Count - 1; i >= 0; i--)
            {
                charms[i].GetComponent<CharmMovement>().ServerDiscard();
            }
        }
    }
}