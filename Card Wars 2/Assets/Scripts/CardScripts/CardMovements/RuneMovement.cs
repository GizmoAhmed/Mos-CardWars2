using CardScripts.CardStatss;
using CardScripts.CardStatss.Runes;
using Mirror;
using Tiles;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class RuneMovement : CardMovement
    {
        protected override bool ValidPlacement(Tile land)
        {
            // if can't get passed global checks, abort
            if (!base.ValidPlacement(land))
                return false;

            if (!land.creature) return false; // there's no creature on this land? nope can't place here

            CreatureStats creature = land.creature.GetComponent<CreatureStats>();
            
            return land.tileOwner &&
                   land.creature != null &&
                   (creature.CanBeRuned); // return, if the land has a creature on it and creature can be runed
        }

        [ClientRpc] // dw, already asked if valid placement above
        protected override void RpcPlaceCardOnTile(GameObject tileObj)
        {
            Tile tileScript = tileObj.GetComponent<Tile>();

            GameObject creatureOnTile = isOwned ? 
                tileScript.creature :
                tileScript.across.GetComponent<Tile>().creature;
            
            CmdBindRune(creatureOnTile);
            
            // ...then discard
            Discard();
        }

        [Command]
        private void CmdBindRune(GameObject creatureOnTile)
        {
            // the creature on this land
            CreatureStats creature = creatureOnTile.GetComponent<CreatureStats>();

            if (creature.currentRune1 == null) // empty
            {
                creature.currentRune1 = GetComponent<RuneBase>(); // goes to RuneChange
            }
            else if (creature.currentRune2 == null && creature.overRuneable)
            {
                creature.currentRune2 = GetComponent<RuneBase>(); // goes to RuneChange
            }
        }
    }
}