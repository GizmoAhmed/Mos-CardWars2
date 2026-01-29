using CardScripts.CardStatss;
using CardScripts.CardStatss.Runes;
using Lands;
using Mirror;
using UnityEngine;

namespace CardScripts.CardMovements
{
    public class RuneMovement : BaseMovement
    {
        protected override bool ValidPlacement(MiddleLand land)
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
            MiddleLand tileScript = tileObj.GetComponent<MiddleLand>();

            GameObject creatureOnTile = isOwned ? 
                tileScript.creature :
                tileScript.across.GetComponent<MiddleLand>().creature;
            
            CmdBindRune(creatureOnTile);
            
            // ...then discard
            Discard();
        }

        [Command]
        public void CmdBindRune(GameObject creatureOnTile)
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