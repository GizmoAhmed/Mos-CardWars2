using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Tiles
{
    public class CharmTile : Tile
    {
        [SyncVar]
        public List<GameObject> InUseCharms = new List<GameObject>();

        public override bool IsOccupied => true; 
    
        public override void SetupNeighbors()
        {
            if (across == null) // then I guess lets just set it here
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
